using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Web.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class LogoutController : Controller
    {
        // GET
        public async Task<IActionResult> Index()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}