namespace BizMate.Api.UserCases.DealerLevel.DeleteDealerLevel
{
    public class DeleteDealerLevelResponseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public DeleteDealerLevelResponseViewModel(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
