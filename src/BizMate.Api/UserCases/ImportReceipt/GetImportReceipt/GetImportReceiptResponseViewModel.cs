using BizMate.Application.UserCases.ImportReceipt.Queries.ImportReceipt;

namespace BizMate.Api.UserCases.ImportReceipt.GetImportReceipt
{
    public class GetImportReceiptResponseViewModel
    {
        public GetImportReceiptResponse ImportReceipt { get; set; }
        public GetImportReceiptResponseViewModel(GetImportReceiptResponse importReceipt)
        {
            ImportReceipt = importReceipt;
        }
    }
}
