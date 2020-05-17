using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iHealth2.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using iHealth2.CustomClasses;
using IhealthPaging;
namespace iHealth2.Controllers.Menu
{
    public class PharmacyController : Controller
    {
        private const int DefaultPageSize = 10;
       
        ApplicationDbContext db = new ApplicationDbContext();
        // this emailSender is a webservice
        EmailSender em = new EmailSender();
        ImgResize imgR = new ImgResize();
        Emails ems = new Emails();
        // GET: Pharmacy
        public ActionResult Find_Pharmacy(int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            var bs = db.BusinessInfoes.Where(b => b.Category.Contains("Pharmaceuticals")
                && b.ApprovedStatus == "A" && b.NotifyStatus == 1)
               .OrderByDescending(b => b.Recommended_Status).ThenByDescending(b => b.VerifiedStatus)
                 .ToPagedList(currentPageIndex, DefaultPageSize); 
            return View(bs);
        }
        public PartialViewResult find_SubCat(int? page,string sbValue)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            object Model = null;
            Model = db.BusinessInfoes.Where(b => b.subCategory1.Contains(sbValue)
                && b.ApprovedStatus == "A" && b.NotifyStatus == 1)
                .OrderByDescending(b => b.Recommended_Status).ThenByDescending(b => b.VerifiedStatus)
                 .ToPagedList(currentPageIndex, DefaultPageSize); 
            return PartialView("_listPharmacy", Model);

        }
        public PartialViewResult search_by_state(int? page,string state)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            object Model = null;
            Model = db.BusinessInfoes.Where(b => b.State.Contains(state) 
                && b.ApprovedStatus == "A" && b.NotifyStatus == 1 && b.Category == "Pharmaceuticals")
                .OrderByDescending(b => b.Recommended_Status).ThenByDescending(b => b.VerifiedStatus)
                .ToPagedList(currentPageIndex, DefaultPageSize); 
            return PartialView("_listPharmacy", Model);
        }
        public ActionResult Search_Drugs(int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            var drugs = db.ProductTb
                .Where(d => d.ProductCategory
                    .Contains("Pharmaceutical drug") && d.ApprovedStatus == "A")
                    .ToList().OrderByDescending(d => d.VerifiedStatus)
                    .ToPagedList(currentPageIndex, DefaultPageSize);
            return View(drugs);
        }
        public ActionResult Search_Drugs_By_Category(int? page, string ctgry)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            var drugs = db.ProductTb
                .Where(d => d.ProductCategory.Contains("Pharmaceutical drug")
                    && d.ApprovedStatus == "A" && d.Pharmaceutic_Drug_Category == ctgry)
                    .ToList().OrderByDescending(d => d.VerifiedStatus)
                    .ToPagedList(currentPageIndex, DefaultPageSize);
            return View("../pharmacy/search_drugs",drugs);
        }
        public ActionResult Search_Drugs_By_Manufacturer(int? page, string ctgry)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            var drugs = db.ProductTb
                .Where(d => d.ProductCategory.Contains("Pharmaceutical drug")
                    && d.ApprovedStatus == "A" && d.Manufacturer == ctgry)
                    .ToList().OrderByDescending(d => d.VerifiedStatus)
                    .ToPagedList(currentPageIndex, DefaultPageSize);
            return View("../pharmacy/search_drugs", drugs);
        }
        public ActionResult Search_Drugs_By_Price(int? page, string price)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            if (price == "0-5000")
            {
                var drugs = db.ProductTb
               .Where(d => d.ProductCategory
                   .Contains("Pharmaceutical drug") && d.ApprovedStatus == "A" && (d.price >= 0 && d.price <= 5000))
                   .ToList().OrderByDescending(d => d.VerifiedStatus)
                   .ToPagedList(currentPageIndex, DefaultPageSize);
                return View("../pharmacy/search_drugs", drugs);  
            }
            else if (price == "5000-10000")
            {
                var drugs = db.ProductTb
               .Where(d => d.ProductCategory
                   .Contains("pharmaceutical drug") && d.ApprovedStatus == "A" && (d.price >= 5000 && d.price <= 10000))
                   .ToList().OrderByDescending(d => d.VerifiedStatus)
                   .ToPagedList(currentPageIndex, DefaultPageSize);
                return View("../pharmacy/search_drugs", drugs);  
            }
            else if (price == "10000-20000")
            {
                var drugs = db.ProductTb
               .Where(d => d.ProductCategory
                   .Contains("pharmaceutical drug") && d.ApprovedStatus == "A" && (d.price >= 10000 && d.price <= 20000))
                   .ToList().OrderByDescending(d => d.VerifiedStatus)
                   .ToPagedList(currentPageIndex, DefaultPageSize);
                return View("../pharmacy/search_drugs", drugs);  
            }
            else if (price == "20000-40000")
            {
                var drugs = db.ProductTb
               .Where(d => d.ProductCategory
                   .Contains("pharmaceutical drug") && d.ApprovedStatus == "A" && (d.price >= 20000 && d.price <= 40000))
                   .ToList().OrderByDescending(d => d.VerifiedStatus)
                   .ToPagedList(currentPageIndex, DefaultPageSize);
                return View("../pharmacy/search_drugs", drugs);  
            }
            else
            {
                var drugs = db.ProductTb
               .Where(d => d.ProductCategory
                   .Contains("pharmaceutical drug") && d.ApprovedStatus == "A" && d.price >= 40000)
                   .ToList().OrderByDescending(d => d.VerifiedStatus)
                   .ToPagedList(currentPageIndex, DefaultPageSize);
                return View("../pharmacy/search_drugs", drugs);  
            }
                     
        }
        public async Task<ActionResult> Drug_Detail(string did)
        {
            string rs = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            did = did.Replace("$25", "/");
            did = did.Replace("$24", "+");
            String ddid = iHealth2.CustomClasses.Cryptoclass.DecryptStringAES(did, rs);
            int id = Convert.ToInt32(ddid);
            var drug = await db.ProductTb.FindAsync(id);
            return View(drug);
        }
        public ActionResult UrgentNeedForDrugs()
        {
            return View();
        }
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<ActionResult> UrgentNeedForDrugs(UrgentNeedForDrugViewModel model, FormCollection f, HttpPostedFileBase prescription)
        {
            try
            {
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
                        var con3ID = Convert.ToInt32(f["Country"]);
                        var con3 = await db.country.FindAsync(con3ID);
                        var st_Id = Convert.ToInt32(f["State"]);
                        var st = await db.State.FindAsync(st_Id);
                       
                        UrgentNeedForDrugsTb urgdr = new UrgentNeedForDrugsTb();
                        urgdr.DrugName = model.DrugName;
                        urgdr.dosageForm = model.dosageForm.dosageFormValue;
                        urgdr.drugStrength = model.DrugStrength;
                        urgdr.Manufacturer = model.Manufacturer;
                        urgdr.manufacturerCountry = model.ManufacturerCountry;
                        urgdr.MoreInformation = model.Description;
                        //if (DrugName.Count() > 1 && dosageForm.Count() > 1)
                        //{
                        //    foreach (string dName in DrugName)
                        //    {
                        //        urgdr.DrugName = dName;
                        //    }                           
                        //}
                       // urgdr.dosageForm = f["dosageForm"];
                        urgdr.Name = model.Name;
                        urgdr.Phone = model.Phone;
                        urgdr.Email = model.Email;
                        urgdr.City = model.City;
                        urgdr.Address = model.Address;
                        urgdr.Country = con3.CountryName;
                        urgdr.State = st.StateName;
                        urgdr.landMark = model.Landmark;
                        urgdr.notifyStatus = 0;
                        urgdr.RequestStatus = "Fresh";
                        urgdr.RequestedDate = DateTime.UtcNow;
                        if (prescription != null)
                        {
                                byte[] img;
                                //image.InputStream.Read(img, 0, (int)image.InputStream.Length);
                                System.Drawing.Image validateWH = System.Drawing.Image.FromStream(prescription.InputStream, true, true);
                                img = imgR.imageToByteArray(validateWH, validateWH.RawFormat, 305, 400);
                                //decimal size = Math.Round(prescription.ContentLength / (decimal)1024, 2);
                                urgdr.prescription = img;
                         
                        }
                       // db.urgentNeedforDrugsTb.Add(urgdr);
                        await db.SaveChangesAsync();
                        //TODO: send an email to ihealth about client request
                        string subject = "Urgent Need For Drugs";
                        string body = model.Name + " with the following contact number " + model.Phone + " , has placed an urgent request for the drug with the following information:" +
                            "" + model.DrugName + ", " + model.Manufacturer + ".";
                        await em.SubcribersMailSender(model.Email, subject, body,model.Name);
                        TempData["success"] = "Your request has been submitted successfully";
                        TempData["info"] = "We get back to you shortly, Thanks for using iHealth GSFM";
                        ModelState.Clear();
                        return View();

                    }
                }
            }
            catch
            {
                TempData["error"] = "There is error processing your request. please try again later, thanks.";
                return View();
            }
            return View();
        }
        
    }
}