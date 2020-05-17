using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iHealth2.Models;
using System.Threading;
using System.Threading.Tasks;
using Ganss.XSS;
namespace iHealth2.Controllers.LatextNews
{
    public class LatestNewsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: LatestNews
        [Authorize(Roles="Admin")]
        public ActionResult ViewNewsbyAdmin()
        {
            var News = db.News.OrderByDescending(b => b.EntryDate).ToList();
            return View(News);
        }

        [Authorize(Roles="Admin")]
        public ActionResult Create_News()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Admin")][ValidateInput(false)]
        public async Task<ActionResult> Create_News(LatestNews c, HttpPostedFileBase img)
        {
            var santizer = new HtmlSanitizer();
            if (ModelState.IsValid)
            {
                LatestNews l = new LatestNews();
                l.Title = c.Title;
                l.Content = santizer.Sanitize(Server.HtmlEncode(c.Content));
                l.EntryDate = DateTime.UtcNow;
                l.Url = c.Url;
                l.ImageUrl = c.ImageUrl;
                if (img != null)
                {
                    byte[] im = new byte[img.InputStream.Length];
                    img.InputStream.Read(im, 0, (int)img.InputStream.Length);
                    l.Image = im;
                }
                db.News.Add(l);
               await db.SaveChangesAsync();
                TempData["success"] = "Latest News Registration is Successfull";
                return Redirect(Request.UrlReferrer.ToString());
            }
            ModelState.AddModelError("", "Correct your errors!");
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> HealthNews(int News_View)
        {
            var news = await db.News.FindAsync(News_View);
            return View(news);
        }
       [AllowAnonymous]
        public ActionResult AllNews()
        {
            var News = db.News.OrderByDescending(b => b.EntryDate).ToList();
            return View(News);
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles="Admin")]
       public async Task<JsonResult> delnews(IEnumerable<int> Id)
       {
           if (Id != null)
           {
               if (Id.Count() > 0)
               {
                   foreach (var id in Id)
                   {
                       var news = await db.News.FindAsync(id);
                       db.News.Remove(news);
                      await db.SaveChangesAsync();
                   }
                   TempData["success"] = "News has been successfully deleted.";
                   return Json(true, JsonRequestBehavior.AllowGet);

               }
           }
           TempData["error"] = "There is an error deleting the selected news";
           return Json("false", JsonRequestBehavior.AllowGet);
       }
    }
}