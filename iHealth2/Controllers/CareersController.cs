using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iHealth2.Models;
using System.IO;
using System.Threading.Tasks;
using iHealth2.CustomClasses;
namespace iHealth2.Controllers
{
    public class CareersController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        EmailSender ems = new EmailSender();
        Emails em = new Emails();
        public ActionResult careers()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> careers(RegisterCareerViewModel model, FormCollection f, HttpPostedFileBase cv)
        {
            try
            {
                int conid = Convert.ToInt32(f["Country"]);
                int stid = Convert.ToInt32(f["state"]);
                var con3 = await db.country.FindAsync(conid);
                var st = await db.State.FindAsync(stid);
                if (ModelState.IsValid)
                {
                    Career cr = new Career();
                    cr.FName = model.FName;
                    cr.LName = model.LName;
                    cr.JobTitle = model.JobTitle;
                    cr.Specialization = model.Specialization;
                    cr.Phone = model.Phone;
                    cr.Email = model.Email;
                    cr.Address = f["Address"];
                    cr.Gender = model.Gender;
                    cr.City = model.City;
                    cr.Nationality = con3.CountryName;
                    cr.State = st.StateName;
                    if (cv != null)
                    {
                        string extension = Path.GetExtension(cv.FileName);
                        if ((extension == ".pdf") || (extension == ".doc") || (extension == ".docx"))//extension  
                        {
                            if (cv.ContentLength <= 5242880)
                            {
                                byte[] cvFile = new byte[cv.ContentLength];
                                cv.InputStream.Read(cvFile, 0, (int)cv.ContentLength);
                                cr.CV = cvFile;
                                cr.CV_name = cv.FileName;
                                cr.CV_size = cvFile.Length;
                                cr.CV_contentType = cv.ContentType;
                            }
                            else
                            {
                                TempData["error"] = "CV size exceed 5mb as required, please rezise the document.";
                                return View();
                            }
                        }
                        else
                        {
                            TempData["error"] = "Invalide file extension. only .pdf, .doc, .docx are supported";
                            return View();
                        }
                    }
                    else
                    {
                        TempData["error"] = "Please, upload a cv!!!";
                        return View();
                    }
                    cr.notificationStatus = 0;
                    cr.SubmittedDate = DateTime.Now;
                    db.career.Add(cr);
                    await db.SaveChangesAsync();
                    string name = model.FName + " " + model.LName;
                    await em.CareerMail(mailFrom: model.Email, subject:
                    "Job Application", Body: em.Notification_Email_Body_Creator("Admin" ,"" + name + " with the following email address \"" + model.Email + " \" just fill our career form, login to your portal for more details. <br/> Thanks."), Name: name);
                    ModelState.Clear();
                    TempData["error"] = "Your Application has been submitted succesfully. kindly check your email for further instructions, Thanks you.";
                    return RedirectToAction("index", "home");
                }
                else
                {
                    ModelState.AddModelError("", "Fill all required field");
                    return View();
                }
            }
            catch (Exception ex)
            {
                return View();
                throw ex;

            }

        }
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult create_Job_Ads()
        {
            return View();
        }
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> create_Job_Ads(FormCollection f, createJobViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Job jb = new Job();
                    jb.JobTitle = model.JobTitle;
                    jb.Specialization = model.specialization.Specialization;
                    jb.Jobdescription = model.Descriptions;
                    jb.Min_Experience = Convert.ToInt32(f["Experience"]);
                    jb.CreatedDate = DateTime.Now;
                    db.jobs.Add(jb);
                    await db.SaveChangesAsync();
                    TempData["success"] = "Job successfully created";
                    ModelState.Clear();
                    return View();
                }
                else
                {
                    ModelState.AddModelError("", "Validate required field");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
                return View();
            }

        }
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult viewJobList()
        {
            return View(db.jobs.ToList().OrderByDescending(j => j.CreatedDate));
        }
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> deletejob(IEnumerable<int> deleteid)
        {
            if (deleteid != null)
            {
                foreach (var id in deleteid)
                {
                    var jb = await db.jobs.FindAsync(id);
                    db.jobs.Remove(jb);
                    await db.SaveChangesAsync();
                }
                TempData["success"] = "Job(s) has been successfully deleted.";
                return RedirectToAction("viewJobList");
            }
            return RedirectToAction("viewJobList");
        }
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult new_applicant()
        {
            var app = db.career.Where(m => m.notificationStatus == 0);          
            return View(app.OrderByDescending(m => m.SubmittedDate).ToList());
        }
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult> MarkApplicantAsSeen(IEnumerable<int> markid)
        {
            if (markid != null)
            
            {
                if (markid.Count() > 0)
                {
                    foreach (var id in markid)
                    {
                        var c = await db.career.FindAsync(id);
                        c.notificationStatus = 1;
                        await db.SaveChangesAsync();
                    }
                }
            }
            TempData["info"] = "operation is successfull!!!";
            return RedirectToAction("new_applicant");
        }
        [Authorize(Roles = "Admin,SuperAdmin")]
        public ActionResult viewApplicant()
        {
            var applicant = db.career.Where(m => m.notificationStatus == 1).OrderByDescending(m => m.SubmittedDate).ToList();
            return View(applicant);
        }
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> del_applicant(IEnumerable<int> deleteid)
        {
            if (deleteid != null)
            {
                if (deleteid.Count() > 0)
                {
                    foreach (var id in deleteid)
                    {
                        var c = await db.career.FindAsync(id);
                        db.career.Remove(c);
                        await db.SaveChangesAsync();
                    }
                }
            }
            TempData["success"] = "Applicant(s) has been successfully deleted.";
            return RedirectToAction("viewApplicant");
        }
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult> download_cv(int id)
        {
            if (id != 0)
            {
                var c = await db.career.FindAsync(id);
                string cvname = c.CV_name;
                string cv_CType = c.CV_contentType;
                int cvSize = c.CV_size;
                byte[] cvData = (byte[])c.CV;
                Response.AddHeader("content type", cv_CType);
                Response.AddHeader("Content-Disposition", "Attachment; Filename=" + cvname);
                Response.OutputStream.Write(cvData, 0, cvSize);
                Response.Flush();
                Response.End();
            }

            return RedirectToAction("viewApplicant");
        }
    }
}