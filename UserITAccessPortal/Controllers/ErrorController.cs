using System.Web.Mvc;

namespace VigoBAS_Start.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            return View("Error");
        }

        public ActionResult NotFound()
        {
            Response.StatusCode = 200;
            return View("NotFound");
        }

    }
}