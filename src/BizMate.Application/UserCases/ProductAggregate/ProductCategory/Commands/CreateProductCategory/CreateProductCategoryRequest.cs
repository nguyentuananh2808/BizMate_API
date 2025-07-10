using MediatR;

namespace BizMate.Application.UserCases.ProductAggregate.ProductCategory.Commands.CreateProductCategory
{
    public class CreateProductCategoryRequest : IRequest<CreateProductCategoryResponse>
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public CreateProductCategoryRequest(string name, string? description)
        {
            Name = name;
            Description = description;
        }
    }
}
