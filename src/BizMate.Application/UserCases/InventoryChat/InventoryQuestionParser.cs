using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using BizMate.Application.Common.Interfaces;

namespace BizMate.Application.UserCases.InventoryChat
{
    public class InventoryQuestionParser : IInventoryQuestionParser
    {
        private static readonly Regex ExplicitDateRegex =
            new(@"\b(?<day>\d{1,2})[\/\-.](?<month>\d{1,2})(?:[\/\-.](?<year>\d{2,4}))?\b",
                RegexOptions.Compiled);

        private static readonly Regex ThresholdRegex =
            new(@"(?:duoi|nho hon|it hon|<|<=)\s*(?<value>\d+)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public ParsedInventoryQuestion Parse(string question)
        {
            var raw = question?.Trim() ?? string.Empty;
            var normalized = Normalize(raw);
            var parsed = new ParsedInventoryQuestion
            {
                RawQuestion = raw,
                Intent = ResolveIntent(normalized)
            };

            var dateRange = ResolveDateRange(normalized);
            parsed.FromUtc = dateRange.FromUtc;
            parsed.ToUtc = dateRange.ToUtc;
            parsed.DateLabel = dateRange.Label;

            parsed.Threshold = ResolveThreshold(normalized);
            parsed.Keyword = ExtractKeyword(normalized, parsed.Intent);

            if (string.IsNullOrWhiteSpace(raw))
            {
                parsed.Intent = InventoryChatIntent.Help;
            }

            return parsed;
        }

        private static InventoryChatIntent ResolveIntent(string normalized)
        {
            if (string.IsNullOrWhiteSpace(normalized)
                || ContainsAny(normalized, "help", "huong dan", "hoi duoc gi", "tro giup"))
            {
                return InventoryChatIntent.Help;
            }

            if (ContainsAny(normalized, "ky thuat", "dang giu", "giu hang", "muon hang", "muon"))
                return InventoryChatIntent.TechnicianHoldings;

            if (ContainsAny(normalized, "lich su", "nhap xuat", "giao dich"))
                return InventoryChatIntent.ProductHistory;

            if (ContainsAny(normalized, "nhap kho", "phieu nhap", "hang nhap"))
                return InventoryChatIntent.ImportByDate;

            if (ContainsAny(normalized, "xuat kho", "phieu xuat", "hang xuat"))
                return InventoryChatIntent.ExportByDate;

            if (ContainsAny(normalized, "sap het", "ton thap", "duoi", "it hon", "nho hon"))
                return InventoryChatIntent.LowStock;

            if (ContainsAny(normalized, "ton kho", "kha dung", "con bao nhieu", "so luong con", "con may"))
                return InventoryChatIntent.CheckStock;

            if (ContainsAny(normalized, "tim", "kiem", "liet ke", "danh sach", "san pham", "hang hoa"))
                return InventoryChatIntent.SearchProduct;

            return InventoryChatIntent.SearchProduct;
        }

        private static int? ResolveThreshold(string normalized)
        {
            var match = ThresholdRegex.Match(normalized);
            if (match.Success && int.TryParse(match.Groups["value"].Value, out var value))
                return value;

            return ContainsAny(normalized, "sap het", "ton thap", "duoi") ? 2 : null;
        }

        private static (DateTime? FromUtc, DateTime? ToUtc, string? Label) ResolveDateRange(string normalized)
        {
            var today = GetBusinessToday();

            if (normalized.Contains("hom nay"))
                return ToUtcRange(today, "hôm nay");

            if (normalized.Contains("hom qua"))
                return ToUtcRange(today.AddDays(-1), "hôm qua");

            var explicitDate = ExplicitDateRegex.Match(normalized);
            if (explicitDate.Success)
            {
                var day = int.Parse(explicitDate.Groups["day"].Value, CultureInfo.InvariantCulture);
                var month = int.Parse(explicitDate.Groups["month"].Value, CultureInfo.InvariantCulture);
                var yearValue = explicitDate.Groups["year"].Value;
                var year = string.IsNullOrWhiteSpace(yearValue)
                    ? today.Year
                    : int.Parse(yearValue, CultureInfo.InvariantCulture);

                if (year < 100)
                    year += 2000;

                if (DateTime.TryParseExact(
                        $"{day:00}/{month:00}/{year:0000}",
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var date))
                {
                    return ToUtcRange(date.Date, date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                }
            }

            if (normalized.Contains("tuan nay"))
            {
                var diff = ((int)today.DayOfWeek + 6) % 7;
                var from = today.AddDays(-diff);
                var to = from.AddDays(7);
                return (ToUtc(from), ToUtc(to), "tuần này");
            }

            if (normalized.Contains("thang nay"))
            {
                var from = new DateTime(today.Year, today.Month, 1);
                var to = from.AddMonths(1);
                return (ToUtc(from), ToUtc(to), "tháng này");
            }

            return (null, null, null);
        }

        private static string? ExtractKeyword(string normalized, InventoryChatIntent intent)
        {
            var text = normalized;
            var patterns = new[]
            {
                @"\b(hom nay|hom qua|tuan nay|thang nay)\b",
                ExplicitDateRegex.ToString(),
                @"\b(con bao nhieu|so luong con|kha dung|ton kho|trong kho|hang hoa|san pham|sp)\b",
                @"\b(tim|kiem|liet ke|danh sach|loc|co|khong|nao|bao nhieu|cho toi|giup toi)\b",
                @"\b(nhap kho|phieu nhap|hang nhap|xuat kho|phieu xuat|hang xuat)\b",
                @"\b(lich su|nhap xuat|giao dich|theo ngay|ngay)\b",
                @"\b(ky thuat|dang giu|giu hang|muon hang|muon|cua|cho|hang|gi|dang|giu)\b",
                @"\b(sap het|ton thap|duoi|nho hon|it hon)\s*\d*\b"
            };

            foreach (var pattern in patterns)
            {
                text = Regex.Replace(text, pattern, " ", RegexOptions.IgnoreCase);
            }

            text = Regex.Replace(text, @"[^\p{L}\p{N}#\-_ ]+", " ");
            text = Regex.Replace(text, @"\s+", " ").Trim();

            if (intent is InventoryChatIntent.ImportByDate or InventoryChatIntent.ExportByDate
                && text.Length < 2)
            {
                return null;
            }

            return text.Length >= 2 ? text : null;
        }

        public static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var lower = value.Trim().ToLowerInvariant().Replace('đ', 'd');
            var normalized = lower.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(normalized.Length);

            foreach (var ch in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                    builder.Append(ch);
            }

            return Regex.Replace(builder.ToString().Normalize(NormalizationForm.FormC), @"\s+", " ").Trim();
        }

        private static bool ContainsAny(string value, params string[] terms)
            => terms.Any(value.Contains);

        private static DateTime GetBusinessToday()
        {
            var timeZone = ResolveBusinessTimeZone();
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone).Date;
        }

        private static (DateTime FromUtc, DateTime ToUtc, string Label) ToUtcRange(DateTime localDate, string label)
            => (ToUtc(localDate.Date), ToUtc(localDate.Date.AddDays(1)), label);

        private static DateTime ToUtc(DateTime localDateTime)
        {
            var timeZone = ResolveBusinessTimeZone();
            var unspecified = DateTime.SpecifyKind(localDateTime, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(unspecified, timeZone);
        }

        private static TimeZoneInfo ResolveBusinessTimeZone()
        {
            foreach (var id in new[] { "SE Asia Standard Time", "Asia/Ho_Chi_Minh" })
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(id);
                }
                catch
                {
                    
                }
            }

            return TimeZoneInfo.Utc;
        }
    }
}
