using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Api.UserCases.ExportReceipt.GetExportReceipts
{
    public class GetExportReceiptsResponseViewModel
    {
        public IEnumerable<ExportReceiptCoreDto> ExportReceipts { get; set; }
        public int TotalCount { get; }
        public GetExportReceiptsResponseViewModel(IEnumerable<ExportReceiptCoreDto> exportReceipts, int totalCount)
        {
            ExportReceipts = exportReceipts;
            TotalCount = totalCount;
        }
    }
}
