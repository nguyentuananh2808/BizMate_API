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
    }
}
