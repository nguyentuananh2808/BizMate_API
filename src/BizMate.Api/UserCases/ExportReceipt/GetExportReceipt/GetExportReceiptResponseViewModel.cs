using BizMate.Application.UserCases.ExportReceipt.Queries.GetExportReceipt;

namespace BizMate.Api.UserCases.ExportReceipt.GetExportReceipt
{
    public class GetExportReceiptResponseViewModel
    {

        public GetExportReceiptResponse ExportReceipt { get; set; }
        public GetExportReceiptResponseViewModel(GetExportReceiptResponse exportReceipt)
        {
            ExportReceipt = exportReceipt;
        }
    }
}
