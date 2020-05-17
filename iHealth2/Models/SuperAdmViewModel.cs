using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace iHealth2.Models
{
    public class ModifyRoleViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Display(Name="Name")]
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
        [Display(Name="Current Role")]
        public string CurrentRole { get; set; }
    }
    public class RegisterAdminViewModel
    {

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Phone]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "Type a valid valid Nigeria mobile number")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [System.Web.Mvc.Remote("CheckEmail", "Account")]
        public string Email { get; set; }

    }

}