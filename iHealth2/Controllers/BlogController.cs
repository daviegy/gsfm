using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iHealth2.Models;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
using System.Diagnostics;
using IhealthPaging;
using iHealth2.CustomClasses;
using System.IO;
using Ganss.XSS;

namespace iHealth2.Controllers
{
    public class BlogController : Controller
    {
        ImgResize ImageRz = new ImgResize();
        ApplicationDbContext db = new ApplicationDbContext();
        private const int DefaultPageSize = 9;
        Emails em = new Emails();
        SEO_Friendly_URL seo = new SEO_Friendly_URL();
        HtmlSanitizer sanitizer = new HtmlSanitizer();
        // GET: Blog
        public ActionResult Index()
        {
            return View();
        }

        // GET: Blog/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Blog/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }
        #region // manage blog post
        // POST: Blog/Create
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create(create_blog_post_viewModel model, FormCollection f, HttpPostedFileBase Feature_img)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(f["short_description"]) && !string.IsNullOrEmpty(f["post_content"]))
                    {
                        blog_Post bp = new blog_Post();
                        bp.post_Title = model.post_Title;
                        bp.short_description = Server.HtmlEncode(sanitizer.Sanitize(f["short_description"]));
                        bp.post_content = Server.HtmlEncode(sanitizer.Sanitize(f["post_content"], "", null));
                        bp.post_tags = model.post_tags;
                        bp.post_url = !string.IsNullOrEmpty(model.post_url) ? seo.generate_title(model.post_url) : seo.generate_title(model.post_Title);
                        bp.meta_description = Server.HtmlEncode(sanitizer.Sanitize(f["meta_description"]));
                        bp.meta_keyword = model.meta_keyword;
                        bp.meta_title = !string.IsNullOrEmpty(model.meta_title) ? model.meta_title : model.post_Title;
                        bp.published_by_Id = Session["Id"].ToString();
                        if (f["chkVideoUrlAsFeatureImg"] != null)
                        {
                            if (f["chkVideoUrlAsFeatureImg"].ToString() == "on")
                            {
                                if (!string.IsNullOrEmpty(model.video_url))
                                {
                                    bp.video_url = model.video_url;
                                    bp.use_video_as_cover_img = true;
                                }
                                else
                                {
                                    TempData["error"] = "Supply URL to your video";
                                    ViewBag.videoerror = "Supply Url to your video";
                                    return View();
                                }

                            }
                            else
                            {

                                bp.use_video_as_cover_img = false;
                            }

                        }
                        else
                        {
                            bp.use_video_as_cover_img = false;
                        }

                        if (Feature_img != null)
                        {
                            var ImageName = Path.GetFileName(Feature_img.FileName);
                            string physicalPath = Server.MapPath("~/Content/Blog_post_image/" + ImageName);
                            byte[] img;
                            System.Drawing.Image image2validate = System.Drawing.Image.FromStream(Feature_img.InputStream, true, true);
                            decimal size = Math.Round(Feature_img.ContentLength / (decimal)1024, 2);
                            if (size <= 2000)
                            {
                                img = ImageRz.imageToByteArray(image2validate, image2validate.RawFormat, 1280, 800);
                                bp.Feature_Image = img;
                                System.IO.File.WriteAllBytes(physicalPath, img);
                                Feature_img.SaveAs(physicalPath);
                                bp.Image_url = ImageName;

                            }
                            else
                            {
                                TempData["error"] = "Uploaded Image size exceeded 2000kb(i.e.2MB)!";
                                return View();
                            }

                        }
                        if (f["chkAccept"] != null)
                        {
                            if (f["chkAccept"].ToString() == "on")
                            {
                                bp.allow_comment = true;
                            }
                            else
                            {
                                bp.allow_comment = false;
                            }
                        }
                        else
                        {
                            bp.allow_comment = false;
                        }
                        bp.publish_date = !string.IsNullOrEmpty(f["publish_date"]) ? Convert.ToDateTime(f["publish_date"]) : DateTime.UtcNow.Date;
                        db.blog_posts.Add(bp);
                        await db.SaveChangesAsync();
                        TempData["success"] = "Article has been published successfully!";
                        return Redirect(Request.Url.ToString());
                    }
                    else
                    {
                        ModelState.AddModelError("", "Field Marked with '*' must be filled");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Field Marked with '*' must be filled");
                    return View();
                }
                // TempData["success"] = "Article has been published successfully!";
                // return Redirect(Request.Url.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
                //return View();
            }
        }

        // GET: Blog/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int id)
        {
            //TODO: CREATE A PLACEHOLDER FOR THIS MESSAGE "last published date", this will be posted to the edit page.;
            var bp = await db.blog_posts.FindAsync(id);
            ViewBag.LastPublishedDate = "Last published date : ";
            return View(new Edit_blog_post_viewModel{
            post_Title = bp.post_Title,
            meta_title = bp.meta_title,
            short_description = Server.HtmlDecode(sanitizer.Sanitize(bp.short_description)),
            post_content = Server.HtmlDecode(sanitizer.Sanitize(bp.post_content)),
            meta_description = Server.HtmlDecode(sanitizer.Sanitize(bp.meta_description)),
            meta_keyword = bp.meta_keyword,
            post_tags = bp.post_tags,
            video_url = bp.video_url,
            post_url =seo.GenerateSlug(bp.post_id.ToString(),bp.post_url),
            publish_date = bp.publish_date,
            Id = bp.post_id,
            use_video_as_cover_img = bp.use_video_as_cover_img,
            allow_comment= bp.allow_comment
            });
        }

        // POST: Blog/Edit/5
        [HttpPost][ActionName("Edit")][ValidateAntiForgeryToken][Authorize][ValidateInput(false)]
        public async Task<ActionResult> Edit_Blog_Post(Edit_blog_post_viewModel model, FormCollection f, HttpPostedFileBase Feature_img)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(f["short_description"]) && !string.IsNullOrEmpty(f["post_content"]))
                    {

                        blog_Post bp = await db.blog_posts.FindAsync(model.Id);
                        bp.post_Title = model.post_Title;
                        bp.short_description = Server.HtmlEncode(sanitizer.Sanitize(f["short_description"]));
                        // var content = sanitizer.Sanitize(f["post_content"], "", null);
                        bp.post_content = Server.HtmlEncode(sanitizer.Sanitize(f["post_content"], "", null));
                        bp.post_tags = model.post_tags;
                        bp.video_url = model.video_url;
                        bp.post_url = !string.IsNullOrEmpty(model.post_url) ? model.post_url : model.post_Title;
                        bp.meta_description = Server.HtmlEncode(sanitizer.Sanitize(f["meta_description"]));
                        bp.meta_keyword = model.meta_keyword;
                        bp.meta_title = !string.IsNullOrEmpty(model.meta_title) ? model.meta_title : model.post_Title;
                        // var che = f["chkVideoUrlAsFeatureImg"];
                        if (f["chkVideoUrlAsFeatureImg"] != null)
                        {
                            if (f["chkVideoUrlAsFeatureImg"].ToString() == "on")
                            {
                                if (!string.IsNullOrEmpty(model.video_url))
                                {
                                    bp.video_url = model.video_url;
                                    bp.use_video_as_cover_img = true;
                                }
                                else
                                {
                                    ViewBag.videoerror = "Supply Url to your video";
                                    return View();
                                }

                            }
                            else
                            {
                                bp.use_video_as_cover_img = false;
                            }

                        }
                        else
                        {
                            bp.use_video_as_cover_img = false;
                        }


                        // (f["chkVideoUrlAsFeatureImg"].ToString() == "false") ? false : true
                        if (Feature_img != null)
                        {
                            var ImageName = Path.GetFileName(Feature_img.FileName);
                            string physicalPath = Server.MapPath("~/content/Blog_post_image/" + ImageName);
                            byte[] img;
                            System.Drawing.Image image2validate = System.Drawing.Image.FromStream(Feature_img.InputStream, true, true);
                            decimal size = Math.Round(Feature_img.ContentLength / (decimal)1024, 2);
                            if (size <= 2000)
                            {
                                img = ImageRz.imageToByteArray(image2validate, image2validate.RawFormat, 1280, 800);
                                bp.Feature_Image = img;
                                System.IO.File.WriteAllBytes(physicalPath, img);
                                Feature_img.SaveAs(physicalPath);
                                bp.Image_url = ImageName;

                            }
                            else
                            {
                                TempData["error"] = "Uploaded Image size exceeded 2000kb(i.e.2MB)!";
                                return View();
                            }

                        }
                        if (f["chkAccept"] != null)
                        {
                            if (f["chkAccept"].ToString() == "on")
                            {
                                bp.allow_comment = true;
                            }
                            else
                            {
                                bp.allow_comment = false;
                            }
                        }
                        else
                        {
                            bp.allow_comment = false;
                        }
                        bp.publish_date = !string.IsNullOrEmpty(f["publish_date"]) ? Convert.ToDateTime(f["publish_date"]) : DateTime.UtcNow.Date;
                        await db.SaveChangesAsync();
                        //Transfer logic back to the blog owner's blog list begins
                        string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];

                        String UserID = iHealth2.CustomClasses.Cryptoclass.EncryptStringAES(bp.published_by_Id, s);
                        UserID = UserID.Replace("/", "$25");
                        UserID = UserID.Replace("+", "$24");
                        //ends
                        TempData["success"] = "Article has been updated successfully!";
                        return RedirectToAction("my_blog_posts", new { post_by_id = UserID });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Field Marked with '*' must be filled");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Field Marked with '*' must be filled");
                    return View();
                }

            }
            catch
            {
                return View();
            }
        }

        // POST: Blog/Delete/5
        [HttpPost][Authorize]
        public async Task<ActionResult> Delete_Post(IEnumerable<int> Id)
        {
            try
            {
                if (Id != null)
                {
                    if (Id.Count() > 0)
                    {
                        foreach (int id in Id)
                        {
                            var b = await db.blog_posts.FindAsync(id);
                            
                            db.blog_posts.Remove(b);
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
        [Authorize]
        public ActionResult my_blog_posts(string post_by_id)//this method allows post creator to view all his/her blogpost
        {
            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            post_by_id = post_by_id.Replace("$25", "/");
            post_by_id = post_by_id.Replace("$24", "+");
            string DecryptId = Cryptoclass.DecryptStringAES(post_by_id, s);
            var bp = db.blog_posts.Where(ps => ps.published_by_Id == DecryptId).OrderByDescending(pd => pd.publish_date);
            return View(bp);
        }
        [AllowAnonymous]
        public ActionResult blog_list(int? page)//this method allow site visitors to view blog
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View(db.blog_posts.ToList().OrderByDescending(b => b.publish_date).ToPagedList(currentPageIndex, DefaultPageSize));
        }
        public async Task<ActionResult> post_details(int id)
        {
            var bp = await db.blog_posts.FindAsync(id);
            return View(new blog_post_details { 
            post_title = bp.post_Title,
            short_description = bp.short_description,
            publishedDate = bp.publish_date,
            author = bp.blog_creator.FullName,
            post_content = bp.post_content,
            featured_img = bp.Feature_Image,
            id = bp.post_id,
            no_comments = bp.number_of_comment,
            post_url = bp.post_url,
            ImageUrl = bp.Image_url,
            meta_description = bp.meta_description,
            meta_keyword = bp.meta_keyword,
            meta_title = bp.meta_title,
            comments = db.blog_post_comments.Where(c=>c.post_id == bp.post_id).ToList().OrderByDescending(d=>d.publish_date)
            });
        }
        #endregion
        #region //comment management section
        [Authorize]
        public ActionResult my_blog_post_comments(string post_by_id)
        {
            //IEnumerable<blog_post_comment> bc;
            return View();           
        }
        [AllowAnonymous][ValidateAntiForgeryToken][HttpPost]
        public async Task<ActionResult> add_comment(blog_post_details model,FormCollection f)
        {           
            if (!string.IsNullOrEmpty(f["Email_of_sender"]) && !string.IsNullOrEmpty(f["Name_of_sender"]) && !string.IsNullOrEmpty(f["contents"]))
            {
                blog_post_comment bc = new blog_post_comment();
                bc.post_id = model.id;
                bc.publish_by_email = f["Email_of_sender"];
                bc.publish_by_name = f["Name_of_sender"];
                bc.content = f["contents"];
                bc.publish_date = DateTime.UtcNow;
                db.blog_post_comments.Add(bc);
                await db.SaveChangesAsync();
               //update number of blog_post for the write            
                var bp = await db.blog_posts.FindAsync(model.id);
                bp.number_of_comment += 1;
                await db.SaveChangesAsync();
                //TODO: send email to blog_post author.
                //string message = "You have a new reply to your blog post on ihealth titled \"" + bp.post_Title + "\". <br/>";
                //em.Notification_Email_Body_Creator(bp.blog_creator.FullName, message);
                return View("post_details", new blog_post_details
                {
                    post_title = bp.post_Title,
                    short_description = bp.short_description,
                    publishedDate = bp.publish_date,
                    author = bp.blog_creator.FullName,
                    post_content = Server.HtmlDecode(bp.post_content),
                    featured_img = bp.Feature_Image,
                    id = bp.post_id,
                    no_comments = bp.number_of_comment,
                    comments = db.blog_post_comments.Where(c => c.post_id == bp.post_id).ToList().OrderByDescending(d => d.publish_date)
                });
              
            }
            else
            {
                ViewBag.error = "All fields mark with (*) must be filled.";
                var bp = await db.blog_posts.FindAsync(model.id);
                return View("post_details", new blog_post_details
                {
                    post_title = bp.post_Title,
                    short_description = bp.short_description,
                    publishedDate = bp.publish_date,
                    author = bp.blog_creator.FullName,
                    post_content = Server.HtmlDecode(bp.post_content),
                    featured_img = bp.Feature_Image,
                    id = bp.post_id,
                    no_comments = bp.number_of_comment,
                    comments = db.blog_post_comments.Where(c => c.post_id == bp.post_id).ToList().OrderByDescending(d => d.publish_date)
                });
              
            }

        }
        [Authorize][HttpPost]
        public async Task<ActionResult> Delete_Comment(IEnumerable<int> id)
        {
            try
            {
                if (id != null)
                {
                    if (id.Count() > 0)
                    {
                        foreach (int ids in id)
                        {
                            var b = await db.blog_post_comments.FindAsync( ids);
                            db.blog_post_comments.Remove(b);
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
        [HttpGet][Authorize]
        public async Task<ActionResult> reply_comment(int id)
        {
            var comment = await db.blog_post_comments.FindAsync(id);
            return View(new reply_comment_viewModel{
            id= comment.comment_id,
            comment_by_email = comment.publish_by_email,
            comment_content = comment.content,
            comment_by_name = comment.publish_by_name,
            reply_by_name = comment.blog_post.blog_creator.FullName,
            post_title = comment.blog_post.post_Title
            });
        }
        [Authorize][HttpPost][ValidateAntiForgeryToken]
        public async Task<ActionResult> reply_comment(reply_comment_viewModel model, FormCollection f)
        {
            try
            {
                Reply_Comments_tb rp = new Reply_Comments_tb();
                rp.comment_id = model.id;
                rp.content = f["reply_content"];
                rp.reply_from = model.reply_by_name;
                rp.Reply_date = DateTime.UtcNow;
                db.reply_comments_tb.Add(rp);
                await  db.SaveChangesAsync();
                await em.reply_blog_post_comment(model.comment_by_email, model.post_title, model.reply_content, model.comment_by_name);
                TempData["success"] = "Reply to comment is successfull";
                return RedirectToAction("my_blog_post_comments");
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
                TempData["error"] = "Reply to comment failed.";
                return View();
            }
        }
        #endregion
    }
}
