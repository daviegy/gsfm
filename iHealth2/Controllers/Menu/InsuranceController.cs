using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iHealth2.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using iHealth2.CustomClasses;
using System.Threading.Tasks;
using System.Net.Mail;
using Elmah;
using IhealthPaging;

namespace iHealth2.Controllers.Menu
{
    public class InsuranceController : Controller
    {
        Emails em = new Emails();
        private const int DefaultPageSize = 10;

        //string MailFrom = "ihealthng@tamife2016.com";
        //ihealth_constants hc = new ihealth_constants();
        ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        // GET: Insurance
        public InsuranceController()
        {

        }
        public InsuranceController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
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
        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }
        public ActionResult Index()
        {
            return View();
        }

        //ING Management
        [AllowAnonymous]
        [HttpGet]
        //public ActionResult INIRegForm(string sp)
        //{
        //    // Check if the Client requesting this page is been Sponsor by an already Existing Site User
        //    if (sp != null)
        //    {
        //        //Session["sp"] = sp;
        //        var sponsor = db.Users.Where(u => u.UserName == sp).Single();
        //        Session["spName"] = sponsor.FullName;
        //        Session["spCode"] = sponsor.MyRefferalCode;

        //    }
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AllowAnonymous] // Ihealth Form Submission Method

        //public async Task<ActionResult> INIRegform(INIRegFormModel model, FormCollection f)
        //{
        //    try
        //    {
        //        Random rd = new Random();
        //        int trackId = rd.Next(10000000);
        //        // PaymentGateways pg = new PaymentGateways();
        //        var response = Request["g-recaptcha-response"];
        //        // const string secret = "6LeP9hETAAAAAPhx2dmOL4eB6euPe_hLvOw1UaBH";
        //        string secret = System.Configuration.ConfigurationManager.AppSettings["recaptchaPrivateKey"];
        //        var client = new WebClient();
        //        var reply =
        //        client.DownloadString(
        //        string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
        //        var captchaResponse = JsonConvert.DeserializeObject<reCaptchaClass>(reply);

        //        StringBuilder sb = new StringBuilder();
        //        if (ModelState.IsValid)
        //        {
        //            //int profVal = Convert.ToInt32(f["Profession"]);
        //            //var prof = await db.Profession.FindAsync(profVal);
        //            if (!captchaResponse.Success)
        //            {
        //                if (captchaResponse.ErrorCodes.Count <= 0) return View();

        //                var error = captchaResponse.ErrorCodes[0].ToLower();
        //                switch (error)
        //                {
        //                    case ("missing-input-secret"):
        //                        ViewBag.Message = "The secret parameter is missing.";
        //                        break;
        //                    case ("invalid-input-secret"):
        //                        ViewBag.Message = "The secret parameter is invalid or malformed.";
        //                        break;

        //                    case ("missing-input-response"):
        //                        ViewBag.Message = "The response parameter is missing.";
        //                        break;
        //                    case ("invalid-input-response"):
        //                        ViewBag.Message = "The response parameter is invalid or malformed.";
        //                        break;

        //                    default:
        //                        ViewBag.Message = "Error occured. Please try again";
        //                        break;
        //                }
        //                ModelState.AddModelError("", "You did not type the verification word correctly. Please try again.");

        //            }
        //            else
        //            {
        //                string Username = model.Username;
        //                //the REFURL stores the newly registering User Unique URL which he or She can Share with Others to 
        //                // register on Ihealth Networking Insurance
        //                var RefUrl = this.Url.Action("INIRegform", "Insurance", new { sp = Username }, this.Request.Url.Scheme);
        //                var con3ID = Convert.ToInt32(f["Country"]);
        //                var con3 = await db.country.FindAsync(con3ID);
        //                var st_Id = Convert.ToInt32(f["State"]);
        //                var st = await db.State.FindAsync(st_Id);
        //                INISubcriberExtraDetail iniSubExtDetails = new INISubcriberExtraDetail();
        //                var user = new ApplicationUser
        //                {
        //                    UserName = model.Username,
        //                    Email = model.Email,
        //                    FirstName = model.FirstName,
        //                    LastName = model.LastName,
        //                    PhoneNumber = model.PhoneNumber,
        //                    RegDate = DateTime.UtcNow,
        //                    MyRefferalCode = trackId,
        //                    NotifyStatus = 0,
        //                    Gender = model.Gender,
        //                    Nationality = con3.CountryName,
        //                    State = st.StateName,
        //                    Refferal_Url = RefUrl,
        //                    UserType = "INISubcriber",
        //                    subscriptionType = "Freemium",
        //                    newsletterStatus = 1
        //                };
        //                var result = await UserManager.CreateAsync(user, model.Password);
        //                if (result.Succeeded)
        //                {
        //                    if (f["PaymentOptions"] == "PayToBank")
        //                    {
        //                        iniSubExtDetails.UserID = user.Id;
        //                        iniSubExtDetails.transaction_Id = string.Concat("ING-", rd.Next(999999));
        //                        //Payment status is set to pending until the user payment has been confirm from banks.
        //                        iniSubExtDetails.PaymentStatus = "Pending";
        //                        iniSubExtDetails.INI_Status = "InActive";
        //                        iniSubExtDetails.BEstatus = "InActive";
        //                        iniSubExtDetails.paymentMethod = f["PaymentOptions"];
        //                        if (f["RefCode"] != "")
        //                        {
        //                            int SpCode = Convert.ToInt32(f["RefCode"]);
        //                            //Retrieve sponsor Tracking ID
        //                            var Sponsor = db.Users.First(u => u.MyRefferalCode == SpCode);
        //                            iniSubExtDetails.SSponsorCode = SpCode.ToString();                                    //check if sponsor already exist in d iniSubExtDetails tb, so dt we can give him credit
        //                            var sp_IN_mlm = db.INISubcriberExtraDetails.Where(sp => sp.User.Id == Sponsor.Id);
        //                            if (sp_IN_mlm.Count() > 0)
        //                            {
        //                                try
        //                                {
        //                                    INISubcriberExtraDetail spIniSubExtDetails = db.INISubcriberExtraDetails.First(sp => sp.User.Id == Sponsor.Id);
        //                                    spIniSubExtDetails.SDowlineSize = spIniSubExtDetails.SDowlineSize + 1;
        //                                    spIniSubExtDetails.DIncrementDate = DateTime.UtcNow.Date;
        //                                    await db.SaveChangesAsync();
        //                                }
        //                                catch (Exception ex)
        //                                {
        //                                    throw ex;
        //                                }
        //                            }
        //                        }

        //                        if (f["BenefitCategory"] == "CarreerBenefit")
        //                        {
        //                            iniSubExtDetails.BenefitCategory = "CarreerBenefit";
        //                            //    iniSubExtDetails.MaxDT2MtTarget = DateTime.UtcNow.AddDays(90);
        //                        }
        //                        else
        //                        {
        //                            iniSubExtDetails.BenefitCategory = f["BenefitCategory"];
        //                            iniSubExtDetails.CurrentStage = "Starter";
        //                            iniSubExtDetails.Stage1Benefit = "No Benefit";
        //                            iniSubExtDetails.Stage2Benefit = "No Benefit";
        //                            iniSubExtDetails.Stage3Benefit = "No Benefit";
        //                            iniSubExtDetails.Stage4Benefit = "No Benefit";
        //                            iniSubExtDetails.Stage5Benefit = "No Benefit";
        //                        }
        //                        iniSubExtDetails.CurrentBenefit = "No Benefit";
        //                        iniSubExtDetails.SDowlineSize = 0;
        //                        db.INISubcriberExtraDetails.Add(iniSubExtDetails);
        //                        await db.SaveChangesAsync();
        //                        var UserRole = UserManager.GetRoles(user.Id);
        //                        if (!UserRole.Contains("INIsubscribers"))
        //                        {
        //                            var addToRole = UserManager.AddToRole(user.Id, "INIsubscribers");
        //                        }
        //                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //                        await UserManager.SendEmailAsync(user.Id,
        //                         "Ihealth Networking Group", em.Notification_Email_Body_Creator(user.FullName, "<div><h2>WELCOME TO IHEALTH NETWORKIING GROUP<h2/>" +
        //                         " <p style=\" font-size: large\">Thanks for signing up for IHEALTH NETWORKING GROUP.</p> "
        //                      + "<p  style=\" font-size: large\">But you are not yet activated, Please kindly pay using the information below:-<br/>" +
        //                      "ACCOUNT NUMBER: " + acctno + "<br/>ACCOUNT NAME:" + AcctName + "<br/> BANK:" + bankName + " <br/>"
        //                      + "</p><p>After payment please forward the following information to us via Email <br/> (a) Name of Depositor <br/>" +
        //                   "(b)Teller Number <br/>(c)Your Membership ID : " + user.MyRefferalCode + " <br/> (d) Amount <br/> (e) Date of Payment</p><br/> Thanks for registering with us<br/><br/>Best Regards, <br/><br/> iHealth Nigeria GSFM Team.</div>"));
        //                        ViewBag.Message = sb.Append("<span style=\"color:green\">Awesome !!!</span> You have Successfully <br/>SIGN UP for <b>iHEALTH NETWORKING <br/>  GROUP</b>.kindly," +
        //                      " Check your <span style=\"color:Red\">email inbox OR <br/> spam folder</span> for more information ").ToString();
        //                        return View("Info");
        //                    }
        //                    else
        //                    {
        //                        iniSubExtDetails.UserID = user.Id;
        //                        iniSubExtDetails.transaction_Id = string.Concat("ING-", rd.Next(999999));
        //                        //Payment status is set to pending until the user payment has been confirm from banks.
        //                        iniSubExtDetails.PaymentStatus = "Pending";
        //                        iniSubExtDetails.INI_Status = "InActive";
        //                        iniSubExtDetails.BEstatus = "InActive";
        //                        iniSubExtDetails.paymentMethod = f["PaymentOptions"];
        //                        iniSubExtDetails.paymentGateway = "Interswitch";
        //                        if (f["RefCode"] != "")
        //                        {
        //                            int SpCode = Convert.ToInt32(f["RefCode"]);
        //                            //Retrieve sponsor Tracking ID
        //                            var Sponsor = db.Users.First(u => u.MyRefferalCode == SpCode);
        //                            iniSubExtDetails.SSponsorCode = SpCode.ToString();                                    //check if sponsor already exist in d iniSubExtDetails tb, so dt we can give him credit
        //                            var sp_IN_mlm = db.INISubcriberExtraDetails.Where(sp => sp.User.Id == Sponsor.Id);
        //                            if (sp_IN_mlm.Count() > 0)
        //                            {
        //                                try
        //                                {
        //                                    INISubcriberExtraDetail spIniSubExtDetails = db.INISubcriberExtraDetails.First(sp => sp.User.Id == Sponsor.Id);
        //                                    spIniSubExtDetails.SDowlineSize = spIniSubExtDetails.SDowlineSize + 1;
        //                                    spIniSubExtDetails.DIncrementDate = DateTime.UtcNow.Date;
        //                                    await db.SaveChangesAsync();
        //                                }
        //                                catch (Exception ex)
        //                                {
        //                                    throw ex;
        //                                }
        //                            }
        //                        }                               
        //                        iniSubExtDetails.CurrentBenefit = "No Benefit";
        //                        iniSubExtDetails.SDowlineSize = 0;
        //                        iniSubExtDetails.BenefitCategory = f["BenefitCategory"];                               
        //                        db.INISubcriberExtraDetails.Add(iniSubExtDetails);
        //                        await db.SaveChangesAsync();
        //                        InterswitchTransactionsTB iswtb = new InterswitchTransactionsTB();
        //                        iswtb.Client_Name = user.FirstName;
        //                        iswtb.Client_Email = user.Email;
        //                        iswtb.Transaction_Amount = amount.ToString();
        //                        iswtb.Transaction_date = DateTime.Now.Date;
        //                        iswtb.Transaction_purpose = "Ihealth Networking Group Subscription";
        //                        iswtb.Transaction_Id = iniSubExtDetails.transaction_Id;
        //                        db.InterswtichTransactionsTables.Add(iswtb);
        //                        await db.SaveChangesAsync();
        //                        TempData["cust_id"] = user.Id;
        //                        TempData["Amount"] = amount;
        //                        TempData["trans_id"] = iniSubExtDetails.transaction_Id;
        //                        TempData["cust_name"] = user.FullName;                               
        //                        return Redirect("/PaymentGateways/pGateway");
        //                    }

        //                } AddErrors(result);
        //            }
        //        }
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //[AllowAnonymous]
        //[HttpGet]
        //public JsonResult SpCode(string param)
        //{
        //    if (!string.IsNullOrEmpty(param))
        //    {
        //        int value = Convert.ToInt32(param);
        //        try
        //        {
        //            var sponsor = db.Users.First(sp => sp.MyRefferalCode == value);

        //            return Json(sponsor.FullName, JsonRequestBehavior.AllowGet);
        //        }
        //        catch
        //        {
        //            return Json("No Record Found!!!", JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    else { return Json("", JsonRequestBehavior.AllowGet); }
        //}
        //Confirm Email
       // [AllowAnonymous]
        //public async Task<ActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        HttpStatusCode.BadRequest.ToString();
        //        return View("Error");
        //    }
        //    var result = await UserManager.ConfirmEmailAsync(userId, code);
        //    if (result.Succeeded)
        //    {
        //        sendRef(userId);
        //        return View("ConfirmEmail");
        //    }
        //    else
        //    {
        //        HttpStatusCode.BadRequest.ToString();
        //        return View("Error");
        //    }
        //    //return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //}

        // // Send Referal Link to Registered Users
        //public void sendRef(string uid)
        //{
        //    var user = db.Users.Find(uid);
        //    if (user != null)
        //    {
        //        // string body = 
        //        try
        //        {
        //            MailAddress
        //   maFrom = new MailAddress(MailFrom, "iHealth", Encoding.UTF8),

        //   maTo = new MailAddress(user.Email, user.Email, Encoding.UTF8);

        //            MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);

        //            mail.Subject = "iHEALTH NETWORKING GROUP";
        //            mail.Body = em.Notification_Email_Body_Creator(user.FullName, "<div><p>iHealth Nigeria GSFM Team Welcome you to the iHealth Networking Group plan <p/>"
        //                  + "<p>Kindly refer 3 or more persons to SIGN UP for IHEALTH NETWORKING GROUP on or before <b style=\"color:Green\"> " + Convert.ToDateTime(user.RegDate).AddDays(90).ToString("dd MMM, yyyy") + ".</b> by sharing this Link <br/> <a href=\"" + user.Refferal_Url + "\"> " + user.Refferal_Url + "</a>"
        //                     + "<br/> and stand a chance to qualify for an amazing benefits. <br/> Thanks for registering with us. <br/><br/> Best Regards, <br/><br/> iHealth Nigeria GSFM Team.</p></div>");

        //            mail.BodyEncoding = Encoding.UTF8;
        //            mail.SubjectEncoding = Encoding.UTF8;
        //            mail.IsBodyHtml = true;
        //            mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
        //            NetworkCredential Credentials = new NetworkCredential(MailFrom, pswd);
        //            smtp.Credentials = Credentials;
        //            smtp.Send(mail);
        //        }
        //        catch (Exception ex)
        //        {
        //            ErrorSignal.FromCurrentContext().Raise(ex);
        //        }
        //    }
        //}
        //this Method Throws Error message
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        #region //Manage Insurance Products
        public ActionResult Health_Insurance(int? page)
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