using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WeebReader.Web.Portal.Controllers
{
    [Authorize]
    public class UserManagerController : Controller
    {
        public IActionResult YourProfile() => View();
    }
}