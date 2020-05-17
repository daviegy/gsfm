using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iHealth2.Models;
using Newtonsoft.Json;
using IhealthPaging;

namespace iHealth2.Controllers.Others
{
    public class HospitalController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private const int DefaultPageSize = 10;
        public ActionResult Find_Hospital(int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            var bs = db.BusinessInfoes.Where(b => b.Category.Contains("Hospital") && b.ApprovedStatus == "A" && b.NotifyStatus == 1).OrderByDescending(b =>b.Recommended_Status).ThenByDescending(b=>b.VerifiedStatus).ToPagedList(currentPageIndex, DefaultPageSize);
           return View(bs);
        }
     
        public PartialViewResult find_Hosp(int? page,string sbValue)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            object Model = null;
            Model = db.BusinessInfoes.Where(b => b.subCategory1.Contains(sbValue) && b.ApprovedStatus == "A" && b.NotifyStatus == 1 && b.Category == "Hospital or Clicnic").OrderByDescending(b => b.Recommended_Status).ThenByDescending(b => b.VerifiedStatus).ToPagedList(currentPageIndex, DefaultPageSize);        
            return PartialView("_listHospitalOrClinic",Model);    
        }
        public PartialViewResult search_by_state(int? page, string state)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            object Model = null;
            Model = db.BusinessInfoes.Where(b => b.State.Contains(state) && b.ApprovedStatus == "A" && b.NotifyStatus == 1 && b.Category == "Hospital or Clicnic").OrderByDescending(b => b.Recommended_Status).ThenByDescending(b => b.VerifiedStatus).ToPagedList(currentPageIndex, DefaultPageSize);
            return PartialView("_listHospitalOrClinic", Model);
        }       
     
    }
}