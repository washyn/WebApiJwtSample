using Microsoft.AspNetCore.Mvc;

namespace Agent.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Redirect("~/swagger");
        }
    }
}