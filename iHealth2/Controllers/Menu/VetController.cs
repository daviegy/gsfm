using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iHealth2.Models;
using IhealthPaging;
using System.Threading.Tasks;
namespace iHealth2.Controllers.Menu
{
    public class VetController : Controller
    {
        private const int  DefaultPageSize = 15;
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Vet
        public ActionResult Find_Vet_Center(int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            var bs = db.BusinessInfoes.Where(b => b.Category.Contains("Veterinary Clinic")
                && b.ApprovedStatus == "A" && b.NotifyStatus == 1)
                 .OrderByDescending(b => b.Recommended_Status).ThenByDescending(b => b.VerifiedStatus)
                  .ToPagedList(currentPageIndex, DefaultPageSize);
            return View(bs);
        }
        public PartialViewResult find_Vet_Centr(int? page,string sbValue)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            object Model = null;
            Model = db.BusinessInfoes.Where(b => b.subCategory1.Contains(sbValue)
                && b.ApprovedStatus == "A" && b.NotifyStatus == 1 
                && b.Category == "Veterinary Clinic")
                .OrderByDescending(b => b.Recommended_Status).ThenByDescending(b => b.VerifiedStatus)
                  .ToPagedList(currentPageIndex, DefaultPageSize); 
            return PartialView("_listVetCenter", Model);
        }
        public PartialViewResult search_by_state(int? page,string state)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            object Model = null;
            Model = db.BusinessInfoes.Where(b => b.State.Contains(state)
                && b.ApprovedStatus == "A" && b.NotifyStatus == 1 
                && b.Category == "Veterinary Clinic")
               .OrderByDescending(b => b.Recommended_Status).ThenByDescending(b => b.VerifiedStatus)
                  .ToPagedList(currentPageIndex, DefaultPageSize); 
            return PartialView("_listVetCenter", Model);
        }
        public ActionResult Search_Drugs(int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            var drugs = db.ProductTb
                .Where(d => d.ProductCategory
                    .Contains("veterinary drugs") && d.ApprovedStatus == "A")
                    .ToList().OrderByDescending(d => d.VerifiedStatus)
                    .ToPagedList(currentPageIndex, DefaultPageSize);
            return View(drugs);
        }

        public async Task<ActionResult> Drug_Detail(string did)
        {
            string rs = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            did = did.Replace("$25", "/");
            did = did.Replace("$24", "+");
            String ddid = iHealth2.CustomClasses.Cryptoclass.DecryptStringAES(did, rs);
            int id = Convert.ToInt32(ddid);
            var drug = await db.ProductTb.FindAsync(id);
            return View(drug);
        }
    }
}