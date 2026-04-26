using MediatR;
using System.ComponentModel.DataAnnotations;


namespace BizMate.Application.UserCases.Order.Commands.UpdateStatusOrder
{
    public class UpdateStatusOrderRequest : IRequest<UpdateStatusOrderResponse>
    {
        [Required]
        public Guid Id { get; set; } = default!;
        [Required]
        public Guid StatusId { get; set; } = default!;
        [Required]
        public string StatusCode { get; set; } = default!;
        [Required]
        public Guid RowVersion { get; set; } = default!;
        public ICollection<UpdateStatusOrderSerialDetail> Details { get; set; } = new List<UpdateStatusOrderSerialDetail>();
    }

    public class UpdateStatusOrderSerialDetail
    {
        public Guid? OrderDetailId { get; set; }
        public Guid ProductId { get; set; }
        public ICollection<string> SerialNumbers { get; set; } = new List<string>();
    }
}
