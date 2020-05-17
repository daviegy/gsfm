using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using iHealth2.Models;
using System.Threading.Tasks;
using iHealth2.CustomClasses;
using System.IO;
using System.Text;
using System.Net;
namespace iHealth2.Controllers.SuperAdmin
{
    [Authorize(Roles= "SuperAdmin")]
    public class SuperAdmController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        Emails em = new Emails();
        public SuperAdmController()
        {

        }
        public SuperAdmController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        public ActionResult index()
        {
            return View();
        }
         // GET: SuperAdm/Create
        /// <summary>
        /// Staff Mgt. Session
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        // POST: SuperAdm/Create
        [HttpPost]
        public async Task<ActionResult> Create(RegisterAdminViewModel model,FormCollection collection)
        {
           try
            {
                // TODO: Add insert logic here
                AlphaNumGen alp = new AlphaNumGen();
                var pwd = alp.GetPassword();
                string username = string.Concat(model.FirstName.Substring(0,1),"_", model.LastName);
                var user = new ApplicationUser
                {
                    UserName = username,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.Phone,
                    RegDate = DateTime.UtcNow,
                    NotifyStatus = 1,
                    UserType = "AD",
                   // EmailConfirmed = true
                    
                };
                //this method add Newly registering Subcriber to AspNetUsers Tb which house is primary info and login permission
                var result = await UserManager.CreateAsync(user, pwd);
                if(result.Succeeded)
                {
                    var UserRole = UserManager.GetRoles(user.Id);
                    if (!UserRole.Contains("Admin"))
                    {
                        var addToRole = UserManager.AddToRole(user.Id, "Admin");
                    }
                 //   await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                  //TODO: newly created admin get an email notification containing is registrations status
                    // eg. username and password
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "superadm",
                       new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account",em.Notification_Email_Body_Creator(user.FullName, "This is to notify you that an account has been created " +
                          "for you on iHealth Nigeria GSFM Portal using this email address " + user.Email + " and the following information: <br/><br/> Username: " + user.UserName + "" +
                          "<br/> Password: " + pwd + " <br/><br/>Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>. <br/> Welcome on board. <br/><br/>" +
                          "Best Regards, <br/><br/> iHealth Nigeria Management"));
                    TempData["success"] = "Registration Succeeded";
                    return RedirectToAction("create","superadm");
                }
                AddErrors(result);
               
            }
            catch
            {
                return View();
            }

           return View();
        }
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
                return View("ConfirmEmail");
            }
            else
            {
                HttpStatusCode.BadRequest.ToString();
                return View("ErrorAlert");
            }
            //return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }
        // GET: SuperAdm/Modify User Role/5 :
        public ActionResult ModifyRole(string id)
        {
            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            id = id.Replace("$25", "/");
            id = id.Replace("$24", "+");
            string DecryptId = Cryptoclass.DecryptStringAES(id, s);
            var user = db.Users.Find(DecryptId);
            return View ( new ModifyRoleViewModel {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Id = user.Id,
            CurrentRole = user.UserType
            });
        }

        // POST: SuperAdm/ModifyUserRole/5
        [HttpPost]
        public async Task<ActionResult> ModifyRole(string id, FormCollection collection)
        {
            try
            {
                string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
                id = id.Replace("$25", "/");
                id = id.Replace("$24", "+");
                string DecryptId = Cryptoclass.DecryptStringAES(id, s);
                //TODO: Add update logic here
                var user = db.Users.Find(DecryptId);
                user.UserType = collection["roles"];
                await db.SaveChangesAsync();
                TempData["warning"] = "Role modificaton successfull";
                return RedirectToAction("ViewAdmins");
            }
            catch
            {
                return View();
            }
        }

        // POST: SuperAdm/Delete/5
        [HttpPost]
        public JsonResult Delete(string id)
        {
            try
            {
                // TODO: Add delete logic here
                string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
                id = id.Replace("$25", "/");
                id = id.Replace("$24", "+");
                string DecryptId = Cryptoclass.DecryptStringAES(id, s);
                ApplicationUser user = UserManager.FindById(DecryptId);
               var result = UserManager.Delete(user);
               if (result.Succeeded)
                   return Json(true, JsonRequestBehavior.AllowGet);
               else
                   return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch(Exception)
            {
                throw ;
            }
        }
        
        public ActionResult ViewAdmins()
        {
            var adm = db.Users.Where(u => u.UserType == "AD" || u.UserType == "SA").OrderByDescending(d=>d.RegDate).ToList();
            
            return View(adm);
        }

        /// <summary>
        /// Dashboard Mgt. session
        /// </summary>
        /// <returns></returns>
        public ActionResult All_users() // View all register subscribers
        {
            var allUser = db.Users.Where(u => u.NotifyStatus == 1 && (u.UserType == "BusinessUser" || u.UserType == "INISubcriber")).OrderByDescending(u => u.RegDate).ToList();
            if (allUser.Count() != 0)
            {
                return View(allUser);
            }
            TempData["error"] = "Alert: NO RECORD FOUND!";
            return RedirectToAction("index", "SuperAdm");
        }
        public ActionResult ViewApprovedComORBiz() // view approved company or biiz
        {
            var bis = db.BusinessInfoes.Where(b => b.ApprovedStatus == "A" && b.NotifyStatus == 1 && b.VerifiedStatus == "NV").ToList().OrderByDescending(b => b.regDate);
            if (bis.Count() > 0)
            {
                return View(bis);
            }
            else
            {
                TempData["error"] = "Alert: No company has been approved yet";
                return RedirectToAction("index", "SuperAdm");

            }
        } 
        [HttpGet]
        public ActionResult VerifiedComp() // view Verify com or bis
        {
            var bis = db.BusinessInfoes.Where(b => b.ApprovedStatus == "A" && b.NotifyStatus == 1 && b.VerifiedStatus == "V").ToList().OrderByDescending(b => b.regDate);
            if (bis.Count() > 0)
            {
                return View(bis);
            }
            else
            {
                TempData["error"] = "Alert: No company has been verified yet";
                return RedirectToAction("index", "SuperAdm");

            }
        }
        public ActionResult ApprovedProducts()// View Approved product
        {
            var bis = db.ProductTb.Where(b => b.ApprovedStatus == "A" && b.NotifyStatus == 1 && b.VerifiedStatus == "NV").ToList().OrderByDescending(b => b.regDate);
            if (bis.Count() > 0)
            {
                return View(bis);
            }
            else
            {
                TempData["error"] = "Alert: No Product has been approved yet";
                return RedirectToAction("index", "SuperAdm");

            }

        }
        public ActionResult VerifiedProduct()// View Verified products
        {
            var p = db.ProductTb.Where(b => b.ApprovedStatus == "A" && b.NotifyStatus == 1 && b.VerifiedStatus == "V").ToList().OrderByDescending(b => b.regDate);
            if (p.Count() > 0)
            {
                return View(p);
            }
            else
            {
                TempData["error"] = "Alert: No Product(s) has been verified yet";
                return RedirectToAction("index", "SuperAdm");

            }
        }
        public ActionResult SettleClaims()
        {
            var sClaims = db.BenefitClaimersTb.Where(c => c.BenefitProcessStatus == "Being Processed" || c.BenefitProcessStatus == "Settled")
                          .OrderByDescending(c => c.ProcessedDate).Distinct();
            if (sClaims.Count() > 0)
            {
                return View(sClaims);
            }
            return View(sClaims);
        }

        /// <summary>
        /// Advert management Session
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult CreateAds()
        {
                  return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> createAds(AdvertManagerViewModel model, HttpPostedFileBase AdsImage)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AdvertManagerTb ads = new AdvertManagerTb();
                    ads.AdsOwner = model.AdsOwner;
                    //ads.AdsName = model.AdsName;
                    ads.Adspage = model.adspage.pageId;
                    ads.AdsLocOnPage = model.AdsLocOnPage.locId;
                    ads.AdsURL = model.AdsUrl;
                    ads.AdsStatus = "Active";
                    ads.CreatedDate = DateTime.UtcNow;
                    var adloc = model.AdsLocOnPage.locId;
                    if (adloc == "SquareBoard")
                        {
                //check if advert already exist in selected location
                string getAdsPage = model.adspage.pageId;
                if (getAdsPage == "FindHospitalPage")
                {
                    var adv = db.AdvertManagerTb.Where(a => a.AdsLocOnPage == adloc && a.AdsStatus == "Active" && a.Adspage == getAdsPage);
                    if (adv.Count() > 0)
                    {
                        ModelState.AddModelError("", "Advert already exist in the page '"+model.adspage.pageId+"' for the location selected, kindly select a different page or location");
                        ViewBag.errorMsg = "Advert already exist in the page '" + model.adspage.pageId + "' for the location selected, kindly select a different page or location";
                        return View();
                    }
                   
                }
                else if (getAdsPage == "SearchPharmacyPage")
                {
                    var adv = db.AdvertManagerTb.Where(a => a.AdsLocOnPage == adloc && a.AdsStatus == "Active" && a.Adspage == getAdsPage);
                    if (adv.Count() > 0)
                    {
                        ModelState.AddModelError("", "Advert already exist in the page '" + model.adspage.pageId + "' for the location selected, kindly select a different page or location");
                        ViewBag.errorMsg = "Advert already exist in the page '" + model.adspage.pageId + "' for the location selected, kindly select a different page or location";
                        return View();
                    }
                 
                }
                else if (getAdsPage == "SearchDrugPage")
                {
                    var adv = db.AdvertManagerTb.Where(a => a.AdsLocOnPage == adloc && a.AdsStatus == "Active" && a.Adspage == getAdsPage);
                    if (adv.Count() > 0)
                    {
                        ModelState.AddModelError("", "Advert already exist in the page '" + model.adspage.pageId + "' for the location selected, kindly select a different page or location");
                        ViewBag.errorMsg = "Advert already exist in the page '" + model.adspage.pageId + "' for the location selected, kindly select a different page or location";
                        return View();
                    }
                   
                }
                else if (getAdsPage == "FindLabPage")
                {
                    var adv = db.AdvertManagerTb.Where(a => a.AdsLocOnPage == adloc && a.AdsStatus == "Active" && a.Adspage == getAdsPage);
                    if (adv.Count() > 0)
                    {
                        ModelState.AddModelError("", "Advert already exist in the page '" + model.adspage.pageId + "' for the location selected, kindly select a different page or location");
                        ViewBag.errorMsg = "Advert already exist in the page '" + model.adspage.pageId + "' for the location selected, kindly select a different page or location";
                        return View();
                    }
                   
                }
                else if (getAdsPage == "MedEquipCompPage")
                {
                    var adv = db.AdvertManagerTb.Where(a => a.AdsLocOnPage == adloc && a.AdsStatus == "Active" && a.Adspage == getAdsPage);
                    if (adv.Count() > 0)
                    {
                        ModelState.AddModelError("", "Advert already exist in the page '" + model.adspage.pageId + "' for the location selected, kindly select a different page or location");
                        ViewBag.errorMsg = "Advert already exist in the page '" + model.adspage.pageId + "' for the location selected, kindly select a different page or location";
                        return View();
                    }
                 
                }
                else if (getAdsPage == "FindMedEquipPage")
                {
                    var adv = db.AdvertManagerTb.Where(a => a.AdsLocOnPage == adloc && a.AdsStatus == "Active" && a.Adspage == getAdsPage);
                    if (adv.Count() > 0)
                    {
                        ModelState.AddModelError("", "Advert already exist in the page '" + model.adspage.pageId + "' for the location selected, kindly select a different page or location");
                        ViewBag.errorMsg = "Advert already exist in the page '" + model.adspage.pageId + "' for the location selected, kindly select a different page or location";
                        return View();
                    }
                  
                }
                else if (getAdsPage == "SearchHerbalCentrePage")
                {
                    var adv = db.AdvertManagerTb.Where(a => a.AdsLocOnPage == adloc && a.AdsStatus == "Active" && a.Adspage == getAdsPage);
                    if (adv.Count() > 0)
                    {
                        ModelState.AddModelError("", "Advert already exist in the page '" + model.adspage.pageId + "' for the location selected, kindly select a different page or location");
                        ViewBag.errorMsg = "Advert already exist in the page '" + model.adspage.pageId + "' for the location selected, kindly select a different page or location";
                        return View();
                    }
                  
                }
                else if (getAdsPage == "SearchHerbalDrugPage")
                {
                    var adv = db.AdvertManagerTb.Where(a => a.AdsLocOnPage == adloc && a.AdsStatus == "Active" && a.Adspage == getAdsPage);
                    if (adv.Count() > 0)
                    {
                        ModelState.AddModelError("", "Advert already exist in the page '" + model.adspage.pageId + "' for the location selected, kindly select a different page or location");
                        ViewBag.errorMsg = "Advert already exist in the page '" + model.adspage.pageId + "' for the location selected, kindly select a different page or location";
                        return View();
                    }
                   
                }
                else if (getAdsPage == "SearchVetDrugPage")
                {
                    var adv = db.AdvertManagerTb.Where(a => a.AdsLocOnPage == adloc && a.AdsStatus == "Active" && a.Adspage == getAdsPage);
                    if (adv.Count() > 0)
                    {
                        ModelState.AddModelError("", "Advert already exist in the page '" + model.adspage.pageId + "' for the location selected, kindly select a different page or location");
                        ViewBag.errorMsg = "Advert already exist in the page '" + model.adspage.pageId + "' for the location selected, kindly select a different page or location";
                        return View();
                    }
                 
                }
                else if (getAdsPage == "SearchVetCentrePage")
                {
                    var adv = db.AdvertManagerTb.Where(a => a.AdsLocOnPage == adloc && a.AdsStatus == "Active" && a.Adspage == getAdsPage);
                    if (adv.Count() > 0)
                    {
                        ModelState.AddModelError("", "Advert already exist in the page '" + model.adspage.pageId + "' for the location selected, kindly select a different page or location");
                        ViewBag.errorMsg = "Advert already exist in the page '" + model.adspage.pageId + "' for the location selected, kindly select a different page or location";
                        return View();
                    }
                   
                }
            }
                    if (AdsImage != null)
                    {
                        byte[] img = new byte[AdsImage.InputStream.Length];
                        AdsImage.InputStream.Read(img, 0, (int)AdsImage.InputStream.Length);
                        System.Drawing.Image image2validate =  System.Drawing.Image.FromStream(AdsImage.InputStream,true,true );
                        if (ads.AdsLocOnPage == "LeaderBoard")
                        {
                            if (image2validate.Height == 90 && image2validate.Width == 728)
                            {                              
                                 ads.AdsImage = img;
                                db.AdvertManagerTb.Add(ads);
                                await db.SaveChangesAsync();
                                TempData["success"] = "Advert created successfully";
                                ModelState.Clear();
                                return View();
                            }
                            else
                            {
                                TempData["error"] = "Advert image does not match Ads location, please kindly upload an image of equal size with image location";
                                ViewBag.errorMsg = "Advert Image does not match Advert location size!";
                                return View();
                            }
                        }
                        else if (ads.AdsLocOnPage == "MediumRectangle1"||ads.AdsLocOnPage == "MediumRectangle2"||ads.AdsLocOnPage == "MediumRectangle3")
                        {
                            if (image2validate.Height == 250 && image2validate.Width == 300)
                            {
                           //     AdsImage.InputStream.Read(img, 0, (int)AdsImage.InputStream.Length);
                                ads.AdsImage = img;
                                db.AdvertManagerTb.Add(ads);
                                await db.SaveChangesAsync();
                                TempData["success"] = "Advert created successfully";
                                ModelState.Clear();
                                return View();
                            }
                            else
                            {
                                TempData["error"] = "Advert image does not match Ads location, please kindly upload an image of equal size with image location";
                                ViewBag.errorMsg = "Advert Image does not match Advert location size!";
                                return View();
                            }
                        }
                        else if (ads.AdsLocOnPage == "Slider")
                        {
                            if (image2validate.Height == 450 && image2validate.Width == 1000)
                            {
                             //   AdsImage.InputStream.Read(img, 0, (int)AdsImage.InputStream.Length);
                                ads.AdsImage = img;
                                db.AdvertManagerTb.Add(ads);
                                await db.SaveChangesAsync();
                                TempData["success"] = "Advert created successfully";
                                ModelState.Clear();
                                return View();
                            }
                            else
                            {
                                TempData["error"] = "Advert image does not match Ads location, please kindly upload an image of equal size with image location";
                                ViewBag.errorMsg = "Advert Image does not match Advert location size!";
                                return View();
                            }
                        }
                        else if(ads.AdsLocOnPage == "SquareBoard")
                        {
                            if (image2validate.Height == 300 && image2validate.Width == 280)
                            {
                               // AdsImage.InputStream.Read(img, 0, (int)AdsImage.InputStream.Length);
                                ads.AdsImage = img;
                                db.AdvertManagerTb.Add(ads);
                                await db.SaveChangesAsync();
                                TempData["success"] = "Advert created successfully";
                                ModelState.Clear();
                                return View();
                            }
                            else
                            {
                                TempData["error"] = "Advert image does not match Ads location, please kindly upload an image of equal size with image location";
                                ViewBag.errorMsg = "Advert Image does not match Advert location size!";
                                return View();
                            }
                          
                        }
                    

                    }
                    else
                    {
                        ModelState.AddModelError("", "All fields marked with (*) are compulsory");
                        TempData["warning"] = "All fields marked with (*) are compulsory";
                        return View();
                    }

                }
                else
                {
                    ModelState.AddModelError("", "All fields marked with (*) are compulsory");
                    TempData["warning"] = "All fields marked with (*) are compulsory";
                    return View();
                }
              
            }
            catch (Exception ex)
            {
                TempData["error"] = "an error has occurred!!!";
                ModelState.AddModelError("", ex );
                return View();
            }
            return View();
          
        }
        public ActionResult viewAdsList()
        {
            var adsList = db.AdvertManagerTb.ToList().OrderByDescending(ads => ads.CreatedDate);
            return View(adsList.ToList());
        }
        public ActionResult ActiveAds()
        {
            var adsList = db.AdvertManagerTb.Where(a => a.AdsStatus == "Active").ToList().OrderByDescending(ads => ads.CreatedDate);
            return View(adsList.ToList());
        }
        public ActionResult InactiveAds()
        {
            var adsList = db.AdvertManagerTb.Where(a => a.AdsStatus == "Inactive").ToList().OrderByDescending(ads => ads.CreatedDate);
            return View(adsList.ToList());
        }
        public async Task<JsonResult> ReactivateAds(string MarkAsReactivateId)
        {
            try
            {
                if (MarkAsReactivateId != null)
                {
                    var MarkAsReactivateIds = MarkAsReactivateId.Split(',');
                    foreach (var id in MarkAsReactivateIds)
                    {
                        int ID = Convert.ToInt32(id);
                       // var ads = db.AdvertManagerTb.Where(s => s.Id == ID);
                            var ads = db.AdvertManagerTb.Single(s => s.Id == ID);
                        var adsloc = db.AdvertManagerTb.Where(loc => loc.AdsLocOnPage == ads.AdsLocOnPage && loc.AdsStatus == "Active");
                            if (adsloc.Count() > 1)
                            {
                                TempData["error"] = "Oops! there exist an active advert in this location for this advert."+
                                    " Kindly, de-activate the existing advert in order to re-activate your selection. ";
                                return Json(false, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                ads.AdsStatus = "Active";
                            }                          
                            await   db.SaveChangesAsync();
                
                    }
                    TempData["success"] = "Advert  has been reactivated successfully.";
                    return Json(true, JsonRequestBehavior.AllowGet);
                   
                }
                else
                {
                    TempData["error"] = "Oops! No Item is selected from the list. ";
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                TempData["error"] = "Oops! there is an error reactivating advert your request ";
                return Json(false, JsonRequestBehavior.AllowGet);
            }


        }
        public async Task<ActionResult> DeactivateAds(IEnumerable<int> ItemId)
        {
            try
            {
                if (ItemId != null)
                {
                    if (ItemId.Count() > 0)
                    {
                        foreach (var id in ItemId)
                        {
                            var ads = db.AdvertManagerTb.Single(s => s.Id == id);
                            ads.AdsStatus = "Inactive";

                            await db.SaveChangesAsync();
                        }
                        TempData["success"] = "Operations is successfull.";
                        return RedirectToAction("ActiveAds");
                    }
                    else
                    {
                        TempData["error"] = "Oops! No Item is selected from the list. ";
                        return RedirectToAction("ActiveAds");
                    }


                }
                else
                {
                    TempData["error"] = "Oops! No Item is selected from the list. ";
                    return RedirectToAction("ActiveAds");
                }
            }
            catch
            {
                TempData["error"] = "Oops! there is an error processing your request ";
                return RedirectToAction("ActiveAds");
            }
          
           
        }
        public async Task<JsonResult> DeleteAds(IEnumerable<int> MarkAsDeleteId)
        {
            try
            {
                if (MarkAsDeleteId != null)
                {
                    if (MarkAsDeleteId.Count() > 0)
                    {
                        foreach (var id in MarkAsDeleteId)
                        {
                            var ads = db.AdvertManagerTb.Single(s => s.Id == id);

                            db.AdvertManagerTb.Remove(ads);
                            await db.SaveChangesAsync();
                        }
                        TempData["success"] = "Advert has been deleted successfully";
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
            catch
            {
                TempData["error"] = "Oops! there is an error deleting advert";
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
        /// <summary>
        /// Profile management session
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

        //check if Advert already exist in the following locations 
        //"leader board,MediumRectangle1-3, and square board "
        public JsonResult is_Ads_In_Loc(AdvertManagerViewModel model)
        {
            try{
                  var adloc = model.AdsLocOnPage.locId;
            if (adloc == "LeaderBoard")
            {
                //check if advert already exist in selected location
                var adv = db.AdvertManagerTb.Where(a=>a.AdsLocOnPage == adloc && a.AdsStatus =="Active");
                if(adv.Count()>0)
                {
                    return Json("Advert already exist in this location selected, kindly select a different location", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }

            }
            else if (adloc == "MediumRectangle1")
            {
                //check if advert already exist in selected location
                var adv = db.AdvertManagerTb.Where(a => a.AdsLocOnPage == adloc && a.AdsStatus == "Active");
                if (adv.Count() > 0)
                {
                    return Json("Advert already exist in this location selected, kindly select a different location", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            else if (adloc == "MediumRectangle2")
            {
                //check if advert already exist in selected location
                var adv = db.AdvertManagerTb.Where(a => a.AdsLocOnPage == adloc && a.AdsStatus == "Active");
                if (adv.Count() > 0)
                {
                    return Json("Advert already exist in this location selected, kindly select a different location", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            else if (adloc == "MediumRectangle3")
            {
                //check if advert already exist in selected location
                var adv = db.AdvertManagerTb.Where(a => a.AdsLocOnPage == adloc && a.AdsStatus == "Active");
                if (adv.Count() > 0)
                {
                    return Json("Advert already exist in this location selected, kindly select a different location", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
          
      //    return Json(true, JsonRequestBehavior.AllowGet);
            }
                catch{
                    return Json("error exist", JsonRequestBehavior.AllowGet);
                }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
     
    }
}
