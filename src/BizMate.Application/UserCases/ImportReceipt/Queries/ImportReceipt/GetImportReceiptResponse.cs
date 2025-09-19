using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;
using System.Text.Json.Serialization;

namespace BizMate.Application.UserCases.ImportReceipt.Queries.ImportReceipt
{
    public class GetImportReceiptResponse : BaseResponse
    {
        public ImportReceiptCoreDto ImportReceipt { get; }
        [JsonConstructor]
        public GetImportReceiptResponse(ImportReceiptCoreDto importReceipt, bool success = true) : base(success)
        {
            ImportReceipt = importReceipt;
        }
        public GetImportReceiptResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
    }
