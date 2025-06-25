using MediatR;

namespace BizMate.Application.UserCases.Product.Queries.Product
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
