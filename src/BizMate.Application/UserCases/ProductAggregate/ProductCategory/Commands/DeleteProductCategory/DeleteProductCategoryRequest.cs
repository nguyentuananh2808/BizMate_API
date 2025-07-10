using MediatR;

namespace BizMate.Application.UserCases.ProductAggregate.ProductCategory.Commands.DeleteProductCategory
{
    public class DeleteProductCategoryRequest : IRequest<DeleteProductCategoryResponse>
    {
        public Guid Id { get; set; }

        public DeleteProductCategoryRequest(Guid id)
        {
            Id = id;
        }
    }
}
