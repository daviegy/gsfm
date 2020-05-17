using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using iHealth2.Models;
namespace iHealth2.Controllers
{
    public class ReferralController : Controller
    {
       
        ApplicationDbContext db = new ApplicationDbContext();
        [HttpGet]
        [Authorize(Roles="Users")]
        public ActionResult Bonus(string id)
        {
           var usr = db.Users.First(u => u.UserName == id);
           ViewBag.refid = usr.Id;
            return View("Bonus");
      
        }
        [Authorize(Roles="Admin")]
        public ActionResult BonusBeneficiary()
        {
            return View();
        }
    }
}