using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace iHealth2.CustomClasses
{
    public class EmailTemplateService
    {
        public async Task<string> NotificationTempBody()
        {
            var templatePath = HostingEnvironment.MapPath("~/EmailTemplates/NotificationTemp.cshtml");
            StreamReader reader = new StreamReader(templatePath);
            var body = await reader.ReadToEndAsync();
            reader.Close();
            return body;
        }
       
    }
}