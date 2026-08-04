using MediatR;

namespace BizMate.Application.UserCases.ProductAggregate.ProductCategory.Commands.CreateProductCategory
{
    public class CreateProductCategoryRequest : IRequest<CreateProductCategoryResponse>
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public Guid? CategoryGroupId { get; set; }
        public CreateProductCategoryRequest(string name, string? description, Guid? categoryGroupId = null)
        {
            Name = name;
            Description = description;
            CategoryGroupId = categoryGroupId;
        }
    }
}
