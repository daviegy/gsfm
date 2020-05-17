using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iHealth2.Models;
using System.Data.Entity;
using iHealth2.CustomClasses;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
using System.Diagnostics;
namespace iHealth2.Controllers.Admin
{
   
    public class CompanyController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        Emails em = new Emails();
        // GET: Company
        public ActionResult Index()
        {
            return View();
        }
        // view newly registered Company
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult NewlyRegBusiness()
        {
            var com = db.BusinessInfoes.Where(b => b.ApprovedStatus == "P" || b.NotifyStatus == 0).Include(b => b.User).OrderByDescending(u => u.regDate);
            if (com.Count() != 0)
            {
                 return View(com); 
            }
            else
            {
                //TempData["error"] = "Alert: No New Registered Company";
                return RedirectToAction("index", "Admin");
            }


        }

        //View a All the details about a Company
         [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult> BizDetails(string id)
        {

            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            id = id.Replace("$25", "/");
            id = id.Replace("$24", "+");
            int refID = Convert.ToInt32(Cryptoclass.DecryptStringAES(id, s));
            BusinessInfo b = await db.BusinessInfoes.FindAsync(refID);
            return View(b);
        }
        //Approve a Company or business
         [HttpPost]
         [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult> Approve(IEnumerable<int> Id)
        {      
            try
            {
                if (Id != null)
                {
                    if (Id.Count() > 0)
                    {
                        foreach (int id in Id)
                        {

                            BusinessInfo b = await db.BusinessInfoes.FirstAsync(r => r.ID == id);
                            b.ApprovedStatus = "A";
                            b.NotifyStatus = 1;
                            b.ApprovedDate = DateTime.UtcNow;
                            b.SignedBy = Session["Name"].ToString();
                            await db.SaveChangesAsync();
                            //send Approval to mail to the business owner.
                            await em.SendBizApprovedMail(b.User.Email, b.businessName,b.User.FirstName);

                        }

                    }
                }

                return Json(true, JsonRequestBehavior.AllowGet);
            }

            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
                return Json(false, JsonRequestBehavior.AllowGet);
            }
          
        }
     
        //Delete a company or business
        [HttpPost]
        [Authorize(Roles = "Admin,Users,INIsubscribers,SuperAdmin")]
        public async Task<JsonResult> Delete(IEnumerable<int> Id)
        {
            try
            {
                if (Id != null)
                {
                    if (Id.Count() > 0)
                    {
                        foreach (int id in Id)
                        {
                            BusinessInfo b = await db.BusinessInfoes.FirstAsync(r => r.ID == id);
                            db.BusinessInfoes.Remove(b);
                            await db.SaveChangesAsync();
                        }
                    }
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            

        }
        //view list of Approved Company to be displayed for verification
         [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult ApprovedComp()
        {
            var bis = db.BusinessInfoes.Where(b => b.ApprovedStatus == "A" &&  b.NotifyStatus == 1 && b.VerifiedStatus == "NV").ToList().OrderByDescending(b=>b.regDate);
            if (bis.Count() > 0)
            {
                return View(bis);
            }
            else
            {
                   return RedirectToAction("index", "Admin");
               
            }
           
        }
        //Verify company
         [HttpPost]
         [Authorize(Roles = "Admin,SuperAdmin")]
        public  async Task<JsonResult> VerifyCom(IEnumerable<int> Id)
        {
            try
            {
                if (Id != null)
                {
                    if (Id.Count() > 0)
                    {
                        foreach (var id in Id)
                        {
                            BusinessInfo b = await db.BusinessInfoes.FirstAsync(r => r.ID == id);
                            b.VerifiedStatus = "V";
                            b.VerifyDate = DateTime.UtcNow;
                            b.SignedBy = Session["Name"].ToString();
                            await db.SaveChangesAsync();
                            //send Verification mail to the business owner.
                            await em.SendBizVerifyMail(b.User.Email, b.businessName,b.User.FirstName);
                        }

                    }
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        //View list of already  verified busineses
         [HttpGet]
         [Authorize(Roles = "Admin,SuperAdmin")]
         public ActionResult VerifiedComp ()
        {
            var bis = db.BusinessInfoes.Where( b => b.ApprovedStatus == "A" && b.NotifyStatus == 1 && b.VerifiedStatus == "V").ToList().OrderByDescending(b => b.VerifyDate);
            if (bis.Count() > 0)    
            {
                return View(bis);
            }
            else
            {
                //TempData["error"] = "Alert: No company has been verified yet";
                return RedirectToAction("index", "Admin");

            }
        }
         [HttpPost]
         [Authorize(Roles = "Admin,SuperAdmin")]
         public async Task<ActionResult> Mark_as_recommended(IEnumerable<int> Id)
         {
             try
             {
                 if (Id != null)
                 {
                     if (Id.Count() > 0)
                     {
                         foreach (var id in Id)
                         {
                             BusinessInfo b = await db.BusinessInfoes.FirstAsync(r => r.ID == id);
                             b.Recommended_Status = "R";
                             b.RecommendedDate = DateTime.UtcNow;
                             b.SignedBy = Session["Name"].ToString();
                             await db.SaveChangesAsync();
                             //send Verification mail to the business owner.
                             await em.SendBizRecommendedMail(b.User.Email, b.businessName, b.User.FirstName);
                         }

                     }
                 }
                 return Json(true, JsonRequestBehavior.AllowGet);
             }
             catch (DbEntityValidationException dbEx)
             {
                 foreach (var validationErrors in dbEx.EntityValidationErrors)
                 {
                     foreach (var validationError in validationErrors.ValidationErrors)
                     {
                         Trace.TraceInformation("Property: {0} Error: {1}",
                                                 validationError.PropertyName,
                                                 validationError.ErrorMessage);
                     }
                 }
                 return Json(false, JsonRequestBehavior.AllowGet);
             }
         }
         //View list of already  recommended busineses
         [HttpGet]
         [Authorize(Roles = "Admin,SuperAdmin")]
         public ActionResult Recommended_Comp()
         {
             var bis = db.BusinessInfoes.Where(b => b.ApprovedStatus == "A" && b.NotifyStatus == 1 && b.VerifiedStatus == "V"&& b.Recommended_Status =="R").ToList().OrderByDescending(b => b.RecommendedDate);
             if (bis.Count() > 0)
             {
                 return View(bis);
             }
             else
             {
                 //TempData["error"] = "Alert: No company has been verified yet";
                 return RedirectToAction("index", "Admin");

             }
         }
    }
}