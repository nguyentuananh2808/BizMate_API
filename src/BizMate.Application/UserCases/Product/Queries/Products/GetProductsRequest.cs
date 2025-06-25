using BizMate.Application.Common.Requests;
using MediatR;

namespace BizMate.Application.UserCases.Product.Queries.Products
{
    public class GetProductsRequest : SearchCore ,  IRequest<GetProductsResponse>
    {
    }
}
