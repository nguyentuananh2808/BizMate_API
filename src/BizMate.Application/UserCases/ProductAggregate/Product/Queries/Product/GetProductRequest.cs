using MediatR;

namespace BizMate.Application.UserCases.ProductAggregate.Product.Queries.Product
{
    public class GetProductRequest : IRequest<GetProductResponse>
    {
        public Guid Id { get; set; }

        public GetProductRequest(Guid id)
        {
            Id = id;
        }
    }
}
