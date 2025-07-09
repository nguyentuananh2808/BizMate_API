using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.UserCases.ProductAggregate.Product.Commands.CreateProduct
{
    public class CreateProductRequest : IRequest<CreateProductResponse>
    {
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        public int Unit { get; set; } = default!;
        public string? ImageUrl { get; set; }
        public Guid? SupplierId { get; set; }
        public string? Description { get; set; }
    }
}
