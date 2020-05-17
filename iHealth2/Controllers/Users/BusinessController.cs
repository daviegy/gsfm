using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data.Entity.Spatial;
using iHealth2.Models;
using iHealth2.CustomClasses;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
using System.Diagnostics;
using Ganss.XSS;
using System.IO;
namespace iHealth2.Controllers.Users
{
    //[Authorize(Roles = "Users,INIsubscribers,Admin,SuperAdmin")]
    public class BusinessController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        //EmailService em = new EmailService();
        Emails e = new Emails();
        ImgResize ImageRz = new ImgResize();
        SEO_Friendly_URL seo = new SEO_Friendly_URL();
        HtmlSanitizer sanitizer = new HtmlSanitizer();
        // SEO_Friendly_URL seo = new SEO_Friendly_URL();
        // GET: Business
        [Authorize(Roles = "Users,INIsubscribers,Admin,SuperAdmin")]
        public ActionResult MyBusiness(string Id)
        {
          
            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            Id = Id.Replace("$25", "/");
            Id = Id.Replace("$24", "+");
            string ID = Cryptoclass.DecryptStringAES(Id, s);            
            var bus = db.BusinessInfoes.Where(b => b.UserID == ID);
            if(bus.Count()>0)
            {
                return View(bus);
            }
            else
            {
                TempData["error"] = "You don't have any registered Business(es).";
                return View(bus);
            }
          
         
        }
        [Authorize(Roles = "Users,INIsubscribers,Admin,SuperAdmin")]
        public async Task<ActionResult> My_Business_Profile(int id)
        {
            var bizDetail = await db.BusinessInfoes.FindAsync(id);
            return View(bizDetail);
        }
        // GET: Business/Details/5
        [Authorize(Roles = "Users,INIsubscribers,Admin,SuperAdmin")]
        public ActionResult Details(int id)
        {
            return View();
        }
        
        // GET: Business/Create
        [AllowAnonymous]
        [Authorize(Roles = "Users,INIsubscribers,Admin,SuperAdmin")]
        public ActionResult Create_Biz()
        {
            country c = new country();
            Category cat = new Category();
            var country = db.country.ToList();
            ViewBag.category = new SelectList(db.category, "ID", "CatName", cat.ID);
            ViewBag.Country = new SelectList(country, "ID", "CountryName", c.ID);
            return View();
        }

        // POST: Business/Create
        [HttpPost][ValidateAntiForgeryToken][ValidateInput(false)]
        [Authorize(Roles = "Users,INIsubscribers,Admin,SuperAdmin")]
        public async Task<ActionResult>Create_Biz(FormCollection collection, [Bind(Exclude = "logo")] BusinessInfo biz, HttpPostedFileBase logo)
            {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(biz);
                }
               // status s = new status();
                var userId = Session["Id"].ToString();
                var u = db.Users.Find(userId);
                   country c = db.country.Find(Convert.ToInt32(biz.Country));
                var st = db.State.Find(Convert.ToInt32(collection["state"]));
                var cat = db.category.Find(Convert.ToInt32(biz.Category));              
                             
                BusinessInfo b = new BusinessInfo();
                b.UserID = userId;
                b.businessName = biz.businessName;
                b.Country = c.CountryName;
                b.State = st.StateName;
                b.City = collection["city"];               
                b.Email = biz.Email;
                b.Phone = biz.Phone;
                b.Category = cat.CatName;
                b.Website = biz.Website;
                b.Address = biz.Address;
                b.NotifyStatus = 0;
                b.regDate = DateTime.UtcNow;
                b.ApprovedStatus = "P";
                b.VerifiedStatus =  "NV";
                b.Description = Server.HtmlEncode(sanitizer.Sanitize(collection["summary"], "", null));
                b.Facebook = biz.Facebook;
                b.Twitter = biz.Twitter;
                b.LinkedIn = biz.LinkedIn;
                b.Google_Plus = biz.Google_Plus;
                b.Custom_Url = seo.generate_title(biz.businessName);
                b.isServiceBooster = (u.isClientBooster == true) ? "Yes" : "No";
                if (collection["subCat1"] != null)
                {
                    var sb1 = db.SubCategory1.Find(Convert.ToInt32(collection["subCat1"]));
                    b.subCategory1 = sb1.SubCat1Name;
                    if (collection["subCat2"] != null)
                    {
                        var sb2 = db.SubCategory2.Find(Convert.ToInt32(collection["subCat2"]));
                        b.subCategory2 = sb2.SubCat2name;
                    }
                }
                
                //if (collection["lat"] != "" && collection["long"] != "")
                //{
                //    string Long = collection["long"];
                //    string Lat = collection["lat"];
                //    string cordinate = string.Format("Point({0} {1})", Lat,Long);
                //    b.mapLocation = DbGeography.FromText(cordinate, 4326);
                //}
                if (logo != null)
                {
                    byte[] img;
                    System.Drawing.Image image2validate = System.Drawing.Image.FromStream(logo.InputStream, true, true);
                    decimal size = Math.Round(logo.ContentLength / (decimal)1024, 2);
                    if (size <= 1000) {
                        img = ImageRz.imageToByteArray(image2validate, image2validate.RawFormat, 111, 119);
                        b.logo = img;                  
                    }
                    else
                    {
                        TempData["error"] = "Uploaded Image size exceed 1000kb(i.e.1MB)!";
                        return View();
                    }      
                   
                }                 
               db.BusinessInfoes.Add(b);
               await db.SaveChangesAsync();
               await e.bizRegistrationMail(u.Email,biz.Email,biz.Phone,biz.businessName, biz.Address, cat.CatName,u.FirstName);
                // TODO: Add insert logic here
                TempData["success"] = "Business registration was successfull!";
                TempData["info"] = "We will Notify you upon approver, Thanks.";
                return Redirect(Request.Url.ToString());
            }
            catch(Exception ex)
            {                      
                TempData["error"] = ex.ToString();
                return View();
            }
        }
        [Authorize(Roles = "Users,INIsubscribers,Admin,SuperAdmin")]
        public async Task<ActionResult> biz_gallery_Img(int id, IEnumerable<HttpPostedFileBase> galleryImg)
        {
            BusinessImages bi = new BusinessImages();
            BusinessInfo b = await db.BusinessInfoes.FindAsync(id);
            if (galleryImg.Count() != 0 || galleryImg.FirstOrDefault() != null)
            {
                foreach (HttpPostedFileBase image in galleryImg)
                {
                    var ImageName = Path.GetFileName(image.FileName);
                    Directory.CreateDirectory(Server.MapPath("~/content/Business_Img_Gallery/" + b.businessName));
                    string physicalPath = Server.MapPath("~/content/Business_Img_Gallery/" + b.businessName + "/" + ImageName);
                    byte[] img;
                    //image.InputStream.Read(img, 0, (int)image.InputStream.Length);
                    System.Drawing.Image validateWH = System.Drawing.Image.FromStream(image.InputStream, true, true);
                    img = ImageRz.imageToByteArray(validateWH, validateWH.RawFormat, 1000, 450);
                    decimal size = Math.Round(image.ContentLength / (decimal)1024, 2);
                    if(size <= 2000)
                    {
                        bi.BizId = id;
                        bi.galleryImage = img;
                        bi.isSliderImage = "false";
                        bi.imageUrl = b.businessName + "/" + ImageName;
                        db.businessImage.Add(bi);
                        await db.SaveChangesAsync();
                        System.IO.File.WriteAllBytes(physicalPath, img);
                        image.SaveAs(physicalPath);
                    }
                    else
                    {   
                        TempData["error"] = "Image Size Exceed 2000kb(2mb).";
                    }
                }
            }
            TempData["success"] = "Image Uploaded Successfully";
            return RedirectToAction("My_Business_Profile", "Business", new { id= id});
        }
        [Authorize(Roles = "Users,INIsubscribers,Admin,SuperAdmin")]
        public async Task<JsonResult> delImg(IEnumerable<int> Id)
        {
            try
            {
                if (Id != null)
                {
                    if (Id.Count() > 0)
                    {
                        foreach (int id in Id)
                        {
                            BusinessImages b = await db.businessImage.FindAsync(id);
                            db.businessImage.Remove(b);
                            await db.SaveChangesAsync();
                        }
                    }
                }
                return Json(true, JsonRequestBehavior.AllowGet);
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
                return Json(false, JsonRequestBehavior.AllowGet);
            }


        }
        [Authorize(Roles = "Users,INIsubscribers,Admin,SuperAdmin")]
        public async Task<JsonResult> featuredImg(IEnumerable<int> Id)
        {
            try
            {
                if (Id != null)
                {
                    if (Id.Count() > 0)
                    {
                        foreach (int id in Id)
                        {
                            BusinessImages b = await db.businessImage.FindAsync(id);
                            var bImg = db.businessImage.Where(m => m.BizId == b.BizId && m.isSliderImage == "true").Count();
                            if (bImg > 3)
                            {
                                TempData["error"] = "Maximum Featured Image Count Exceeded";
                                return Json(false, JsonRequestBehavior.AllowGet);                     
                            }
                            else
                            {
                                b.isSliderImage = "true";
                                await db.SaveChangesAsync();                                  
                            }
                          
                        }
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        TempData["error"] = "No item selected!";
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    TempData["error"] = "No item selected!";
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
               
             
               
               
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
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
        [Authorize(Roles = "Users,INIsubscribers,Admin,SuperAdmin")]
        public async Task<JsonResult> unfeaturedImg(IEnumerable<int> Id)
        {
            try
            {
                if (Id != null)
                {
                    if (Id.Count() > 0)
                    {
                        foreach (int id in Id)
                        {
                            BusinessImages b = await db.businessImage.FindAsync(id);
                            b.isSliderImage = "false";
                            await db.SaveChangesAsync();

                        }
                    }
                }
                return Json(true, JsonRequestBehavior.AllowGet);
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
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
        // GET: Business/Edit/5
        //public ActionResult edit_business_info(int id)
        //{
        //    return View();
        //}

        // POST: Business/Edit/5
        [HttpPost][ValidateAntiForgeryToken][ValidateInput(false)]
        [Authorize(Roles = "Users,INIsubscribers,Admin,SuperAdmin")]
        public async Task<ActionResult> edit_business_info(int id, FormCollection collection)
        {
            try
            {
                int sv = Convert.ToInt32(collection["state"]);
                int cv = Convert.ToInt32(collection["Country"]);
                var c = await db.country.FindAsync(cv);
                var st = await db.State.FindAsync(sv);
                var biz = await db.BusinessInfoes.FindAsync(id);
                biz.Phone = collection["Phone"];
                biz.Description = Server.HtmlEncode(sanitizer.Sanitize(collection["summary"], "", null));
                biz.Country = c.CountryName;
                biz.State = st.StateName;
                biz.City = collection["city"];
                biz.Address = collection["Address"];
                biz.Website = collection["Website"];
                biz.Facebook = collection["Facebook"];
                biz.Twitter = collection["Twitter"];
                biz.LinkedIn = collection["LinkedIn"];
                biz.Google_Plus = collection["Google_Plus"];
                await db.SaveChangesAsync();
                TempData["success"] = "Business profile updated successfully";
                return RedirectToAction("My_Business_Profile", "Business", new { id = id });
            }
            catch
            {
                return RedirectToAction("My_Business_Profile", "Business", new { id = id });
            }
        }
        // GET: Business/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }
        // POST: Business/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public JsonResult getState(int Id)
        {
            var st = from data in db.State
                     where data.CountryID == Id
                     select data;
            List<SelectListItem> State = new List<SelectListItem>();
            foreach (var s in st)
            {
                State.Add(new SelectListItem() { Text = s.StateName, Value = s.ID.ToString() });
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
            foreach(var sb2 in sub2)
            {
                subcat2.Add(new SelectListItem { Value = sb2.ID.ToString(), Text = sb2.SubCat2name });
            }
            return Json(new SelectList(subcat2, "Value", "Text"));
        }
        [AllowAnonymous]
        public ActionResult biz_profile(int? id)
        {
            var bInfo = db.BusinessInfoes.Find(id);
            return View(new view_biz_profile { 
            Business_name = bInfo.businessName,
            addr = bInfo.Address,
            phone = bInfo.Phone,
            email = bInfo.Email,
            websit =  bInfo.Website,
            fb = bInfo.Facebook,
            tw = bInfo.Twitter,
            ln = bInfo.LinkedIn,
            gPlus = bInfo.Google_Plus,
            Id = bInfo.ID,
            Overview = bInfo.Description,
           // Bimg = bInfo.BusinessImages
            });
        }

        #region // Business/Company Rating      
        public async Task<ActionResult> submitReview(FormCollection f)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["reviemFormCollection"] = f;
                TempData["AutoSave"] = true;
                return new HttpUnauthorizedResult();
            }
            else if(Convert.ToBoolean(TempData["AutoSave"]) == true) {
                TempData["AutoSave"] = false;
                var fcollection = TempData["reviemFormCollection"] as FormCollection;
                var login_user = User.Identity.Name;
                int id = Convert.ToInt32(fcollection["companyId"]);
                var company = await db.BusinessInfoes.FindAsync(id);
                reviewTb rv = new reviewTb();
                rv.product_or_company_id = id;
                rv.reviewerName = (!string.IsNullOrEmpty(fcollection["reviewerName"])) ? fcollection["reviewerName"] : login_user;
                rv.reviewerId = fcollection["reviewerID"];
                rv.reviewTitle = fcollection["Title"];
                rv.review_content = fcollection["review_content"];
                rv.ratingValue = (!string.IsNullOrEmpty(fcollection["ratingValue"])) ? Convert.ToInt32(fcollection["ratingValue"]) : 0;
                rv.reviewedDate = DateTime.UtcNow;
                db.reviewTb.Add(rv);
                await db.SaveChangesAsync();
                string url = "/" + seo.generate_title(company.businessName) + "/business/biz_profile/" + seo.GenerateSlug(company.ID.ToString(), company.businessName);
                return Redirect(url);
            }
            else
            {
                var login_user = User.Identity.Name;
                int id = Convert.ToInt32(f["companyId"]);
                var company = await db.BusinessInfoes.FindAsync(id);
                reviewTb rv = new reviewTb();
                rv.product_or_company_id = id;
                rv.reviewerName = (!string.IsNullOrEmpty(f["reviewerName"])) ? f["reviewerName"] : login_user;
                rv.reviewerId = f["reviewerID"];
                rv.reviewTitle = f["Title"];
                rv.review_content = f["review_content"];
                rv.ratingValue = (!string.IsNullOrEmpty(f["ratingValue"])) ? Convert.ToInt32(f["ratingValue"]) : 0;
                rv.reviewedDate = DateTime.UtcNow;
                db.reviewTb.Add(rv);
                await db.SaveChangesAsync();
                string url = "/" + seo.generate_title(company.businessName) + "/business/biz_profile/" + seo.GenerateSlug(company.ID.ToString(), company.businessName);
                return Redirect(url);
            }
            
        }
        #endregion
    }
}
