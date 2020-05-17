using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using iHealth2.CustomClasses;
using iHealth2.Models;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Owin.Security;

namespace iHealth2.Controllers.Users
{
    [Authorize(Roles = "Users")]
    public class UsersController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        Emails em = new Emails();

        //string MailFrom = "ihealthng@tamife2016.com";
       // string MailFrom = "noreply@ihealthgsfm.com"; //Domain Email for sending mail to fresh subscribers, newly register business or product
        int acctno = 1018361479;
        string bankName = "United Bank of Africa(UBA)";
        string AcctName = "iHealth Nigeria GSFM Ltd.";
        int amount = 5000;
        public UsersController()
        {

        }
        public UsersController(ApplicationUserManager userManager)
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
        // GET: Users

        public ActionResult Userpanel()
        {
            //string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            //ids = ids.Replace("$25", "/");
            //ids = ids.Replace("$24", "+");
            //string DecryptId = Cryptoclass.DecryptStringAES(ids, s);
            //var usr = UserManager.FindByName(DecryptId);
            //ViewBag.UserName = usr.Name;
            //Session["Ids"] = usr.Id;
            return View();
        }

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
            //sr = db.Users.ToList();
            //{
            //    id = data.Id,
            //    name = data.Name,
            //    TrackingID = data.TrackingID,
            //    Email = data.Email,
            //    phone = data.PhoneNumber

            //};
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

        public ActionResult UgradeForm(string ids)
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
            return View(new UpgradeFormViewModel
            {
                FirstName = usr.FirstName,
                LastName = usr.LastName,
                Email = usr.Email,
                PhoneNumber = usr.PhoneNumber,
                Username = usr.UserName,

            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UgradeForm(UpgradeFormViewModel model, FormCollection f)
        {
            string username = Session["Id"].ToString();
            var usr = UserManager.FindById(username);
            if (usr == null)
            {
                return HttpNotFound("Requested user not found");
            }

            #region
            try
            {


                Random rd = new Random();
                int trackId = rd.Next(10000000);
                var response = Request["g-recaptcha-response"];
                // const string secret = "6LeP9hETAAAAAPhx2dmOL4eB6euPe_hLvOw1UaBH";
                string secret = System.Configuration.ConfigurationManager.AppSettings["recaptchaPrivateKey"];
                var client = new WebClient();
                var reply =
                client.DownloadString(
                string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
                var captchaResponse = JsonConvert.DeserializeObject<reCaptchaClass>(reply);
                StringBuilder sb = new StringBuilder();
                if (ModelState.IsValid)
                {
                    //int profVal = Convert.ToInt32(f["Profession"]);
                    //var prof = await db.Profession.FindAsync(profVal);
                    if (!captchaResponse.Success)
                    {
                        if (captchaResponse.ErrorCodes.Count <= 0)
                            return View(); ;

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

                    }
                    else
                    {
                        string Username = usr.UserName;
                        //the REFURL stores the newly registering User Unique URL which he or She can Share with Others to 
                        // register on Ihealth Networking Insurance
                        var RefUrl = this.Url.Action("INIRegform", "Account",    new { sp = Username }, this.Request.Url.Scheme);
                        //var con3ID = Convert.ToInt32(f["Country"]);
                        //var con3 = await db.country.FindAsync(con3ID);
                        //var st_Id = Convert.ToInt32(f["State"]);
                        //var st = await db.State.FindAsync(st_Id);
                        usr.RegDate = DateTime.UtcNow;
                        usr.NotifyStatus = 0;
                        usr.Gender = model.Gender;
                        //usr.Nationality = con3.CountryName;
                        //usr.State = st.StateName;
                        usr.Refferal_Url = RefUrl;
                        usr.UserType = "INISubcriber";
                        usr.EmailConfirmed = false;
                        //this method Update Normal user records who is upgrading to ING Subcriber status
                        var result = await UserManager.UpdateAsync(usr);
                        if (result.Succeeded)
                        {
                            INISubcriberExtraDetail iniSubExtDetails = new INISubcriberExtraDetail();
                            // If Newly registering Subcriber has a sponsor
                            if (f["RefCode"] != "")
                            {
                                int SpCode = Convert.ToInt32(f["RefCode"]);
                                //Retrieve sponsor Tracking ID
                                var Sponsor = db.Users.First(u => u.MyRefferalCode == SpCode);
                                //Add newly registering Subscriber to INISubscriberExtraDetails tb along with sponsor refcode
                                iniSubExtDetails.My_Sponsor_Referral_Code = SpCode.ToString();
                                ////check if sponsor already exist in d iniSubExtDetails tb, so dt we can give him credit
                                //var sp_IN_mlm = db.INISubcriberExtraDetails.Where(sp => sp.User.Id == Sponsor.Id);
                                //if (sp_IN_mlm.Count() > 0)
                                //{
                                //    try
                                //    {
                                //        INISubcriberExtraDetail spIniSubExtDetails = db.INISubcriberExtraDetails.First(sp => sp.User.Id == Sponsor.Id);
                                //        //this check if the sponsor of this new member subscribe for ING promotion target
                                //        //if yes, we give the sponsor a promo credit for his/her referral within the promo
                                //        //period, ELSE, nothin is added but noth withstanding the dl size of the sponsor 
                                //        // still increases./
                                //        if (spIniSubExtDetails.Promotional_Target_Subscription_Status == "Active")
                                //        {
                                //            spIniSubExtDetails.promo_dl_size = spIniSubExtDetails.promo_dl_size + 1;
                                //        }
                                //        spIniSubExtDetails.Total_dl_Size = spIniSubExtDetails.Total_dl_Size + 1;
                                //        spIniSubExtDetails.Most_recent_DL_Reg_date = DateTime.UtcNow.Date;
                                //        await db.SaveChangesAsync();                                       
                                //    }
                                //    catch (Exception ex)
                                //    {
                                //        throw ex;
                                //    }
                                //}
                                //// Session["sp"] = null;

                            }
                            // If Newly registering Subcriber does not have sponsor

                            // Newly registering Subscriber is added to INISubcriberExtraDetail tb where sponsor id is null
                          
                           
                            if (f["PaymentOptions"] == "PayToBank")
                            {
                                iniSubExtDetails.UserID = usr.Id;
                                iniSubExtDetails.paymentMethod = f["PaymentOptions"];
                                iniSubExtDetails.transaction_Id = string.Concat("ING-", rd.Next(999999));
                                //Payment status is set to pending until the user payment has been confirm from banks.
                                iniSubExtDetails.PaymentStatus = "Pending";
                                iniSubExtDetails.INI_Status = "InActive";
                                iniSubExtDetails.BEstatus = "InActive";
                              
                                //if (f["BenefitCategory"] == "CarreerBenefit")
                                //{
                                    iniSubExtDetails.BenefitCategory = "CarreerBenefit";
                                    //    iniSubExtDetails.MaxDT2MtTarget = DateTime.UtcNow.AddDays(90);
                                //}
                                //else
                                //{
                                //    iniSubExtDetails.BenefitCategory = f["BenefitCategory"];
                                //    iniSubExtDetails.CurrentStage = "Starter";
                                //    iniSubExtDetails.Stage1Benefit = "No Benefit";
                                //    iniSubExtDetails.Stage2Benefit = "No Benefit";
                                //    iniSubExtDetails.Stage3Benefit = "No Benefit";
                                //    iniSubExtDetails.Stage4Benefit = "No Benefit";
                                //    iniSubExtDetails.Stage5Benefit = "No Benefit";
                                //}
                                iniSubExtDetails.CurrentBenefit = "No Benefit";
                                iniSubExtDetails.Total_dl_Size = 0;
                                db.INISubcriberExtraDetails.Add(iniSubExtDetails);
                                await db.SaveChangesAsync();
                                // After Payment has been made and confirm by Ihealth official the Payment status is set to Paid
                                
                                //Assign user to Role if not already added
                                var u = UserManager.FindById(usr.Id);
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
                                await UserManager.SendEmailAsync(usr.Id,
                                  "Ihealth Networking Group", em.Notification_Email_Body_Creator(usr.FullName, "<h2>WELCOME TO IHEALTH NETWORKIING GROUP<h2/>" +
                                  " <p>Thank you for signing up for IHEALTH NETWORKING GROUP.</p> "
                               + "<p>But you are not yet activated. Kindly, pay into the account below:-<br/>" +
                               "ACCOUNT NUMBER: " + acctno + "<br/>ACCOUNT NAME: " + AcctName + "<br/> BANK:" + bankName + " <br/>"
                               + "</p><p>After payment is made, kindly email the information below to <a href=\"mailto:info@ihealthgsfm.com\">info@ihealthgsfm.com</a><br/> (a) Name of Depositor <br/>" +
                            "(b)Teller Number <br/>(c)Your Membership ID: " + usr.MyRefferalCode + " <br/> (d) Amount <br/> (e) Date of Payment</p><br/> Thanks for registering with us<br/><br/>Best Regards, <br/><br/> iHealth Nigeria GSFM Team."));
                                ViewBag.Message = sb.Append("<span style=\"color:green\">Awesome !!!</span> You have Successfully <br/>SIGN UP for <b>iHEALTH NETWORKING <br/>  GROUP</b>.kindly," +
                              " Check your <span style=\"color:Red\">email inbox OR <br/> spam folder</span> for more information ").ToString();
                                AuthenticationManager.SignOut();
                                Session.Clear();
                                return View("Info");
                            }
                            else
                            {
                                iniSubExtDetails.UserID = usr.Id;
                                iniSubExtDetails.paymentMethod = f["PaymentOptions"];
                                iniSubExtDetails.transaction_Id = string.Concat("ING-", rd.Next(999999));
                                iniSubExtDetails.PaymentStatus = "Pending";
                                iniSubExtDetails.INI_Status = "InActive";
                                iniSubExtDetails.BEstatus = "InActive";
                                iniSubExtDetails.paymentMethod = f["PaymentOptions"];
                                iniSubExtDetails.paymentGateway = "Interswitch";
                                iniSubExtDetails.CurrentBenefit = "No Benefit";
                                iniSubExtDetails.Total_dl_Size = 0;
                                iniSubExtDetails.BenefitCategory = "CarreerBenefit";
                                db.INISubcriberExtraDetails.Add(iniSubExtDetails);
                                await db.SaveChangesAsync();
                                InterswitchTransactionsTB iswtb = new InterswitchTransactionsTB();
                                iswtb.Client_Name = usr.FirstName;
                                iswtb.Client_Email = usr.Email;
                                iswtb.Transaction_Amount = amount.ToString();
                                iswtb.Transaction_date = DateTime.Now.Date;
                                iswtb.Transaction_purpose = "Ihealth Networking Group Subscription";
                                iswtb.Transaction_Id = iniSubExtDetails.transaction_Id;
                                db.InterswtichTransactionsTables.Add(iswtb);
                                await db.SaveChangesAsync();
                                TempData["cust_id"] = usr.Id ;
                                TempData["Amount"] = amount;
                                TempData["trans_id"] = iniSubExtDetails.transaction_Id;
                                TempData["cust_name"] = usr.FullName;
                                return Redirect("/PaymentGateways/upgradeGateway");                                
                            }
                        
                        }                
                        AddErrors(result);
                    }
                }
                return View(new UpgradeFormViewModel
                {
                    FirstName = usr.FirstName,
                    LastName = usr.LastName,
                    Email = usr.Email,
                    PhoneNumber = usr.PhoneNumber,
                    Username = usr.UserName,

                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #endregion
        }
        public async Task<ActionResult> Direct_Members(string dm)
        {
            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            dm = dm.Replace("$25", "/");
            dm = dm.Replace("$24", "+");
            string DecryptId = Cryptoclass.DecryptStringAES(dm, s);
            var u = await UserManager.FindByNameAsync(DecryptId);
            var downline = db.IHealthUsersMLM.Where(d => d.MySponsorRefCode == u.MyRefferalCode.ToString());
            return View(downline.ToList());
        }

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