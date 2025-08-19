namespace BizMate.Api.UserCases.DealerPrice.DeleteDealerPrice
{
    public class DeleteDealerPriceResponseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public DeleteDealerPriceResponseViewModel(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
