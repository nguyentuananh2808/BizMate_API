using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.UserCases.Export.Queries.ExportOrders
{
    public class ExportOrdersRequest : IRequest<ExportOrdersResponse>
    {
        [Required]
        public Guid CustomerId { get; set; }
        [Required]
        public DateTime DateFrom { get; set; }
        [Required]
        public DateTime DateTo { get; set; }
    }
}
