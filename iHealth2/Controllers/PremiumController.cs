using iHealth2.CustomClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using iHealth2.Models;

using System.Threading.Tasks;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using ClosedXML.Excel;
using System.Globalization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
namespace iHealth2.Controllers
{
    public class PremiumController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        EmailSender ems = new EmailSender();
        Emails em = new Emails();
        string plan;
        [AllowAnonymous]
        public ActionResult About()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult premium_package_signup(String plan)
        {
            ViewBag.plan = plan;
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> premium_package_signup(premium_user_Reg_ViewModel model, FormCollection f)
        {
            try
            {
                Random rd = new Random();
                int referenceId = rd.Next(99999999);
                //string referenceId = ""
                var response = Request["g-recaptcha-response"];
                //const string secret = "6LeP9hETAAAAAPhx2dmOL4eB6euPe_hLvOw1UaBH";
                string secret = System.Configuration.ConfigurationManager.AppSettings["recaptchaPrivateKey"];
                var client = new WebClient();
                var reply =
                client.DownloadString(
                string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
                var captchaResponse = JsonConvert.DeserializeObject<reCaptchaClass>(reply);
                int con3_id = Convert.ToInt32(f["Nationality"]);
                var con3 = await db.country.FindAsync(con3_id);
                int stid = Convert.ToInt32(model.State);
                var st = await db.State.FindAsync(stid);
                int sponsorid = !string.IsNullOrEmpty(f["RefCode"])? Convert.ToInt32(f["RefCode"]): 0;
                DateTime dob = DateTime.ParseExact(model.dob, "dd-MM-yyyy", CultureInfo.CurrentCulture); 
                StringBuilder sb = new StringBuilder();               
                //check if the person registering is an ING member
                var is_ING_member = db.Users.Where(u => u.Email.Equals(model.Email, StringComparison.CurrentCultureIgnoreCase) && u.UserType == "INISubcriber");                
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
                    #region
                    else
                    {
                        
                        premium_user puser = new premium_user();
                        puser.premiumUserId = Guid.NewGuid().ToString();
                        puser.referrence_Id = referenceId.ToString();
                        puser.First_Name = model.First_Name;
                        puser.Last_Name = model.Last_Name;
                        puser.Gender = model.Gender;
                        puser.D_O_B = dob;
                        puser.Occupation = model.job;
                        puser.Email = model.Email;
                        puser.is_ING_Member = (is_ING_member.Count() > 0) ? true : false;
                        puser.Phone = model.Phone;
                        puser.Nationality = con3.CountryName;
                        puser.State = st.StateName;
                        puser.City = model.City;
                        puser.Address = model.Address;
                        puser.plan = model.premium_plan.planID.ToString();
                        puser.Hospital_List = model.Hospital_List;                        
                        puser.Amount = model.Price;
                        puser.Register_Date = DateTime.UtcNow;
                        puser.sponsorID = (is_ING_member.Count() > 0) ? 0 : sponsorid;
                        puser.PaymentStatus = "Pending";
                        puser.paymentMethod = "PayToBank";
                        puser.NotifyStatus = 0;
                        db.premium_user.Add(puser);
                        await db.SaveChangesAsync();
                        string hplist = puser.Hospital_List.Replace("_", " ");
                        var urlHosplist = Url.Action("Hospital_list_for_ING", "ing", null, protocol: Request.Url.Scheme);
                        string mailbody = "<p>You have been profiled as a premium user on iHealth Nigeria GSFM so as to enjoy our group health insurance plan. </p>" +
                            "<p>Find below the details of the plan you have been profiled for: <br/>  Premium Plan: " + puser.plan + " <br/>Hospital List: " + hplist + " <a href=\"" + urlHosplist + "\">click here</a> to find out more about your hospital list." +
                            "<br/>Amount: ₦" + String.Format("{0:n0}", puser.Amount) + " <br/>Reference ID: " + referenceId + "</p>" +
                            "<p>Kindly, send your proof of payment to <a href=\"mailto:support@ihealthgsfm.com\">support@ihealthgsfm.com</a> with the following details:<br/> Reference ID <br/>Depositor/Sender's Name<br/>Transaction ID<br/>Amount<br/>Teller Number(Optional)<br/>" +
                            "Date of payment<br/>Time of payment</p><p>Thank you. <br/><br/> Best Regards,<br/><br/>iHealthGSFM Team</p>";
                        await em.premiumuserRegEmail(model.Email, "iHealth Premium Subcription", mailbody, model.First_Name);
                        ViewBag.Message = sb.Append("Your request to be profile as a premium user on iHealth is successful. Kindly, check your email <span style=\"color:Red\">inbox OR spam </span>folder for further information. ").ToString();
                        return View("Info");
                    }
                    #endregion
                }
                else
                {
                    ModelState.AddModelError("", "Fill all field mark with (*)");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(model);
        }
        
        [AllowAnonymous]
        public JsonResult get_Hosp_List(string PlanId, int? loc)
        {
           
            if (!string.IsNullOrEmpty(PlanId))
            {
                switch (PlanId)
                {
                    case "FHLPA":
                        plan = "First_Hospital_List";
                        break;
                    case "FHLPB":
                        plan = "First_Hospital_List";
                        break;
                    case "SHLPA":
                        plan = "second_hospital_List";
                        break;
                    case "SHLPB":
                        plan = "second_hospital_List";
                        break;
                    default:
                        plan = "High_Class_Networker";
                        break;
                }
            }
            //|| h.State.Contains(state)
            string stateName = (loc != 0 && loc != null) ? db.State.Find(loc).StateName : null;
            stateName = (!string.IsNullOrEmpty(stateName) && stateName.ToLower() == "Fct, Abuja".ToLower()) ? "Abuja" : stateName;           
            var Hosp_list = !string.IsNullOrEmpty(stateName) ? db.ING_Hospital_List
                .Where(h => h.category.ToLower() == plan.ToLower() && h.State.Contains(stateName)).ToList()
                : db.ING_Hospital_List.Where(h => h.category.ToLower() == plan.ToLower()).ToList();           
            List<SelectListItem> pHLs = new List<SelectListItem>();
            if (Hosp_list.Count>0)
            {
                foreach (var pHL in Hosp_list)
                {
                    pHLs.Add(new SelectListItem() { Text = pHL.Hospital +", "+ pHL.City +" "+ pHL.State, Value = pHL.Hospital + ", " + pHL.City + " " + pHL.State });
                }
            }
            return Json(new SelectList(pHLs, "Value", "Text"));
        }
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
        #region // Admin section for Premium User
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult NewPremiumUser()
        {
            var premiumUser = db.premium_user.Where(pu => pu.NotifyStatus == 0 || pu.PaymentStatus == "Pending").OrderBy(pu => pu.Register_Date).ToList();
            return View(premiumUser);
        }
        [Authorize(Roles = "Admin,SuperAdmin")]// those who have not paid
        public ActionResult pendingPaymentPremiumUser()
        {
            var premiumUser = db.premium_user.Where(pu => pu.PaymentStatus == "Pending").OrderBy(pu => pu.Register_Date).ToList();
            return View(premiumUser);
        }
         [Authorize(Roles = "Admin,SuperAdmin")]//those who have paid
        public ActionResult confirmedPaymentPremiumUsers()
        {
            var premiumUser = db.premium_user.Where(pu => pu.PaymentStatus == "Paid").OrderBy(pu => pu.Register_Date).ToList();
            return View(premiumUser);
        }
         [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<JsonResult> ApprovePayment(IEnumerable<string> markAsPaidbyId)
         {
            try
            {
                #region
                if (markAsPaidbyId != null)
                {
                    if (markAsPaidbyId.Count() > 0)
                    {
                        foreach (var id in markAsPaidbyId)
                        {
                            var usrpaid = db.premium_user.Single(u => u.premiumUserId == id);
                            //check if the premium client's email already exist in registered users table is an ING member
                            var is_Premium_User_Registered_in_all_user_db = db.Users.Where(u => u.Email.Equals(usrpaid.Email, StringComparison.CurrentCultureIgnoreCase));
                            if (is_Premium_User_Registered_in_all_user_db.Count() <= 0)
                            {
                                Random rd = new Random();
                                int myReferralCode = rd.Next(10000000);
                                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                                PasswordHasher pdH = new PasswordHasher();
                                AlphaNumGen anGen = new AlphaNumGen();
                                string password = anGen.GetPassword();
                                string username = string.Concat(usrpaid.First_Name.Substring(0,1), usrpaid.Last_Name);
                                var RefUrl = this.Url.Action("Register", "Account", new { sp = username }, this.Request.Url.Scheme);
                                ApplicationUser rgUser = new ApplicationUser();
                                rgUser.FirstName = usrpaid.First_Name;
                                rgUser.LastName = usrpaid.Last_Name;
                                rgUser.Email = usrpaid.Email;
                                rgUser.PhoneNumber = usrpaid.Phone;
                                rgUser.Address = usrpaid.Address;
                                rgUser.City = usrpaid.City;
                                rgUser.State = usrpaid.State;
                                rgUser.Gender = usrpaid.Gender;
                                rgUser.Health_Service_Provider = "No";
                                rgUser.isClientBooster = false;
                                rgUser.UserType = "BusinessUser";
                                rgUser.NotifyStatus = 1;
                                rgUser.Nationality = usrpaid.Nationality;
                                rgUser.MyRefferalCode = myReferralCode;
                                rgUser.subscriptionType = "Health_Insurance";
                                rgUser.UserName = username;
                                rgUser.Refferal_Url = RefUrl;
                                rgUser.PasswordHash = pdH.HashPassword(password);
                                rgUser.SecurityStamp = Guid.NewGuid().ToString();
                                rgUser.RegDate = DateTime.UtcNow;
                                rgUser.EmailConfirmed = true;
                                db.Users.Add(rgUser);
                                await db.SaveChangesAsync();
                                rgUser = userManager.FindByName(rgUser.UserName);
                                if (rgUser != null)
                                {
                                    var addtorole = userManager.AddToRole(rgUser.Id, "Users");
                                }
                                IHealthUsersMLM iHmlm = new IHealthUsersMLM();
                                iHmlm.UserID = rgUser.Id;
                                iHmlm.MyRefferalCode = myReferralCode;
                                iHmlm.MySponsorRefCode = usrpaid.sponsorID.ToString();
                                iHmlm.MyDownlineCount = 0;
                                db.IHealthUsersMLM.Add(iHmlm);
                                await db.SaveChangesAsync();                               
                                
                                #region //Confirm payment
                                usrpaid.PaymentStatus = "Paid";
                                usrpaid.Transaction_date = DateTime.UtcNow;
                                usrpaid.SignedBy = Session["Name"].ToString();
                                usrpaid.NotifyStatus = 1;
                                //  usrpaid.INISubcriberExtraDetails.MaxDT2MtTarget = DateTime.UtcNow.AddDays(30);
                                await db.SaveChangesAsync();
                                //check if the new member has a sponsor so that we can give the sponsor bonus
                                if (usrpaid.sponsorID != 0)
                                {
                                    //retrieve the sponsor of this new registrant details from the all users table
                                    var sponsor = db.Users.Single(u => u.MyRefferalCode.ToString() == usrpaid.sponsorID.ToString());
                                    //check if sponsor is an ihealth networking subscriber, so dt we can give him credit
                                    var sp_IN_mlm = db.INISubcriberExtraDetails.Single(sp => sp.User.Id == sponsor.Id);
                                    referral_BonusTb refbonus = new referral_BonusTb();
                                    refbonus.user_ID = sponsor.Id;
                                    // refbonus.Downline_Id = usrpaid.INISubcriberExtraDetails.User.MyRefferalCode;
                                    refbonus.Downline_Name = usrpaid.First_Name + " " + usrpaid.Last_Name;
                                    refbonus.Bonus_Type = "Premium Referral Bonus";
                                    refbonus.Subscription_Fee = usrpaid.Amount;
                                    refbonus.Bonus_created_date = DateTime.UtcNow;
                                    refbonus.Bonus = refbonus.Subscription_Fee * 0.05; // bonus is 5% of the subscription
                                    db.referral_bonus_tb.Add(refbonus);
                                    await db.SaveChangesAsync();
                                    sp_IN_mlm.CurrentBonus = sp_IN_mlm.CurrentBonus + (refbonus.Subscription_Fee * 0.05);
                                    await db.SaveChangesAsync();
                                }
                                string mailbody = "<p>We are glad to let you know that your payment for iHealth premium user package has been acknowledged.</p> "
                              + "<p>Kindly note that we will batch and forward your details to the assigned HMO to activate your plan.</p>" +
                              "<p>We will notify you via email, once your plan becomes active.</p>"
                              + "<p>Kindly, note that a user account has also been created for you on our platform. Below are your login credentials;<br/>"
                              + "<b> Username: " +username + " <br/> Password: " +password + "</b></p>"
                               + "<p>Thank you. <br/><br/>Best Regards, <br/><br/>iHealthGSFM Team.</p>";
                                await em.premiumUserPaymentApprovalMail(usrpaid.Email, usrpaid.Last_Name, "Premium Plan Payment Approval", mailbody);
                                TempData["success"] = "Operations Successfull";

                                #endregion
                            }
                            else
                            {
                                #region //confirm payment

                                usrpaid.PaymentStatus = "Paid";
                                usrpaid.Transaction_date = DateTime.UtcNow;
                                usrpaid.SignedBy = Session["Name"].ToString();
                                usrpaid.NotifyStatus = 1;
                                //  usrpaid.INISubcriberExtraDetails.MaxDT2MtTarget = DateTime.UtcNow.AddDays(30);
                                await db.SaveChangesAsync();
                                //check if the new member has a sponsor so that we can give the sponsor bonus

                                if (usrpaid.sponsorID != 0)
                                {
                                    //retrieve the sponsor of this new registrant details from the all users table
                                    var sponsor = db.Users.Single(u => u.MyRefferalCode.ToString() == usrpaid.sponsorID.ToString());
                                    //check if sponsor is an ihealth networking subscriber, so dt we can give him credit
                                    var sp_IN_mlm = db.INISubcriberExtraDetails.Single(sp => sp.User.Id == sponsor.Id);
                                    referral_BonusTb refbonus = new referral_BonusTb();
                                    refbonus.user_ID = sponsor.Id;
                                    // refbonus.Downline_Id = usrpaid.INISubcriberExtraDetails.User.MyRefferalCode;
                                    refbonus.Downline_Name = usrpaid.First_Name + " " + usrpaid.Last_Name;
                                    refbonus.Bonus_Type = "Premium Referral Bonus";
                                    refbonus.Subscription_Fee = usrpaid.Amount;
                                    refbonus.Bonus_created_date = DateTime.UtcNow;
                                    refbonus.Bonus = refbonus.Subscription_Fee * 0.05; // bonus is 5% of the subscription
                                    db.referral_bonus_tb.Add(refbonus);
                                    await db.SaveChangesAsync();
                                    sp_IN_mlm.CurrentBonus = sp_IN_mlm.CurrentBonus + (refbonus.Subscription_Fee * 0.05);
                                    await db.SaveChangesAsync();
                                }
                                
                                    string mailbody = "<p>We are glad to let you know that your payment for iHealth premium user package has been acknowledged.</p> "
                            + "<p>Kindly note that we will batch and forward your details to the assigned HMO to activate your plan.</p>" +
                            "<p>We will notify you via email, once your plan becomes active.</p>"
                            + "<p>Thank you. <br/><br/>Best Regards, <br/><br/>iHealthGSFM Team.</p>";
                                    await em.premiumUserPaymentApprovalMail(usrpaid.Email, usrpaid.Last_Name, "Premium Plan Payment Approval", mailbody);
                                    TempData["success"] = "Operations Successfull";
                               
                                #endregion
                            }
                          
                        }
                        return Json(true, JsonRequestBehavior.AllowGet);
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
                #endregion
            }
            catch (Exception)
            {

                throw ;
            }
           
        }
        [Authorize(Roles = "Admin,SuperAdmin")]
         [HttpPost]
         [ValidateAntiForgeryToken]
        public ActionResult Export_to_Excel(FormCollection f)
         {
            using (ApplicationDbContext db=new ApplicationDbContext())
            {
                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[18]{
                new DataColumn("Reference ID"),
                new DataColumn("Name"),
                new DataColumn("Gender"),
                new DataColumn("Email"),
                new DataColumn("Mobile"),
                new DataColumn("Occupation"),
                new DataColumn("Birthday"),
                new DataColumn("Country"),
                new DataColumn("State"),
                new DataColumn("City"),
                new DataColumn("Address"),
                new DataColumn("Premium Plan"),
                new DataColumn("Hospital List"),
                new DataColumn("Amount"),
                new DataColumn("Payment Status"),
                new DataColumn("Register Date"),
                new DataColumn("Payment Date"),
                new DataColumn("Referral")
                });
                var data = db.premium_user.Where(p=>p.PaymentStatus=="Paid").ToList(); ;
                //get data from db
                // stdate = !string.IsNullOrEmpty(f["startdate"])? Convert.ToDateTime(f["startdate"]): ;
                if ( !string.IsNullOrEmpty(f["startdate"]) && !string.IsNullOrEmpty(f["enddate"]))
                {
                    DateTime sdate = Convert.ToDateTime(f["startdate"]);
                    DateTime edate = Convert.ToDateTime(f["enddate"]);
                    data = db.premium_user.Where(d => d.PaymentStatus=="Paid"&& d.Transaction_date >= sdate && d.Transaction_date <= edate).ToList();
                }
               

                foreach (var item in data)
                {
                    dt.Rows.Add(item.referrence_Id, item.First_Name+" "+ item.Last_Name,item.Gender,
                        item.Email,item.Phone,item.Occupation,Convert.ToDateTime(item.D_O_B).ToString("dd MMM, yyy"),
                        item.Nationality, item.State,item.City,item.Address,item.plan,item.Hospital_List.Replace("_"," "),
                        item.Amount,item.PaymentStatus,Convert.ToDateTime(item.Register_Date).ToString("dd MMM,yyy"),
                        Convert.ToDateTime(item.Transaction_date).ToString("dd MMM,yyy"),item.sponsorID);
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Premium User Data "+DateTime.UtcNow.ToShortDateString()+".xlsx");
                    }
                }
                //GridView grid = new GridView();
                ////assign data to gridview
                //grid.DataSource = data;
                ////bind data
                //grid.DataBind();
                //Response.ClearContent();
                //Response.Buffer = true;
                ////Adding name to excel file
                //Response.AddHeader("content-disposition", "attachment;filename=Premium_Users_Excel.xls");
                ////specify content type of file
                ////Here i specified "ms-excel" format
                ////you can also specify it "ms-word" to get word document
                //Response.ContentType = "application/ms-excel";
                //Response.Charset = "";
                //using (StringWriter sw=new StringWriter())
                //{
                //    using (HtmlTextWriter htwriter=new HtmlTextWriter(sw))
                //    {
                //        grid.RenderControl(htwriter);
                //        Response.Output.Write(sw.ToString());
                //        Response.Flush();
                //        Response.Close();
                //    }
                //}

              
      
            }
         }
        #endregion
        [Authorize(Roles = "INIsubscribers")]
        public ActionResult Premium_Referral (string dm)
         {
             string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
             dm = dm.Replace("$25", "/");
             dm = dm.Replace("$24", "+");
             string DecryptId = Cryptoclass.DecryptStringAES(dm, s);
             int rid = Convert.ToInt32(DecryptId);
             return View(db.premium_user.Where(p => p.sponsorID == rid).OrderByDescending(p => p.Register_Date).ToList());
         }

        #region//plan covverages 
        public ActionResult FHLPA()
        {
            return View();
        }
        public ActionResult FHLPB()
        {
            return View();
        }
        public ActionResult SHLPA() {
            return View();
        }
        public ActionResult SHLPB()
        {
            return View();
        }
        public ActionResult HCN1()
        {
            return View();
        }
        public ActionResult HCN2()
        {
            return View();
        }
        public ActionResult HCN()
        {
            return View();
        }
        #endregion
    }
}