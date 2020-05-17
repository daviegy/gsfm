using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iHealth2.Models;
using System.Data.Entity;
using iHealth2.CustomClasses;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Data.Entity.Validation;
using System.Diagnostics;
using Ganss.XSS;
namespace iHealth2.Controllers.Products
{

    [Authorize]
    public class ProductsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        Emails em = new Emails();
        ImgResize imgR = new ImgResize();
        HtmlSanitizer sanitizer = new HtmlSanitizer();
        // GET: Products
        [Authorize(Roles = "Users,INIsubscribers,Admin,Sub-Admin,SuperAdmin")]
        public ActionResult myProducts(string Id)
        {
            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            Id = Id.Replace("$25", "/");
            Id = Id.Replace("$24", "+");
            string ID = Cryptoclass.DecryptStringAES(Id, s);
            var pro = db.ProductTb.Where(p => p.UserID == ID);
            if (pro.Count() > 0)
            {
                return View(pro);
            }
            else
            {
                TempData["error"] = "You don't have any registered product(s).";
                return View(pro);
            }

        }
        [Authorize(Roles = "Users,INIsubscribers,Admin,Sub-Admin,SuperAdmin")]
        [AllowAnonymous]
        public ActionResult Register_Product()
        {
            return View();
        }
        [Authorize(Roles = "Users,INIsubscribers,Admin,Sub-Admin,SuperAdmin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Register_Product(FormCollection frm, RegisterProductViewModel rgp, IEnumerable<HttpPostedFileBase> ProductImage)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Random rd = new Random();
                    int productcode = rd.Next(99999);
                    int pcv = Convert.ToInt32(frm["ProductCategory"]);
                    int cv = Convert.ToInt32(frm["Country"]);
                    int sv = Convert.ToInt32(frm["State"]);
                    var userId = Session["Id"].ToString();
                    ProductsInfo pr = new ProductsInfo();
                    var c = await db.country.FindAsync(cv);
                    var pc = await db.productCategory.FindAsync(pcv);
                    var st = await db.State.FindAsync(sv);
                    pr.ProductCode = productcode;
                    pr.ProductName = rgp.ProductName;
                    pr.Manufacturer = rgp.Manufacturer;
                    pr.ProductCategory = pc.PCatName;
                    pr.Pharmaceutic_Drug_Category = rgp.drugs_categories.Drug_Category_Id;
                    pr.Product_Summary = Server.HtmlEncode(sanitizer.Sanitize(rgp.Product_Summary));
                    pr.ProductDescription = Server.HtmlEncode(sanitizer.Sanitize(rgp.ProductDescription, "", null));
                    pr.price = Convert.ToDecimal(frm["price"]);
                    pr.Address = rgp.Address;
                    pr.Country = c.CountryName;
                    pr.State = st.StateName;
                    pr.location = frm["city"];
                    pr.UserID = userId;
                    pr.ApprovedStatus = "P";
                    pr.VerifiedStatus = "NV";
                    pr.NotifyStatus = 0;
                    pr.regDate = DateTime.UtcNow;
                    pr.ProductCondition = rgp.productCond.id;
                    if (ProductImage.Count() != 0 || ProductImage.FirstOrDefault() != null)
                    {
                        #region
                        int counter = 0;
                        foreach (HttpPostedFileBase image in ProductImage)
                        {
                            counter++;
                            byte[] img ;
                            //image.InputStream.Read(img, 0, (int)image.InputStream.Length);
                            System.Drawing.Image validateWH = System.Drawing.Image.FromStream(image.InputStream, true, true);
                            img = imgR.imageToByteArray(validateWH, validateWH.RawFormat, 305, 400);
                            decimal size = Math.Round(image.ContentLength / (decimal)1024, 2);
                            if (size <= 1000)
                            {
                                if (counter == 1)
                                {
                                    pr.ProductImage = img;
                                }
                                else if (counter == 2)
                                {
                                    pr.ProductImage2 = img;
                                }
                                else if (counter == 3)
                                {
                                    pr.ProductImage3 = img;
                                }
                                else
                                {
                                    pr.ProductImage4 = img;

                                }

                            }
                            else
                            {
                                TempData["error"] = "Image " + counter + "'s size exceeded 1000kb(i.e 1MB) ";
                                return View(rgp);
                            }
                        #endregion
                        }
                    }
                    else
                    {
                        TempData["error"] = "Please Upload at least one product's image";
                        return View(rgp);
                    }
                    db.ProductTb.Add(pr);
                    await db.SaveChangesAsync();

                    TempData["success"] = "Product was created successfull!";
                    TempData["info"] = "We will Notify you upon approver, Thanks.";
                    ViewBag.Alert = "Add More Products";

                    return Redirect(Request.Url.ToString());
                }
                ModelState.AddModelError("", "Fill All Required field!");
                return View(rgp);
            }
            catch
            {
                TempData["error"] = "Error occurred";
                return View(rgp);
            }

        }

        [Authorize(Roles = "Admin,Sub-Admin,SuperAdmin")]
        [HttpGet]
        public ActionResult NewlyRegisteredProducts()
        {
            var prod = db.ProductTb.Where(p => p.NotifyStatus == 0 && p.ApprovedStatus == "P").Include(b => b.OwnerInfo).OrderByDescending(p => p.regDate);
            if (prod.Count() != 0)
            {
                return View(prod);
            }
            else
            {
                TempData["error"] = "Alert: No New Registered Products";
                return RedirectToAction("index", "Admin");
            }
        }
        [Authorize(Roles = "Admin,Sub-Admin,SuperAdmin")]
        public async Task<ActionResult> ProductDetails(string id)
        {

            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            id = id.Replace("$25", "/");
            id = id.Replace("$24", "+");
            int refID = Convert.ToInt32(Cryptoclass.DecryptStringAES(id, s));
            ProductsInfo p = await db.ProductTb.FindAsync(refID);
            return View(p);
        }
        //Approve a product
        [Authorize(Roles = "Sub-Admin,Admin,SuperAdmin")]
        [HttpPost]
        public async Task<JsonResult> Approve(IEnumerable<int> Id)
        {
            try
            {
                if (Id != null)
                {
                    if (Id.Count() > 0)
                    {
                        foreach (int id in Id)
                        {

                            ProductsInfo p = await db.ProductTb.FirstAsync(r=>r.Id == id);
                            p.ApprovedStatus = "A";
                            p.NotifyStatus = 1;
                            p.ApprovedDate = DateTime.UtcNow;
                            p.Signedby = Session["Name"].ToString();
                            await db.SaveChangesAsync();
                            //send Approval to mail to the business owner.
                           await em.SendProductApprovedMail(p.OwnerInfo.Email, p.ProductName,p.OwnerInfo.FirstName);

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
        //Delete a product
        [Authorize(Roles = "Admin,Users,INIsubscribers,SuperAdmin")]
        [HttpPost]
        public async Task<JsonResult> Delete(IEnumerable<int>Id)
        {
            try
            {
                if (Id != null)
                {
                    if (Id.Count() > 0)
                    {
                        foreach (int id in Id)
                        {
                            var p = await db.ProductTb.FindAsync(id);
                            db.ProductTb.Remove(p);
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
        //view list of Approved poducts to be displayed for verification
        [Authorize(Roles = "Admin,Sub-Admin,SuperAdmin")]
        public ActionResult ApprovedProducts()
        {
            var bis = db.ProductTb.Where(b => b.ApprovedStatus == "A" && b.NotifyStatus == 1 && b.VerifiedStatus == "NV").ToList().OrderByDescending(b => b.regDate);
            if (bis.Count() > 0)
            {
                return View(bis);
            }
            else
            {
                TempData["error"] = "Alert: No Product has been approved yet";
                return RedirectToAction("index", "Admin");

            }

        }
        //Verify product
        [Authorize(Roles = "Admin,Sub-Admin,SuperAdmin")]
        [HttpPost]
        public async Task<JsonResult> VerifyProduct(IEnumerable<int> Id)
        {
            try
            {
                if (Id != null)
                {
                    if (Id.Count() > 0)
                    {
                        foreach (var id in Id)
                        {
                            ProductsInfo p = await db.ProductTb.FirstAsync(r => r.Id == id);
                            p.VerifiedStatus = "V";
                            p.VerifyDate = DateTime.UtcNow;
                            p.Signedby = Session["Name"].ToString();
                            await db.SaveChangesAsync();
                            //send Verification mail to the business owner.
                            await em.SendProductVerifyMail(p.OwnerInfo.Email, p.ProductName,p.OwnerInfo.FirstName);
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

        //View list of already  verified products
        [Authorize(Roles = "Admin,Sub-Admin,SuperAdmin")]
        [HttpGet]
        public ActionResult VerifiedProduct()
        {
            var p = db.ProductTb.Where(b => b.ApprovedStatus == "A" && b.NotifyStatus == 1 && b.VerifiedStatus == "V").ToList().OrderByDescending(b => b.regDate);
            if (p.Count() > 0)
            {
                return View(p);
            }
            else
            {
                TempData["error"] = "Alert: No Product(s) has been verified yet";
                return View(p);

            }
        }
    }
}