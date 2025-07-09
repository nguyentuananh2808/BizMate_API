using BizMate.Application.Common.Requests;
using MediatR;

namespace BizMate.Application.UserCases.ProductAggregate.Product.Queries.Products
{
    public class GetProductsRequest : SearchCore, IRequest<GetProductsResponse>
    {
    }
}
