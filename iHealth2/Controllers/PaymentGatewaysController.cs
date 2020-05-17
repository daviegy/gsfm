using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net;
using iHealth2.CustomClasses;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using iHealth2.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace iHealth2.Controllers
{
    public class PaymentGatewaysController : Controller
    {
        //HttpClient client;
        ApplicationDbContext db = new ApplicationDbContext();
        Emails em = new Emails();
        ihealth_constants hc = new ihealth_constants();
        #region //text environment
        //string hc.product_id = "1076";
        //string hc.Mac_key = "D3D1D05AFE42AD50818167EAC73C109168A0F108F32645C8B59E897FA930DA44F9230910DAC9E20641823799A107A02068F7BC0F4CC41D2952E249552255710F";
        //string hc.interswitchJson = "https://sandbox.interswitchng.com/collections/api/v1/gettransaction.json?";
        #endregion
        //GET: PaymentGateways
        public PaymentGatewaysController() { }
        public PaymentGatewaysController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        private ApplicationUserManager _userManager;
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
        public ActionResult pGateway()
        {
            return View();
        }
        public ActionResult upgradeGateway()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ing_pmt_response_status(string txnref, string apprAmt)
        {
            try
            {
                var input = string.Concat(hc.product_id, txnref, hc.Mac_key);
                var hash = Cryptoclass.GenerateSHA512String(input);
                string response_url = hc.interswitchJson + txnref + "&amount=" + apprAmt + "";
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new WebClient();
                client.Headers.Add("Hash", hash);
                var content = client.DownloadString(response_url);
                // Create the Json serializer and parse the response
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PaymentResponse));
                using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(content)))
                {
                   
                        var pmtData = (PaymentResponse)serializer.ReadObject(ms);

                    if (pmtData.ResponseCode == "00")
                    {
                        INISubcriberExtraDetail iniSubExtDetails = db.INISubcriberExtraDetails.Single(s => s.transaction_Id == txnref);
                        iniSubExtDetails.PaymentStatus = "Paid";
                        iniSubExtDetails.INI_Status = "ACTIVE";
                        iniSubExtDetails.BEstatus = "Active";
                        iniSubExtDetails.Transaction_date = Convert.ToDateTime(pmtData.TransactionDate);
                        iniSubExtDetails.Amounts = Convert.ToInt32(apprAmt);
                        iniSubExtDetails.Amounts /= 100;
                        //check if the new member has a sponsor so that we can give the sponsor bonus
                        if (!string.IsNullOrEmpty(iniSubExtDetails.My_Sponsor_Referral_Code))
                        {
                            //retrieve the sponsor of this new member details from the users table
                            var sponsor = db.Users.Single(u => u.MyRefferalCode.ToString() == iniSubExtDetails.My_Sponsor_Referral_Code);
                            //check if sponsor is an ihealth networking subscriber, so dt we can give him credit
                            var sp_IN_mlm = db.INISubcriberExtraDetails.Single(sp => sp.User.Id == sponsor.Id);
                                                   
                                //this check if the sponsor of this new member subscribe for ING promotion target
                                //if yes, we increase the number of downlines within the promo period
                                //ELSE, we will only increase the number of downlines outside the promo period. 
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
                            referral_BonusTb refbonus = new referral_BonusTb();
                            refbonus.user_ID = sponsor.Id;
                            refbonus.Downline_Id = iniSubExtDetails.User.MyRefferalCode;
                            refbonus.Downline_Name = iniSubExtDetails.User.FirstName + " " + iniSubExtDetails.User.LastName;
                            refbonus.Bonus_Type = "Ihealth Networking Referral Bonus";
                            refbonus.Subscription_Fee = iniSubExtDetails.Amounts;
                            refbonus.Bonus = refbonus.Subscription_Fee * 0.5; // bonus is 50% of the subscription                           
                            //refbonus.totalBonus = refbonus.totalBonus + refbonus.Bonus;
                            refbonus.Bonus_created_date = DateTime.UtcNow;
                            //check if the sponsor of this new member is currently subscribe to ING promo target
                            if (sp_IN_mlm.Promotional_Target_Subscription_Status == "Active")
                            {
                                refbonus.promo_period_bonus = true;
                            }
                            db.referral_bonus_tb.Add(refbonus);
                            await db.SaveChangesAsync();
                            sp_IN_mlm.CurrentBonus = sp_IN_mlm.CurrentBonus + (refbonus.Subscription_Fee * 0.5);
                            await db.SaveChangesAsync();
                        }
                        await db.SaveChangesAsync();
                        var iswtb = db.InterswtichTransactionsTables.Single(s => s.Transaction_Id == txnref);
                        iswtb.Transaction_ResponseCode = pmtData.ResponseCode;
                        iswtb.Transaction_Response = pmtData.ResponseDescription;
                        await db.SaveChangesAsync();

                        StringBuilder sb = new StringBuilder();
                        //Assign user to Role if not already added
                        var UserRole = UserManager.GetRoles(iniSubExtDetails.User.Id);
                        if (!UserRole.Contains("INIsubscribers"))
                        {
                            var addToRole = UserManager.AddToRole(iniSubExtDetails.User.Id, "INIsubscribers");
                        }
                        await SignInManager.SignInAsync(iniSubExtDetails.User, isPersistent: false, rememberBrowser: false);
                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(iniSubExtDetails.User.Id);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account",
                           new { userId = iniSubExtDetails.User.Id, code = code }, protocol: Request.Url.Scheme);
                        await UserManager.SendEmailAsync(iniSubExtDetails.User.Id,
                           "Confirm your account", em.Notification_Email_Body_Creator(iniSubExtDetails.User.FullName, "<div><p style=\" font-size: large\">Welcome to iHealth Networking Group, please kindly confirm your account by clicking on this <a href=\""
                           + callbackUrl + "\"> here.</a><br/><br/>Thanks for registering with us.<br/><br/> Best Regards, <br/><br/>iHealth Nigeria GSFM Team.</p></div>"));
                        ViewBag.tRef = txnref;
                        ViewBag.pRef = pmtData.PaymentReference;
                        ViewBag.Message = sb.Append("<p><span style=\"color:green\">Awesome !!!</span> You have Successfully SIGN UP for <b>iHEALTH NETWORKING INSURANCE</b>. kindly," +
                       "Check your <span style=\"color:Red\">email inbox OR spam folder</span> to confirm your account within 24 hours.<strong> You must be confirmed "
                                                + "before you can log in.<strong></p>").ToString();
                        return View("payment_Status");
                        // return PartialView("_PaymentSuccess");
                    }                   
                    else
                    {
                        var iswtb = db.InterswtichTransactionsTables.Single(s => s.Transaction_Id == txnref);
                        iswtb.Transaction_Response = pmtData.ResponseDescription;
                        iswtb.Transaction_ResponseCode = pmtData.ResponseCode;
                        await db.SaveChangesAsync();
                        INISubcriberExtraDetail inidetatils = db.INISubcriberExtraDetails.Single(s => s.transaction_Id == txnref);
                        if (inidetatils.PaymentStatus != "Paid")
                        {
                           
                            //remove about to registered member from community, since they refuse to pay online
                            ApplicationUser newuser = db.Users.Find(inidetatils.User.Id);
                            db.INISubcriberExtraDetails.Remove(inidetatils);
                            db.Users.Remove(newuser);
                            await db.SaveChangesAsync();

                            ViewBag.responseMsg = pmtData.ResponseDescription;
                            ViewBag.tRef = txnref;
                            ViewBag.pRef = pmtData.PaymentReference;
                            ViewBag.partial = "_PaymentFail";
                            return View("payment_Status");
                        }
                        else
                        {
                            return RedirectToAction("index", "home");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpPost]
        public async Task<ActionResult> upg_pmt_status(string txnref, string apprAmt)
        {
            try
            {
                var input = string.Concat(hc.product_id, txnref, hc.Mac_key);
                var hash = Cryptoclass.GenerateSHA512String(input);
                string response_url = "https://webpay.interswitchng.com/collections/api/v1/gettransaction.json?productid=8308&transactionreference=" + txnref + "&amount=" + apprAmt + "";
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new WebClient();
                client.Headers.Add("Hash", hash);
                var content = client.DownloadString(response_url);
                // Create the Json serializer and parse the response
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PaymentResponse));
                using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(content)))
                {
                    var pmtData = (PaymentResponse)serializer.ReadObject(ms);

                    if (pmtData.ResponseCode == "00")
                    {
                        INISubcriberExtraDetail iniSubExtDetails = db.INISubcriberExtraDetails.Single(s => s.transaction_Id == txnref);
                        iniSubExtDetails.PaymentStatus = "Paid";
                        iniSubExtDetails.INI_Status = "ACTIVE";
                        iniSubExtDetails.BEstatus = "Active";
                        iniSubExtDetails.Transaction_date = Convert.ToDateTime(pmtData.TransactionDate);
                        iniSubExtDetails.Amounts = Convert.ToInt32(apprAmt);
                        iniSubExtDetails.Amounts /= 100;
                        //check if the new member has a sponsor so that we can give the sponsor bonus
                        if (!string.IsNullOrEmpty(iniSubExtDetails.My_Sponsor_Referral_Code))
                        {
                            //retrieve the sponsor of this new member details from the users table
                            var sponsor = db.Users.Single(sp => sp.MyRefferalCode.ToString() == iniSubExtDetails.My_Sponsor_Referral_Code);
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
                            refbonus.Downline_Id = iniSubExtDetails.User.MyRefferalCode;
                            refbonus.Downline_Name = iniSubExtDetails.User.FirstName + " " + iniSubExtDetails.User.LastName;
                            refbonus.Bonus_Type = "Ihealth Networking Referral Bonus";
                            refbonus.Subscription_Fee = iniSubExtDetails.Amounts;
                            refbonus.Bonus = refbonus.Subscription_Fee * 0.5; // bonus is 50% of the subscription                           
                            //refbonus.totalBonus = refbonus.totalBonus + refbonus.Bonus;
                            refbonus.Bonus_created_date = DateTime.UtcNow;
                            //check if the sponsor of this new member is currently subscribe to ING promo target
                            if (sp_IN_mlm.Promotional_Target_Subscription_Status == "Active")
                            {
                                refbonus.promo_period_bonus = true;
                            }
                            db.referral_bonus_tb.Add(refbonus);
                            await db.SaveChangesAsync();
                            sp_IN_mlm.CurrentBonus = sp_IN_mlm.CurrentBonus + (refbonus.Subscription_Fee * 0.5);
                            await db.SaveChangesAsync();
                        }
                        await db.SaveChangesAsync();
                        var iswtb = db.InterswtichTransactionsTables.Single(s => s.Transaction_Id == txnref);
                        iswtb.Transaction_ResponseCode = pmtData.ResponseCode;
                        iswtb.Transaction_Response = pmtData.ResponseDescription;
                        await db.SaveChangesAsync();
                        StringBuilder sb = new StringBuilder();
                        //Assign user to Role if not already added
                        var u = UserManager.FindById(iniSubExtDetails.User.Id);
                        //this next lines of code delete the newly upgraded user details from IhealthUserMLM table
                        //Note: the ihealthUserMLM table is the tb that store normal users downlines size
                        IHealthUsersMLM delUserFromThisTable = await db.IHealthUsersMLM.FindAsync(u.Id);
                        db.IHealthUsersMLM.Remove(delUserFromThisTable);
                        await db.SaveChangesAsync();

                        //this next line of codes modify upgrading user roles from User to INIsubscribers
                        var uOldRoleID = u.Roles.SingleOrDefault().RoleId;
                        var uOldRoleName = db.Roles.SingleOrDefault(r => r.Id == uOldRoleID).Name;
                        if (uOldRoleName != "INIsubscribers")
                        {
                            UserManager.RemoveFromRole(u.Id, uOldRoleName);
                            var addrole = await UserManager.AddToRoleAsync(u.Id, "INIsubscribers");
                        }
                        await SignInManager.SignInAsync(iniSubExtDetails.User, isPersistent: false, rememberBrowser: false);
                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(iniSubExtDetails.User.Id);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account",
                           new { userId = iniSubExtDetails.User.Id, code = code }, protocol: Request.Url.Scheme);
                        await UserManager.SendEmailAsync(iniSubExtDetails.User.Id,
                           "Confirm your account", em.Notification_Email_Body_Creator(iniSubExtDetails.User.FullName, "<div><p style=\" font-size: large\">Welcome to iHealth Networking Group, please kindly confirm your account by clicking on this <a href=\""
                           + callbackUrl + "\"> here.</a><br/><br/>Thanks for registering with us.<br/><br/> Best Regards, <br/><br/>iHealth Nigeria GSFM Team.</p></div>"));
                        ViewBag.tRef = txnref;
                        ViewBag.pRef = pmtData.PaymentReference;
                        ViewBag.Message = sb.Append("<span style=\"color:green\">Awesome !!!</span> You have Successfully SIGN UP for <b>iHEALTH NETWORKING INSURANCE</b>. kindly," +
                       "Check your <span style=\"color:Red\">email inbox OR spam folder</span> to confirm your account within 24 hours.<strong> You must be confirmed "
                                                + "before you can log in.<strong>").ToString();

                        AuthenticationManager.SignOut();
                        Session.Clear();
                        return View("payment_Status");

                    }
                    else
                    {
                        var iswtb = db.InterswtichTransactionsTables.Single(s => s.Transaction_Id == txnref);
                        iswtb.Transaction_Response = pmtData.ResponseDescription;
                        iswtb.Transaction_ResponseCode = pmtData.ResponseCode;
                        await db.SaveChangesAsync();
                        INISubcriberExtraDetail inidetatils = db.INISubcriberExtraDetails.Single(s => s.transaction_Id == txnref);
                        if (inidetatils.PaymentStatus != "Paid")
                        {
                            //remove about to registered member from the Ihealth Netwoorking community, since they refuse to pay online
                            db.INISubcriberExtraDetails.Remove(inidetatils);
                            await db.SaveChangesAsync();
                            ViewBag.responseMsg = pmtData.ResponseDescription;
                            ViewBag.tRef = txnref;
                            ViewBag.pRef = pmtData.PaymentReference;
                            ViewBag.partial = "_PaymentFail";
                            AuthenticationManager.SignOut();
                            Session.Clear();
                            return View("payment_Status");
                        }
                        else
                        {
                            return RedirectToAction("index", "home");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult payment_Status()
        {
            return View();
        }
        //this Method Throws Error message
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
    }
}