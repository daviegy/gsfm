using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iHealth2.CustomClasses;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using iHealth2.Models;
using System.Threading.Tasks;
namespace iHealth2.Controllers
{
    [Authorize]
    public class profileController : Controller
    {
        private ApplicationUserManager _userManager;
        ApplicationDbContext db = new ApplicationDbContext();
        public profileController()
        {

        }
        public profileController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult Profiles(string ids)
        {
            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            ids = ids.Replace("$25", "/");
            ids = ids.Replace("$24", "+");
            string DecryptId = Cryptoclass.DecryptStringAES(ids, s);
            var usr = UserManager.FindByName(DecryptId);

            if (usr == null)
            {
                return HttpNotFound("Requested user not found");
            }

            if (usr.Photo != null)
            {
                return View(new profileViewModel
                {
                    Id = usr.Id,
                    Name = usr.FullName,
                    Email = usr.Email,
                    phone = usr.PhoneNumber,
                    tID = usr.MyRefferalCode,
                    FName = usr.FirstName,
                    LName = usr.LastName,
                    Nationality = usr.Nationality,
                    state = usr.State,
                    city = usr.City,
                    Address = usr.Address,
                    profession = usr.Profession,
                    backgroundinfo = usr.backgroundinfo,
                    fbUrl = usr.facebookUrl,
                    lnUrl = usr.linkedinUrl,
                    twUrl = usr.twitterUrl,
                    User_type = usr.UserType,
                    photo =
                     "data:image/png;base64," + Convert.ToBase64String(usr.Photo)
                });
            }
            else
            {
                return View(new profileViewModel
                {
                    Id = usr.Id,
                    Name = usr.FullName,
                    Email = usr.Email,
                    phone = usr.PhoneNumber,
                    tID = usr.MyRefferalCode,
                    FName = usr.FirstName,
                    LName = usr.LastName,
                    Nationality = usr.Nationality,
                    state = usr.State,
                    city = usr.City,
                    Address = usr.Address,
                    profession = usr.Profession,
                    backgroundinfo = usr.backgroundinfo,
                    fbUrl = usr.facebookUrl,
                    lnUrl = usr.linkedinUrl,
                    twUrl = usr.twitterUrl,
                    User_type = usr.UserType
                });
            }


        }
        [HttpPost]
        public async Task<ActionResult> edprofile(string ids, [Bind(Exclude = "ID,TrackingID,Email")] profileViewModel apuser, FormCollection f)
        {
            if (ids == null)
            {
                return HttpNotFound("Error: Requested user not found!");
            }
            string s = System.Configuration.ConfigurationManager.AppSettings["MaxLevel"];
            ids = ids.Replace("$25", "/");
            ids = ids.Replace("$24", "+");
            string DecryptId = Cryptoclass.DecryptStringAES(ids, s);
            var usr = UserManager.FindById(DecryptId);

            if (ModelState.IsValid)
            {
                var A_O_S = f["other_Profession"];
                if(!string.IsNullOrEmpty(f["subCat1"]))
                {
                    int subcatId1 = Convert.ToInt32(f["subCat1"]);
                    var sbcat1 = await db.SubCategory1.FindAsync(subcatId1);
                    A_O_S = (A_O_S != "") ? A_O_S + ", " + sbcat1.SubCat1Name : sbcat1.SubCat1Name;
                }
                if(!string.IsNullOrEmpty(f["subCat2"]))
                {
                    int subcatId2 = Convert.ToInt32(f["subCat2"]);
                    var sbcat2 = await db.SubCategory2.FindAsync(subcatId2);
                    A_O_S = (A_O_S != "") ? A_O_S + ", " + sbcat2.SubCat2name : sbcat2.SubCat2name;
                }
                //string Username = model.UserName;
                int profVal = (!string.IsNullOrEmpty(f["Profession"])) ? Convert.ToInt32(f["Profession"]) : 0;
                var prof = (profVal != 0) ? await db.Profession.FindAsync(profVal) : null;
                if(f["Country"] != "")
                {
                    int conid = Convert.ToInt32(f["Country"]); var con3 = await db.country.FindAsync(conid); usr.Nationality = con3.CountryName;
                }
                if(apuser.state != null)
                {
                    int stid = Convert.ToInt32(apuser.state); var st = await db.State.FindAsync(stid); usr.State = st.StateName;
                }
                usr.City = apuser.city;
                usr.Address = apuser.Address;
                usr.PhoneNumber = apuser.phone;
                usr.backgroundinfo = (f["summary"] != "") ? f["summary"] : usr.backgroundinfo;
                usr.facebookUrl = (f["fbUrl"] != "") ? f["fbUrl"] : usr.facebookUrl;
                usr.twitterUrl = (f["twUrl"] != "") ? f["twUrl"] : usr.twitterUrl;
                usr.linkedinUrl = (f["lnUrl"] != "") ? f["lnUrl"] : usr.linkedinUrl;
                usr.Health_Service_Provider = f["HSP"];
                usr.Profession = (f["HSP"] == "Yes" && prof != null) ? prof.Nomenclature : "Information_Seeker";
                usr.AreaOfSpecialization = A_O_S;
                UserManager.Update(usr);
                TempData["Success"] = "Your Details has been Updated Successfully";
                String EncryptId = Cryptoclass.EncryptStringAES(usr.UserName, s);
                EncryptId = EncryptId.Replace("/", "$25");
                EncryptId = EncryptId.Replace("+", "$24");
                return RedirectToAction("Profiles", new { ids = EncryptId });
            }
            String EId = Cryptoclass.EncryptStringAES(usr.Id, s);
            EId = EId.Replace("/", "$25");
            EId = EId.Replace("+", "$24");
            ids = EId;
            TempData["error"] = "Check that the Phone Number field is correclty typed PHONE NUMBR FORMAT";
            ModelState.AddModelError("", "validate the error field");
            return RedirectToAction("profiles", new { ids = EId });
        }
    }
}