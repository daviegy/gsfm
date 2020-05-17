using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iHealth2.Controllers
{
    public class StoreController : Controller
    {
        // GET: Store
        public ActionResult store_product_list()
        {
            return View();
        }
    }
}