using MediatR;

namespace BizMate.Application.UserCases.Order.Commands.CreateOrder
{
    public class CreateOrderRequest : IRequest<CreateOrderResponse>
    {
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        // 1 = Retail (khách lẻ), 2 = Wholesale (khách sỉ)
        public bool IsDraft { get; set; }
        public int CustomerType { get; set; }

        // Nếu là khách sỉ thì có CustomerId
        public Guid? CustomerId { get; set; }

        // Thông tin khách hàng 
        public string CustomerName { get; set; } = default!;
        public string CustomerPhone { get; set; } = default!;
        public string DeliveryAddress { get; set; } = default!;
        public string? Description { get; set; }
        public ICollection<CreateOrderDetailRequest> Details { get; set; } = new List<CreateOrderDetailRequest>();
    }

    public class CreateOrderDetailRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
