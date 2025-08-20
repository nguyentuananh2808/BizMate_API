using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;
using System.Text.Json.Serialization;

namespace BizMate.Application.UserCases.ImportReceipt.Queries.ImportReceipts
{
    public class GetImportReceiptsResponse : BaseResponse
    {
        public IEnumerable<ImportReceiptCoreDto> ImportReceipts { get; }
        public int TotalCount { get; }

        [JsonConstructor]
        public GetImportReceiptsResponse(IEnumerable<ImportReceiptCoreDto> importReceipts, int totalCount, bool success = true) : base(success)
        {
            ImportReceipts = importReceipts;
            TotalCount = totalCount;
        }

        public GetImportReceiptsResponse(bool success = false, string? message = null) : base(success, message)
        {
        }
    }
}
