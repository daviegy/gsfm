using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iHealth2.Models;
using IhealthPaging;

namespace iHealth2.Controllers
{
    public class Shop_for_health_productsController : Controller
    {
        private const int DefaultPageSize = 10;
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Shop_for_health_products
        public ActionResult Index()
        {
            return View();
        }
        #region //Manage Insurance Products
        public ActionResult Buy_Health_Insurance(int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            var HI = db.ProductTb
                .Where(d => d.ProductCategory
                    .Contains("Health Insurance") && d.ApprovedStatus == "A")
                    .ToList().OrderByDescending(d => d.VerifiedStatus)
                    .ToPagedList(currentPageIndex, DefaultPageSize);
            return View(HI);
        }

        #endregion
    }
}