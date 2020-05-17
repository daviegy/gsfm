using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iHealth2.Models;
using System.Threading.Tasks;
using System.Threading;
using IhealthPaging;

namespace iHealth2.Controllers.SearchEngine
{
    public class SearchController : Controller
    {
        private const int DefaultPageSize = 10;
        ApplicationDbContext db = new ApplicationDbContext();
        country c = new country();
        Category cat = new Category();
        // GET: Search
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Searchresults(FormCollection frm)
        {
           
                List<SearchResultModel> srmList = new List<SearchResultModel>();
                country c = new country();
                Category cat = new Category();
                string ca = frm["Category"]; string suc1 = frm["subCat1"]; string suc2 = frm["subCat2"];
                string Co = frm["Country"]; string S = frm["state"]; string city = frm["city"];
                if (!string.IsNullOrEmpty(ca) && !string.IsNullOrEmpty(Co) && !string.IsNullOrEmpty(S) && !string.IsNullOrEmpty(city))
                {
                    if (!string.IsNullOrEmpty(suc1))
                    {
                        if (!string.IsNullOrEmpty(suc2))
                        {

                            int cate = Convert.ToInt32(ca); int sub1 = Convert.ToInt32(suc1); int sub2 = Convert.ToInt32(suc2); var coun3 = Convert.ToInt32(Co); var sta = Convert.ToInt32(S);
                            var ct = db.category.Find(cate); var sb1 = db.SubCategory1.Find(sub1); var sb2 = db.SubCategory2.Find(sub2); var con3 = db.country.Find(coun3); var st = db.State.Find(sta);
                            var ctname = ct.CatName;
                            var result = db.BusinessInfoes.OrderByDescending(b => b.VerifiedStatus)
                                .Where(b => b.Category == ct.CatName && b.subCategory1 == sb1.SubCat1Name && b.subCategory2 == sb2.SubCat2name && b.Country == con3.CountryName && b.State == st.StateName && b.City.Contains(city) && b.NotifyStatus == 1 && b.ApprovedStatus == "A").OrderByDescending(b => b.VerifiedStatus)
                                .Select(b => new SearchResultModel
                                {
                                    ID = b.ID,
                                    CompanyName = b.businessName,
                                    CompanyEmail = b.Email,
                                    CompanyPhone = b.Phone,
                                    subCat1 = b.subCategory1,
                                    subCat2 = b.subCategory2,
                                    Address = b.Address,
                                    Category = b.Category,
                                    Website = b.Website,
                                    Logo = b.logo
                                });
                            ViewBag.category = new SelectList(db.category, "ID", "CatName", cat.ID);
                            ViewBag.Country = new SelectList(db.country, "ID", "CountryName", c.ID);

                            return View(result);

                        }
                        else
                        {

                            int cate = Convert.ToInt32(ca); int sub1 = Convert.ToInt32(suc1); var coun3 = Convert.ToInt32(Co); var sta = Convert.ToInt32(S);
                            var ct = db.category.Find(cate); var sb1 = db.SubCategory1.Find(sub1); var con3 = db.country.Find(coun3); var st = db.State.Find(sta);
                            var ctname = ct.CatName;
                            var result = db.BusinessInfoes.OrderByDescending(b => b.VerifiedStatus)
                                .Where(b => b.Category == ct.CatName && b.subCategory1 == sb1.SubCat1Name && b.Country == con3.CountryName && b.State == st.StateName && b.City.Contains(city) && b.NotifyStatus == 1 && b.ApprovedStatus == "A").OrderByDescending(b => b.VerifiedStatus)
                                .Select(b => new SearchResultModel
                                {
                                    ID = b.ID,
                                    CompanyName = b.businessName,
                                    CompanyEmail = b.Email,
                                    CompanyPhone = b.Phone,
                                    subCat1 = b.subCategory1,
                                    subCat2 = b.subCategory2,
                                    Address = b.Address,
                                    Category = b.Category,
                                    Website = b.Website,
                                    Logo = b.logo
                                });


                            //if (Request.IsAjaxRequest())
                            //{
                            //    return PartialView("_SearchResultPartial", result);
                            //}

                            ViewBag.category = new SelectList(db.category, "ID", "CatName", cat.ID);
                            ViewBag.Country = new SelectList(db.country, "ID", "CountryName", c.ID);
                            return View(result);
                        }

                    }
                    else
                    {

                        int cate = Convert.ToInt32(ca); var coun3 = Convert.ToInt32(Co); var sta = Convert.ToInt32(S);
                        var ct = db.category.Find(cate); var con3 = db.country.Find(coun3); var st = db.State.Find(sta);
                        var ctname = ct.CatName;
                        var result = db.BusinessInfoes.OrderByDescending(b => b.VerifiedStatus)
                            .Where(b => b.Category == ct.CatName && b.Country == con3.CountryName && b.State == st.StateName && b.City.Contains(city) && b.NotifyStatus == 1 && b.ApprovedStatus == "A").OrderByDescending(b => b.VerifiedStatus)
                            .Select(b => new SearchResultModel
                            {
                                ID = b.ID,
                                CompanyName = b.businessName,
                                CompanyEmail = b.Email,
                                CompanyPhone = b.Phone,
                                subCat1 = b.subCategory1,
                                subCat2 = b.subCategory2,
                                Address = b.Address,
                                Category = b.Category,
                                Website = b.Website,
                                Logo = b.logo
                            });

                        ViewBag.category = new SelectList(db.category, "ID", "CatName", cat.ID);
                        ViewBag.Country = new SelectList(db.country, "ID", "CountryName", c.ID);
                        return View(result);

                    }
                }
                TempData["error"] = "Search Error: Filled all Necessary Field in the Advance Search Box";
                return RedirectToAction("Index", "Home");
           
        }

        public async Task<ActionResult> QsearchResult(int? page,string SearchText)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            if (SearchText == "Medical Doctor")
            {
                ViewBag.profession = SearchText;
                return await Task.Run(() =>
                {
                    var result = db.Users.Where(b => b.Profession.Contains(SearchText) ||
                          b.Nationality.Contains(SearchText) || b.State.Contains(SearchText) || b.City.Contains(SearchText))
                       .Select(b => new find_professionals_viewModel
                       {
                           Name = b.LastName + b.FirstName,
                           profession = b.Profession,
                           summary = b.backgroundinfo,
                           fburl = b.facebookUrl,
                           twUrl = b.twitterUrl,
                           lnUrl = b.linkedinUrl,
                           image = b.Photo
                       });
                    if (result != null)
                    {
                        return View("Professionals", result.OrderBy(p => p.Name).ToPagedList(currentPageIndex, DefaultPageSize));
                    }
                    return View("Professionals", result.OrderBy(p => p.Name).ToPagedList(currentPageIndex, DefaultPageSize));
                });
            }
            else
            {
                return await Task.Run(() =>
                {
                    var result = db.BusinessInfoes.Where(b => b.businessName.Contains(SearchText) || b.Category.Contains(SearchText) ||
                       b.subCategory1.Contains(SearchText) || b.subCategory2.Contains(SearchText) ||
                       b.Country.Contains(SearchText) || b.State.Contains(SearchText) || b.City.Contains(SearchText)).OrderByDescending(b => b.VerifiedStatus)
                       .Select(b => new SearchResultModel
                       {
                           ID = b.ID,
                           CompanyName = b.businessName,
                           CompanyEmail = b.Email,
                           CompanyPhone = b.Phone,
                           subCat1 = b.subCategory1,
                           subCat2 = b.subCategory2,
                           Address = b.Address,
                           Category = b.Category,
                           Website = b.Website,
                           Logo = b.logo
                       });
                    if (result != null)
                    {

                        return View("Searchresults", result);
                    }


                    return View("Searchresults", result);


                });
            }
          

        }
        //TODO
        public ActionResult Search_Internal(FormCollection f)
        {
           // var url = 
            string SearchTerm = f["terms"];
            string location = f["CitySearch"];
            if(!string.IsNullOrEmpty(SearchTerm) && !string.IsNullOrEmpty(location))
            {
                var results = db.BusinessInfoes.Where(b => b.subCategory1.Contains(SearchTerm) && b.City.Contains(location))
                    .Select(b => new SearchResultModel {
                        ID = b.ID,
                        CompanyName = b.businessName,
                        CompanyEmail = b.Email,
                        CompanyPhone = b.Phone,
                        subCat1 = b.subCategory1,
                        subCat2 = b.subCategory2,
                        Address = b.Address,
                        Category = b.Category,
                        Website = b.Website,
                        Logo = b.logo
                    });
                ViewBag.category = new SelectList(db.category, "ID", "CatName", cat.ID);
                ViewBag.Country = new SelectList(db.country, "ID", "CountryName", c.ID);
                return View("Searchresults", results);
            }
            return Redirect(Request.UrlReferrer.ToString());
        }           
        public ActionResult HomePageAutocomplete(string term)
        {
            var model = db.category.Where(c => c.CatName.Contains(term)).Select(c => c.CatName).ToArray();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult HospAutocomplete(string term)
        {
            var model = db.SubCategory1.Where(s => s.SubCat1Name.Contains(term)).Select(b => b.SubCat1Name).Distinct();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Professionals(int?page,string profession)
        {
            ViewBag.profession = profession;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            var prof = from user in db.Users
                       where user.Profession == profession
                       select new find_professionals_viewModel()
                       {
                           Name = user.LastName + user.FirstName,
                           profession = user.Profession,
                           summary = user.backgroundinfo,
                           fburl = user.facebookUrl,
                           twUrl = user.twitterUrl,
                           lnUrl = user.linkedinUrl,
                           image = user.Photo
                       };
            return View(prof.OrderBy(p=>p.Name).ToPagedList(currentPageIndex, DefaultPageSize));
        }
    }
}