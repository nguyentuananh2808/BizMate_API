namespace BizMate.Api.UserCases.DealerPrice.DealerPricePrice
{
    public class CreateDealerPriceResponseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public CreateDealerPriceResponseViewModel(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
