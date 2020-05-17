using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using iHealth2.Models;
using System.Web.Mvc;
using System.Net;
using Microsoft.AspNet.Identity;
using Elmah;
using System.IO;
using System.Web.Hosting;
namespace iHealth2.CustomClasses
{
    //TODO: Don't forget to change this MAILTO to final Admin recipient mail.
  public class Emails:Controller
    {
        ihealth_constants hc = new ihealth_constants();
      ApplicationDbContext db = new ApplicationDbContext();    
     // string hc.MailFrom = ; //Domain Email for sending mail to fresh subscribers, newly register business or product
      EmailSender em = new EmailSender();
      /// <summary>
      ///  This method notify admin upon about certain activities on the site
      /// </summary>
      /// <param name="Subjectofmail">Title of the Email to be recieve by admin</param>
      /// <returns></returns>
      public async Task notifyAdmin(string Subjectofmail)
      {
         
          try
          {
              #region notify admin about new business registration
              if (Subjectofmail == "NewBusinessRegistration")
              {
                  MailAddress
  maFrom = new MailAddress(hc.MailFrom, hc.displayName, Encoding.UTF8),

  maTo = new MailAddress(hc.Mail_support);
                  MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);
                  mail.Subject = "New Business Registration";
                  mail.Body = Notification_Email_Body_Creator("Admin","<div style=\"text-align:justify\">Dear <b>Admin</b>,<br/><p> This is to inform you that someone just registered a business or company on the iHealth portal" +
                              "<br/> Please login to the <a href =\"" + hc.ihealthUrl + "\"> portal</a> to review this activity </p></div>");
                  mail.BodyEncoding = Encoding.UTF8;
                  mail.SubjectEncoding = Encoding.UTF8;
                  mail.IsBodyHtml = true;
                  mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    //smtp.Port = 465;
                    hc.smtp.EnableSsl = true;
                  NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                    hc.smtp.Credentials = Credentials;
                    await hc.smtp.SendMailAsync(mail);
              }
              #endregion
              #region notify admin about new user registration
              else if (Subjectofmail == "NewUserRegistration")
              {
                  MailAddress
 maFrom = new MailAddress(hc.MailFrom, hc.displayName, Encoding.UTF8),

 maTo = new MailAddress(hc.Mail_support);
                  MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);
                  mail.Subject = "New User Registration";
                  mail.Body = Notification_Email_Body_Creator("Admin","<div style=\"text-align:justify\"><p>Good day, please be inform that a new user just sign up with ihealthgsfm." +
                              "<br/> Please login to the <a href =\"" + hc.ihealthUrl + "\"> portal</a> to review this activity </p></div>");
                  mail.BodyEncoding = Encoding.UTF8;
                  mail.SubjectEncoding = Encoding.UTF8;
                  mail.IsBodyHtml = true;
                  mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    //smtp.Port = 465;
                    hc.smtp.EnableSsl = true;
                  NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                    hc.smtp.Credentials = Credentials;
                    await hc.smtp.SendMailAsync(mail);
              }
              #endregion
              #region notify admin about new user registration
              else if (Subjectofmail == "AppointmentBooking")
              {
                  MailAddress
 maFrom = new MailAddress(hc.MailFrom, hc.displayName, Encoding.UTF8),

 maTo = new MailAddress(hc.Mail_support);
                  MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);
                  mail.Subject = "New Appointment Booking";
                  mail.Body = Notification_Email_Body_Creator("Admin", "<div style=\"text-align:justify\"><p>Good day, please be inform that an appointment has been booked on our site." +
                              "<br/> Please login to the <a href =\"" + hc.ihealthUrl + "\"> portal</a> to review this activity </p></div>");
                  mail.BodyEncoding = Encoding.UTF8;
                  mail.SubjectEncoding = Encoding.UTF8;
                  mail.IsBodyHtml = true;
                  mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                  NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                    hc.smtp.Credentials = Credentials;
                    hc.smtp.EnableSsl = true;
                    await hc.smtp.SendMailAsync(mail);
              }
              #endregion

          }
          catch (Exception ex)
          {
              ErrorSignal.FromCurrentContext().Raise(ex);
          }
      }
      // This Method notify New Registered user to confirm their emails
      public async Task UserConfirmMail(IdentityMessage message)
      {
          try
          {
              MailAddress
     maFrom = new MailAddress(hc.MailFrom, hc.displayName, Encoding.UTF8),
                  // maFrom = new MailAddress("info@ihealthng.somee.com", "iHealth"),
     maTo = new MailAddress(message.Destination, message.Destination, Encoding.UTF8);

              MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);
              mail.Subject = message.Subject;
              mail.Body = message.Body;
              mail.BodyEncoding = Encoding.UTF8;
              mail.SubjectEncoding = Encoding.UTF8;
              mail.IsBodyHtml = true;
              mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
              //smtp.Port = 465; smtp.EnableSsl = true;
              NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                hc.smtp.Credentials = Credentials;
                hc.smtp.EnableSsl = true;
                //smtp.UseDefaultCredentials = false;
                await hc.smtp.SendMailAsync(mail);
              await notifyAdmin("NewUserRegistration");
          }
          catch (Exception ex)
          {
              ErrorSignal.FromCurrentContext().Raise(ex);
          }
      }
      /// <summary>
      /// Send mail to the user that register a business notfity him / her of successfull business registration
      /// </summary>
      /// <param name="mailto">Business Owner Email</param>
      /// <param name="companyMail">Business Email</param>
      /// <param name="phone">Business or Owner's Phone</param>
      /// <param name="bizname">Business Name</param>
      /// <param name="addr">Address of Business</param>
      /// <param name="bizType">Business Category</param>
      /// <returns></returns>
      public async Task bizRegistrationMail(string mailto, string companyMail, string phone, string bizname, string addr, string bizType, string ownersName)
      {
          try
          {
              MailAddress
     maFrom = new MailAddress(hc.MailFrom, hc.displayName, Encoding.UTF8),
                  // maFrom = new MailAddress("info@ihealthng.somee.com", "iHealth"),
     maTo = new MailAddress(mailto);

              MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);

              mail.Subject = "Business Registration on IHEALTH";
              mail.Body = Notification_Email_Body_Creator(ownersName,"<div style=\"text-align:justify\"><p>Your business registration on <strong>iHealth</strong> was recieved and awaits approval. <br/>" +
                  "<span style=\" color: Red\"><strong> Note :</strong></span><b>YOU WILL BE NOTIFIED UPON APPROVAL.</b></p>"
                  + "<p> The following details was registered by you: <br/><br/>"
                  + "Company Name: <strong>" + bizname + "</strong> <br/><br/> Business Type: <strong>" + bizType + "</strong> <br/><br/> Address: <strong>" + addr + "</strong><br/><br/> Company Email: <strong>" + companyMail + "</strong><br/> <br/> Company Phone Number: <strong>" + phone + "</strong>  </p>" +
                  "<p> As soon as your Business is approved, it will be available for display on our site.<br/> Thanks for registering with us. <br/><br/>Best Regards,<br/><br/>iHealth Nigeria GSFM Team.</p></div>");
              mail.BodyEncoding = Encoding.UTF8;
              mail.SubjectEncoding = Encoding.UTF8;
              mail.IsBodyHtml = true;
              mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
              NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                hc.smtp.Credentials = Credentials;
                hc.smtp.EnableSsl = true;
                await hc.smtp.SendMailAsync(mail);
              await notifyAdmin("NewBusinessRegistration");

          }
          catch (Exception ex)
          {
              ErrorSignal.FromCurrentContext().Raise(ex);
          }
      }

      //Notify Business owner on businee approval
      public async Task SendBizApprovedMail(string mailto, string BName, string ownersName)
      {
          try
          {
              MailAddress
     maFrom = new MailAddress(hc.MailFrom, hc.displayName, Encoding.UTF8),
                  // maFrom = new MailAddress("info@ihealthng.somee.com", "iHealth"),
     maTo = new MailAddress(mailto);

              MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);

              mail.Subject = "iHealth Nigeria Gsfm";
              mail.Body = Notification_Email_Body_Creator(ownersName,"<div style=\"text-align:justify;\"><h2 style=\"text-align: center; font-weight: 700\">Business Approval</h2>" +
                  "<p>Dear <b>"+ownersName+"</b>, <br/> The business name \"<b>'" + BName + "'</b>\", you registered on <a href=\"" + hc.ihealthUrl + "\">ihealthgsfm.com</a> has been <b> Approved </b>. <br/>" +
                  "Now <b>'" + BName + "'</b> will be available to our site users during search that relate to your business category." +
                  "<br/><br/> Thank you for choosing <a href=\"" + hc.ihealthUrl + "\">iHealth Nigeria Gsfm</a> <br/><br/>Best Regards,<br/><br/> iHealth Nigeria Gsfm Team.</p></div>");

              mail.BodyEncoding = Encoding.UTF8;
              mail.SubjectEncoding = Encoding.UTF8;
              mail.IsBodyHtml = true;
              mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
              NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                hc.smtp.Credentials = Credentials; hc.smtp.EnableSsl = true;
                await hc.smtp.SendMailAsync(mail);
          }
          catch (Exception ex)
          {
              ErrorSignal.FromCurrentContext().Raise(ex);
          }
      }

      //Notify Business owner on busines verification
      public async Task SendBizVerifyMail(string mailto, string BName, string ownersname)
      {
          try
          {
              MailAddress
     maFrom = new MailAddress(hc.MailFrom, hc.displayName, Encoding.UTF8),
                  // maFrom = new MailAddress("info@ihealthng.somee.com", "iHealth"),
     maTo = new MailAddress(mailto);

              MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);

              mail.Subject = "iHealth Nigeria Gsfm";
              mail.Body = Notification_Email_Body_Creator(ownersname,"<div style=\"text-align:justify\"><h2 style=\"text-align: center; font-weight: 700\">Business Verified</h2>" +
                  "<p> This is to inform you that <b> '" + BName.ToUpper() + "' </b> just got Verified on <a href=\"" + hc.ihealthUrl + "\">iHealthgsfm.com</a>. <br/><br/>" +
                  "Now <b> '" + BName.ToUpper() + "' </b> will be among the first set of businesses or services to appear on our site, whenever clients made a search relating to your business category." +
                  "<br/><br/> Thank you for choosing <a href=\"" + hc.ihealthUrl + "\">iHealth Nigeria Gsfm </a>.<br/><br/>Best Regards,<br/><br/> iHealth Nigeria Gsfm Team.</p></div>");

              mail.BodyEncoding = Encoding.UTF8;
              mail.SubjectEncoding = Encoding.UTF8;
              mail.IsBodyHtml = true;
              mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
              NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                hc.smtp.Credentials = Credentials; hc.smtp.EnableSsl = true;
                await hc.smtp.SendMailAsync(mail);
          }
          catch (Exception ex)
          {
              TempData["error"] = "Verification email was not sent to business owner due to: ";
              ErrorSignal.FromCurrentContext().Raise(ex);
          }
      }
      //Notify Business owner once business is marked as Recommended
      public async Task SendBizRecommendedMail(string mailto, string BName, string ownersname)
      {
          try
          {
              MailAddress
     maFrom = new MailAddress(hc.MailFrom, hc.displayName, Encoding.UTF8),
                  // maFrom = new MailAddress("info@ihealthng.somee.com", "iHealth"),
     maTo = new MailAddress(mailto);

              MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);

              mail.Subject = "iHealth Nigeria Gsfm";
              mail.Body = Notification_Email_Body_Creator(ownersname, "<div style=\"text-align:justify\"><h2 style=\"text-align: center; font-weight: 700\">Business Verified</h2>" +
                  "<p> This is to inform you that <b> '" + BName.ToUpper() + "' </b> has been placed on recommended list of businesses or services for our site visitors. <br/><br/>" +
                  "Now <b> '" + BName.ToUpper() + "' </b> will be among the first set of businesses or services to appear on our site, whenever clients made a search relating to your business or service category." +
                  "<br/><br/> Thank you for choosing <a href=\"" + hc.ihealthUrl + "\">iHealth Nigeria Gsfm </a>.<br/><br/>Best Regards,<br/><br/> iHealth Nigeria Gsfm Team.</p></div>");

              mail.BodyEncoding = Encoding.UTF8;
              mail.SubjectEncoding = Encoding.UTF8;
              mail.IsBodyHtml = true;
              mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
              NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                hc.smtp.Credentials = Credentials; hc.smtp.EnableSsl = true;
                await hc.smtp.SendMailAsync(mail);
          }
          catch (Exception ex)
          {
              TempData["error"] = "Verification email was not sent to business owner due to: ";
              ErrorSignal.FromCurrentContext().Raise(ex);
          }
      }

      //Notify Product owner upon product's approval
      public async Task SendProductApprovedMail(string mailto, string PName, string ownersname)
      {
          try
          {
              MailAddress
     maFrom = new MailAddress(hc.MailFrom, hc.displayName, Encoding.UTF8),
                  // maFrom = new MailAddress("info@ihealthng.somee.com", "iHealth"),
     maTo = new MailAddress(mailto);

              MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);

              mail.Subject = "iHealth Nigeria GSFM";
              mail.Body = Notification_Email_Body_Creator(ownersname,"<div style=\"text-align:justify\"><h2 style=\"text-align: center; font-weight: 700\">Product Approval </h2>" +
                  "<p>Dear <b>"+ownersname+"</b><br/>The product name \"<b>'" + PName + "'</b>\", you registered on <a href=\"" + hc.ihealthUrl + "\">iHealthgsfm.com</a> has been <b> Approved </b>. <br/><br/>" +
                  "Now <b>'" + PName + "'</b> will be available to our site users during search that relate to your product categories. <br/><br/> Thank you for choosing  <a href=\"" + hc.ihealthUrl + "\">iHealth Nigeria Gsfm.</a>" +
                  " <br/><br/>Best Regards,<br/><br/> iHealth Nigeria Gsfm Team.</p></div>");

              mail.BodyEncoding = Encoding.UTF8;
              mail.SubjectEncoding = Encoding.UTF8;
              mail.IsBodyHtml = true;
              mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
              NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                hc.smtp.Credentials = Credentials; hc.smtp.EnableSsl = true;
                await hc.smtp.SendMailAsync(mail);
          }
          catch (Exception ex)
          {
              TempData["error"] = "An approval email was not sent to Product owner due to: " + ex;
              ErrorSignal.FromCurrentContext().Raise(ex);
          }
      }

      //Notify Product owner upon product's Verification
      public async Task SendProductVerifyMail(string mailto, string PName, string ownersname)
      {
          try
          {
              MailAddress
     maFrom = new MailAddress(hc.MailFrom, hc.displayName, Encoding.UTF8),
                  // maFrom = new MailAddress("info@ihealthng.somee.com", "iHealth"),
     maTo = new MailAddress(mailto);

              MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);

              mail.Subject = "iHealth Nigeria Gsfm";
              mail.Body = Notification_Email_Body_Creator(ownersname,"<div style=\"text-align:justify\"><h2 style=\"text-align: center; font-weight: 700\">Product Verification</h2>" +
                  "<p> This is to inform you that<b> '" + PName.ToUpper() + "' </b> just got Verified on <a href=\"" + hc.ihealthUrl + "\">iHealthgsfm.com</a>. <br/><br/>" +
                  "Now <b> '" + PName.ToUpper() + "' </b> will be among the first set of products to appear on our site, whenever clients made a search relating to your Productss categories." +
                  "<br/><br/> Thank you for choosing <a href=\"" + hc.ihealthUrl + "\"> iHealth Nigeria Gsfm </a> <br/><br/>Best Regards,<br/><br/> iHealth Nigeria Gsfm Team.</p></div>");

              mail.BodyEncoding = Encoding.UTF8;
              mail.SubjectEncoding = Encoding.UTF8;
              mail.IsBodyHtml = true;
              mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
              NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                hc.smtp.Credentials = Credentials; hc.smtp.EnableSsl = true;
                await hc.smtp.SendMailAsync(mail);
              return;
          }
          catch (Exception ex)
          {
              TempData["error"] = "Verification email was not sent to product's owner due to: ";
              ErrorSignal.FromCurrentContext().Raise(ex);
          }
      }

      //TODO: Don't forget to change this MAILTO to final Admin recipient mail.
      /// <summary>
      /// This mail notify admin about a feedbacks from site users
      /// </summary>
      /// <param name="SenderName"></param>
      /// <param name="SenderEmail"></param>
      /// <param name="Message"></param>
      /// <param name="FeedBackType"></param>
      /// <returns></returns>
      public async Task NotifyAdminAbtFB(string SenderName, string SenderEmail, String Message, string subject,string FeedBackType)
      {
          try
          {
              MailAddress
     maFrom = new MailAddress(hc.MailFrom, hc.displayName, Encoding.UTF8),
                  // maFrom = new MailAddress("info@ihealthng.somee.com", "iHealth"),
     maTo = new MailAddress(hc.Mail_support);

              MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);

              mail.Subject = subject;
              mail.Body = Notification_Email_Body_Creator("Admin","Sender Name: <b style=\"text-transform:uppercase\">" + SenderName + "</b><br/><br/>Sender Email: <span style=\"color:green\"> "
                  + SenderEmail + "</span><br/><br/> Feedback Type: <span style=\"color:red\">" + FeedBackType + "</span><br/><br/>Message Content:<br/> "
                  + Message);
              mail.BodyEncoding = Encoding.UTF8;
              mail.SubjectEncoding = Encoding.UTF8;
              mail.IsBodyHtml = true;
              mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
              NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                hc.smtp.Credentials = Credentials; hc.smtp.EnableSsl = true;
                await hc.smtp.SendMailAsync(mail);
              await em.notifysender(SenderEmail, FeedBackType, SenderName);
          }
          catch (Exception ex)
          {
              TempData["error"] = "Email could not be sent due to: ";
              ErrorSignal.FromCurrentContext().Raise(ex);
          }
      }
      /// <summary>
      /// This mail reply user about feedbacks they submitted earlier
      /// </summary>
      /// <param name="mailto">User Email</param>
      /// <param name="subject">Subject of Mail</param>
      /// <param name="Message">Mail Body</param>
      /// <returns></returns>
      public async Task ReplyFeedBack(string mailto, string subject, string message)
      {
          try
          {
              MailAddress
     maFrom = new MailAddress(hc.MailFrom, hc.displayName, Encoding.UTF8),
                
     maTo = new MailAddress(mailto);
              MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);

              mail.Subject = subject;
              mail.Body = message;
              mail.BodyEncoding = Encoding.UTF8;
              mail.SubjectEncoding = Encoding.UTF8;
              mail.IsBodyHtml = true;
              mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
              NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                hc.smtp.Credentials = Credentials; hc.smtp.EnableSsl = true;
                await hc.smtp.SendMailAsync(mail);
          }
          catch (Exception ex)
          {
              TempData["error"] = "Email could not be sent due to: " + ex;
              ErrorSignal.FromCurrentContext().Raise(ex);
          }
      }

      internal async Task CareerMail(string mailFrom, string subject, string Body, string Name)
      {
          try
          {
              MailAddress
              maFrom = new MailAddress(hc.MailFrom), // support@ihealthgsfm.com email address
              maTo = new MailAddress(hc.Mail_support); // ihealth Admin email
              MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);
              mail.Subject = subject;
              mail.Body = Body;
              mail.BodyEncoding = Encoding.UTF8;
              mail.SubjectEncoding = Encoding.UTF8;
              mail.IsBodyHtml = true;
              mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
              NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                hc.smtp.Credentials = Credentials; hc.smtp.EnableSsl = true;
                await hc.smtp.SendMailAsync(mail);
              await notifyCareerApplicant(mailFrom, subject, Name);
              //result = "Success";
              return;
          }
          catch (Exception ex)
          {
              ErrorSignal.FromCurrentContext().Raise(ex);
          }
      }

      private async Task notifyCareerApplicant(string mailto, string subject, string name)
      {
          try
          {
              MailAddress
              maFrom = new MailAddress(hc.MailFrom), // subscriber email address
              maTo = new MailAddress(mailto); // subscriber email
              MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);
              mail.Subject = "RE: " + subject;
              mail.Body = Notification_Email_Body_Creator(name.ToUpper(), "<p>Thank you for your interest in employment with <a href=\"" + hc.ihealthUrl + "\"> iHealth Nigeria GSFM</a>." +
                  "We successfully received your application. <br/> Should a position become available that matches your qualifications, we will contact you." +
                  "<br/><br/> Best Regards, <br/><br/> iHealth Nigeria GSFM Team.</p> ");
              mail.BodyEncoding = Encoding.UTF8;
              mail.SubjectEncoding = Encoding.UTF8;
              mail.IsBodyHtml = true;
              mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
              NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                hc.smtp.Credentials = Credentials; hc.smtp.EnableSsl = true;
                await hc.smtp.SendMailAsync(mail);
              //result = "Success";
              return;
          }
          catch (Exception ex)
          {
              ErrorSignal.FromCurrentContext().Raise(ex);
          }
      }
      /// <summary>
      /// Notify ihealth Admin about fresh benefit claim request 
      /// </summary>
      /// <param name="beneficiarymail">benefit claimer's email address</param>
      /// <param name="subject">subject of email</param>
      /// <param name="body1">body of message that will be sent to benefit claimer</param>
      /// <param name="body2">body of message that will be sent to Admin notifying him or her of a fresh benefit request</param>
      /// <returns></returns>
      internal async Task FreshBenefitClaimNotificationRequest(string beneficiarymail, string subject, string body1, string body2)
      {
          try
          {
              MailAddress
              maFrom = new MailAddress(hc.MailFrom), // domain email
              maTo = new MailAddress(hc.Mail_support); // admin email
              MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);
              mail.Subject = subject;
              mail.Body = body2;
              mail.BodyEncoding = Encoding.UTF8;
              mail.SubjectEncoding = Encoding.UTF8;
              mail.IsBodyHtml = true;
              mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
              NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                hc.smtp.Credentials = Credentials; hc.smtp.EnableSsl = true;
                await hc.smtp.SendMailAsync(mail);
              //result = "Success";
              await notifybenefitClaimer(beneficiarymail, subject, body1);
              return;
          }
          catch (Exception ex)
          {
              ErrorSignal.FromCurrentContext().Raise(ex);
          }
      }
      /// <summary>
      /// This will send an automatic email to benefit claimer, notifyiing 
      /// him/her that the benefit request made has been recieved by ihealth team
      /// </summary>
      /// <param name="beneficiarymail">benefit claimer's email address</param>
      /// <param name="subject">subject of email</param>
      /// <param name="body1">body of message that will be sent to benefit claimer</param>
      /// <returns></returns>
      internal async Task notifybenefitClaimer(string beneficiarymail, string subject, string body1)
      {
          try
          {
              MailAddress
              maFrom = new MailAddress(hc.MailFrom), // subscriber email address
              maTo = new MailAddress(beneficiarymail); // subscriber email
              MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);
              mail.Subject = "RE: " + subject;
              mail.Body = body1;
              mail.BodyEncoding = Encoding.UTF8;
              mail.SubjectEncoding = Encoding.UTF8;
              mail.IsBodyHtml = true;
              mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
              NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                hc.smtp.Credentials = Credentials; hc.smtp.EnableSsl = true;
                await hc.smtp.SendMailAsync(mail);
              return;

          }
          catch (Exception ex)
          {
              ErrorSignal.FromCurrentContext().Raise(ex);
          }
      }
      /// <summary>
      /// this method send a reply email to blog_post audience who ask questions on any post on the blog
      /// </summary>
      /// <param name="mailto">Email of recipient</param>
      /// <param name="subject">subject of mail</param>
      /// <param name="body">body</param>
      /// <param name="name_of_recipient">name of recipient</param>
      /// <returns></returns>
      internal async Task reply_blog_post_comment(string mailto, string subject, string body, string name_of_recipient)
      {
          try
          {
              MailAddress
              maFrom = new MailAddress(hc.MailFrom), // noreply email address
              maTo = new MailAddress(mailto); // subscriber email
              MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);
              mail.Subject = "RE: " + subject;
              mail.Body = Notification_Email_Body_Creator(name_of_recipient,body);
              mail.BodyEncoding = Encoding.UTF8;
              mail.SubjectEncoding = Encoding.UTF8;
              mail.IsBodyHtml = true;
              mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
              NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                hc.smtp.Credentials = Credentials; hc.smtp.EnableSsl = true;
                await hc.smtp.SendMailAsync(mail);
              return;

          }
          catch (Exception ex)
          {
              ErrorSignal.FromCurrentContext().Raise(ex);
          }
      }
      internal async Task appointmentBookingNotification(string name, string body, string subject, string mailto)
      {
          try
          {
              MailAddress
              maFrom = new MailAddress(hc.MailFrom), // noreply email address
              maTo = new MailAddress(mailto); // subscriber email
              MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);
              mail.Subject = "RE: " + subject;
              mail.Body = Notification_Email_Body_Creator(name, body);
              mail.BodyEncoding = Encoding.UTF8;
              mail.SubjectEncoding = Encoding.UTF8;
              mail.IsBodyHtml = true;
              mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
              NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                hc.smtp.Credentials = Credentials; hc.smtp.EnableSsl = true;
                await hc.smtp.SendMailAsync(mail);
              await notifyAdmin("AppointmentBooking");
              return;

          }
          catch (Exception ex)
          {
              ErrorSignal.FromCurrentContext().Raise(ex);
          }
      }
      internal async Task appointmentBookingApproval(string name, string subject, string mailto, string body)
      {
          try
          {
              MailAddress
              maFrom = new MailAddress(hc.MailFrom), // noreply email address
              maTo = new MailAddress(mailto); // subscriber email
              MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);
              mail.Subject = "RE: " + subject;
              mail.Body = Notification_Email_Body_Creator(name, body);
              mail.BodyEncoding = Encoding.UTF8;
              mail.SubjectEncoding = Encoding.UTF8;
              mail.IsBodyHtml = true;
              mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
              NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                hc.smtp.Credentials = Credentials; hc.smtp.EnableSsl = true;
                await hc.smtp.SendMailAsync(mail);
           //   await notifyAdmin("AppointmentBooking");
              return;

          }
          catch (Exception ex)
          {
              ErrorSignal.FromCurrentContext().Raise(ex);
          }
      }

      public string Notification_Email_Body_Creator(string Name, string Message)
      {
          string body = string.Empty;
          string path = HostingEnvironment.MapPath("~/EmailTemplates/NotificationTemp.cshtml");
          using (StreamReader reader = new StreamReader(path))
          {
              body = reader.ReadToEnd();
          }
          body = body.Replace("Name", Name);
          body = body.Replace("Message", Message);
          return body;
      }



      internal async Task serviceBoosterSignupEmail(string name,string mailto, string subject,string body)
      {
          try
          {
              MailAddress
              maFrom = new MailAddress(hc.MailFrom), // noreply email address
              maTo = new MailAddress(mailto); // subscriber email
              MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);
              mail.Subject = "RE: " + subject;
              mail.Body = Notification_Email_Body_Creator(name,body );
              mail.BodyEncoding = Encoding.UTF8;
              mail.SubjectEncoding = Encoding.UTF8;
              mail.IsBodyHtml = true;
              mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
              NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                hc.smtp.Credentials = Credentials; hc.smtp.EnableSsl = true;
                await hc.smtp.SendMailAsync(mail);
              //   await notifyAdmin("AppointmentBooking");
              return;

          }
          catch (Exception ex)
          {
              ErrorSignal.FromCurrentContext().Raise(ex);
          }
      }
      //this send registration notification to premium user
      internal async Task premiumuserRegEmail(string mailto, string subject, string body, string name)
      {
          try
          {
              MailAddress
              maFrom = new MailAddress(hc.MailFrom), // noreply email address
              maTo = new MailAddress(mailto); // subscriber email
              MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);
              mail.Subject = "RE: " + subject;
              mail.Body = Notification_Email_Body_Creator(name, body);
              mail.BodyEncoding = Encoding.UTF8;
              mail.SubjectEncoding = Encoding.UTF8;
              mail.IsBodyHtml = true;
              mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
              NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                hc.smtp.Credentials = Credentials; hc.smtp.EnableSsl = true;
                await hc.smtp.SendMailAsync(mail);
              //   await notifyAdmin("AppointmentBooking");
              return;

          }
          catch (Exception ex)
          {
              ErrorSignal.FromCurrentContext().Raise(ex);
          }
      }
      //this send payment approval notification to premium user
      internal async Task premiumUserPaymentApprovalMail(string mailto, string name, string subject, string body)
      {
          try
          {
              MailAddress
              maFrom = new MailAddress(hc.MailFrom), // noreply email address
              maTo = new MailAddress(mailto); // subscriber email
              MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);
              mail.Subject = "RE: " + subject;
              mail.Body = Notification_Email_Body_Creator(name, body);
              mail.BodyEncoding = Encoding.UTF8;
              mail.SubjectEncoding = Encoding.UTF8;
              mail.IsBodyHtml = true;
              mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
              NetworkCredential Credentials = new NetworkCredential(hc.MailFrom, hc.pswd);
                hc.smtp.Credentials = Credentials; hc.smtp.EnableSsl = true;
                await hc.smtp.SendMailAsync(mail);
              //   await notifyAdmin("AppointmentBooking");
              return;

          }
          catch (Exception ex)
          {
              ErrorSignal.FromCurrentContext().Raise(ex);
          }
      }
    }
}
