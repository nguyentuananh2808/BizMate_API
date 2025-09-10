using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Api.UserCases.Order.GetOrders
{
    public class GetOrdersResponseViewModel
    {
        public IEnumerable<OrderCoreDto> Orders { get; set; }
        public int TotalCount { get; }
        public GetOrdersResponseViewModel(IEnumerable<OrderCoreDto> orders, int totalCount)
        {
            Orders = orders;
            TotalCount = totalCount;
        }
    }
}
