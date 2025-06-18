using Microsoft.AspNetCore.Mvc;

namespace BizMate.Api.UserCases.User
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
