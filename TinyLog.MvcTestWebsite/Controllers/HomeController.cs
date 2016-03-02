using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TinyLog.Mvc;

namespace TinyLog.MvcTestWebsite.Controllers
{
    
    public class HomeController : Controller
    {
        [OutputCache(Duration = 0)]
        public async Task<ActionResult> Index()
        {
            if (Session != null)
            {
                Session["USER"] = "MYUSER";
                ViewData["MyItem"] = new { Name = "Michael", Age = 44 };
            }
            IEnumerable<LogEntry> model = await Log.Default.ReadLogEntriesAsync(new LogEntryFilter());
            return View(model);
        }

        public async Task<ActionResult> LogEntry(Guid Id)
        {
            LogEntry model = await Log.Default.ReadLogEntryAsync(Id);
            if (model == null)
            {
                throw new ApplicationException(string.Format("A log entry with the Id '{0}' was not found", Id));
            }
            return View(model);
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