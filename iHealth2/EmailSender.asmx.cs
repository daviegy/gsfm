using System;
using iHealth2.CustomClasses;
using System.Web.Services;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Text;
using Elmah;
using System.Net;
using System.Web.Hosting;
using System.IO;
namespace iHealth2
{
    /// <summary>
    /// Summary description for EmailSender
    /// </summary>
    [WebService(Namespace = "http://ihealthgsfm.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class EmailSender : System.Web.Services.WebService
    {
        ihealth_constants hc = new ihealth_constants();       
        /// <summary>
        /// this Webservice will allow Admin to send email to subscibers @ anytime 
        /// </summary>
        /// <param name="mailto"> the Recipient email address</param>
        /// <param name="Subject"> the subject of mail</param>
        /// <param name="Body"> the body/content of mail</param>
        /// <returns>return success if mail is sent</returns>
        [WebMethod]
        public async Task AdminMailSender(string mailto,String Subject,string Body)
        {
            try
            {
                MailAddress
                maFrom = new MailAddress(hc.Mail_support, hc.displayName, Encoding.UTF8),
                maTo = new MailAddress(mailto);
                MailMessage mail = new MailMessage(maFrom.Address, maTo.Address);
                mail.Subject = Subject;
                mail.Body = Body;
                mail.BodyEncoding = Encoding.UTF8;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.IsBodyHtml = true;
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                NetworkCredential Credentials = new NetworkCredential(hc.Mail_support, hc.pswd);
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
        /// this Webservice will allow Subscribers to send email to Admin @ anytime 
        /// </summary>
        /// <param name="name">the sender name</param>
        /// <param name="mailFrom"> the sender email address</param>
        /// <param name="Subject"> the subject of mail</param>
        /// <param name="Body"> the body/content of mail</param>
        /// <returns>return success if mail is sent</returns>
        [WebMethod]
        public async Task SubcribersMailSender (string mailFrom, string subject, string Body, string senderName)
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
                await notifysender(mailFrom,subject,senderName);
                //result = "Success";
                return;
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        public async Task notifysender(string mailto,string subject,string senderName)
        {
            try
            {
                MailAddress
                mailFrom = new MailAddress(hc.MailFrom), // ihealth email address
                maTo = new MailAddress(mailto); // subscriber email
                MailMessage mail = new MailMessage(mailFrom.Address, maTo.Address);
                mail.Subject = "RE: " + subject;
                mail.Body =Notification_Email_Body_Creator(senderName,"<p>Good day, we have received your request and a member of our team will contact you shortly.<br/> <br/>"+
                    "Thanks for choosing iHealth Nigeria GSFM. <br/><br/>Kind Regards, <br/><br/>iHealth GSFM Team.</p>");
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
    }
}
