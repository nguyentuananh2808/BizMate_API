using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.Export.Queries.ExportOrders;
using System.Net;

namespace BizMate.Api.UserCases.Export.ExportOrder
{
    public class ExportOrdersPresenter : IOutputPort<ExportOrdersResponse>
    {
        public JsonContentResult ContentResult { get; }

        public ExportOrdersPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(ExportOrdersResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new ExportOrdersResponseViewModel(response.ExportOrders.Orders))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
