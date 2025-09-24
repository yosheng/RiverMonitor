using Microsoft.AspNetCore.Mvc;

namespace RiverMonitor.Api.Controllers
{
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
