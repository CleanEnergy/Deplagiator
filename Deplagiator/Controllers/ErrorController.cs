using Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Deplagiator.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index(string error, string stamp)
        {
            ViewBag.Error = ErrorLog.Base64Decode(error);
            ViewBag.Stamp = ErrorLog.Base64Decode(stamp);

            return View();
        }

        public ActionResult Information()
        {
            ViewBag.Messages = TempData["Messages"];

            return View();
        }
    }
}