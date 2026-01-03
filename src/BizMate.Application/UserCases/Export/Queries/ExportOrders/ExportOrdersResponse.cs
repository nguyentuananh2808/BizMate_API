using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;
using System.Text.Json.Serialization;

namespace BizMate.Application.UserCases.Export.Queries.ExportOrders
{
    public class ExportOrdersResponse : BaseResponse
    {
        public ExportOrderCoreDto ExportOrders { get; }


        [JsonConstructor]
        public ExportOrdersResponse(ExportOrderCoreDto exportOrders, bool success = true) : base(success)
        {
            ExportOrders = exportOrders;
        }

        public ExportOrdersResponse(bool success = false, string? message = null) : base(success, message)
        {
        }
    }
}
