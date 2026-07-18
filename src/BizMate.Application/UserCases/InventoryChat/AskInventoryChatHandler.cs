using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.InventoryChat
{
    public class AskInventoryChatHandler : IRequestHandler<AskInventoryChatRequest, InventoryChatResponse>
    {
        private const int DefaultLimit = 20;

        private readonly IInventoryQuestionParser _parser;
        private readonly IInventoryChatRepository _repository;
        private readonly IUserSession _userSession;
        private readonly ILogger<AskInventoryChatHandler> _logger;

        public AskInventoryChatHandler(
            IInventoryQuestionParser parser,
            IInventoryChatRepository repository,
            IUserSession userSession,
            ILogger<AskInventoryChatHandler> logger)
        {
            _parser = parser;
            _repository = repository;
            _userSession = userSession;
            _logger = logger;
        }

        public async Task<InventoryChatResponse> Handle(
            AskInventoryChatRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var question = request.Question?.Trim();
                if (string.IsNullOrWhiteSpace(question))
                {
                    return BuildHelpResponse("Bạn hãy nhập câu hỏi về tồn kho, sản phẩm hoặc kỹ thuật đang giữ hàng.");
                }

                var parsed = _parser.Parse(question);
                var storeId = _userSession.StoreId;

                return parsed.Intent switch
                {
                    InventoryChatIntent.Help => BuildHelpResponse(),
                    InventoryChatIntent.StockSummary => await BuildStockSummaryResponse(storeId, cancellationToken),
                    InventoryChatIntent.OutOfStock => await BuildOutOfStockResponse(storeId, cancellationToken),
                    InventoryChatIntent.ReservedStock => await BuildReservedStockResponse(storeId, parsed, cancellationToken),
                    InventoryChatIntent.SerialTrackedProducts => await BuildSerialTrackedResponse(storeId, parsed, cancellationToken),
                    InventoryChatIntent.LowStock => await BuildLowStockResponse(storeId, parsed, cancellationToken),
                    InventoryChatIntent.TechnicianHoldings => await BuildHoldingResponse(storeId, parsed, cancellationToken),
                    InventoryChatIntent.ImportByDate => await BuildReceiptResponse(storeId, parsed, true, cancellationToken),
                    InventoryChatIntent.ExportByDate => await BuildReceiptResponse(storeId, parsed, false, cancellationToken),
                    InventoryChatIntent.ProductHistory => await BuildHistoryResponse(storeId, parsed, cancellationToken),
                    InventoryChatIntent.CheckStock => await BuildProductStockResponse(storeId, parsed, true, cancellationToken),
                    InventoryChatIntent.SearchProduct => await BuildProductStockResponse(storeId, parsed, false, cancellationToken),
                    _ => BuildHelpResponse("Mình chưa hiểu câu hỏi này. Bạn có thể hỏi theo một trong các mẫu bên dưới.")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể xử lý câu hỏi trợ lý kho.");
                return new InventoryChatResponse(false, "Không thể xử lý câu hỏi. Vui lòng thử lại.")
                {
                    Intent = InventoryChatIntent.Unknown.ToString(),
                    Answer = "Không thể xử lý câu hỏi. Vui lòng thử lại.",
                    Suggestions = DefaultSuggestions()
                };
            }
        }

        private async Task<InventoryChatResponse> BuildStockSummaryResponse(
            Guid storeId,
            CancellationToken cancellationToken)
        {
            var summary = await _repository.GetStockSummaryAsync(storeId, 2, cancellationToken);
            return new InventoryChatResponse
            {
                Intent = InventoryChatIntent.StockSummary.ToString(),
                Answer = $"Kho đang có {summary.ProductCount:N0} sản phẩm, tổng tồn {summary.TotalQuantity:N0}, khả dụng {summary.TotalAvailable:N0}, đang giữ {summary.TotalReserved:N0}. Hết khả dụng: {summary.OutOfStockCount:N0}, tồn thấp dưới 2: {summary.LowStockCount:N0}, quản lý serial: {summary.SerialTrackedCount:N0}.",
                Table = BuildSummaryTable(summary),
                Suggestions = DefaultSuggestions()
            };
        }

        private async Task<InventoryChatResponse> BuildOutOfStockResponse(
            Guid storeId,
            CancellationToken cancellationToken)
        {
            var products = await _repository.GetLowStockProductsAsync(
                storeId,
                1,
                DefaultLimit,
                cancellationToken);

            if (products.Count == 0)
            {
                return BuildEmptyResponse(
                    InventoryChatIntent.OutOfStock,
                    "Hiện không có sản phẩm nào hết tồn khả dụng.",
                    "Thử hỏi: sản phẩm nào còn tồn dưới 5?");
            }

            return new InventoryChatResponse
            {
                Intent = InventoryChatIntent.OutOfStock.ToString(),
                Answer = $"Có {products.Count:N0} sản phẩm đang hết tồn khả dụng.",
                Table = BuildProductTable(products),
                Suggestions = DefaultSuggestions()
            };
        }

        private async Task<InventoryChatResponse> BuildReservedStockResponse(
            Guid storeId,
            ParsedInventoryQuestion parsed,
            CancellationToken cancellationToken)
        {
            var products = await _repository.GetReservedStockProductsAsync(
                storeId,
                parsed.Keyword,
                DefaultLimit,
                cancellationToken);

            if (products.Count == 0)
            {
                return BuildEmptyResponse(
                    InventoryChatIntent.ReservedStock,
                    "Hiện không tìm thấy sản phẩm nào đang được giữ chỗ phù hợp.",
                    "Thử hỏi: sản phẩm nào đang bị giữ?");
            }

            return new InventoryChatResponse
            {
                Intent = InventoryChatIntent.ReservedStock.ToString(),
                Answer = $"Có {products.Count:N0} sản phẩm đang được giữ chỗ, tổng đã giữ {products.Sum(x => x.Reserved):N0}.",
                Table = BuildProductTable(products),
                Suggestions = DefaultSuggestions()
            };
        }

        private async Task<InventoryChatResponse> BuildSerialTrackedResponse(
            Guid storeId,
            ParsedInventoryQuestion parsed,
            CancellationToken cancellationToken)
        {
            var products = await _repository.GetSerialTrackedProductsAsync(
                storeId,
                parsed.Keyword,
                DefaultLimit,
                cancellationToken);

            if (products.Count == 0)
            {
                return BuildEmptyResponse(
                    InventoryChatIntent.SerialTrackedProducts,
                    "Không tìm thấy sản phẩm quản lý serial phù hợp.",
                    "Thử hỏi: sản phẩm nào quản lý serial?");
            }

            return new InventoryChatResponse
            {
                Intent = InventoryChatIntent.SerialTrackedProducts.ToString(),
                Answer = $"Tìm thấy {products.Count:N0} sản phẩm đang quản lý theo serial.",
                Table = BuildProductTable(products),
                Suggestions = DefaultSuggestions()
            };
        }

        private async Task<InventoryChatResponse> BuildProductStockResponse(
            Guid storeId,
            ParsedInventoryQuestion parsed,
            bool isStockQuestion,
            CancellationToken cancellationToken)
        {
            var products = await _repository.SearchProductsAsync(
                storeId,
                parsed.Keyword,
                DefaultLimit,
                cancellationToken);

            var intent = isStockQuestion ? InventoryChatIntent.CheckStock : InventoryChatIntent.SearchProduct;
            if (products.Count == 0)
            {
                return BuildEmptyResponse(
                    intent,
                    "Không tìm thấy sản phẩm phù hợp trong kho.",
                    "Thử hỏi: camera h5ae còn bao nhiêu?");
            }

            var totalAvailable = products.Sum(x => x.Available);
            var answer = products.Count == 1
                ? $"{products[0].Name} còn {products[0].Available:N0} khả dụng trên tổng tồn {products[0].Quantity:N0}."
                : $"Tìm thấy {products.Count:N0} sản phẩm phù hợp, tổng khả dụng {totalAvailable:N0}.";

            return new InventoryChatResponse
            {
                Intent = intent.ToString(),
                Answer = answer,
                Table = BuildProductTable(products),
                Suggestions = DefaultSuggestions()
            };
        }

        private async Task<InventoryChatResponse> BuildLowStockResponse(
            Guid storeId,
            ParsedInventoryQuestion parsed,
            CancellationToken cancellationToken)
        {
            var threshold = Math.Max(parsed.Threshold ?? 2, 0);
            var products = await _repository.GetLowStockProductsAsync(
                storeId,
                threshold,
                DefaultLimit,
                cancellationToken);

            if (products.Count == 0)
            {
                return BuildEmptyResponse(
                    InventoryChatIntent.LowStock,
                    $"Không có sản phẩm nào có tồn khả dụng dưới {threshold:N0}.",
                    "Thử hỏi: sản phẩm nào còn tồn dưới 5?");
            }

            return new InventoryChatResponse
            {
                Intent = InventoryChatIntent.LowStock.ToString(),
                Answer = $"Có {products.Count:N0} sản phẩm đang có tồn khả dụng dưới {threshold:N0}.",
                Table = BuildProductTable(products),
                Suggestions = DefaultSuggestions()
            };
        }

        private async Task<InventoryChatResponse> BuildHoldingResponse(
            Guid storeId,
            ParsedInventoryQuestion parsed,
            CancellationToken cancellationToken)
        {
            var holdings = await _repository.SearchTechnicianHoldingsAsync(
                storeId,
                parsed.Keyword,
                DefaultLimit,
                cancellationToken);

            if (holdings.Count == 0)
            {
                return BuildEmptyResponse(
                    InventoryChatIntent.TechnicianHoldings,
                    "Không tìm thấy hàng kỹ thuật đang giữ phù hợp.",
                    "Thử hỏi: kỹ thuật Tuấn Anh đang giữ hàng gì?");
            }

            var totalQuantity = holdings.Sum(x => x.Quantity);
            return new InventoryChatResponse
            {
                Intent = InventoryChatIntent.TechnicianHoldings.ToString(),
                Answer = $"Tìm thấy {holdings.Count:N0} dòng hàng kỹ thuật đang giữ, tổng số lượng {totalQuantity:N0}.",
                Table = BuildHoldingTable(holdings),
                Suggestions = DefaultSuggestions()
            };
        }

        private async Task<InventoryChatResponse> BuildReceiptResponse(
            Guid storeId,
            ParsedInventoryQuestion parsed,
            bool isImport,
            CancellationToken cancellationToken)
        {
            var fromUtc = parsed.FromUtc ?? DateTime.UtcNow.Date;
            var toUtc = parsed.ToUtc ?? fromUtc.AddDays(1);
            var label = parsed.DateLabel ?? "hôm nay";

            var receipts = isImport
                ? await _repository.GetImportReceiptsAsync(storeId, fromUtc, toUtc, parsed.Keyword, DefaultLimit, cancellationToken)
                : await _repository.GetExportReceiptsAsync(storeId, fromUtc, toUtc, parsed.Keyword, DefaultLimit, cancellationToken);

            var intent = isImport ? InventoryChatIntent.ImportByDate : InventoryChatIntent.ExportByDate;
            var name = isImport ? "nhập kho" : "xuất kho";

            if (receipts.Count == 0)
            {
                return BuildEmptyResponse(
                    intent,
                    $"Không có phiếu {name} phù hợp trong {label}.",
                    isImport ? "Thử hỏi: hôm nay có nhập kho sản phẩm nào?" : "Thử hỏi: hôm nay có xuất kho sản phẩm nào?");
            }

            return new InventoryChatResponse
            {
                Intent = intent.ToString(),
                Answer = $"Có {receipts.Count:N0} phiếu {name} trong {label}, tổng số lượng {receipts.Sum(x => x.TotalQuantity):N0}.",
                Table = BuildReceiptTable(receipts, isImport),
                Suggestions = DefaultSuggestions()
            };
        }

        private async Task<InventoryChatResponse> BuildHistoryResponse(
            Guid storeId,
            ParsedInventoryQuestion parsed,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(parsed.Keyword))
            {
                return BuildEmptyResponse(
                    InventoryChatIntent.ProductHistory,
                    "Bạn cần nhập thêm tên hoặc mã sản phẩm để xem lịch sử.",
                    "Ví dụ: lịch sử sản phẩm H5AE");
            }

            var history = await _repository.GetProductHistoryAsync(
                storeId,
                parsed.Keyword,
                parsed.FromUtc,
                parsed.ToUtc,
                DefaultLimit,
                cancellationToken);

            if (history.Count == 0)
            {
                return BuildEmptyResponse(
                    InventoryChatIntent.ProductHistory,
                    "Không tìm thấy lịch sử nhập/xuất cho sản phẩm này.",
                    "Thử hỏi: lịch sử sản phẩm H5AE tháng này");
            }

            return new InventoryChatResponse
            {
                Intent = InventoryChatIntent.ProductHistory.ToString(),
                Answer = $"Tìm thấy {history.Count:N0} giao dịch gần nhất liên quan đến sản phẩm này.",
                Table = BuildHistoryTable(history),
                Suggestions = DefaultSuggestions()
            };
        }

        private static InventoryChatResponse BuildHelpResponse(string? answer = null)
            => new()
            {
                Intent = InventoryChatIntent.Help.ToString(),
                Answer = answer ?? "Bạn có thể hỏi tồn kho, tìm sản phẩm, hàng tồn thấp, kỹ thuật đang giữ hàng, nhập/xuất theo ngày và lịch sử sản phẩm.",
                Suggestions = DefaultSuggestions()
            };

        private static InventoryChatResponse BuildEmptyResponse(
            InventoryChatIntent intent,
            string answer,
            string suggestion)
            => new()
            {
                Intent = intent.ToString(),
                Answer = answer,
                Suggestions = DefaultSuggestions().Prepend(suggestion).Distinct().ToArray()
            };

        private static InventoryChatTableDto BuildSummaryTable(InventoryChatStockSummaryDto summary)
            => new()
            {
                Title = "Tổng quan kho",
                Columns = new[]
                {
                    Column("metric", "Chỉ số"),
                    Column("value", "Số lượng")
                },
                Rows = new[]
                {
                    Row("Tổng sản phẩm", summary.ProductCount),
                    Row("Tổng tồn kho", summary.TotalQuantity),
                    Row("Tồn khả dụng", summary.TotalAvailable),
                    Row("Đang giữ chỗ", summary.TotalReserved),
                    Row("Hết tồn khả dụng", summary.OutOfStockCount),
                    Row("Tồn thấp dưới 2", summary.LowStockCount),
                    Row("Quản lý serial", summary.SerialTrackedCount)
                }
            };

        private static InventoryChatTableDto BuildProductTable(IReadOnlyList<InventoryChatProductStockDto> rows)
            => new()
            {
                Title = "Danh sách sản phẩm",
                Columns = new[]
                {
                    Column("code", "Mã"),
                    Column("name", "Sản phẩm"),
                    Column("category", "Loại"),
                    Column("quantity", "Tồn kho"),
                    Column("reserved", "Đã giữ"),
                    Column("available", "Khả dụng"),
                    Column("serial", "Quản lý SN")
                },
                Rows = rows.Select(x => new Dictionary<string, string>
                {
                    ["code"] = x.Code,
                    ["name"] = x.Name,
                    ["category"] = x.CategoryName ?? "-",
                    ["quantity"] = x.Quantity.ToString("N0"),
                    ["reserved"] = x.Reserved.ToString("N0"),
                    ["available"] = x.Available.ToString("N0"),
                    ["serial"] = x.IsSerialTracked ? "Theo SN" : "Theo SL"
                }).ToArray()
            };

        private static InventoryChatTableDto BuildHoldingTable(IReadOnlyList<InventoryChatHoldingDto> rows)
            => new()
            {
                Title = "Kỹ thuật đang giữ hàng",
                Columns = new[]
                {
                    Column("technician", "Kỹ thuật"),
                    Column("phone", "SĐT"),
                    Column("code", "Mã SP"),
                    Column("product", "Sản phẩm"),
                    Column("type", "Loại giữ"),
                    Column("quantity", "Số lượng"),
                    Column("lastBorrowedAt", "Mượn gần nhất")
                },
                Rows = rows.Select(x => new Dictionary<string, string>
                {
                    ["technician"] = x.TechnicianName,
                    ["phone"] = x.TechnicianPhone ?? "-",
                    ["code"] = x.ProductCode,
                    ["product"] = x.ProductName,
                    ["type"] = x.BorrowType,
                    ["quantity"] = x.Quantity.ToString("N0"),
                    ["lastBorrowedAt"] = FormatDateTime(x.LastBorrowedAt)
                }).ToArray()
            };

        private static InventoryChatTableDto BuildReceiptTable(
            IReadOnlyList<InventoryChatReceiptDto> rows,
            bool isImport)
            => new()
            {
                Title = isImport ? "Phiếu nhập kho" : "Phiếu xuất kho",
                Columns = new[]
                {
                    Column("code", "Mã phiếu"),
                    Column("date", "Ngày"),
                    Column("partner", isImport ? "Nhà cung cấp" : "Khách hàng"),
                    Column("quantity", "Tổng SL"),
                    Column("amount", "Tổng tiền"),
                    Column("products", "Sản phẩm")
                },
                Rows = rows.Select(x => new Dictionary<string, string>
                {
                    ["code"] = x.Code,
                    ["date"] = FormatDateTime(x.CreatedDate),
                    ["partner"] = x.PartnerName ?? "-",
                    ["quantity"] = x.TotalQuantity.ToString("N0"),
                    ["amount"] = $"{x.TotalAmount:N0} vnd",
                    ["products"] = x.ProductsSummary
                }).ToArray()
            };

        private static InventoryChatTableDto BuildHistoryTable(IReadOnlyList<InventoryChatHistoryDto> rows)
            => new()
            {
                Title = "Lịch sử sản phẩm",
                Columns = new[]
                {
                    Column("date", "Ngày"),
                    Column("source", "Nguồn"),
                    Column("reference", "Chứng từ"),
                    Column("code", "Mã SP"),
                    Column("product", "Sản phẩm"),
                    Column("quantity", "SL"),
                    Column("actor", "Liên quan"),
                    Column("note", "Ghi chú")
                },
                Rows = rows.Select(x => new Dictionary<string, string>
                {
                    ["date"] = FormatDateTime(x.Date),
                    ["source"] = x.Source,
                    ["reference"] = x.ReferenceCode ?? "-",
                    ["code"] = x.ProductCode,
                    ["product"] = x.ProductName,
                    ["quantity"] = x.Quantity.ToString("N0"),
                    ["actor"] = x.Actor ?? "-",
                    ["note"] = x.Note ?? "-"
                }).ToArray()
            };

        private static InventoryChatColumnDto Column(string key, string label)
            => new() { Key = key, Label = label };

        private static Dictionary<string, string> Row(string metric, int value)
            => new()
            {
                ["metric"] = metric,
                ["value"] = value.ToString("N0")
            };

        private static string[] DefaultSuggestions()
            => new[]
            {
                "Tổng quan kho hiện tại",
                "Sản phẩm nào hết hàng?",
                "Sản phẩm nào đang bị giữ?",
                "Sản phẩm nào quản lý serial?",
                "Tuần trước có xuất kho sản phẩm nào?",
                "Tháng trước có nhập kho sản phẩm nào?",
                "7 ngày qua có nhập kho sản phẩm nào?",
                "camera H5AE còn bao nhiêu?",
                "sản phẩm nào còn tồn dưới 2?",
                "kỹ thuật Tuấn Anh đang giữ hàng gì?",
                "hôm nay có nhập kho sản phẩm nào?",
                "hôm nay có xuất kho sản phẩm nào?",
                "lịch sử sản phẩm H5AE tháng này"
            };

        private static string FormatDateTime(DateTime value)
            => value.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
    }
}
