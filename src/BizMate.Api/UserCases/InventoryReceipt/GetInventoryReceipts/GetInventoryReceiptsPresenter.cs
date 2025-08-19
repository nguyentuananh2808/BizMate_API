//using BizMate.Api.Serialization;
//using BizMate.Application.Common.Interfaces;
//using BizMate.Application.UserCases.InventoryReceipt.Queries.InventoryReceipts;
//using System.Net;

//namespace BizMate.Api.UserCases.InventoryReceipt.GetInventoryReceipts
//{
//    public class GetInventoryReceiptsPresenter : IOutputPort<GetInventoryReceiptsResponse>
//    {
//        public JsonContentResult ContentResult { get; }

//        public GetInventoryReceiptsPresenter()
//        {
//            ContentResult = new JsonContentResult();
//        }

//        public void Handle(GetInventoryReceiptsResponse response)
//        {
//            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
//            ContentResult.Content = response.Success
//                ? CommonJsonSerializer.SerializeObject(
//                    new GetInventoryReceiptsResponseViewModel(response.InventoryReceipts, response.TotalCount))
//                : CommonJsonSerializer.SerializeObject(response);
//        }
//    }
//}
