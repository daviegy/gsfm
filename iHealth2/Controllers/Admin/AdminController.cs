using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iHealth2.Models;
using iHealth2.CustomClasses;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity.Validation;
using System.Diagnostics;
using Elmah;
using Ganss.XSS;
namespace iHealth2.Controllers.Admin
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        string url = "ihealthgsfm.com";
        Emails em = new Emails();
        EmailSender ems = new EmailSender();
        int amount = 10000;
        public AdminController()
        {

        }
        public AdminController(ApplicationUserManager userManager)
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
        // GET: Admin
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Manage All Normal users
        /// </summary>
        /// <returns></returns>
        public ActionResult NewBUsers()
        {
            var newUser = db.Users.Where(u => u.NotifyStatus == 0 && u.UserType == "BusinessUser");
            if (newUser.Count() != 0)
            {
                return View(newUser);
            }
            TempData["error"] = "Alert: No new Business users!";
            return RedirectToAction("index", "admin");
        }
        public ActionResult All_Busers()
        {
            var allUser = db.Users.Where(u => u.NotifyStatus == 1 && u.UserType == "BusinessUser").OrderByDescending(u => u.RegDate).ToList();
            if (allUser.Count() != 0)
            {
                return View(allUser);
            }
            TempData["error"] = "Alert: NO RECORD FOUND!";
            return RedirectToAction("index", "admin");
        }
        [HttpPost]
        public async Task<JsonResult> ApproveUser(IEnumerable<string> Id)
        {
            string ids;
            if (Id != null)
            {
                if (Id.Count() > 0)
                {
                    foreach (var id in Id)
                    {
                        try
                        {
                            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
                            ids = id.Replace("$25", "/");
                            ids = ids.Replace("$24", "+");
                            string refID = Cryptoclass.DecryptStringAES(ids, s);
                            ApplicationUser b = db.Users.First(r => r.Id == refID);
                            b.NotifyStatus = 1;
                            await db.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            return Json("error occured" + ex, JsonRequestBehavior.AllowGet);
                            throw ex;
                        }
                    }
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            } return Json(false, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Feedback Management Session
        /// </summary>
        /// <returns></returns>
        public ActionResult GetContactUs()
        {
            var cnt = db.Feedback.Where(f => f.FBType == "ContactUs" && f.FBStatus == "Fresh").OrderByDescending(f => f.SentDate);
            return View(cnt.ToList());
        }
        public ActionResult GetFraudReport()
        {
            var rpt = db.Feedback.Where(f => f.FBType == "ReportFraud" && f.FBStatus == "Fresh").OrderByDescending(f => f.SentDate);
            return View(rpt);
        }
        [HttpGet]
        public ActionResult ViewFeedBack(int id)
        {
            var fb = db.Feedback.Find(id);
            return View(fb);
        }
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> ReplyFeedback(FormCollection f)
        {
            var sanitizer = new HtmlSanitizer();
            try
            {
                int id = Convert.ToInt32(Session["fbID"]);
                FeedBack fb = db.Feedback.Find(id);
                fb.ReplyDate = DateTime.UtcNow;
                // var message = sanitizer.Sanitize(f["message"]);
                fb.RepliedMsg = Server.HtmlEncode(sanitizer.Sanitize(f["message"], "", null));
                fb.FBStatus = "Treated";
                await db.SaveChangesAsync();
                var body = ems.Notification_Email_Body_Creator(fb.FRName, f["message"]);
                await ems.AdminMailSender(f["email_to"], f["subject"], body);
                TempData["success"] = "Message Sent Successfully";
                Session["fbID"] = null;
                string fbType = Session["fbTyp"].ToString();
                if (fbType == "ContactUs")
                {
                    Session["fbTyp"] = null;
                    return RedirectToAction("GetContactUs", "Admin");
                }
                else
                {
                    Session["fbTyp"] = null;
                    return RedirectToAction("GetFraudReport", "Admin");
                }
            }
            catch (Exception)
            {

                TempData["error"] = "Message Not Sent!";
                return RedirectToAction("GetFraudReport", "Admin");
            }


        }
        public ActionResult TreatedFeedBack()
        {
            var fb = db.Feedback.Where(f => f.FBStatus == "Treated");
            return View(fb.ToList());
        }
        /// <summary>
        /// Profile management Session
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult Profiles(string ids)
        {
            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            ids = ids.Replace("$25", "/");
            ids = ids.Replace("$24", "+");
            string DecryptId = Cryptoclass.DecryptStringAES(ids, s);
            var usr = UserManager.FindByName(DecryptId);

            if (usr == null)
            {
                return HttpNotFound("Requested user not found");
            }

            if (usr.Photo != null)
            {
                return View(new profileViewModel
                {
                    Id = usr.Id,
                    Name = usr.FullName,
                    Email = usr.Email,
                    phone = usr.PhoneNumber,
                    tID = usr.MyRefferalCode,

                    photo =
                     "data:image/png;base64," + Convert.ToBase64String(usr.Photo)
                });
            }
            else
            {
                return View(new profileViewModel
                {
                    Id = usr.Id,
                    Name = usr.FullName,
                    Email = usr.Email,
                    phone = usr.PhoneNumber,
                    tID = usr.MyRefferalCode,
                    state = usr.State,
                    Nationality = usr.Nationality,
                    city = usr.City,
                    Address = usr.Address

                });
            }

        }
        public ActionResult Edit(string id)
        {

            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            id = id.Replace("$25", "/");
            id = id.Replace("$24", "+");
            string DecryptId = Cryptoclass.DecryptStringAES(id, s);

            var user = UserManager.FindById(DecryptId);
            if (user == null)
            {
                return HttpNotFound("Error: Requested user not found");
            }

            //     ApplicationUser usr = user;
            return View(new EditViewModel
            {
                ID = user.Id,
                Name = user.FullName,
                Nationality = user.Nationality,
                Phone = user.PhoneNumber,
                // username = user.UserName,
                TrackingID = user.MyRefferalCode

            });
        }
        [ActionName("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edituser(string Id, [Bind(Exclude = "ID, Email, username,TrackingID")] EditViewModel apuser)
        {
            if (Id == null)
            {
                return HttpNotFound("Error: Requested user not found!");
            }
            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            Id = Id.Replace("$25", "/");
            Id = Id.Replace("$24", "+");
            string DecryptId = Cryptoclass.DecryptStringAES(Id, s);
            var usr = UserManager.FindById(DecryptId);
            if (ModelState.IsValid)
            {
                //   usr.FirstName = apuser.Name;
                usr.PhoneNumber = apuser.Phone;
                UserManager.Update(usr);
                TempData["Success"] = "Your Details has been Updated Successfully";

                String EncryptId = Cryptoclass.EncryptStringAES(usr.UserName, s);
                EncryptId = EncryptId.Replace("/", "$25");
                EncryptId = EncryptId.Replace("+", "$24");
                return RedirectToAction("Profiles", "users", new { ids = EncryptId });
            }
            String EId = Cryptoclass.EncryptStringAES(usr.Id, s);
            EId = EId.Replace("/", "$25");
            EId = EId.Replace("+", "$24");
            TempData["error"] = "Check that the Phone Number field is correclty typed PHONE NUMBR FORMAT";
            // ModelState.AddModelError("", "validate the error field");
            return RedirectToAction("Edit", new { id = EId });

        }
        public JsonResult uploadimage(string id, HttpPostedFileBase Pimg)
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Ihealth Networking Group Management
        /// </summary>
        /// <returns></returns>

        public ActionResult NewINIsubsciber()
        {
            var newUser = db.Users.Where(u => u.NotifyStatus == 0 && u.UserType == "INISubcriber").OrderByDescending(u => u.RegDate);
            if (newUser.Count() != 0)
            {
                return View(newUser);
            }
            TempData["error"] = "Alert: No new INI subcribers!";
            return RedirectToAction("index", "admin");
        }
        public ActionResult PaidSubscriber()
        {
            var paidsub = db.Users.Where(u => u.INISubcriberExtraDetails.PaymentStatus == "paid" && u.UserType == "INISubcriber").OrderByDescending(u => u.RegDate);
            if (paidsub.Count() != 0)
            {
                return View(paidsub);
            }
            TempData["error"] = "Alert: Empty Request due to non payment by subscibers";
            return RedirectToAction("index", "admin");
        }
        public ActionResult NoPaymentSubscriber()
        {
            var NoPaymentsub = db.Users.Where(u => u.INISubcriberExtraDetails.PaymentStatus == "pending" && u.UserType == "INISubcriber").OrderByDescending(u => u.RegDate);
            if (NoPaymentsub.Count() != 0)
            {
                return View(NoPaymentsub);
            }
            TempData["error"] = "Alert: No Debtors Found";
            return RedirectToAction("index", "admin");
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles ="SuperAdmin")]
        public async Task<JsonResult> MarkAsPaid(IEnumerable<string> markAsPaidbyId)
        {
            if (markAsPaidbyId != null)
            {
                if (markAsPaidbyId.Count() > 0)
                {
                    foreach (var id in markAsPaidbyId)
                    {
                        var usrpaid = db.Users.Single(u => u.Id == id);
                        try
                        {
                            usrpaid.INISubcriberExtraDetails.PaymentStatus = "Paid";
                            usrpaid.INISubcriberExtraDetails.BEstatus = "Active";
                            usrpaid.INISubcriberExtraDetails.INI_Status = "Active";
                            usrpaid.INISubcriberExtraDetails.Amounts = amount;
                            usrpaid.INISubcriberExtraDetails.paymentGateway = "Bank";
                            usrpaid.INISubcriberExtraDetails.Transaction_date = DateTime.UtcNow;
                            //  usrpaid.INISubcriberExtraDetails.MaxDT2MtTarget = DateTime.UtcNow.AddDays(30);
                            await db.SaveChangesAsync();
                            //check if the new member has a sponsor so that we can give the sponsor bonus
                            if (!string.IsNullOrEmpty(usrpaid.INISubcriberExtraDetails.My_Sponsor_Referral_Code))
                            {
                                //retrieve the sponsor of this new registrant details from the all users table
                                var sponsor = db.Users.Single(u => u.MyRefferalCode.ToString() == usrpaid.INISubcriberExtraDetails.My_Sponsor_Referral_Code);
                                //check if sponsor is an ihealth networking subscriber, so dt we can give him credit
                                var sp_IN_mlm = db.INISubcriberExtraDetails.Single(sp => sp.User.Id == sponsor.Id);
                                try
                                {

                                    //this check if the sponsor of this new member subscribe for ING promotion target
                                    //if yes, we give the sponsor a promo credit for his/her referral within the promo
                                    //period, ELSE, nothin is added but noth withstanding the dl size of the sponsor 
                                    // still increases./
                                    if (sp_IN_mlm.Promotional_Target_Subscription_Status == "Active")
                                    {
                                        sp_IN_mlm.promo_dl_size = sp_IN_mlm.promo_dl_size + 1;
                                    }
                                    else
                                    {
                                        sp_IN_mlm.Non_promo_dl_size = sp_IN_mlm.Non_promo_dl_size + 1;
                                    }
                                    sp_IN_mlm.Total_dl_Size = sp_IN_mlm.Total_dl_Size + 1;
                                    sp_IN_mlm.Most_recent_DL_Reg_date = DateTime.UtcNow;
                                    await db.SaveChangesAsync();
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }

                                referral_BonusTb refbonus = new referral_BonusTb();
                                refbonus.user_ID = sponsor.Id;
                                refbonus.Downline_Id = usrpaid.INISubcriberExtraDetails.User.MyRefferalCode;
                                refbonus.Downline_Name = usrpaid.INISubcriberExtraDetails.User.FirstName + " " + usrpaid.INISubcriberExtraDetails.User.LastName;
                                refbonus.Bonus_Type = "Ihealth Networking Referral Bonus";
                                refbonus.Subscription_Fee = usrpaid.INISubcriberExtraDetails.Amounts;
                                refbonus.Bonus_created_date = DateTime.UtcNow;
                                refbonus.Bonus = refbonus.Subscription_Fee * 0.5; // bonus is 50% of the subscription
                                //check if the sponsor of this new member subscribe for ING promo
                                if (sponsor.INISubcriberExtraDetails.Promotional_Target_Subscription_Status == "Active")
                                {
                                    refbonus.promo_period_bonus = true;
                                }
                                db.referral_bonus_tb.Add(refbonus);
                                await db.SaveChangesAsync();
                                sp_IN_mlm.CurrentBonus = sp_IN_mlm.CurrentBonus + (refbonus.Subscription_Fee * 0.5);
                                await db.SaveChangesAsync();
                            }
                            string code = await UserManager.GenerateEmailConfirmationTokenAsync(usrpaid.Id);
                            var callbackUrl = Url.Action("ConfirmEmail", "Account",
                               new { userId = usrpaid.Id, code = code }, protocol: Request.Url.Scheme);
                            await UserManager.SendEmailAsync(usrpaid.Id,
                            "Confirm your account", em.Notification_Email_Body_Creator(usrpaid.FullName, "<p>Welcome to iHealth Networking Group.</p> "
                         + "<p>This is to notify you that your payment information has been confirm and your subscription activated. <br/>Please, confirm your account by clicking <a href=\""
                            + callbackUrl + "\">here</a> in order to login to your dashboard</p><p>Thanks for registering with us. <br/><br/>Best Regards, <br/><br/>iHealth Nigeria GSFM Team.</p>"));
                            TempData["success"] = "Operations Successfull";
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }
                    return Json(true,JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TempData["error"] = "Oops! No Item is selected from the list. ";
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                TempData["error"] = "Oops! No Item is selected from the list. ";
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult CSubscriber()
        {
            var CINGSub = db.Users.Where(u => u.UserType == "INISubcriber" && u.INISubcriberExtraDetails.BenefitCategory == "CarreerBenefit").OrderByDescending(u => u.RegDate);
            if (CINGSub.Count() > 0)
            {
                return View(CINGSub);
            }
            return View(CINGSub);
        }
        public ActionResult HSubscriber()
        {
            var HINGSub = db.Users.Where(u => u.UserType == "INISubcriber" && u.INISubcriberExtraDetails.BenefitCategory == "HealthBenefit").OrderByDescending(u => u.RegDate);
            if (HINGSub.Count() > 0)
            {
                return View(HINGSub);
            }
            return View(HINGSub);
        }
        public ActionResult FreshBenefitClaims()
        {
            var fClaims = db.BenefitClaimersTb.Where(c => c.BenefitProcessStatus == "Fresh")
               .OrderByDescending(c => c.B_applicationDate).Distinct();
            if (fClaims.Count() > 0)
            {
                return View(fClaims);
            }
            return View(fClaims);
        }
        public ActionResult BProcesedClaims()
        {
            var pClaims = db.BenefitClaimersTb.Where(c => c.BenefitProcessStatus == "Being Processed")
              .OrderByDescending(c => c.ProcessedDate).Distinct();
            if (pClaims.Count() > 0)
            {
                return View(pClaims);
            }
            return View(pClaims);
        }
        public ActionResult SettledClaims()
        {
            var sClaims = db.BenefitClaimersTb.Where(c => c.BenefitProcessStatus == "Settled")
                           .OrderByDescending(c => c.ProcessedDate).Distinct();
            if (sClaims.Count() > 0)
            {
                return View(sClaims);
            }
            return View(sClaims);
        }
        public async Task<ActionResult> BPReport(string bpd)// this will generate a
            //report whenever the admin click PROCESS benefit button
        {
            var ReturnUrl = Request.UrlReferrer;
            double cash_bonus;
            try
            {
                string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
                bpd = bpd.Replace("$25", "/");
                bpd = bpd.Replace("$24", "+");
                string DecryptId = Cryptoclass.DecryptStringAES(bpd, s);
                int Id = Convert.ToInt32(DecryptId);
                var bc = db.BenefitClaimersTb.Find(Id);
                bc.BenefitProcessStatus = "Being Processed";
                bc.ProcessedDate = DateTime.UtcNow;
                bc.SignedBy = Session["Name"].ToString();
                await db.SaveChangesAsync();
                var getUser = db.Users.Single(x => x.MyRefferalCode == bc.SubRefCode);
                string mailto = getUser.Email;
                var subject = "RE: Benefit Claim Request";
                var body = em.Notification_Email_Body_Creator(getUser.FullName,
                   "<p style=\"line-height: 150%;\">This is to inform you that your iHealth bonuses is currently been process.<br/>" +
                   "You will be notify via email once your benefit is ready. <br/> Thank you for choosing <a href=\"http://" + url + "\">iHealth Nigeria GSFM</a></p> " +
                   "<br/><br/>Best Regards, <br/><br/> iHealth Nigeria Team.");
                await ems.AdminMailSender(mailto: mailto, Subject: subject, Body: body);
                var bf = db.Users.Join(db.BenefitClaimersTb.Where(b => b.SubRefCode == bc.SubRefCode),
                    u => u.MyRefferalCode,
                    b => b.SubRefCode,
                    (u, b) => new { ApplicationUser = u, BenefitClaimersTb = b }).Single(u => u.BenefitClaimersTb.ID == bc.ID);
                if (bf.BenefitClaimersTb.Meet_promo_target == true)
                {
                    cash_bonus = bf.BenefitClaimersTb.totalCashBenefits /*+ (bf.BenefitClaimersTb.CashBenefits * 0.1)*/;
                }
                else
                {
                    cash_bonus = bf.BenefitClaimersTb.totalCashBenefits;
                }
                
                return View(new BPReportViewModel
                {
                    MembershipID = bf.BenefitClaimersTb.SubRefCode.ToString(),
                    Name = bf.ApplicationUser.FirstName + " " + bf.ApplicationUser.LastName,
                    Phone = bf.ApplicationUser.PhoneNumber,
                    Email = bf.ApplicationUser.Email,
                    AccountName = (!string.IsNullOrEmpty(bf.BenefitClaimersTb.AccountName)) ? bf.BenefitClaimersTb.AccountName : "",
                    AccountNo = (!string.IsNullOrEmpty(bf.BenefitClaimersTb.AccountNumber)) ? bf.BenefitClaimersTb.AccountNumber : "",
                    BankName = (!string.IsNullOrEmpty(bf.BenefitClaimersTb.BankName)) ? bf.BenefitClaimersTb.BankName : "",
                    Benefit = (bf.BenefitClaimersTb.BenefitCategory == "Cash") ? cash_bonus.ToString() : bf.BenefitClaimersTb.HealthBenefits,
                    ProcessingId = bf.BenefitClaimersTb.ProcessingId,
                    //Stage = bf.BenefitClaimersTb.BenefitStage,
                    BenefitCat = bf.BenefitClaimersTb.BenefitCategory,
                    CurrentStatus = bf.BenefitClaimersTb.BenefitProcessStatus,
                    SignedBy = bf.BenefitClaimersTb.SignedBy,
                    ProcessedDate = bf.BenefitClaimersTb.ProcessedDate
                });
            }
            catch (Exception ex)
            {
                TempData["error"] = "Operation fail due to the following exceptions " + ex;
                return Redirect(ReturnUrl.ToString());
            }

        }
        public async Task<ActionResult> generateProcessingRPT(string bpd)
        {
            double cash_bonus;
            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            bpd = bpd.Replace("$25", "/");
            bpd = bpd.Replace("$24", "+");
            string DecryptId = Cryptoclass.DecryptStringAES(bpd, s);
            int Id = Convert.ToInt32(DecryptId);
            var bc = await db.BenefitClaimersTb.FindAsync(Id);
            var bf = db.Users.Join(db.BenefitClaimersTb.Where(b => b.SubRefCode == bc.SubRefCode),
                   u => u.MyRefferalCode,
                   b => b.SubRefCode,
                   (u, b) => new { ApplicationUser = u, BenefitClaimersTb = b }).Single(u => u.BenefitClaimersTb.ID == bc.ID);
            if (bf.BenefitClaimersTb.Meet_promo_target == true)
            {
                cash_bonus = bf.BenefitClaimersTb.totalCashBenefits /*+ (bf.BenefitClaimersTb.CashBenefits * 0.1)*/;
            }
            else
            {
                cash_bonus = bf.BenefitClaimersTb.totalCashBenefits;
            }
            return View("BPReport", new BPReportViewModel
            {
                MembershipID = bf.BenefitClaimersTb.SubRefCode.ToString(),
                Name = bf.ApplicationUser.FirstName + " " + bf.ApplicationUser.LastName,
                Phone = bf.ApplicationUser.PhoneNumber,
                Email = bf.ApplicationUser.Email,
                AccountName = (!string.IsNullOrEmpty(bf.BenefitClaimersTb.AccountName)) ? bf.BenefitClaimersTb.AccountName : "",
                AccountNo = (!string.IsNullOrEmpty(bf.BenefitClaimersTb.AccountNumber)) ? bf.BenefitClaimersTb.AccountNumber : "",
                BankName = (!string.IsNullOrEmpty(bf.BenefitClaimersTb.BankName)) ? bf.BenefitClaimersTb.BankName : "",     
                Benefit = (bf.BenefitClaimersTb.BenefitCategory == "Cash") ? cash_bonus.ToString() : bf.BenefitClaimersTb.HealthBenefits,
                ProcessingId = bf.BenefitClaimersTb.ProcessingId,
                //Stage = bf.BenefitClaimersTb.BenefitStage,
                BenefitCat = bf.BenefitClaimersTb.BenefitCategory,
                CurrentStatus = bf.BenefitClaimersTb.BenefitProcessStatus,
                SignedBy = bf.BenefitClaimersTb.SignedBy,
                ProcessedDate = bf.BenefitClaimersTb.ProcessedDate
            });
        }// this will generate a processing report foreach beneficiary of benefits
        public async Task<ActionResult> BSReport(string bpd)// this will generate a report whenever the admin click SETTLE benefit button
        {
            double cash_bonus;
            var ReturnUrl = Request.UrlReferrer;
            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            bpd = bpd.Replace("$25", "/");
            bpd = bpd.Replace("$24", "+");
            string DecryptId = Cryptoclass.DecryptStringAES(bpd, s);
            int Id = Convert.ToInt32(DecryptId);
            var bc = await db.BenefitClaimersTb.FindAsync(Id);
            try
            {                
                var getUser = db.Users.Single(x => x.MyRefferalCode == bc.SubRefCode);
                getUser.INISubcriberExtraDetails.Promotional_Target_Subscription_Status = "Active";
                getUser.INISubcriberExtraDetails.Promo_start_date = DateTime.UtcNow.Date;
                getUser.INISubcriberExtraDetails.Promo_end_date = DateTime.UtcNow.Date.AddDays(30);
                getUser.INISubcriberExtraDetails.promo_dl_size = 0;
                getUser.INISubcriberExtraDetails.Non_promo_dl_size = 0;
                double currt_bonus = getUser.INISubcriberExtraDetails.CurrentBonus - bc.CashBenefits;
                if (currt_bonus <= 0)
                {
                    getUser.INISubcriberExtraDetails.CurrentBonus = 0;
                }
                else
                {
                    getUser.INISubcriberExtraDetails.CurrentBonus = currt_bonus;
                }
                await db.SaveChangesAsync();
                var refbonus = db.referral_bonus_tb.Where(r => r.user_ID == getUser.Id);
                foreach (referral_BonusTb userbonus in refbonus)
                {
                    var refb = db.referral_bonus_tb.Find(userbonus.id);
                    DateTime bonusCreatedTime = default(DateTime).Add(userbonus.Bonus_created_date.Value.TimeOfDay);
                    DateTime benefitApplicationTime = default(DateTime).Add(bc.B_applicationDate.Value.TimeOfDay);
                    if (refb.Bonus_created_date < bc.B_applicationDate)
                    {
                        refb.Bonus = 0;
                        refb.Bonus_Used_Date = DateTime.UtcNow.Date;
                    }
                    else if(refb.Bonus_created_date == bc.B_applicationDate)
                    {
                        if (bonusCreatedTime.Hour < benefitApplicationTime.Hour)
                        {
                            refb.Bonus = 0;
                            refb.Bonus_Used_Date = DateTime.UtcNow.Date;
                        }
                      else if (bonusCreatedTime.Hour == benefitApplicationTime.Hour)
                        {
                           if (bonusCreatedTime.Minute < benefitApplicationTime.Minute)
                            {
                                refb.Bonus = 0;
                                refb.Bonus_Used_Date = DateTime.UtcNow.Date;
                            }
                            else if (bonusCreatedTime.Minute == benefitApplicationTime.Minute)
                            {
                                if (bonusCreatedTime.Second < benefitApplicationTime.Second)
                                {
                                    refb.Bonus = 0;
                                    refb.Bonus_Used_Date = DateTime.UtcNow.Date;
                                }
                                else if (bonusCreatedTime.Second == benefitApplicationTime.Second)
                                {
                                    if (bonusCreatedTime.Millisecond < benefitApplicationTime.Millisecond)
                                    {
                                        refb.Bonus = 0;
                                        refb.Bonus_Used_Date = DateTime.UtcNow.Date;
                                    }
                                }
                            }
                        }
                    }
                }
                await db.SaveChangesAsync();
              
                bc.BenefitProcessStatus = "Settled";
                bc.ProcessedDate = DateTime.UtcNow;
                bc.SignedBy = Session["Name"].ToString();
                if(bc.Meet_promo_target == true)
                {
                    bc.CashBenefits = bc.CashBenefits; // + (bc.CashBenefits * 0.1);
                }
                await db.SaveChangesAsync();
                string mailto = getUser.Email;
                var subject = "RE: Benefit Claim Request";
                var body = em.Notification_Email_Body_Creator(getUser.FullName,
                     "<p style=\"line-height: 150%;\">Congratulations! This is to inform you that your benefit claim request has been processed and a new promotional period has been activated for you automatically.<br/>"+
                     "Kindly, refer more people using your referral link or code to get more out of iHealth.<br/>" +
                     "Thank you for choosing <a href=\"http://" + url + "\">iHealth Nigeria GSFM</a></p> " +
                     "<br/><br/>Best Regards, <br/><br/> iHealth Nigeria Team.");
                await ems.AdminMailSender(mailto: mailto, Subject: subject, Body: body);

                ///*
                // TODO: UPDATE iniSubscriberExtraDetails where benefit has been claim and set current benefit to SETTLED
                // */
                //Check iniUserExtraDetails

                //INISubcriberExtraDetail Inisub = db.INISubcriberExtraDetails.Single(u => u.User.MyRefferalCode == bc.SubRefCode);
                //if (bc.BenefitCategory == "CarreerBenefit")
                //{
                //    Inisub.CurrentBenefit = "Settled";
                //    await db.SaveChangesAsync();
                //}


            }
            catch (Exception ex)
            {
                TempData["error"] = "Operation fail due to the following exceptions " + ex;
                return Redirect(ReturnUrl.ToString());

            }
            var bf = db.Users.Join(db.BenefitClaimersTb.Where(b => b.SubRefCode == bc.SubRefCode),
                u => u.MyRefferalCode,
                b => b.SubRefCode,
                (u, b) => new { ApplicationUser = u, BenefitClaimersTb = b }).Single(u => u.BenefitClaimersTb.ID == bc.ID);
            if (bf.BenefitClaimersTb.Meet_promo_target == true)
            {
                cash_bonus = bf.BenefitClaimersTb.totalCashBenefits; /*+ (bf.BenefitClaimersTb.CashBenefits * 0.1)*/;
            }
            else
            {
                cash_bonus = bf.BenefitClaimersTb.totalCashBenefits;
            }
            return View(new BPReportViewModel
            {
                MembershipID = bf.BenefitClaimersTb.SubRefCode.ToString(),
                Name = bf.ApplicationUser.FirstName + " " + bf.ApplicationUser.LastName,
                Phone = bf.ApplicationUser.PhoneNumber,
                Email = bf.ApplicationUser.Email,
                AccountName = (!string.IsNullOrEmpty(bf.BenefitClaimersTb.AccountName)) ? bf.BenefitClaimersTb.AccountName : "",
                AccountNo = (!string.IsNullOrEmpty(bf.BenefitClaimersTb.AccountNumber)) ? bf.BenefitClaimersTb.AccountNumber : "",
                BankName = (!string.IsNullOrEmpty(bf.BenefitClaimersTb.BankName)) ? bf.BenefitClaimersTb.BankName : "",

                Benefit = (bf.BenefitClaimersTb.BenefitCategory == "Cash") ? cash_bonus.ToString() : bf.BenefitClaimersTb.HealthBenefits,
                ProcessingId = bf.BenefitClaimersTb.ProcessingId,
                Stage = bf.BenefitClaimersTb.BenefitStage,
                BenefitCat = bf.BenefitClaimersTb.BenefitCategory,
                CurrentStatus = bf.BenefitClaimersTb.BenefitProcessStatus,
                SignedBy = bf.BenefitClaimersTb.SignedBy,
                ProcessedDate = bf.BenefitClaimersTb.ProcessedDate
            });
        }
        public async Task<ActionResult> generateSettledRPT(string bpd)// this will generate a settlement report foreach beneficiary of benefits
        {
            double cash_bonus;
            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            bpd = bpd.Replace("$25", "/");
            bpd = bpd.Replace("$24", "+");
            string DecryptId = Cryptoclass.DecryptStringAES(bpd, s);
            int Id = Convert.ToInt32(DecryptId);
            var bc = await db.BenefitClaimersTb.FindAsync(Id);
            var bf = db.Users.Join(db.BenefitClaimersTb.Where(b => b.SubRefCode == bc.SubRefCode),
                 u => u.MyRefferalCode,
                 b => b.SubRefCode,
                 (u, b) => new { ApplicationUser = u, BenefitClaimersTb = b }).Single(u => u.BenefitClaimersTb.ID == bc.ID);
            if (bf.BenefitClaimersTb.Meet_promo_target == true)
            {
                cash_bonus = bf.BenefitClaimersTb.totalCashBenefits; //+ (bf.BenefitClaimersTb.CashBenefits * 0.1);
            }
            else
            {
                cash_bonus = bf.BenefitClaimersTb.totalCashBenefits;
            }
            return View("BSReport", new BPReportViewModel
            {
                MembershipID = bf.BenefitClaimersTb.SubRefCode.ToString(),
                Name = bf.ApplicationUser.FirstName + " " + bf.ApplicationUser.LastName,
                Phone = bf.ApplicationUser.PhoneNumber,
                Email = bf.ApplicationUser.Email,
                AccountName = (!string.IsNullOrEmpty(bf.BenefitClaimersTb.AccountName)) ? bf.BenefitClaimersTb.AccountName : "",
                AccountNo = (!string.IsNullOrEmpty(bf.BenefitClaimersTb.AccountNumber)) ? bf.BenefitClaimersTb.AccountNumber : "",
                BankName = (!string.IsNullOrEmpty(bf.BenefitClaimersTb.BankName)) ? bf.BenefitClaimersTb.BankName : "",  
                Benefit = (bf.BenefitClaimersTb.BenefitCategory == "Cash") ? cash_bonus.ToString() : bf.BenefitClaimersTb.HealthBenefits,
                ProcessingId = bf.BenefitClaimersTb.ProcessingId,
                Stage = bf.BenefitClaimersTb.BenefitStage,
                BenefitCat = bf.BenefitClaimersTb.BenefitCategory,
                CurrentStatus = bf.BenefitClaimersTb.BenefitProcessStatus,
                SignedBy = bf.BenefitClaimersTb.SignedBy,
                ProcessedDate = bf.BenefitClaimersTb.ProcessedDate
            });
        }
        /// <summary>
        /// Drug Request Mgt. session
        /// </summary>
        /// <returns></returns>
        public ActionResult urgentDrugRequest()
        {
            var urgdrug = db.urgentNeedforDrugsTb.Where(u => u.notifyStatus == 0).OrderByDescending(u => u.RequestedDate).ToList();

            return View(urgdrug);
        }
        public async Task<ActionResult> MarkAsTreatedForDrugRequest(IEnumerable<int> markAsSeenbyId)
        {
            try
            {
                if (markAsSeenbyId != null)
                {
                    if (markAsSeenbyId.Count() > 0)
                    {
                        foreach (var id in markAsSeenbyId)
                        {
                            var urgdrg = db.urgentNeedforDrugsTb.Single(s => s.id == id);
                            urgdrg.notifyStatus = 1;
                            urgdrg.RequestStatus = "Treated";
                            urgdrg.TreatedDate = DateTime.UtcNow;

                            await db.SaveChangesAsync();
                        }
                        TempData["success"] = "Operations is successfull.";
                        return RedirectToAction("urgentDrugRequest");
                    }
                    else
                    {
                        TempData["error"] = "Oops! No Item is selected from the list. ";
                        return RedirectToAction("urgentDrugRequest");
                    }
                }
                else
                {
                    TempData["error"] = "Oops! No Item is selected from the list. ";
                    return RedirectToAction("urgentDrugRequest");
                }
            }
            catch
            {
                TempData["error"] = "Oops! No Item is selected from the list. ";
                return RedirectToAction("urgentDrugRequest");
            }

        }
        public ActionResult TreatedDrugRequest()
        {
            var Trdrg = db.urgentNeedforDrugsTb.Where(dr => dr.RequestStatus == "Treated").OrderByDescending(dr => dr.TreatedDate);
            return View(Trdrg.ToList());
        }
        /// <summary>
        /// Admin Notification mgt. session
        /// </summary>
        /// <returns></returns>
        #region//Jquery Callback function to perform Notification Update on Admin page
        public JsonResult getTUsersize()
        {
            var tUsersize = db.Users.Where(u => u.NotifyStatus == 0).Count();
            if (tUsersize > 0)
                return Json(tUsersize, JsonRequestBehavior.AllowGet);
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult NewBuserNotif()
        {
            var tUsersize = db.Users.Where(u => u.NotifyStatus == 0 && u.UserType == "BusinessUser").Count();
            if (tUsersize > 0)
                return Json(tUsersize, JsonRequestBehavior.AllowGet);
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult NewINGuserNotif()
        {
            var tUsersize = db.Users.Where(u => u.NotifyStatus == 0 && u.UserType == "INISubcriber").Count();
            if (tUsersize > 0)
                return Json(tUsersize, JsonRequestBehavior.AllowGet);
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult NewPremuserNotif()
        {
            var tUsersize = db.premium_user.Where(pu => pu.NotifyStatus == 0 || pu.PaymentStatus == "Pending").Count(); ;
            if (tUsersize > 0)
                return Json(tUsersize, JsonRequestBehavior.AllowGet);
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult getNewCompanyProductSize()
        {
            int productsize = db.ProductTb.Where(p => p.NotifyStatus == 0).Count();
            int companySize = db.BusinessInfoes.Where(b => b.NotifyStatus == 0).Count();
            int total = productsize + companySize;
            if (total > 0)
                return Json(total, JsonRequestBehavior.AllowGet);
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult getNewProducts()
        {
            int productsize = db.ProductTb.Where(p => p.NotifyStatus == 0).Count();
            if (productsize > 0)
                return Json(productsize, JsonRequestBehavior.AllowGet);
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult getNewCompany()
        {
            int companySize = db.BusinessInfoes.Where(b => b.NotifyStatus == 0).Count();
            if (companySize > 0)
                return Json(companySize, JsonRequestBehavior.AllowGet);
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult getFreshClaims()
        {
            var fClaims = db.BenefitClaimersTb.Where(c => c.BenefitProcessStatus == "Fresh").Count();
            if (fClaims > 0)
                return Json(fClaims, JsonRequestBehavior.AllowGet);
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult getFeedbackSize()
        {
            var fb = db.Feedback.Where(f => f.FBStatus == "Fresh").Count();
            if (fb > 0)
                return Json(fb, JsonRequestBehavior.AllowGet);
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult getContactusJson()
        {
            var cnt = db.Feedback.Where(f => f.FBType == "ContactUs" && f.FBStatus == "Fresh").Count();
            if (cnt > 0)
                return Json(cnt, JsonRequestBehavior.AllowGet);
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult getReportFraudJson()
        {
            var rpt = db.Feedback.Where(f => f.FBType == "ReportFraud" && f.FBStatus == "Fresh").Count();
            if (rpt > 0)
                return Json(rpt, JsonRequestBehavior.AllowGet);
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult getSupportRequestSize()
        {
            var sp = db.supports.Where(s => s.support_status == "Fresh").Count();
            if (sp > 0) { return Json(sp, JsonRequestBehavior.AllowGet); }
            else { return Json("", JsonRequestBehavior.AllowGet); }
        }
        #endregion
        /// <summary>
        ///  Admin use this function to send mail to subscribers/clients
        /// </summary>
        /// <param name="mailto">mail recipient</param>
        /// <param name="subject"> subject of mail</param>
        /// <param name="body">content of mail</param>
        /// <returns>return mail sent success/failure</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> sendmail(string emailto, string subject, string Message)
        {
            var ReturnUrl = Request.UrlReferrer;
            try
            {
                var sendto = db.Users.Single(e => e.Email == emailto);
                string body = ems.Notification_Email_Body_Creator(sendto.FirstName, Message);
                await ems.AdminMailSender(mailto: emailto, Subject: subject, Body: body);
                TempData["Success"] = "Mail has been sent successfully";
                return Redirect(ReturnUrl.ToString());

            }
            catch
            {
                TempData["error"] = "Mail Sending Fail!!!";
                return Redirect(ReturnUrl.ToString());

            }


        }
        /// <summary>
        /// Retrieve benefit claimer's details or basic information
        /// </summary>
        /// <param name="Id">benefit claimer's reference code</param>
        /// <returns>return benefit claimer's details</returns>
        public JsonResult getDetails(int Id, string pid)
        {
            IEnumerable<benefitclaimersDetails> userlist = new List<benefitclaimersDetails>();

            try
            {
                var bclaimtb = from x in db.BenefitClaimersTb
                               join y in db.Users
                               on x.SubRefCode equals y.MyRefferalCode
                               where (x.SubRefCode == Id) && (x.ProcessingId == pid)
                               select new benefitclaimersDetails()
             {
                 Name = x.Name,
                 Email = y.Email,
                 Phone = y.PhoneNumber,
                 AccName = (!string.IsNullOrEmpty(x.AccountName)) ? x.AccountName : "",
                 AccNo = (!string.IsNullOrEmpty(x.AccountNumber)) ? x.AccountNumber : "",
                 BankName = (!string.IsNullOrEmpty(x.BankName)) ? x.BankName : "",
                 Downline = x.DownlineSize,

             };
                return Json(bclaimtb.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex, JsonRequestBehavior.AllowGet);
            }

        }

    }
}