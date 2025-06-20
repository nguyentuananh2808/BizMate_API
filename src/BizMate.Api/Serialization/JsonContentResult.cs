using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.Serialization
{
    public class JsonContentResult : ContentResult
    {
        public JsonContentResult()
        {
            base.ContentType = "application/json";
        }
    }
}
