using BizMate.Application.Common.Requests;
using MediatR;

namespace BizMate.Application.UserCases.Product.Queries.Products
{
    public class ProductsRequest : SearchCore ,  IRequest<ProductsResponse>
    {
    }
}
