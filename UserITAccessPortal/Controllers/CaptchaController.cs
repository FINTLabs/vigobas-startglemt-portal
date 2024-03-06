using CaptchaMvc.HtmlHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace VigoBAS_Start.Controllers
{
    public class CaptchaController : Controller
    {
        // GET: Captcha
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(HomeController HomeController)
        {
            //Here the validation will happen
            if (this.IsCaptchaValid("Captcha is invalid"))
            {
                //redirect to this controller after verification
                return RedirectToAction("AfterVerifying");
            }

            ViewBag.Error = "Invalid Captcha";
            return View();
        }

        public ActionResult AfterVerifying()
        {
            return View();
        }
    }
}