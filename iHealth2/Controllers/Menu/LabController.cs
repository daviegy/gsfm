using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iHealth2.Models;
using IhealthPaging;
namespace iHealth2.Controllers.Menu
{
    public class LabController : Controller
    {
        private const int DefaultPageSize = 10;
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Lab
        public ActionResult Find_Lab(int?page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            var bs = db.BusinessInfoes.Where(b => b.Category.Contains("Medical Laboratories")
                && b.ApprovedStatus == "A" && b.NotifyStatus == 1)
                .OrderByDescending(b => b.Recommended_Status).ThenByDescending(b => b.VerifiedStatus)
                 .ToPagedList(currentPageIndex, DefaultPageSize); 
            return View(bs);
        }
        public PartialViewResult search_by_state(int?page,string state)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            object Model = null;
            Model = db.BusinessInfoes.Where(b => b.State.Contains(state)
                && b.ApprovedStatus == "A" && b.NotifyStatus == 1 && 
                b.Category == "Medical Laboratories")
               .OrderByDescending(b => b.Recommended_Status).ThenByDescending(b => b.VerifiedStatus)
                  .ToPagedList(currentPageIndex, DefaultPageSize); 
            return PartialView("_listMedLab", Model);
        }
    }
}