using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;
using System.Text.Json.Serialization;

namespace BizMate.Application.UserCases.ExportReceipt.Queries.GetExportReceipts
{
    public class GetExportReceiptsResponse : BaseResponse
    {
        public IEnumerable<ExportReceiptCoreDto> ExportReceipts { get; }
        public int TotalCount { get; }

        [JsonConstructor]
        public GetExportReceiptsResponse(IEnumerable<ExportReceiptCoreDto> exportReceipts, int totalCount, bool success = true) : base(success)
        {
            ExportReceipts = exportReceipts;
            TotalCount = totalCount;
        }

        public GetExportReceiptsResponse(bool success = false, string? message = null) : base(success, message)
        {
        }
    }
}
