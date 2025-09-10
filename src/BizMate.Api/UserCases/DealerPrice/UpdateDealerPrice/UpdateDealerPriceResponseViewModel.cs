namespace BizMate.Api.UserCases.DealerPrice.UpdateDealerPrice
{
    public class UpdateDealerPriceResponseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public UpdateDealerPriceResponseViewModel(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
