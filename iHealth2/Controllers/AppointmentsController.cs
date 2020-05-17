using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iHealth2.Models;
using System.Threading;
using System.Threading.Tasks;
using Ganss.XSS;
using System.Globalization;
using iHealth2.CustomClasses;
namespace iHealth2.Controllers
{
    public class AppointmentsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        HtmlSanitizer sanitizer = new HtmlSanitizer();
        Emails em = new Emails();
        // GET: Appointments
        [Authorize(Roles = "Admin")]
        public ActionResult newapp()
        {
            var app = db.AppBookingTb.Where(a => a.notifyStatus == 0).ToList().OrderByDescending(a => a.Appointment_Date);
            return View(app);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult approvedapp()//view list of Approved appointment
        {
            var app = db.AppBookingTb.Where(a => a.ApprovedStatus == "approved" && a.notifyStatus == 1).ToList().OrderByDescending(a => a.Appointment_Date);
            return View(app);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult pendingapp()//view list of pending appointments
        {
            var app = db.AppBookingTb.Where(a => a.ApprovedStatus == "pending" && a.notifyStatus == 1).ToList().OrderByDescending(a => a.Appointment_Date);
            return View(app);
        }
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> pendapp(IEnumerable<int> Id)//mark appointments has pending
        {
            string approverId = Session["Id"].ToString();
            var approverDetail = db.Users.Find(approverId);
            if (Id != null)
            {
                if (Id.Count() > 0)
                {
                    foreach (var id in Id)
                    {
                        try
                        {
                            AppBookingTb b = db.AppBookingTb.First(r => r.id == id);
                            b.ApprovedStatus = "pending";
                            b.notifyStatus = 1;
                            b.ApprovedBy = approverDetail.FullName;
                            await db.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {

                            return Json("error occured" + ex, JsonRequestBehavior.AllowGet);
                            throw ex;
                        }
                    }
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            } return Json(false, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> ApproveAppointment(int id)
        {
            var app = await db.AppBookingTb.FindAsync(id);
            return View(new ApproveApptViewModel
            {
                id = app.id,
                appid = app.app_id,
                Name = app.Name,
                email = app.Email,
                phone = app.Phone,
                appointmentDate = app.Appointment_Date,
                apptwith = app.Appointment_With,
                message = app.Message,
                city = app.city,
                state = app.state
            });

        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ApproveAppointment(int ids, FormCollection f)
        {
            string approverId = Session["Id"].ToString();
            var approverDetail = db.Users.Find(approverId);
            string body;
            AppBookingTb app = await db.AppBookingTb.FindAsync(ids);
            app.ApprovedStatus = "approved";
            app.notifyStatus = 1;
            app.ApproveDate = DateTime.UtcNow;
            app.ApprovedBy = approverDetail.FullName;
            if (!string.IsNullOrEmpty(f["appdate"]))
            {
                string appdate = f["appdate"];
                app.RescheduledDate = DateTime.ParseExact(appdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            await db.SaveChangesAsync();
            if (!string.IsNullOrEmpty(f["Message"]))
            {
                body = f["Message"];
            }
            else
            {
                body = "<p>We want to inform you that your appointment request with a one of our "
                + "health professional has been approved.<br/> Date and time will be communicated" +
                " to you shortly. <br/> Thanks for choosing iHealth Nigeria GSFM. </br></br>Best Regards</br></br>iHealth Nigeria GSFM Team.</p>";
            }
          await  em.appointmentBookingApproval(app.Name,"Appointment Approved", app.Email, body);
            TempData["success"] = "Appointment has been approved successfully";
            return RedirectToAction("newapp");
        }
        //[Authorize(Roles = "Admin")]
        //public async Task<ActionResult> approveapp(IEnumerable<int> Id)// mark appointments has approved
        //{
        //    if (Id != null)
        //    {
        //        if (Id.Count() > 0)
        //        {
        //            foreach (var id in Id)
        //            {
        //                try
        //                {
        //                    AppBookingTb b = db.AppBookingTb.First(r => r.id == id);
        //                    b.ApprovedStatus = "approved";
        //                    b.notifyStatus = 1;
        // app.ApprovedBy = approverDetail.FullName;
        //                    await db.SaveChangesAsync();
        //                    //send email to appointment requestor
        //                    //string body = "This is to notify you that your appointment request has been approved and schedule for the"
        //                }
        //                catch (Exception ex)
        //                {

        //                    return Json("error occured" + ex, JsonRequestBehavior.AllowGet);
        //                    throw ex;
        //                }
        //            }
        //            return Json(true, JsonRequestBehavior.AllowGet);
        //        }
        //    } return Json(false, JsonRequestBehavior.AllowGet);      
        //}
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> delapp(IEnumerable<int> Id)//delete appointments
        {
            if (Id != null)
            {
                if (Id.Count() > 0)
                {
                    foreach (var id in Id)
                    {
                        try
                        {
                            AppBookingTb b = db.AppBookingTb.First(r => r.id == id);
                            db.AppBookingTb.Remove(b);
                            await db.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            return Json("error occured" + ex, JsonRequestBehavior.AllowGet);
                            throw ex;
                        }
                    }
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            } return Json(false, JsonRequestBehavior.AllowGet);
        }
    }
}