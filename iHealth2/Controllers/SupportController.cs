using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using iHealth2.Models;
using iHealth2.CustomClasses;
using System.Net;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Ganss.XSS;
namespace iHealth2.Controllers
{
    public class SupportController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        Emails em = new Emails();
        EmailSender ems = new EmailSender();
        HtmlSanitizer sanitizer = new HtmlSanitizer();
        private ApplicationUserManager _userManager;
          public SupportController()
        {

        }
          public SupportController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ActionResult ContactUs()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ContactUs(FeedBackViewModel fbVm, FormCollection f)
        {
            FeedBack fb = new FeedBack();
            var response = Request["g-recaptcha-response"];
            //const string secret = "6LeP9hETAAAAAPhx2dmOL4eB6euPe_hLvOw1UaBH";
            string secret = System.Configuration.ConfigurationManager.AppSettings["recaptchaPrivateKey"];
            var client = new WebClient();
            var reply =
            client.DownloadString(
             string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
            var captchaResponse = JsonConvert.DeserializeObject<reCaptchaClass>(reply);
            if (ModelState.IsValid)
            {
                if (!captchaResponse.Success)
                {
                    if (captchaResponse.ErrorCodes.Count <= 0) return View();

                    var error = captchaResponse.ErrorCodes[0].ToLower();
                    switch (error)
                    {
                        case ("missing-input-secret"):
                            ViewBag.Message = "The secret parameter is missing.";
                            break;
                        case ("invalid-input-secret"):
                            ViewBag.Message = "The secret parameter is invalid or malformed.";
                            break;

                        case ("missing-input-response"):
                            ViewBag.Message = "The response parameter is missing.";
                            break;
                        case ("invalid-input-response"):
                            ViewBag.Message = "The response parameter is invalid or malformed.";
                            break;

                        default:
                            ViewBag.Message = "Error occured. Please try again";
                            break;
                    }

                    ModelState.AddModelError("", "You did not type the verification word correctly. Please try again.");
                    return View();
                }
                else
                {
                    fb.FRName = fbVm.FRName;
                    fb.FREmail = fbVm.FREmail;
                    fb.FRPhone = fbVm.FRPhone;
                    fb.FBMessage = f["FBMessage"];
                    fb.FBStatus = "Fresh";
                    fb.SentDate = DateTime.UtcNow;
                    fb.FBType = "ContactUs";
                    fb.subject = fbVm.subject;
                    db.Feedback.Add(fb);
                    await db.SaveChangesAsync();
                    await em.NotifyAdminAbtFB(fbVm.FRName, fbVm.FREmail, fbVm.FBMessage,fb.subject, "Contact Us");

                    TempData["success"] = "Message sent successfully";
                    TempData["warning"] = "We will get back to you shortly, Thank you!";

                    return RedirectToAction("index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "All fields with (*) must be fill");
                return View();
            }

        }
        public ActionResult ReportFraud()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ReportFraud(FeedBackViewModel fbVm, FormCollection f)
        {
            FeedBack fb = new FeedBack();
            var response = Request["g-recaptcha-response"];
            //const string secret = "6LeP9hETAAAAAPhx2dmOL4eB6euPe_hLvOw1UaBH";
            string secret = System.Configuration.ConfigurationManager.AppSettings["recaptchaPrivateKey"];
            var client = new WebClient();
            var reply =
         client.DownloadString(
         string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
            var captchaResponse = JsonConvert.DeserializeObject<reCaptchaClass>(reply);
            if (ModelState.IsValid)
            {
                if (!captchaResponse.Success)
                {
                    if (captchaResponse.ErrorCodes.Count <= 0) return View();

                    var error = captchaResponse.ErrorCodes[0].ToLower();
                    switch (error)
                    {
                        case ("missing-input-secret"):
                            ViewBag.Message = "The secret parameter is missing.";
                            break;
                        case ("invalid-input-secret"):
                            ViewBag.Message = "The secret parameter is invalid or malformed.";
                            break;

                        case ("missing-input-response"):
                            ViewBag.Message = "The response parameter is missing.";
                            break;
                        case ("invalid-input-response"):
                            ViewBag.Message = "The response parameter is invalid or malformed.";
                            break;

                        default:
                            ViewBag.Message = "Error occured. Please try again";
                            break;
                    }

                    ModelState.AddModelError("", "You did not type the verification word correctly. Please try again.");
                    return View();
                }
                else
                {
                    fb.FRName = fbVm.FRName;
                    fb.FREmail = fbVm.FREmail;
                    fb.FRPhone = fbVm.FRPhone;
                    fb.FBMessage = f["FBMessage"];
                    fb.FBStatus = "Fresh";
                    fb.SentDate = DateTime.UtcNow;
                    fb.FBType = "ReportFraud";
                    fb.ReportType = f["ReportType"];
                    fb.subject = fbVm.subject;
                    db.Feedback.Add(fb);
                    await db.SaveChangesAsync();
                    await em.NotifyAdminAbtFB(fbVm.FRName, fbVm.FREmail, fbVm.FBMessage,fbVm.subject, "Report Fraud");
                    TempData["success"] = "Message sent successfully";
                    TempData["warning"] = "We will get back to you shortly, Thank you!";
                    return RedirectToAction("index", "home");
                }
            }
            else
            {
                ModelState.AddModelError("", "All fields with (*) must be fill");
                return View();
            }

        }
        [Authorize]
        [HttpGet]
        public ActionResult HelpDesk()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken][ValidateInput(false)]
        public async Task<ActionResult> HelpDesk(string hlpids, FormCollection f)
            {
                Random rd = new Random();
                int TicketNo = rd.Next(10000);
                string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
                hlpids = hlpids.Replace("$25", "/");
                hlpids = hlpids.Replace("$24", "+");
                string DecryptId = Cryptoclass.DecryptStringAES(hlpids, s);
                var usr = await UserManager.FindByNameAsync(DecryptId);
                string Name = usr.FirstName + " " + usr.LastName;
                Support suprt = new Support();
                suprt.Subject = f["subject"];
                suprt.Content = Server.HtmlEncode(sanitizer.Sanitize(f["Message"],"",null));
                suprt.Support_createdDate = DateTime.Now;
                suprt.Support_Requestor_Name = Name;
                suprt.Support_Requestor_Email = usr.Email;
                suprt.support_status = "Fresh";
                suprt.TicketId = TicketNo;
                db.supports.Add(suprt);
                await  db.SaveChangesAsync();

                string body = em.Notification_Email_Body_Creator("Admin", Name +
                     ", with the following email address <a href=\"mailto:" + usr.Email + "\">"
                     + usr.Email + "</a> just requested for support on our platform. Below is his/her message content: <br/><br/>" +
                     "<div style=\"text-align:justify; border-left: 6px outset #0094ff;background-color: #00ffff; padding-left:5px; padding-right:5px\">" + f["Message"]
                     + "</div><br/><br/> Kindly, attend to him/her ASAP.");
                await ems.SubcribersMailSender(mailFrom: usr.Email, subject: f["subject"], Body: body, senderName:Name);
                TempData["success"] = "Message sent successfully, our team will get back to you via email shortly.";
                return Redirect(Request.UrlReferrer.ToString());
            }
        [Authorize(Roles="Admin")]
        public ActionResult View_Support_Request(string type)
        {
            Session["type"] = type;
            if (type == "F")
            {
                return View(db.supports.Where(s => s.support_status == "Fresh" || s.support_status == "Pending").OrderBy(s => s.Support_createdDate).ToList());
            }
            else
            {
                return View(db.supports.Where(s => s.support_status == "Processed").OrderBy(s => s.Support_createdDate).ToList());
            }
           
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Process_Support_Request(int id)
        {
            return View(db.supports.Find(id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Authorize(Roles="Admin")]
        public async Task<ActionResult> Process_Support_Request(Support model,FormCollection f)
        {
            Support s = await db.supports.FindAsync(model.Id);
            s.support_status = "Processed";
            s.Support_Reply_Text_From_Admin = Server.HtmlEncode(sanitizer.Sanitize(f["Message"]));
            s.Support_ProcessedDate = DateTime.Now;
            s.Support_Processed_By_Name = Session["Name"].ToString();
            await  db.SaveChangesAsync();
            string type = Session["type"].ToString();
            // send reply email to the client who requested support
            string subject = "[Ticket ID: " + s.TicketId + "] RE: " + s.Subject;
            string body = "<div style=\"text-align:justify; border-left: 6px outset #0094ff;background-color: #00ffff; padding-left:5px; padding-right:5px\">" + f["Message"]
                + "</div>";
            await ems.AdminMailSender(mailto: s.Support_Requestor_Email, Subject: subject, Body:body);
            TempData["success"] = "Message sent successfully!";
            return RedirectToAction("View_Support_Request", new { type= type});
        }
        [Authorize]
        public async Task<ActionResult>Delete_Support_Request(int id)
        {
            Support s = await db.supports.FindAsync(id);
            db.supports.Remove(s);
            await  db.SaveChangesAsync();
         
            TempData["success"] = "Support Request successfully deleted!";
            return Redirect(Request.UrlReferrer.ToString());

        }
    }
}