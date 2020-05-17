using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iHealth2.Models
{
    public class Email_Notification_Model
    {
        public string Name_of_Recipient { get; set; }
        public string Email_Body { get; set; }
    }
    public class Welcome_Email_Model
    {
        public string Name_of_Recipient { get; set; }
        //public string Email_Body { get; set; }
        public string Url { get; set; }
    }
    public class Admin_Email_Notification_Model
    {
       public string Email_Body { get; set; }
    }
}