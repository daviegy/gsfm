using iHealth2.CustomClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iHealth2.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using IhealthPaging;
using PagedList;
namespace iHealth2.Controllers.Users
{
    [Authorize(Roles = "INIsubscribers,Admin,SuperAdmin")]
    public class INGController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        EmailSender ems = new EmailSender();
        Emails em = new Emails();       
        private const int DefaultPageSize = 10;
        private int currentPageIndex = 1;
       
        private ApplicationUserManager _userManager;
        private DateTime _DeadLine;
        public DateTime Deadline
        {
            get
            {
                return _DeadLine;
            }
            set
            {
                _DeadLine = value;
            }

        }
        public INGController()
        {

        }
        public INGController(ApplicationUserManager userManager)
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

        // GET: INI_Subscriber
        public ActionResult Userpanel(string bc)
        {
            return View("UserpanelCarreer");
        }
        public ActionResult UserpanelCarreer()
        {
            return View();
        }
        public ActionResult UserpanelHealth()
        {

            return View();

        }
        public ActionResult Direct_Members(string dm)
        {
            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            dm = dm.Replace("$25", "/");
            dm = dm.Replace("$24", "+");
            string DecryptId = Cryptoclass.DecryptStringAES(dm, s);
            var downline = db.INISubcriberExtraDetails.Where(d => d.My_Sponsor_Referral_Code == DecryptId && d.PaymentStatus == "Paid");
            return View(downline.ToList());
        }
        public ActionResult ref_bonus(string dm)
        {
            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            dm = dm.Replace("$25", "/");
            dm = dm.Replace("$24", "+");
            string DecryptId = Cryptoclass.DecryptStringAES(dm, s);
            var usr = UserManager.FindByName(DecryptId);
            var ref_bonus_model = from b in db.referral_bonus_tb
                                  where b.user_ID == usr.Id
                                  select new referral_bonus_view_model()
                                  {
                                      downline_name = b.Downline_Name,
                                      subcription_fee = b.Subscription_Fee,
                                      my_bonus = b.Bonus,
                                      created_date = b.Bonus_created_date,
                                      bonus_type = b.Bonus_Type
                                  };
            return View(ref_bonus_model.ToList().OrderByDescending(b => b.created_date));

        }
        public async Task<ActionResult> activate_promo(string p)
        {
            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            p = p.Replace("$25", "/");
            p = p.Replace("$24", "+");
            string DecryptId = Cryptoclass.DecryptStringAES(p, s);
            INISubcriberExtraDetail u = db.INISubcriberExtraDetails.Find(DecryptId);
            if (u.Promotional_Target_Subscription_Status != "Active")
            {
                u.Promotional_Target_Subscription_Status = "Active";
                u.Promo_start_date = DateTime.UtcNow;
                u.Promo_end_date = DateTime.UtcNow.AddDays(30);
                await db.SaveChangesAsync();
                TempData["success"] = "iHealth Networking Group Promo Activation is successful";
            }
            else
            {
                TempData["error"] = "ERROR: Your promotional period is still active.";
            }

            return View("UserpanelCarreer");
        }
        public async Task<ActionResult> ClaimBenefits(string ids)
        {
            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            ids = ids.Replace("$25", "/");
            ids = ids.Replace("$24", "+");
            string DecryptId = Cryptoclass.DecryptStringAES(ids, s);
            var user = await db.INISubcriberExtraDetails.FindAsync(DecryptId);
            Random rd = new Random();
            int RD = rd.Next(100000);
            string ProcessingID = string.Concat("PID-", RD.ToString());
            // if today's date is 
            //greater than the promo end date (Maximum date for the subscriber to meet his/her target)
            //and the member has bonus available then proceed by executing the if condition
            if (user.CurrentBonus != 0)
            {
                if (user.Promotional_Target_Subscription_Status != "Active")
                {
                    if (user.Promotional_Target_Subscription_Status == "Expired")
                    {
                        if (user.promo_dl_size >= 2)
                        {
                            TempData["dl_size"] = user.promo_dl_size;
                            TempData["bonus"] = user.CurrentBonus;
                            return RedirectToAction("Choose_Benefit", new { refcode = user.User.MyRefferalCode });
                        }
                    }
                    var bank_details = db.bank_info.Where(bd => bd.Account_holder_id == user.UserID);
                    if (bank_details.Count() > 0)
                    {
                        //Check if claim has already been submitted: this will help eliminate
                        //double claims submission for a single claim
                        var checkClaims = db.BenefitClaimersTb.Where(c => c.SubRefCode == user.User.MyRefferalCode && c.BenefitProcessStatus != "Settled");
                        if (checkClaims.Count() > 0)
                        {
                            TempData["warning"] = "Oops! You still have an active benefit that is currently being process. Do not click the  claim Benefit link again.";
                            return RedirectToAction("UserpanelCarreer");
                        }
                        BenefitClaimersTb bctb = new BenefitClaimersTb();
                        bctb.SubRefCode = user.User.MyRefferalCode;
                        bctb.Name = string.Concat(user.User.FirstName, " ", user.User.LastName);
                        bctb.DownlineSize = user.promo_dl_size + user.Non_promo_dl_size;
                        bctb.BankName = user.User.bank_acct_info.BankName;
                        bctb.AccountName = user.User.bank_acct_info.AccountName;
                        bctb.AccountNumber = user.User.bank_acct_info.AccountNumber;
                        bctb.BenefitCategory = "Cash";
                        //  bctb.MaxDateToMeetTarget = usr.INISubcriberExtraDetails.MaxDT2MtTarget;
                        bctb.B_applicationDate = DateTime.UtcNow;
                        bctb.CashBenefits = user.CurrentBonus;
                        bctb.cashBonus = 0;
                        bctb.totalCashBenefits = bctb.CashBenefits + bctb.cashBonus;
                        bctb.BenefitProcessStatus = "Fresh";
                        // bctb.BenefitStage = "Career";
                        bctb.ProcessingId = ProcessingID;
                        bctb.Meet_promo_target = false;
                        // bctb.username = usr.UserName;
                        db.BenefitClaimersTb.Add(bctb);
                        await db.SaveChangesAsync();
                        //Todo : Send mail to Admin on new benefit claims
                        // body1 contain email content that will be sent to beneficiary of claims by the ihealth mail server
                        string body1 = ems.Notification_Email_Body_Creator(user.User.FullName, "Please, be informed that your request to claim " +
                            "benefit has been recieved. <br/><br/>  We will notify you as soon as your benefit is ready." +
                            "<br/><br/> Thank you for choosing iHealth Nigeria GSFM <br/><br/> Best Regards, <br/><br/> iHeath Nigeria GSFM Team.");
                        // body2 will contain message notifying admin of new benefit request.
                        string body2 = ems.Notification_Email_Body_Creator("Admin", "Please, be informed that a fresh request for benefits has been submitted.<br/><br/> Kindly, login to the portal for processing." +
                            "<br/><br/>Thanks.<br/><br/>From: iHealth Nigeria GSFM Mail Sender. ");
                        await em.FreshBenefitClaimNotificationRequest(user.User.Email, "Fresh Benefit Claim Request", body1, body2);

                        TempData["success"] = "Congratulations, Your application for benefit claims having been submitted successfully. We will get back to you shortly";
                        return RedirectToAction("UserpanelCarreer");
                    }
                    else
                    {
                        String UserID = iHealth2.CustomClasses.Cryptoclass.EncryptStringAES(user.UserID, s);
                        UserID = UserID.Replace("/", "$25");
                        UserID = UserID.Replace("+", "$24");
                        return RedirectToAction("BankInfo", "ING", new { ids = UserID });
                    }
                }
                else
                {
                    TempData["error"] = "You can only claim benefit at the end of the current promo period, Thanks.";
                }
                

            }
            else
            {
                TempData["error"] = "You currently do not have any bonus available";
            }
            return View("UserpanelCarreer");
        }
        public ActionResult Choose_Benefit( int refcode)
        {
            var checkClaims = db.BenefitClaimersTb.Where(c => c.SubRefCode == refcode && c.BenefitProcessStatus != "Settled");
            if (checkClaims.Count() > 0)
            {
                TempData["warning"] = "Oops! You still have an active benefit that is currently being process. Do not click the  claim Benefit link again.";
                return RedirectToAction("UserpanelCarreer");
            }
            return View();
        }

        public async Task<ActionResult> Choose_Benefits(string type, string claimer)
        {
            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            claimer = claimer.Replace("$25", "/");
            claimer = claimer.Replace("$24", "+");
            string DecryptId = Cryptoclass.DecryptStringAES(claimer, s);
            var user = await db.INISubcriberExtraDetails.FindAsync(DecryptId);
            Random rd = new Random();
            int RD = rd.Next(100000);
            string ProcessingID = string.Concat("PID-", RD.ToString());
            #region //Cash_Benefits
            if (type == "Cash_Benefit")
            {
                var bank_details = db.bank_info.Where(bd => bd.Account_holder_id == user.UserID);
                if (bank_details.Count() > 0)
                {
                    //Check if claim has already been submitted: this will help eliminate
                    //double claims submission for a single claim
                    var checkClaims = db.BenefitClaimersTb.Where(c => c.SubRefCode == user.User.MyRefferalCode && c.BenefitProcessStatus != "Settled");
                    if (checkClaims.Count() > 0)
                    {
                        TempData["warning"] = "Oops! You still have an active benefit that is currently being process. Do not click the  claim Benefit link again.";
                        return RedirectToAction("UserpanelCarreer");
                    }
                    BenefitClaimersTb bctb = new BenefitClaimersTb();
                    bctb.SubRefCode = user.User.MyRefferalCode;
                    bctb.Name = string.Concat(user.User.FirstName, " ", user.User.LastName);
                    bctb.DownlineSize = user.promo_dl_size + user.Non_promo_dl_size;
                    bctb.BankName = user.User.bank_acct_info.BankName;
                    bctb.AccountName = user.User.bank_acct_info.AccountName;
                    bctb.AccountNumber = user.User.bank_acct_info.AccountNumber;
                    bctb.BenefitCategory = "Cash";
                    //  bctb.MaxDateToMeetTarget = usr.INISubcriberExtraDetails.MaxDT2MtTarget;
                    bctb.B_applicationDate = DateTime.UtcNow;
                    bctb.CashBenefits = user.CurrentBonus ;
                    bctb.cashBonus = user.CurrentBonus * 0.1;
                    bctb.totalCashBenefits = bctb.CashBenefits + bctb.cashBonus;
                    bctb.BenefitProcessStatus = "Fresh";
                    // bctb.BenefitStage = "Career";
                    bctb.ProcessingId = ProcessingID;
                    bctb.Meet_promo_target = true;
                    // bctb.username = usr.UserName;
                    db.BenefitClaimersTb.Add(bctb);
                    await db.SaveChangesAsync();
                    //Todo : Send mail to Admin on new benefit claims
                    // body1 contain email content that will be sent to beneficiary of claims by the ihealth mail server
                    string body1 = ems.Notification_Email_Body_Creator(user.User.FullName, "Please, be informed that your request to claim " +
                        "benefit has been recieved. <br/><br/>  We will notify you as soon as your benefit is ready." +
                        "<br/><br/> Thank you for choosing iHealth Nigeria GSFM <br/><br/> Best Regards, <br/><br/> iHeath Nigeria GSFM Team.");
                    // body2 will contain message notifying admin of new benefit request.
                    string body2 = ems.Notification_Email_Body_Creator("Admin", "Please, be informed that a fresh request for benefits has been submitted.<br/><br/> Kindly, login to the portal for processing." +
                        "<br/><br/>Thanks.<br/><br/>From: iHealth Nigeria GSFM Mail Sender. ");
                    await em.FreshBenefitClaimNotificationRequest(user.User.Email, "Fresh Benefit Claim Request", body1, body2);

                    TempData["success"] = "Congratulations, Your application for benefit claims having been submitted successfully. We will get back to you shortly";
                    return RedirectToAction("UserpanelCarreer");
                }
                else
                {
                    String UserID = iHealth2.CustomClasses.Cryptoclass.EncryptStringAES(user.UserID, s);
                    UserID = UserID.Replace("/", "$25");
                    UserID = UserID.Replace("+", "$24");
                    return RedirectToAction("BankInfo", "ING", new { ids = UserID });
                }
            }
            #endregion
            #region //Health_Benefits
            else
            {
                //Check if claim has already been submitted: this will help eliminate
                //double claims submission for a single claim
                var checkClaims = db.BenefitClaimersTb.Where(c => c.SubRefCode == user.User.MyRefferalCode && c.BenefitProcessStatus != "Settled");
                if (checkClaims.Count() > 0)
                {
                    TempData["warning"] = "Oops! You still have an active benefit that is currently being process. Do not click the  claim Benefit link again.";
                    return RedirectToAction("UserpanelCarreer");
                }
                BenefitClaimersTb bctb = new BenefitClaimersTb();
                bctb.SubRefCode = user.User.MyRefferalCode;
                bctb.Name = string.Concat(user.User.FirstName, " ", user.User.LastName);
                bctb.DownlineSize = user.promo_dl_size;
                bctb.BenefitCategory = "Health";
                bctb.B_applicationDate = DateTime.UtcNow;
                if (user.promo_dl_size >= 2 && user.promo_dl_size < 5)
                {
                    bctb.HealthBenefits = "Health Insurance of 2 to 4 referrals";
                    
                }
                else if (user.promo_dl_size >= 4 && user.promo_dl_size < 10)
                {
                    bctb.HealthBenefits = "Health Insurance of 4 to 8 referrals";
                }
                else if (user.promo_dl_size >= 10)
                {
                    bctb.HealthBenefits = "High-Class-Networkers";
                }
                bctb.BenefitProcessStatus = "Fresh";
                bctb.CashBenefits = user.CurrentBonus;
                // bctb.BenefitStage = "Career";
                bctb.ProcessingId = ProcessingID;
                bctb.Meet_promo_target = true;
                // bctb.username = usr.UserName;
                db.BenefitClaimersTb.Add(bctb);
                await db.SaveChangesAsync();
                //Todo : Send mail to Admin on new benefit claims
                // body1 contain email content that will be sent to beneficiary of claims by the ihealth mail server
                string body1 = ems.Notification_Email_Body_Creator(user.User.FullName, "Please, be informed that your request to claim " +
                    "benefit has been recieved. <br/><br/>  We will notify you as soon as your benefit is ready." +
                    "<br/><br/> Thank you for choosing iHealth Nigeria GSFM <br/><br/> Best Regards, <br/><br/> iHeath Nigeria GSFM Team.");
                // body2 will contain message notifying admin of new benefit request.
                string body2 = ems.Notification_Email_Body_Creator("Admin", "Please, be informed that a fresh request for benefits has been submitted.<br/><br/> Kindly, login to the portal for processing." +
                    "<br/><br/>Thanks.<br/><br/>From: iHealth Nigeria GSFM Mail Sender. ");
                await em.FreshBenefitClaimNotificationRequest(user.User.Email, "Fresh Benefit Claim Request", body1, body2);

                TempData["success"] = "Congratulations, Your application for benefit claims having been submitted successfully. We will get back to you shortly";
                return RedirectToAction("UserpanelCarreer");
            }
            #endregion
            //return View("UserpanelCarreer");
        }
        public ActionResult BenefitHistory(string ids)
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
            var bHist = db.BenefitClaimersTb.Where(u => u.SubRefCode == usr.MyRefferalCode && u.BenefitProcessStatus == "Settled").ToList().OrderByDescending(d=>d.ProcessedDate);
            return View(bHist);
        }
        [HttpGet]
        public ActionResult BankInfo(string ids)
        {
            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            ids = ids.Replace("$25", "/");
            ids = ids.Replace("$24", "+");
            string DecryptId = Cryptoclass.DecryptStringAES(ids, s);
            var usr = UserManager.FindById(DecryptId);
            if (usr == null)
            {
                return HttpNotFound("Requested user not found");
            }
            Bank_Account_Info bk = db.bank_info.SingleOrDefault(b => b.Account_holder_id == usr.Id);
            if (bk != null)
            {
                var model = new BankInfoFormModel
                {
                    AccountName = bk.AccountName,
                    AcountNumber = bk.AccountNumber,
                    BankName = bk.BankName,
                    // SubcriberRefCode = usr.MyRefferalCode,
                    Id = bk.Account_holder_id
                };
                return View(model);
            }
            else
            {
                var model = new BankInfoFormModel
                {
                    Id = usr.Id
                };
                return View(model);
            }


        }
        [HttpPost]
        [ActionName("BankInfo")]
        public async Task<ActionResult> UpdateBankInfo(BankInfoFormModel bm)
        {
            var bank_details = new Bank_Account_Info();
            bank_details.Account_holder_id = bm.Id;
            bank_details.AccountName = bm.AccountName;
            bank_details.AccountNumber = bm.AcountNumber;
            bank_details.BankName = bm.BankName;
            bank_details.CreatedDate = DateTime.UtcNow.Date;
            db.bank_info.Add(bank_details);
            await db.SaveChangesAsync();
            //await em.notify_Bank_Account_Changes()
            TempData["success"] = "Bank Information has been successfully added.";
            Bank_Account_Info bk = db.bank_info.Find(bank_details.Account_holder_id);
            if (bk != null)
            {
                var model = new BankInfoFormModel
                {
                    AccountName = bk.AccountName,
                    AcountNumber = bk.AccountNumber,
                    BankName = bk.BankName,
                    //   SubcriberRefCode = bk.MyRefferalCode,
                    Id = bk.Account_holder_id
                };
                return View(model);
            }
            else
            {
                var model = new BankInfoFormModel
                {
                    Id = bm.Id
                };
                return View(model);
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
        [AllowAnonymous]
        public ActionResult Membership_benefits()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult ClientBooster()
        {
            return View();
        }
      
        //admin view list of service boosters
          [Authorize(Roles = "Admin,SuperAdmin")]  
        public ActionResult Service_Booster_List()
        {
            var sb = db.Users.Where(u => u.UserType == "INISubcriber" && u.Health_Service_Provider == "Yes" && u.isClientBooster == true)
                .Select(sbs=> new service_booster_client_VM { 
                Name = sbs.FirstName +" "+ sbs.LastName,
                Email = sbs.Email,
                Phone = sbs.PhoneNumber,
                Profession = sbs.Profession,
                Address = sbs.Address  ,
               sbid = sbs.Id
                });
            return View(sb.ToList());
        }
       
        [Authorize(Roles = "Admin,SuperAdmin")]
          //admin view list of service boosters businesses
        public ActionResult sp_Service_Booster_Biz_List(string sbid)
        {
            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            sbid = sbid.Replace("$25", "/");
            sbid = sbid.Replace("$24", "+");
            string DecryptId = Cryptoclass.DecryptStringAES(sbid, s);
            var sb = db.BusinessInfoes.Where(b => b.UserID == DecryptId).ToList();
            return View(sb);
        }
        [AllowAnonymous] //Service Boosters Registered and approved businesses
        public ActionResult Client_Booster_List(int? page)
        {
            currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            var ClientBoosters = db.BusinessInfoes.Where(cb => cb.isServiceBooster == "Yes" && cb.ApprovedStatus == "A" && cb.NotifyStatus == 1)
                .OrderByDescending(b => b.Recommended_Status)
                .ThenByDescending(b => b.VerifiedStatus);
            return View(PagingExtensions.ToPagedList(ClientBoosters,currentPageIndex,DefaultPageSize));
        }
        [AllowAnonymous]
        public PartialViewResult Client_Booster_List_by_category(int? page, string sbValue)
        {
            currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            var ClientBoosters = db.BusinessInfoes.Where(cb =>cb.Category.Contains(sbValue) && cb.isServiceBooster == "Yes" && cb.ApprovedStatus == "A" && cb.NotifyStatus == 1)
                .OrderByDescending(b => b.Recommended_Status)
                .ThenByDescending(b => b.VerifiedStatus);
            return PartialView("_ClientBoosterList", PagingExtensions.ToPagedList(ClientBoosters,currentPageIndex,DefaultPageSize));
        }
        [AllowAnonymous]
        public PartialViewResult search_by_state(int? page, string state)
        {
            currentPageIndex = page.HasValue ? page.Value - 1 : 0;           
           var ClientBoosters = db.BusinessInfoes.Where(b => b.State.Contains(state) && b.ApprovedStatus == "A" && b.NotifyStatus == 1 && b.isServiceBooster == "Yes")
                .OrderByDescending(b => b.Recommended_Status)
                .ThenByDescending(b => b.VerifiedStatus);
               
            return PartialView("_ClientBoosterList", PagingExtensions.ToPagedList(ClientBoosters, currentPageIndex, DefaultPageSize));
        }
        #region
        [AllowAnonymous]
        public ActionResult Hospital_list_for_ING()
        {
            return View();
            //return Redirect("~/PDF.JS/web/viewer.html?File=%2FHospital list for iHealth Networking Members .pdf");
        }
        [AllowAnonymous]
        public ActionResult Hospital_list(string ct)
        {
            var hosplist = db.ING_Hospital_List.Where(h => h.category == ct).OrderBy(h => h.State)
                .Select(h => new Hospital_list_viewModel
                {
                    State = h.State,
                    City = h.City,
                    Hospital = h.Hospital,
                    Address = h.Address,
                    Plans = h.Plans,
                    Speciality = h.Speciality,
                   reg_no = h.reg_no
                });
            TempData["catergory"] = ct;
            return View(hosplist.ToList());

        }
        [AllowAnonymous]
        public ActionResult Health_Providers(string ct, int? page)
        {
            ViewBag.HList = ct;
            ViewBag.Action = "Health_Providers";
            currentPageIndex = page.HasValue ? page.Value : 1;
            var providers = db.ING_Hospital_List.Where(h => h.category == ct).OrderBy(p=>p.Hospital);

            return View(PagedListExtensions.ToPagedList(providers,currentPageIndex,DefaultPageSize));
        }
        [AllowAnonymous]
        
        public ActionResult Search_Health_Providers(int? page, string ct, string searchTerm)
        {
            ViewBag.HList = ct;
            ViewBag.searchTerm = searchTerm;
            ViewBag.Action = "Search_Health_Providers";
            currentPageIndex = page.HasValue ? page.Value : 1;
            var providers = db.ING_Hospital_List.Where(cb => (cb.Hospital.Contains(searchTerm) || cb.City.Contains(searchTerm) ||cb.Speciality.Contains(searchTerm) ||cb.State.Contains(searchTerm)) && cb.category == ct)
                .OrderBy(b => b.Hospital);
            return View("Health_Providers", PagedListExtensions.ToPagedList(providers, currentPageIndex, DefaultPageSize));
        }
        #endregion
        public async Task<ActionResult> Become_a_service_booster(string j)
        {
            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            j = j.Replace("$25", "/");
            j = j.Replace("$24", "+");
            string DecryptId = Cryptoclass.DecryptStringAES(j, s);
            var usr = db.Users.Find(DecryptId);
            usr.isClientBooster = true;
            await db.SaveChangesAsync();
            var urlClientBooster = Url.Action("ClientBooster", "ing",null,protocol:Request.Url.Scheme);
            var urlRegBiz = Url.Action("create_biz", "business", null, protocol: Request.Url.Scheme);
            string body = "<p>Thanks for choosing to join others as a service boosters on our platform. <br/>" +
                "Our service booster package is design to further expose you to both our new and existing subscribers. <a href=\"" + urlClientBooster + "\" target=\"_blank\"> click here  </a> to find out more.<br/>" +
                "We will like to know about your product/service. Kindly, <a href=\""+urlRegBiz+"\" target=\"_blank\"> register </a> your business/company profile with us.<br/>" +
                "We welcome you on board once again.<br/><br/>" +
                "Thanks.<br/><br/>" +
                "Best Regards,<br/><br/>" +
                "IhealthGSFM Team </p>";
            await em.serviceBoosterSignupEmail(usr.LastName,usr.Email,"iHealth Service Boosters",body);
            TempData["success"] = "Your subscription to become a service booster is successfull";
            return RedirectToAction("UserpanelCarreer");
        }
        [AllowAnonymous]
        public ActionResult Health_Insurance_Benefits()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Training_guide()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Networking()
        {
            return View();
        }
        #region // Premium User 
        [AllowAnonymous]
        public ActionResult premium()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult premium_package_signup()
        {
            return View();
        }
        //[AllowAnonymous][HttpPost]
        //public async Task<ActionResult> premium_package_signup(premium_user_Reg_ViewModel model, FormCollection f)
        //{
        //    try
        //    {
        //        Random rd = new Random();
        //        int referenceId = rd.Next(99999999);
        //        //string referenceId = ""
        //        var response = Request["g-recaptcha-response"];
        //        //const string secret = "6LeP9hETAAAAAPhx2dmOL4eB6euPe_hLvOw1UaBH";
        //        string secret = System.Configuration.ConfigurationManager.AppSettings["recaptchaPrivateKey"];
        //        var client = new WebClient();
        //        var reply =
        //        client.DownloadString(
        //        string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
        //        var captchaResponse = JsonConvert.DeserializeObject<reCaptchaClass>(reply);
        //        int con3_id = Convert.ToInt32(f["Nationality"]);
        //        var con3 = await db.country.FindAsync(con3_id);
        //        StringBuilder sb = new StringBuilder();
        //        if (ModelState.IsValid)
        //        {
        //            if (!captchaResponse.Success)
        //            {
        //                if (captchaResponse.ErrorCodes.Count <= 0)
        //                    return View();

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
        //                premium_user puser = new premium_user();
        //                puser.referrence_Id = referenceId.ToString();
        //                puser.First_Name = model.First_Name;
        //                puser.Last_Name = model.Last_Name;
        //                puser.Gender = model.Gender;
        //                puser.D_O_B = Convert.ToDateTime(model.dob).Date;
        //                puser.Email = model.Email;
        //                puser.Phone = model.Phone;
        //                puser.Nationality = con3.CountryName;
        //                puser.State = model.State;
        //                puser.City = model.City;
        //                puser.Address = model.Address;
        //                puser.plan = model.premium_plan.ToString(); ;
        //                puser.Hospital_List = model.Hospital_List;
        //                puser.Amount = model.Price;
        //                puser.Register_Date = DateTime.UtcNow.Date;
        //                puser.sponsorID = model.referralcode;
        //                puser.PaymentStatus = "Pending" ;
        //                puser.paymentMethod = "PayToBank";
        //                puser.NotifyStatus = 0;
        //                db.premium_user.Add(puser);
        //                await db.SaveChangesAsync();
        //                string mailbody = "<p> </p>";
        //                await  em.premiumuserRegEmail(model.Email, "iHealth Premium Subcription", mailbody, model.First_Name);
        //                ViewBag.Message = sb.Append("Your request to be profile as a premium user on iHealth is successful. Kindly, check your email <span style=\"color:Red\">inbox OR spam </span>folder for further information. ").ToString();
        //                return View("Info");
        //            }
                   
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Fill all field mark with (*)");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return View(model);
        //}
        //[AllowAnonymous]
        //public JsonResult get_Plan_Hosp_List(string Id)
        //{
        //    premium_plan_HospitalListModel pHosp_list = new premium_plan_HospitalListModel();

        //  var hosp_list = from hosp in pHosp_list.Get_Hosp_List()
        //                  where hosp.planId == Id
        //                  select hosp;
        //  List<SelectListItem> pHLs = new List<SelectListItem>();
        //  if (hosp_list != null)
        //  {
        //      foreach (var pHL in hosp_list)
        //      {
        //          pHLs.Add(new SelectListItem() { Text = pHL.Hosp_List_Name, Value = pHL.Hosp_List_Id });
        //      }
        //  }
        //  return Json(new SelectList(pHLs, "Value", "Text"));
        //}
        [AllowAnonymous]
        public JsonResult get_Plan_Price_List(string Id)
        {
            Premium_Package_Price pPrice_list = new Premium_Package_Price();

            var Price_list = from p in pPrice_list.Get_Price_List()
                             where p.planID == Id
                             select p;
            List<SelectListItem> pPLs = new List<SelectListItem>();
            if (Price_list != null)
            {
                foreach (var pHL in Price_list)
                {
                    pPLs.Add(new SelectListItem() { Text = pHL.priceName.ToString(), Value = pHL.priceValue.ToString() });
                }
            }
            return Json(new SelectList(pPLs, "Value", "Text"));
        }
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult NewPremiumUser()
        {
            var premiumUser = db.premium_user.Where(pu => pu.NotifyStatus == 0).OrderBy(pu=>pu.Register_Date).ToList();
            return View(premiumUser);
        }
        #endregion
        public ActionResult SBoosterREGCompanyAlert()
        {
            return View();
        }
    }
}