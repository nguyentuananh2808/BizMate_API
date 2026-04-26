namespace BizMate.Application.Common.Extensions
{
    public static class SerialNumberNormalizer
    {
        public static List<string> Normalize(IEnumerable<string>? serialNumbers)
        {
            return serialNumbers?
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!.ToUpperInvariant())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? new List<string>();
        }

        public static List<string> FindDuplicates(IEnumerable<string>? serialNumbers)
        {
            return serialNumbers?
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!.ToUpperInvariant())
                .GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList() ?? new List<string>();
        }
    }
}
