using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.UserCases.ProductAggregate.Product.Commands.UpdateProduct
{
    public class UpdateProductRequest : IRequest<UpdateProductResponse>
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public uint RowVersion { get; set; }
        public Guid? ProductCategoryId { get; set; }
        public string Name { get; set; }
        public int Unit { get; set; }
        public string? ImageUrl { get; set; }
        public Guid? SupplierId { get; set; }
        public string? Description { get; set; }
    }
}
