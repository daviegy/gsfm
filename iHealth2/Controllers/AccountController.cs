using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using iHealth2.Models;
using iHealth2.CustomClasses;
using System.Net;
using System.Net.Mail;
using Newtonsoft.Json;
using Elmah;

namespace iHealth2.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        Emails em = new Emails();
        ihealth_constants hc = new ihealth_constants();
        private ApplicationUserManager _userManager;
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            
            return View();
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

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
       // [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (AuthenticationManager.User.Identity.IsAuthenticated)
            {
                AuthenticationManager.SignOut();
                return RedirectToAction("Login");
            }
            System.Web.Helpers.AntiForgery.Validate();
            ApplicationDbContext db = new ApplicationDbContext();
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                // var user = await UserManager.FindAsync(model.Email, model.Password);
                var AuthUserEmail =(!string.IsNullOrEmpty(model.Email))?await UserManager.FindByEmailAsync(model.Email): null;
                var AuthUserUsername =(!string.IsNullOrEmpty(model.Email))? await UserManager.FindByNameAsync(model.Email):null;
                var UnAuthUser = AuthUserEmail ?? AuthUserUsername;
                if (UnAuthUser != null)
                {
                    var user = await UserManager.FindAsync(UnAuthUser.UserName, model.Password);
                    if (user != null)
                    {
                        if (user.EmailConfirmed == true)
                        {
                            var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, false, shouldLockout: false);
                            switch (result)
                            {
                                case SignInStatus.Success:
                                    Session["Name"] = user.FirstName + " " + user.LastName;
                                    Session["Id"] = user.Id;                                    
                                    return RedirectToLocal(returnUrl);
                                case SignInStatus.LockedOut:
                                    return View("Lockout");
                                case SignInStatus.RequiresVerification:
                                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                                case SignInStatus.Failure:
                                default:
                                    ModelState.AddModelError("", "Invalid login attempt.");
                                    return View(model);
                            }
                        }                        
                        else
                        {
                            ViewBag.Error = " YOUR EMAIL ADDRESS HAS NOT YET BEEN CONFIRM, PLEASE KINDLY CLICK THE CONFIRMATION LINK SENT TO YOUR  EMAIL ADDRESS TO CONFIRM YOUR EMAIL, THANKS.";
                            return
                                 View("Error");
                        }
                    }
                }
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                //var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
                //switch (result)
                //{
                //    case SignInStatus.Success:

                //        var user = UserManager.FindByName(model.Email);
                //            //var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                //            if (user != null)
                //            {
                //                Session["Name"] = user.Name;
                //                Session["Id"] = user.Id;
                //            }

                //            return RedirectToLocal(returnUrl);



                //        // var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                //        //if(user != null)
                //        //Session["Name"] = user.Name.ToString() ;
                //     // return RedirectToLocal(returnUrl);
                //    case SignInStatus.LockedOut:
                //        return View("Lockout");
                //    case SignInStatus.RequiresVerification:
                //        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                //    case SignInStatus.Failure:
                //    default:
                //        ModelState.AddModelError("", "Invalid login attempt.");
                //        return View(model);
                //}




            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            var user = await UserManager.FindByIdAsync(await SignInManager.GetVerifiedUserIdAsync());
            if (user != null)
            {
                var code = await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        [HttpGet]
        public ActionResult reg_option()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Register(string sp)
        {
            // Check if the Client requesting this page is been Sponsor by an already Existing Site User
            if (sp != null)
            {
                Session["sp"] = sp;
                var sponsor = db.Users.Where(u => u.UserName == sp).Single();
                Session["spName"] = sponsor.FullName;

            }

            return View();
        }
        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, FormCollection f)
        {
            Session.Clear();
            try
            {
                Random rd = new Random();
                int trackId = rd.Next(10000000);
                var response = Request["g-recaptcha-response"];
                //const string secret = "6LeP9hETAAAAAPhx2dmOL4eB6euPe_hLvOw1UaBH";
                string secret = System.Configuration.ConfigurationManager.AppSettings["recaptchaPrivateKey"];
                var client = new WebClient();
                var reply =
                client.DownloadString(
                string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
                var captchaResponse = JsonConvert.DeserializeObject<reCaptchaClass>(reply);
              
                StringBuilder sb = new StringBuilder();
                if (ModelState.IsValid)
                {
                    #region
                    if (!captchaResponse.Success)
                    {
                        if (captchaResponse.ErrorCodes.Count <= 0)
                            return View();

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
                    #endregion
                    else
                    {
                        #region
                        var A_O_S = f["other_Profession"];

                        if (!string.IsNullOrEmpty(f["subCat1"]))
                        {
                            int subcatId1 = Convert.ToInt32(f["subCat1"]);
                            var sbcat1 = await db.SubCategory1.FindAsync(subcatId1);
                            A_O_S = (A_O_S != "") ? A_O_S + ", " + sbcat1.SubCat1Name : sbcat1.SubCat1Name;
                        }
                        if (!string.IsNullOrEmpty(f["subCat2"]))
                        {
                            int subcatId2 = Convert.ToInt32(f["subCat2"]);
                            var sbcat2 = await db.SubCategory2.FindAsync(subcatId2);
                            A_O_S = (A_O_S != "") ? A_O_S + ", " + sbcat2.SubCat2name : sbcat2.SubCat2name;
                        }

                        string Username = model.UserName;
                        int profVal = (!string.IsNullOrEmpty(f["Profession"])) ? Convert.ToInt32(f["Profession"]) : 0;
                        var prof = (profVal != 0)? await db.Profession.FindAsync(profVal): null;
                        if (!string.IsNullOrEmpty(f["Profession"]))
                        {
                            //int profVal = Convert.ToInt32(f["Profession"]);
                            //prof = await db.Profession.FindAsync(profVal);
                        }
                        //the REFURL stores the newly registering User Unique URL which he or She can Share with Others to 
                        // register on Ihealth
                        var RefUrl = this.Url.Action("Register", "Account", new { sp = Username }, this.Request.Url.Scheme);
                        #endregion
                        var user = new ApplicationUser
                        {
                            UserName = model.UserName,
                            Email = model.Email,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            PhoneNumber = model.Phone,
                            RegDate = Convert.ToDateTime(DateTime.UtcNow.ToShortDateString()),
                            MyRefferalCode = trackId,
                            NotifyStatus = 0,                            
                            AreaOfSpecialization = A_O_S, // area of specialisation
                            Refferal_Url = RefUrl,
                            UserType = "BusinessUser",
                            Health_Service_Provider = f["HSP"],
                            Profession = (f["HSP"] == "Yes" && prof != null)?prof.Nomenclature:"Information_Seeker",
                            subscriptionType = "Freemium",
                            newsletterStatus = 1
                        };
                        var result = await UserManager.CreateAsync(user, model.Password);                     
                        if (result.Succeeded)
                        {
                            #region  // If Newly registering user/subscriber has a sponsor/referral
                            // then implement the IF condition, else implement the ELSE condition
                            if (Session["sp"] != null)
                            {
                                string SpUsrName = Session["sp"].ToString();
                                //Retrieve sponsor/referrer Tracking/referal ID
                                var Sponsor = db.Users.First(u => u.UserName == SpUsrName);
                                //Add newly registering user/subscriber to Business user referal table(i.e. IhealthUserMLM) along with sponsor/referrer TrackingID/ReferalID
                                IHealthUsersMLM ihUserMLM = new IHealthUsersMLM();
                                ihUserMLM.UserID = user.Id;
                                ihUserMLM.MyRefferalCode = trackId;
                                ihUserMLM.MySponsorRefCode = Sponsor.MyRefferalCode.ToString();
                                ihUserMLM.MyDownlineCount = 0;
                                db.IHealthUsersMLM.Add(ihUserMLM);
                                await db.SaveChangesAsync();
                                //check if sponsor/referree already exist in d business user referal table(iheaLthUserMLM),
                                //so dt we can give him/her credit(i.e. increase his/her downline size by 1)
                                var sp_IN_mlm = db.IHealthUsersMLM.Where(sp => sp.MyRefferalCode == Sponsor.MyRefferalCode);
                                if (sp_IN_mlm.Count() > 0)
                                {
                                    IHealthUsersMLM ihSpMLM = db.IHealthUsersMLM.First(sp => sp.MyRefferalCode == Sponsor.MyRefferalCode);
                                    ihSpMLM.MyDownlineCount = ihSpMLM.MyDownlineCount + 1;
                                    await db.SaveChangesAsync();
                                }
                                Session["sp"] = null;// 

                            }
                          
                            else
                            {
                                // Newly reg user  is added to mlm tb where sponsor id is null
                                IHealthUsersMLM ihUserMLM = new IHealthUsersMLM();
                                ihUserMLM.UserID = user.Id;
                                ihUserMLM.MyRefferalCode = trackId;
                                ihUserMLM.MyDownlineCount = 0;
                                db.IHealthUsersMLM.Add(ihUserMLM);
                                await db.SaveChangesAsync();
                            }
                            #endregion
                            //Assign user to Role if not already added
                            var UserRole = UserManager.GetRoles(user.Id);
                            if (!UserRole.Contains("Users"))
                            {
                                var addToRole = UserManager.AddToRole(user.Id, "Users");
                            }
                            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                            var callbackUrl = Url.Action("ConfirmEmail", "Account",
                               new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                            await UserManager.SendEmailAsync(user.Id,
                               "Confirm your account", em.Notification_Email_Body_Creator(user.FullName,"<div style=\"text-align:justify\">" +
                               "<p>Welcome to iHealth Networking Group</p> "
                            + "<p>Please, confirm your account by clicking on this <a href=\""
                               + callbackUrl + "\">link</a> within 24hours. <br/><br/> Best Regards, <br/><br/>iHealth Nigeria GSFM Team.</p></div>"));
                            //// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                            // Send an email with this link
                            //  string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                            //  var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                            //  await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                            ViewBag.Message = sb.Append("Check your <span style=\"color:Red\">email inbox OR spam folder</span> to confirm your account within 24 hours. <strong>You must be confirmed "
                                                    + "before you can log in.<strong>").ToString();
                            return View("Info");
                        }
                        AddErrors(result);
                    }
                }
                // If we got this far, something failed, redisplay form
                return View(model);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region//Ihealth Networking group registration form 

        [AllowAnonymous]
        [HttpGet]
        public ActionResult iniRegformLandingPage()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult INIRegForm(string sp)
        {
            // Check if the Client requesting this page is been Sponsor by an already Existing Site User
            if (sp != null)
            {
                //Session["sp"] = sp;
                var sponsor = db.Users.Where(u => u.UserName == sp).Single();
                Session["spName"] = sponsor.FullName;
                Session["spCode"] = sponsor.MyRefferalCode;

            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous] // Ihealth Form Submission Method
        public async Task<ActionResult> INIRegform(INIRegFormModel model, FormCollection f)
        {
            Session.Clear();
            try
             {
                Random rd = new Random();
                int trackId = rd.Next(10000000);
                // PaymentGateways pg = new PaymentGateways();
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

                    }
                    else
                    {
                      
                        var A_O_S = f["other_Profession"];

                        if (!string.IsNullOrEmpty(f["subCat1"]))
                        {
                            int subcatId1 = Convert.ToInt32(f["subCat1"]);
                            var sbcat1 = await db.SubCategory1.FindAsync(subcatId1);
                            A_O_S = (A_O_S != "") ? A_O_S + ", " + sbcat1.SubCat1Name : sbcat1.SubCat1Name;
                        }
                        if (!string.IsNullOrEmpty(f["subCat2"]))
                        {
                            int subcatId2 = Convert.ToInt32(f["subCat2"]);
                            var sbcat2 = await db.SubCategory2.FindAsync(subcatId2);
                            A_O_S = (A_O_S != "") ? A_O_S + ", " + sbcat2.SubCat2name : sbcat2.SubCat2name;
                        }

                        string Username = model.Username;
                        int profVal = (!string.IsNullOrEmpty(f["Profession"])) ? Convert.ToInt32(f["Profession"]) : 0;
                        var prof = (profVal != 0) ? await db.Profession.FindAsync(profVal) : null;
                        //the REFURL stores the newly registering User Unique URL which he or She can Share with Others to 
                        // register on Ihealth Networking Group
                        var RefUrl = this.Url.Action("INIRegform", "Account", new { sp = Username }, this.Request.Url.Scheme);
                        var con3ID = Convert.ToInt32(f["Country"]);
                        var con3 = await db.country.FindAsync(con3ID);
                        var st_Id = Convert.ToInt32(f["State"]);
                        var st = await db.State.FindAsync(st_Id);
                        INISubcriberExtraDetail iniSubExtDetails = new INISubcriberExtraDetail();
                        var user = new ApplicationUser
                        {
                            UserName = model.Username,
                            Email = model.Email,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            PhoneNumber = model.PhoneNumber,
                            RegDate = DateTime.UtcNow,
                            MyRefferalCode = trackId,
                            NotifyStatus = 0,
                            Gender = model.Gender,
                            Nationality = con3.CountryName,
                            State = st.StateName,
                            Refferal_Url = RefUrl,
                            UserType = "INISubcriber",
                            AreaOfSpecialization = A_O_S, // area of specialisation
                            isClientBooster = (f["isClientBooster"] == "Yes")?true:false,
                            Health_Service_Provider = f["HSP"],
                            Profession = (f["HSP"] == "Yes" && prof != null) ? prof.Nomenclature : "Information_Seeker",
                            subscriptionType = "Freemium",
                            newsletterStatus = 1
                        };
                        var result = await UserManager.CreateAsync(user, model.Password);
                        if (result.Succeeded)
                        {
                            if (f["PaymentOptions"] == "PayToBank")
                            {
                                iniSubExtDetails.UserID = user.Id;
                                iniSubExtDetails.transaction_Id = string.Concat("ING-", rd.Next(999999));
                                //Payment status is set to pending until the user payment has been confirm from banks.
                                iniSubExtDetails.PaymentStatus = "Pending";
                                iniSubExtDetails.INI_Status = "InActive";
                                iniSubExtDetails.BEstatus = "InActive";
                                //iniSubExtDetails.Promotional_Target_Subscription_Status = "InActive";
                                iniSubExtDetails.paymentMethod = f["PaymentOptions"];
                                if (f["RefCode"] != "")
                                {
                                    int SpCode = Convert.ToInt32(f["RefCode"]);
                                    //Retrieve sponsor Tracking ID
                                    var Sponsor = db.Users.First(u => u.MyRefferalCode == SpCode);
                                    iniSubExtDetails.My_Sponsor_Referral_Code = SpCode.ToString();                                    
                                }
                                iniSubExtDetails.BenefitCategory = "CarreerBenefit";   
                                iniSubExtDetails.Total_dl_Size = 0;
                                db.INISubcriberExtraDetails.Add(iniSubExtDetails);
                                await db.SaveChangesAsync();
                                var UserRole = UserManager.GetRoles(user.Id);
                                if (!UserRole.Contains("INIsubscribers"))
                                {
                                    var addToRole = UserManager.AddToRole(user.Id, "INIsubscribers");
                                }
                                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                                await UserManager.SendEmailAsync(user.Id,
                                 "Ihealth Networking Group", em.Notification_Email_Body_Creator(user.FullName, "<div>" +
                                 " <p>Welcome to iHealth Networking Group.</p> "
                              + "<p>But you are not yet activated. Kindly, pay using the information below:-<br/>" +
                              "ACCOUNT NUMBER: " + hc.acctno + "<br/>ACCOUNT NAME:" + hc.AcctName + "<br/> BANK:" + hc.bankName + " <br/>"
                              + "</p><p>After payment kindly email the information below to <a href=\"mailto:info@ihealthgsfm.com\">info@ihealthgsfm.com</a> <br/> (a) Name of Depositor <br/>" +
                           "(b)Teller Number <br/>(c)Your Membership ID : " + user.MyRefferalCode + " <br/> (d) Amount <br/> (e) Date of Payment</p><br/> Thanks for registering with us<br/><br/>Best Regards, <br/><br/> iHealth Nigeria GSFM Team.</div>"));
                                ViewBag.Message = sb.Append("<span style=\"color:green\">Awesome !!!</span> You have Successfully SIGN UP for <b>iHEALTH NETWORKING GROUP</b>.kindly," +
                              " Check your <span style=\"color:Red\">email inbox OR spam folder</span> for more information ").ToString();
                                return View("Info");
                            }
                            else
                            {
                                iniSubExtDetails.UserID = user.Id;
                                iniSubExtDetails.transaction_Id = string.Concat("ING-", rd.Next(999999));
                                //Payment status is set to pending until the user payment has been confirm from banks.
                                iniSubExtDetails.PaymentStatus = "Pending";
                                iniSubExtDetails.INI_Status = "InActive";
                                iniSubExtDetails.BEstatus = "InActive";
                                iniSubExtDetails.paymentMethod = f["PaymentOptions"];
                                iniSubExtDetails.paymentGateway = "Interswitch";
                                if (f["RefCode"] != "")
                                {
                                    int SpCode = Convert.ToInt32(f["RefCode"]);
                                    //Retrieve sponsor Tracking ID
                                    var Sponsor = db.Users.First(u => u.MyRefferalCode == SpCode);
                                    iniSubExtDetails.My_Sponsor_Referral_Code = SpCode.ToString();
                                }
                                iniSubExtDetails.BenefitCategory = "CarreerBenefit";
                                db.INISubcriberExtraDetails.Add(iniSubExtDetails);
                                await db.SaveChangesAsync();
                                InterswitchTransactionsTB iswtb = new InterswitchTransactionsTB();
                                iswtb.Client_Name = user.FirstName;
                                iswtb.Client_Email = user.Email;
                                iswtb.Transaction_Amount = hc.amount.ToString();
                                iswtb.Transaction_date = DateTime.Now.Date;
                                iswtb.Transaction_purpose = "Ihealth Networking Group Subscription";
                                iswtb.Transaction_Id = iniSubExtDetails.transaction_Id;
                                db.InterswtichTransactionsTables.Add(iswtb);
                                await db.SaveChangesAsync();
                                TempData["cust_id"] = user.Id;
                                TempData["Amount"] = hc.amount;
                                TempData["trans_id"] = iniSubExtDetails.transaction_Id;
                                TempData["cust_name"] = user.FullName;
                                TempData["Email"] = user.Email;
                                return Redirect("/PaymentGateways/pGateway");
                            }

                        } AddErrors(result);
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [AllowAnonymous]
        public async Task<ActionResult> cancelRegistration(string t)
        {
            var newuser =  db.Users.Find(t);
            var inidetatils = await db.INISubcriberExtraDetails.FindAsync(t);
            db.INISubcriberExtraDetails.Remove(inidetatils);
            db.Users.Remove(newuser);
            await db.SaveChangesAsync();
            return Redirect("/account/iniregform");
        }
        #endregion

        [AllowAnonymous]
        public JsonResult IsUserExist(string UserName)
        {
            var usernm = db.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (usernm != null)
            {
                return Json("Username already exist, Please type a new one", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        [AllowAnonymous]
        public JsonResult CheckEmail(string Email)
        {
            var mail = db.Users.Where(u => u.Email.Equals(Email, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (mail != null)
            {
                return Json("Email already exist, Please type a new one", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
      
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
              if (userId == null || code == null)
                {
                    HttpStatusCode.BadRequest.ToString();
                    return View("ErrorAlert");
                }
                var result = await UserManager.ConfirmEmailAsync(userId, code);
                if (result.Succeeded)
                {
                    var member = db.Users.Find(userId);
                    if (member.UserType == "INISubcriber")
                    {
                        INISubcriberExtraDetail inimember = await db.INISubcriberExtraDetails.FindAsync(member.Id);
                        inimember.Promo_start_date = DateTime.UtcNow.Date;
                        inimember.Promo_end_date = DateTime.UtcNow.Date.AddDays(30);
                        inimember.Promotional_Target_Subscription_Status = "Active";
                        await db.SaveChangesAsync();
                    }
                    sendRef(userId);
                    return View("ConfirmEmail");
                }
                else
                {
                   ViewBag.err=   HttpStatusCode.BadRequest.ToString();
                   return View("ErrorAlert");
                }
           
          
           
            //return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        // // Send Referal Link to Registered Users
        public void sendRef(string uid)
        {
            var user = db.Users.Find(uid);
            if (user != null)
            {
                // string body = 
                try
                {
                    if (user.UserType == "INISubcriber")
                    {
                         MailAddress
           maFrom = new MailAddress(hc.MailFrom, "iHealth", Encoding.UTF8),

           maTo = new MailAddress(user.Email, user.Email, Encoding.UTF8);

                         MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);

                         mail.Subject = "iHealth Networking Group";
                         mail.Body = em.Notification_Email_Body_Creator(user.FullName, "<div><p>Your online iHealth fortune account has been opened successfully, you can now market or buy registered and quality Healthcare products and services at discounted prices. We will keep record of all your sells and bonuses using your reference ID ("+user.MyRefferalCode+") <p/>"
                               + "<p>You can also make more money by referring your friends and loved ones to join us by sharing this Link <br/> <a href=\"" + user.Refferal_Url + "\"> " + user.Refferal_Url + "</a></p>"
                                  + "<p>Note: You will have the option of either to benefit from our annual group health insurance plan OR to gain additional 10% cash bonus with your normal 50% entitlement when you refer 2 persons and above within 30days. </p>"+
                                  "<p>We wish you a long and successful networking career with us and assure you of our support when necessary</p>"
                              +"<p>Welcome to iHealth Networking Group. <br/><br/> Sincerely, <br/><br/> iHealth Nigeria GSFM Team.</p></div>");

                         mail.BodyEncoding = Encoding.UTF8;
                         mail.SubjectEncoding = Encoding.UTF8;
                         mail.IsBodyHtml = true;
                         mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                         NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                         hc.smtp.Credentials = Credentials; hc.smtp.EnableSsl = true;
                         hc.smtp.Send(mail);
                    }
                    else
                    {
                        MailAddress
           maFrom = new MailAddress(hc.MailFrom, "iHealth", Encoding.UTF8),

           maTo = new MailAddress(user.Email, user.Email, Encoding.UTF8);

                        MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);
                        mail.Subject = "Ihealth";
                        mail.Body = em.Notification_Email_Body_Creator(user.FullName, "<p style=\"font-size: large; text-align: justify\">Thank you for registering with iHealth Nigeria GSFM.<br/> To know more about iHealth Nigeria GSFM kindly <a href =\"" + Url.Action("Index", "Home", routeValues: null, protocol: Request.Url.Scheme) + "\"> Click Here </a>  </p>" +
                      "<p style=\"font-size: large; text-align: justify\">You can get amazing benefits by upgrading to our iHealth Networking Group.<br/> For information on how to upgrade, kindly login to your dashboard." +
                      "<br/><br/>Best Regards,<br/><br/> iHealth Nigeria GSFM Team. </p>");
                        mail.BodyEncoding = Encoding.UTF8;
                        mail.SubjectEncoding = Encoding.UTF8;
                        mail.IsBodyHtml = true;
                        mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                        NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                        hc.smtp.Credentials = Credentials; hc.smtp.EnableSsl = true;
                        hc.smtp.Send(mail);
                    }
                    
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }
        }

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", em.Notification_Email_Body_Creator(user.FullName,"<p>Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>.</p>"+
                    "<p> If you are not the one that initiated this request, kindly ignore this message.</p><br/><br/> Best Regards, <br/><br/> iHealth Nigeria Gsfm Team. "));
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("ErrorAlert") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }
        // the change password is a platform for autorize user to change password anytime
        [Authorize]
        public async Task<ActionResult> Changepassword(string ids)
        {
            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            ids = ids.Replace("$25", "/");
            ids = ids.Replace("$24", "+");
            string DecryptId = Cryptoclass.DecryptStringAES(ids, s);
            var usr = UserManager.FindByName(DecryptId);
            if (usr == null || !(await UserManager.IsEmailConfirmedAsync(usr.Id)))
            {
                return HttpNotFound("Requested user not found");
            }
            string code = await UserManager.GeneratePasswordResetTokenAsync(usr.Id);
            return View(new ResetPasswordViewModel
            {
                Code = code,
                Email = usr.Email
            });
        }
       [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> changepassword(ResetPasswordViewModel model)
        {
            var url = Request.UrlReferrer.ToString();
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                TempData["error"] = "Password Reset Fail.";
                return RedirectToAction("index", "admin");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                TempData["success"] = "Password Reset successfully.";
                return Redirect(url);
            }

            return View();
        }
        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}
