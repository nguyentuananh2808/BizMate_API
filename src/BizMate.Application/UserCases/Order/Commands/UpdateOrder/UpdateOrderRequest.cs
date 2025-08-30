using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.UserCases.Order.Commands.UpdateOrder
{
    public class UpdateOrderRequest : IRequest<UpdateOrderResponse>
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid RowVersion { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        // 1 = Retail (khách lẻ), 2 = Wholesale (khách sỉ)
        public int CustomerType { get; set; }

        // Nếu là khách sỉ thì có CustomerId
        public Guid? CustomerId { get; set; }

        // Thông tin khách hàng 
        public string CustomerName { get; set; } = default!;
        public string CustomerPhone { get; set; } = default!;
        public string DeliveryAddress { get; set; } = default!;

        public decimal TotalAmount { get; set; }
        public string? Description { get; set; }
        public Guid StatusId { get; set; }
        public ICollection<UpdateOrderDetailRequest> Details { get; set; } = new List<UpdateOrderDetailRequest>();
    }

    public class UpdateOrderDetailRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
