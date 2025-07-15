using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.UserCases.ProductAggregate.ProductCategory.Commands.UpdateProductCategory
{
    public class UpdateProductCategoryRequest : IRequest<UpdateProductCategoryResponse>
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public uint RowVersion { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
    }
}
