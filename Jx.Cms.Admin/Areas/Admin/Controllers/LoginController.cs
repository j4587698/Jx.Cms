using System.Threading.Tasks;
using Jx.Cms.Service.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Jx.Cms.Admin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        private IAdminUserService _adminUserService;

        public LoginController(IAdminUserService adminUserService)
        {
            _adminUserService = adminUserService;
        }
        
        // GET
        [HttpGet]
        public IActionResult Index(string redirect)
        {
            ViewData["redirect"] = redirect;
            return View();
        }

        [HttpPost]
        public Task<IActionResult> Index(string username, string password, string rememberme, string redirect)
        {
            if (_adminUserService.LoginCheck(username, password))
            {
                
            }
            return View();
        }
    }
}