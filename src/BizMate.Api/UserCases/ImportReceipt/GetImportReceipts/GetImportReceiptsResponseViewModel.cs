using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Api.UserCases.ImportReceipt.GetImportReceipts
{
    public class GetImportReceiptsResponseViewModel
    {
        public IEnumerable<ImportReceiptCoreDto> ImportReceipts { get; set; }
        public int TotalCount { get; }
        public GetImportReceiptsResponseViewModel(IEnumerable<ImportReceiptCoreDto> importReceipts, int totalCount)
        {
            ImportReceipts = importReceipts;
            TotalCount = totalCount;
        }
    }
}
