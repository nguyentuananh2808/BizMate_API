using MediatR;

namespace BizMate.Application.UserCases.ProductAggregate.ProductCategory.Queries.ProductCategory
{
    public class GetProductCategoryRequest : IRequest<GetProductCategoryResponse>
    {
        public Guid Id { get; set; }

        public GetProductCategoryRequest(Guid id)
        {
            Id = id;
        }
    }
}
