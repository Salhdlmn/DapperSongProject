using Microsoft.AspNetCore.Mvc;

namespace DapperSongProject.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
