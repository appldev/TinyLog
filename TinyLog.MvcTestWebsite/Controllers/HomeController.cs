using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TinyLog.MvcTestWebsite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Session["USER"] = "MYUSER";
            ViewData["MyItem"] = new { Name = "Michael", Age = 44 };
            return View();
        }

        private static Random r = new Random();
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            if (r.Next(1, 10) % 2 == 0)
            {
                throw new HttpUnhandledException("A unhandled error occured. go Fish.");
            }
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}