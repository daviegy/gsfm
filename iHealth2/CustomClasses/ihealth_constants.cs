using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace iHealth2.CustomClasses
{
    public class ihealth_constants
    {
        public SmtpClient smtp = new SmtpClient("smtp.zoho.com", 587);
        public string MailFrom = "noreply@ihealthgsfm.com"; //Domain Email for sending mail to fresh subscribers, newly register business or product 
        public string pswd = "Speedy@123";
        public int acctno = 1018361479;
        public string bankName = "United Bank of Africa(UBA)";
        public string AcctName = "iHealth Nigeria GSFM Ltd.";
        public int amount = 10000;
        public string Mail_support = "support@ihealthgsfm.com"; //Domain Email for sending mail to subscriber incase of any support or help      
        public string displayName = "iHealthGSFM";
        public string ihealthUrl = "http://ihealthgsfm.com";
        public string product_id = "8308";
        public string Mac_key = "pk68bGDQ14RZT64i5yuX3lR8xuFs2gtkZRxGIF9KSlrcRr4nAeO82k86NDQDzyH9CFbkoyTM522iKpv8GN54AzUvNULAfPKIAzY27y0jx89b6ekDTVPKayVFAqNNMK6k";
        public string interswitchJson = "https://webpay.interswitchng.com/collections/api/v1/gettransaction.json?productid=8308&transactionreference=";


    }
}