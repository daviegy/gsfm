using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iHealth2.Models;
using System.Threading;
using System.Threading.Tasks;
using IhealthPaging;
namespace iHealth2.Controllers.Menu
{
    public class Med_EquipmentsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private const int DefaultPageSize = 10;
        // GET: Med_Equipments
    
        public ActionResult Find_Equipment(int? page)
        {
             int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
             var Equipmnt = db.ProductTb
                .Where(d => d.ProductCategory.Contains("Medical Equipment") && d.ApprovedStatus == "A")
                .ToList().OrderByDescending(d => d.VerifiedStatus)
                 .ToPagedList(currentPageIndex, DefaultPageSize);
             return View(Equipmnt);
        }
        public async Task<ActionResult> Equipment_Detail(string did)
        {
            string rs = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            did = did.Replace("$25", "/");
            did = did.Replace("$24", "+");
            String ddid = iHealth2.CustomClasses.Cryptoclass.DecryptStringAES(did, rs);
            int id = Convert.ToInt32(ddid);
            var eq = await db.ProductTb.FindAsync(id);
            return View(eq);
        }
        public ActionResult medEquipCompany(int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            var _listmedEquipCom = db.BusinessInfoes.Where(b => b.Category == "Medical Equipment").ToList()
                .OrderByDescending(b => b.Recommended_Status).ThenByDescending(b => b.VerifiedStatus)
                .ToPagedList(currentPageIndex, DefaultPageSize);
            return View(_listmedEquipCom);
        }
        public PartialViewResult search_by_state(int? page, string state)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            object Model = null;
            Model = db.BusinessInfoes.Where(b => b.State.Contains(state) && b.ApprovedStatus == "A" && b.NotifyStatus == 1 && b.Category == "Medical Equipment")
               .OrderByDescending(b => b.Recommended_Status).ThenByDescending(b => b.VerifiedStatus)
                .ToPagedList(currentPageIndex, DefaultPageSize);
            return PartialView("_listMedEquipCom", Model);
        }
        public ActionResult search_equipt_by_manuf(int? page, string m)
        {
           int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            var model = db.ProductTb
                .Where(d => d.ProductCategory.Contains("Medical Equipment")
                    && d.ApprovedStatus == "A" && d.Manufacturer == m)
                    .ToList().OrderByDescending(d => d.VerifiedStatus)
                    .ToPagedList(currentPageIndex, DefaultPageSize);
            return View("../Med_Equipments/Find_Equipment", model);
        }
        public ActionResult search_equipt_by_State(int? page, string s)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            var model = db.ProductTb
                .Where(d => d.ProductCategory.Contains("Medical Equipment")
                    && d.ApprovedStatus == "A" && d.State == s)
                    .ToList().OrderByDescending(d => d.VerifiedStatus)
                    .ToPagedList(currentPageIndex, DefaultPageSize);
            return View("../Med_Equipments/Find_Equipment", model);
        }
        public ActionResult Search_Equipt_By_Price(int? page, string price)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            if (price == "0-5000")
            {
                var drugs = db.ProductTb
               .Where(d => d.ProductCategory
                   .Contains("Medical Equipment") && d.ApprovedStatus == "A" && (d.price >= 0 && d.price <= 5000))
                   .ToList().OrderByDescending(d => d.VerifiedStatus)
                   .ToPagedList(currentPageIndex, DefaultPageSize);
                return View("../Med_Equipments/Find_Equipment", drugs);
            }
            else if (price == "5000-10000")
            {
                var drugs = db.ProductTb
               .Where(d => d.ProductCategory
                   .Contains("Medical Equipment") && d.ApprovedStatus == "A" && (d.price >= 5000 && d.price <= 10000))
                   .ToList().OrderByDescending(d => d.VerifiedStatus)
                   .ToPagedList(currentPageIndex, DefaultPageSize);
                return View("../Med_Equipments/Find_Equipment", drugs);
            }
            else if (price == "10000-20000")
            {
                var drugs = db.ProductTb
               .Where(d => d.ProductCategory
                   .Contains("Medical Equipment") && d.ApprovedStatus == "A" && (d.price >= 10000 && d.price <= 20000))
                   .ToList().OrderByDescending(d => d.VerifiedStatus)
                   .ToPagedList(currentPageIndex, DefaultPageSize);
                return View("../Med_Equipments/Find_Equipment", drugs);
            }
            else if (price == "20000-40000")
            {
                var drugs = db.ProductTb
               .Where(d => d.ProductCategory
                   .Contains("Medical Equipment") && d.ApprovedStatus == "A" && (d.price >= 20000 && d.price <= 40000))
                   .ToList().OrderByDescending(d => d.VerifiedStatus)
                   .ToPagedList(currentPageIndex, DefaultPageSize);
                return View("../Med_Equipments/Find_Equipment", drugs);
            }
            else
            {
                var drugs = db.ProductTb
               .Where(d => d.ProductCategory
                   .Contains("Medical Equipment") && d.ApprovedStatus == "A" && d.price >= 40000)
                   .ToList().OrderByDescending(d => d.VerifiedStatus)
                   .ToPagedList(currentPageIndex, DefaultPageSize);
                return View("../Med_Equipments/Find_Equipment", drugs);
            }

        }

    }
}