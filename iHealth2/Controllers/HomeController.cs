using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hangfire;
using iHealth2.CustomClasses;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
//using Microsoft.Owin;
using System.Net;
using iHealth2.Models;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using IhealthPaging;
namespace iHealth2.Controllers
{
    public class HomeController : Controller
    {

        //  private ApplicationUserManager _userManager;
        ApplicationDbContext db = new ApplicationDbContext();
        Emails em = new Emails();
        EmailSender ems = new EmailSender();
        ImgResize imgR = new ImgResize();
        private const int DefaultPageSize = 10;
        public ActionResult Index()
        {
            //Emails em = new Emails();
            //string url = Url.Action("index", "Admin", null, protocol: Request.Url.Scheme);
            //RecurringJob.AddOrUpdate(() => em.NewRegisterUserMailToAdmin(url), Cron.Daily()); //job Schedullers

            country c = new country();
            Category cat = new Category();
            ViewBag.category = new SelectList(db.category, "ID", "CatName", cat.ID);
            ViewBag.Country = new SelectList(db.country, "ID", "CountryName", c.ID);

            return View();
        }
        public JsonResult getState(int Id)
        {
            var st = from data in db.State
                     where data.CountryID == Id
                     select data;
            List<SelectListItem> State = new List<SelectListItem>();
            if (st != null)
            {
                foreach (var s in st)
                {
                    State.Add(new SelectListItem() { Text = s.StateName, Value = s.ID.ToString() });
                }
            }

            return Json(new SelectList(State, "Value", "Text"));
        }
        public JsonResult getSubcat1(int catId)
        {
            var sub1 = from data in db.SubCategory1
                       where data.CategoryID == catId
                       select data;
            List<SelectListItem> subcat1 = new List<SelectListItem>();

            foreach (var sb1 in sub1)
            {
                subcat1.Add(new SelectListItem() { Value = sb1.ID.ToString(), Text = sb1.SubCat1Name });
            }
            return Json(new SelectList(subcat1, "Value", "Text"));


        }
        public JsonResult getSubcat2(int subcat1Id)
        {
            var sub2 = from data in db.SubCategory2
                       where data.SubCategory1ID == subcat1Id
                       select data;
            List<SelectListItem> subcat2 = new List<SelectListItem>();
            foreach (var sb2 in sub2)
            {
                subcat2.Add(new SelectListItem { Value = sb2.ID.ToString(), Text = sb2.SubCat2name });
            }
            return Json(new SelectList(subcat2, "Value", "Text"));
        }
        public JsonResult getSubPro1(int catId)
        {
            var catgory = db.category.First(ct => ct.CatName == "Medical Doctor");
            var sub1 = from data in db.SubCategory1
                       where data.CategoryID == catgory.ID
                       select data;
            List<SelectListItem> subcat1 = new List<SelectListItem>();

            foreach (var sb1 in sub1)
            {
                subcat1.Add(new SelectListItem() { Value = sb1.ID.ToString(), Text = sb1.SubCat1Name });
            }
            return Json(new SelectList(subcat1, "Value", "Text"));


        }
       [HttpPost]
        public JsonResult CheckSponsor(int RefCode)
        {
            try
            {
                var sponsor = db.Users.First(u => u.MyRefferalCode == RefCode);
                if (sponsor.UserType == "INISubcriber")
                {
                    return Json(sponsor.FullName, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Sponsor is not an iHealth Networking Group Member.", JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json("This referral code does not exist", JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult FAQ()
        {
            ViewBag.div = "<div style=\"text-align:justify\"><h2 style=\"color: #2f96b4;\" >WELCOME TO IHEALTH NIGERIA GSFM <h2/>" +
                                "<p style=\"font-size:large\">Thanks for registering with us @iHealth Nigeria GSFM.</p> "
                             + "<p  style=\" font-size: large\">Please kindly confirm your account by clicking on this <a href=\"http://facebook.com\">link</a> within 24hours. <br/><br/> Best Regards, <br/><br/>iHealth Nigeria GSFM Team.</p></div>";
            return View();
        }
        public ActionResult Terms()
        {
            return View();

        }
        public ActionResult PrivacyPolicy()
        {
            return View();
        }
        [Authorize]
        public ActionResult Photoupload()
        {
            var ReturnUrl = Request.UrlReferrer;
            Session["ReturnUrl"] = ReturnUrl;
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PhotoUpload(HttpPostedFileBase ProfileImg)
        {
            var userManager = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            // var name = Session["Name"]; 
            if (Session["Id"] != null)
            {
                var usr = userManager.FindById(Session["Id"].ToString());
                if (usr == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (ProfileImg != null)
                {
                    byte[] img;                  
                    System.Drawing.Image maxImgsze = System.Drawing.Image.FromStream(ProfileImg.InputStream, true, true);
                    decimal size = Math.Round(ProfileImg.ContentLength / (decimal)1024, 2);
                    if (size <= 1000) { 
                    img = imgR.imageToByteArray(maxImgsze, maxImgsze.RawFormat, 180, 180);
                    usr.Photo = img;
                    }
                    else
                    {
                        TempData["error"] = "Uploaded Image size exceed 1000kb(i.e.1MB)!";
                        return View();
                    }                   
                    await userManager.UpdateAsync(usr);
                    return Redirect(Session["ReturnUrl"].ToString());

                }
            }
            else
            {
                return RedirectToAction("login", "Account");
            }
            return View();

        }
        [Authorize]
        public async Task<ActionResult> subnewsletter(string sbn)
        {
            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            sbn = sbn.Replace("$25", "/");
            sbn = sbn.Replace("$24", "+");
            string DecryptId = Cryptoclass.DecryptStringAES(sbn, s);
            var usr = db.Users.Find(DecryptId);
            try
            {
               
                    usr.newsletterStatus = 1;
                    await db.SaveChangesAsync();
                    TempData["msg"] = "You have successfully subscribed to our Newsletter.";
                    return View("newsletter");
              
               
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
                return View();
            }

        }
        [Authorize]
        public async Task<ActionResult> unsubnewsletter(string sbn)
        {
            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            sbn = sbn.Replace("$25", "/");
            sbn = sbn.Replace("$24", "+");
            string DecryptId = Cryptoclass.DecryptStringAES(sbn, s);
            var usr = db.Users.Find(DecryptId);
            try
            {
               
                    usr.newsletterStatus = 0;
                    await db.SaveChangesAsync();
                    TempData["msg"] = "You have successfully unsubscribe to our Newsletter.";
                    return View("newsletter");
           

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
                return View();
            }

        }
        public ActionResult bookappointmentPage()
        {
            return View();
        }
        public async Task<ActionResult> bookapp(appBooking_ViewModel model,FormCollection f)
        {
            if (ModelState.IsValid)
            {
                if(!string.IsNullOrEmpty(f["appwith"]))
                {
                    Random rd = new Random();
                    int rdn = rd.Next(9999999);
                    AppBookingTb appbk = new AppBookingTb();
                    appbk.Name = model.Name;
                    appbk.app_id = rdn;
                    appbk.Email = model.Email;
                    appbk.Phone = model.Phone;
                    appbk.Message = model.Message;
                    appbk.Appointment_With = f["appwith"];
                    string appdate = f["appdate"];
                    appbk.state = f["state"];
                    appbk.city = f["city"];
                    appbk.notifyStatus = 0;
                    appbk.Appointment_Date = DateTime.ParseExact(appdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    db.AppBookingTb.Add(appbk);
                    await db.SaveChangesAsync();
                    //TODO: send appointment information to both admin and  appnt requestor via email.
                    string body = "<p>Your request to have an appointment with a "+appbk.Appointment_With+""+
                    " has been received and you will be communicated once approved."+
                    "<br/>Thank you for choosing iHealth Nigeria GSFM. <br/></br> Best Regards, <br/></br> iHealth Nigeria GSFM Team.</p>";
                    await em.appointmentBookingNotification(model.Name, body, "Appointment Received",model.Email);
                    TempData["success"] = "Appointment has been created successfully!";
                    return View("Index");
                }
                {
                    ModelState.AddModelError("", "Select who you want to have appointment with.");
                    return View("Index");
                }
            }          
            else
            {
                ModelState.AddModelError("", "All field mark with (*) be fill.");
                return View("Index");
            }
        }
        public ActionResult Hmos(int? page)
        {
           int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
           var hmos = db.BusinessInfoes.Where(b => b.Category.Contains("HMOs") && b.ApprovedStatus == "A" && b.NotifyStatus == 1).OrderByDescending(b => b.VerifiedStatus).ToPagedList(currentPageIndex, DefaultPageSize);
            return View(hmos);
           
        }
        public ActionResult NGOs(int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            var hmos = db.BusinessInfoes.Where(b => b.Category.Contains("NGO") && b.ApprovedStatus == "A" && b.NotifyStatus == 1).OrderByDescending(b => b.VerifiedStatus).ToPagedList(currentPageIndex, DefaultPageSize);
            return View(hmos);

        }
        public PartialViewResult search_hmos_by_state(int? page, string state)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            object Model = null;
            Model = db.BusinessInfoes.Where(b => b.State.Contains(state) && b.ApprovedStatus == "A" && b.NotifyStatus == 1 && b.Category == "HMOs").OrderByDescending(b => b.VerifiedStatus).ToPagedList(currentPageIndex, DefaultPageSize);
            return PartialView("_hmospv", Model);
        }
        public PartialViewResult search_ngos_by_state(int? page, string state)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            object Model = null;
            Model = db.BusinessInfoes.Where(b => b.State.Contains(state) && b.ApprovedStatus == "A" && b.NotifyStatus == 1 && b.Category == "NGO").OrderByDescending(b => b.VerifiedStatus).ToPagedList(currentPageIndex, DefaultPageSize);
            return PartialView("_ngospv", Model);
        }
        public ActionResult Comingsoon(string type) //Service and Seller Agreement
        {
            ViewBag.type = type;
            return View();
        }
        public ActionResult sample_page()
        {
            return View();
        }

        //update the promo status of ING subscribers whose promo date is less than today.
        public async Task<JsonResult> UpdateINGsubscriberPromoStatus()
        {
           try {
                var INGMembers = db.Users.Where(u => u.UserType == "INISubcriber" && u.INISubcriberExtraDetails.Promotional_Target_Subscription_Status == "Active").ToList();
                if (INGMembers.Count() > 0)
                {
                    foreach (var ActiveMembers in INGMembers)
                    {
                        if (ActiveMembers.INISubcriberExtraDetails.Promo_end_date <= DateTime.UtcNow.Date)
                        {
                            var us = await db.INISubcriberExtraDetails.FindAsync(ActiveMembers.Id);
                            us.Promotional_Target_Subscription_Status = "Expired";
                            await db.SaveChangesAsync();
                        }
                    }
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("not found", JsonRequestBehavior.AllowGet);
                }
            } catch(Exception ex)
            {
                return Json("error occured: "+ ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
      
    }
}
