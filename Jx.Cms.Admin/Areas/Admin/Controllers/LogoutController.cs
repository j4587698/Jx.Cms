using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Admin.Areas.Admin.Controllers
{
    public class LogoutController : Controller
    {
        // GET
        public async Task<bool> Index()
        {
            await HttpContext.SignOutAsync();
            return true;
        }
    }
}