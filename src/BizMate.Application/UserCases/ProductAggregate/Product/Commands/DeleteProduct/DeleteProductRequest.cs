using MediatR;

namespace BizMate.Application.UserCases.ProductAggregate.Product.Commands.DeleteProduct
{
    public class DeleteProductRequest : IRequest<DeleteProductResponse>
    {
        public Guid Id { get; set; }

        public DeleteProductRequest(Guid id)
        {
            Id = id;
        }
    }
}
