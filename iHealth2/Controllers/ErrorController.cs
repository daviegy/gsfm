using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iHealth2.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ErrorAlert()
        {
            return View();
        }
        public ActionResult NotFound404()
        {
            return View();
        }
    }
}