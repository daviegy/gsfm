using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using iHealth2.CustomClasses;
using System.Web.Mvc;
namespace iHealth2.Models
{
    //Users View Model
    public class EditViewModel
    {
        public string ID { get; set; }
        public int TrackingID { get; set; }

        public string Name { get; set; }

        public string Nationality { get; set; }
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]

        public string Phone { get; set; }
        public string Address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        //public string username { get; set; }
    }
    public class profileViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public string profession { get; set; }
        [Phone]
        public string phone { get; set; }
        public int tID { get; set; }
        public string photo { get; set; }
        public string Address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string Nationality { get; set; }
        public string backgroundinfo { get; set; }
        public string fbUrl { get; set; }
        public string twUrl { get; set; }
        public string lnUrl { get; set; }
        public string User_type { get; set; }
    }

    //Admin View model
    public class CompanyViewModel
    {
        public int ID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public string Category { get; set; }
        public string subCat1 { get; set; }
        public string subCat2 { get; set; }
        public string Address { get; set; }
        public DateTime? regDate { get; set; }

        public string UserId { get; set; }
        public string Name { get; set; }

    }
    public class SearchResultModel
    {
        public int ID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public string Category { get; set; }
        public string subCat1 { get; set; }
        public string subCat2 { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public byte[] Logo { get; set; }
    }

    public class LatestNewViewModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public DateTime? Date { get; set; }
    }
    public class RegisterProductViewModel
    {
        //[Key]
        //public int ProductId { get; set; }
        //public string UserID { get; set; }
        [Display(Name = "Product Name")]
        [Required]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "The Product Category Field is Required")]
        [Display(Name = "Product Category")]
        public string ProductCategory { get; set; }
        [Required(ErrorMessage = "Product Manufacturer Required")]
        public string Manufacturer { get; set; }
        [Display(Name = "Products Summary")]
        [Required(ErrorMessage = "Product summarized description is required")]
        public string Product_Summary { get; set; }
        [Required(ErrorMessage = "The Product Detail Description Field is Required")]
        [Display(Name = "Product Description")]
        public String ProductDescription { get; set; }
        [Required]
        [Display(Name = "Product Price")]
        [Range(0, 999999999)]
        public Decimal price { get; set; }
        [Required(ErrorMessage = "The Country Field is Required")]
        [Display(Name = "Country")]
        public string Country { get; set; }
        [Required]
        [Display(Name = "State")]
        public string State { get; set; }
        //[Required]
        [Display(Name = "City")]
        public string city { get; set; }
        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }
        public string location { get; set; }
        public drugsCategoriesModel drugs_categories { get; set; }
        public ProductConditionModel productCond { get; set; }
        //public byte[] ProductImage { get; set; }
    }
    //Depends on the RegisterProductViewModel
    public class drugsCategoriesModel
    {

        [Display(Name = "Drug's Category ID")]
        public string Drug_Category_Id { get; set; }
        [Display(Name = "Drug's Category")]
        public string Drug_Category_Name { get; set; }
        public IEnumerable<drugsCategoriesModel> get_Drug_Categories()
        {
            return new List<drugsCategoriesModel>
            {
                 new drugsCategoriesModel(){Drug_Category_Id = "Analgesics", Drug_Category_Name = "Analgesics"},
                 new drugsCategoriesModel(){Drug_Category_Id = "Anti-Biotics", Drug_Category_Name = "Anti-Biotics"},
                 new drugsCategoriesModel(){Drug_Category_Id = "Anti-Diabetes", Drug_Category_Name = "Anti-Diabetes"},
                 new drugsCategoriesModel(){Drug_Category_Id = "Anti-Depressants", Drug_Category_Name = "Anti-Depressants"},
                 new drugsCategoriesModel(){Drug_Category_Id = "Anti-Fungal", Drug_Category_Name = "Anti-Fungal"},
                 new drugsCategoriesModel(){Drug_Category_Id = "Anti-Protozoal", Drug_Category_Name = "Anti-Protozoal"},
                 new drugsCategoriesModel(){Drug_Category_Id = "Anti-Retroviral", Drug_Category_Name = "Anti-Retroviral"},
                 new drugsCategoriesModel(){Drug_Category_Id = "Anti-Viral", Drug_Category_Name = "Anti-Viral"},
                 new drugsCategoriesModel(){Drug_Category_Id = "Cardio-Vascular", Drug_Category_Name = "Cardio-Vascular"},
                 new drugsCategoriesModel(){Drug_Category_Id = "Cough_&_Cold", Drug_Category_Name = "Cough & Cold"},
                   new drugsCategoriesModel(){Drug_Category_Id = "Dermatologic_Care", Drug_Category_Name = "Dermatologic Care"},
                new drugsCategoriesModel(){Drug_Category_Id ="Dietary_Supplements", Drug_Category_Name ="Dietary Supplements"},
                new drugsCategoriesModel(){Drug_Category_Id = "Digestive_Care", Drug_Category_Name = "Digestive Care"},
                 new drugsCategoriesModel(){Drug_Category_Id = "Eye_Care", Drug_Category_Name = "Eye Care"},
                  new drugsCategoriesModel(){Drug_Category_Id = "Hormonal", Drug_Category_Name = "Hormonal"},
                   new drugsCategoriesModel(){Drug_Category_Id = "Malaria-Treatment", Drug_Category_Name = "Malaria-Treatment"},
                  new drugsCategoriesModel(){Drug_Category_Id = "Supplement", Drug_Category_Name = "Supplements"},
                new drugsCategoriesModel(){Drug_Category_Id = "Vitamins_&_Multivitamins", Drug_Category_Name = "Vitamins & Multivitamins"},
         
            };
        }
    }
    //Depends on the RegisterProductViewModel
    public class ProductConditionModel
    {
        public string id { get; set; }
        public string value { get; set; }
        public IEnumerable<ProductConditionModel> GetProductionConditon()
        {
            return new List<ProductConditionModel>(){
                new ProductConditionModel(){id="New",value="Brand New"},
                new ProductConditionModel(){id="Fairly Used", value = "Fairly Used"}
            };
        }
    }
    public class BusinessDetailViewModel
    {
        public IEnumerable<BusinessInfo> bizViewModel { get; set; }
    }
    public class FeedBackViewModel
    {
        public int ID { get; set; }
        // public string FBType { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string FRName { get; set; }
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string FREmail { get; set; }
        [Display(Name = "Phone Number")]
        // [Required]
        [Phone]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "Type a valid valid mobile number not more than 11 digits")]

        public string FRPhone { get; set; }

        [Required]
        [Display(Name = "Message")]
        public string FBMessage { get; set; }
         [Required]
        public string subject { get; set; }
        // public string FBStatus { get; set; }
        //public DateTime? SentDate { get; set; }

    }

    public class INIRegFormModel
    {
        //Personal Info

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [System.Web.Mvc.Remote("CheckEmail", "Account")]
        public string Email { get; set; }
        [Required]
        [Phone]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "Type a valid valid Nigeria mobile number.(Note: '+234 initial is not required')")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        // Login Info
        [Required]
        [System.Web.Mvc.Remote("IsUserExist", "Account")]
        public string Username { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The confirmation password does not match.")]
        public string ConfirmPassword { get; set; }
        // payment Method
        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentOptions { get; set; }
        [BooleanRequired(ErrorMessage = "You Must Accept Terms and Conditions")]
        [Display(Name = "Terms and Conditons")]
        public bool TermsAndConditions { get; set; }

        [Display(Name = "Sponsor Code")]
        [System.Web.Mvc.Remote("CheckSponsor", "Account")]
        public string RefCode { get; set; }
        // [Display(Name="Become a Service Booster")]
        //public bool isClientBooster { get; set; }
        //[Required]
        //public string BenefitCategory { get; set; }
    }
    public class UpgradeFormViewModel
    {


        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        public string Gender { get; set; }
        //[Required]
        //public string Country { get; set; }
        //[Required]
        //public string State { get; set; }
        [Display(Name = "Email")]
        [System.Web.Mvc.Remote("CheckEmail", "Account")]
        public string Email { get; set; }
        // [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        // Login Info

        public string Username { get; set; }

        // payment Method
        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentOptions { get; set; }
        [BooleanRequired(ErrorMessage = "You Must Accept Terms and Conditions")]
        [Display(Name = "Terms and Conditons")]
        public bool TermsAndConditions { get; set; }
        [Display(Name = "Sponsor Code")]

        public string RefCode { get; set; }
        //[Required]
        //public string BenefitCategory { get; set; }
    }
    public class BankInfoFormModel
    {
        public string Id { get; set; }
        //[Display(Name = "Membership ID")]
        //public int SubcriberRefCode { get; set; }
        [Display(Name = "Bank Name*")]
        [Required]
        public string BankName { get; set; }
        [Display(Name = "Account Name*")]
        [Required]
        public string AccountName { get; set; }
        [MaxLength(10, ErrorMessage = "Account Number cannot be more than 10digits")]
        [MinLength(10, ErrorMessage = "Account Number cannot be less than 10-digit")]
        [Required]
        [Display(Name = "Account Number*")]
        public string AcountNumber { get; set; }
    }

    public class BPReportViewModel
    {
        public string MembershipID { get; set; }
        [Display(Name = "PROCESSING ID")]
        public string ProcessingId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        //Bank Info
        public string AccountName { get; set; }
        public string AccountNo { get; set; }
        public string BankName { get; set; }
        // Benefit Info
        public string Stage { get; set; }
        public string Benefit { get; set; }
        public string BenefitCat { get; set; }

        public string CurrentStatus { get; set; }
        public string SignedBy { get; set; }
        public DateTime? ProcessedDate { get; set; }
    }

    public class UrgentNeedForDrugViewModel
    {
        [Required]
        public string DrugName { get; set; }
        [Display(Name = "Dosage Form")]
        public dosageFormModel dosageForm { get; set; }
        public String Manufacturer { get; set; }
        public string ManufacturerCountry { get; set; }
        [Display(Name = "Drug Strength")]
        public string DrugStrength { get; set; }
        [Display(Name = "More Information")]
        public string Description { get; set; }
        //Drug Buy/Requestor information
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Phone]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "Type a valid valid Nigeria mobile number")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Landmark { get; set; }
        [Display(Name = "Requested Date")]
        public DateTime? date { get; set; }

    }
    public class dosageFormModel
    {
        [Display(Name = "Dosage Form")]
        [Required]
        public string dosageFormValue { get; set; }
        public string dosageFormName { get; set; }
        public IEnumerable<dosageFormModel> GetForm()
        {
            return new List<dosageFormModel>{
            new dosageFormModel(){dosageFormValue = "Capsule", dosageFormName = "Capsule"},
            new dosageFormModel(){dosageFormValue = "Cream", dosageFormName = "Cream"},
            new dosageFormModel(){dosageFormValue = "Injection", dosageFormName = "Injection"},
            new dosageFormModel(){dosageFormValue = "Tablet", dosageFormName = "Tablet"},
            new dosageFormModel(){dosageFormValue = "Syrup", dosageFormName = "Syrup"}
            };
        }
    }
    public class AdvertManagerViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Advert Owner *")]
        [Required]
        public string AdsOwner { get; set; }
        // [Required]
        //[Display(Name = "Advert Name")]
        //public string AdsName { get; set; }
        //[Display(Name = "Advert Image *")]
        ////public byte[] AdsImage { get; set; }
        // public string UniqueID { get; set; }
        // this will contain information on which page 
        // the Ads will be displayed
        // [Required]
        public AdspageModel adspage { get; set; }
        //this will contain information on the particular location where the Ads 
        // will be displayed on a spefic page

        public AdsLocationOnPageModel AdsLocOnPage { get; set; }
        //[Display(Name = "Advert Size")]
        //[Required]
        //public string AdsSize { get; set; }
        [Display(Name = "Advert URL")]
        public string AdsUrl { get; set; }

    }
    //Depends on the AdvertManagerViewModel
    public class AdspageModel
    {
        [Required]
        [Display(Name = "Advert Page*")]
        public string pageId { get; set; }
        [Display(Name = "Advert Page*")]
        public string pageName { get; set; }
        public IEnumerable<AdspageModel> GetPage()
        {
            return new List<AdspageModel>{
                new AdspageModel(){pageId = "HomePage", pageName = "Home Page"},
               new AdspageModel(){pageId = "FindHospitalPage", pageName = "Find Hospital Page"},
               new AdspageModel(){pageId = "SearchPharmacyPage", pageName = "Search Pharmacy Page"},
               new AdspageModel(){pageId = "SearchDrugPage", pageName = "Search Drugs Page"},
               new AdspageModel(){pageId = "FindLabPage", pageName = "Find Lab. Page"},
               new AdspageModel(){pageId = "MedEquipCompPage", pageName = "Med. Equipment Company Page"},
               new AdspageModel(){pageId = "FindMedEquipPage", pageName = "Find Med. Equipment page"},
               new AdspageModel(){pageId = "SearchHerbalCentrePage", pageName = "Search Herbal Centre Page"},
               new AdspageModel(){pageId = "SearchHerbalDrugPage", pageName = "Search Herbal Drug Page"},
               new AdspageModel(){pageId = "SearchVetCentrePage", pageName = "Search Vet. Centre Page"},
               new AdspageModel(){pageId = "SearchVetDrugPage", pageName = "Search Vet. Drug Page"},
            };
        }
    }
    //Depends on the AdvertManagerViewModel
    public class AdsLocationOnPageModel
    {
        [Required]
        [Display(Name = "Ads Location*")]
        [System.Web.Mvc.Remote("is_Ads_In_Loc", "superadm")]
        public string locId { get; set; }

        [Display(Name = "Ads Location*")]
        public string locName { get; set; }
        public IEnumerable<AdsLocationOnPageModel> GetLocation()
        {
            return new List<AdsLocationOnPageModel>{
                new AdsLocationOnPageModel(){locId = "LeaderBoard", locName = "LeaderBoard (728 X 90 (px))"},
               new AdsLocationOnPageModel(){locId = "MediumRectangle1", locName = "Medium Rectangle1 (300 X 250 (px))"},
               new AdsLocationOnPageModel(){locId = "MediumRectangle2", locName = "Medium Rectangle2 (300 X 250 (px))"},
               new AdsLocationOnPageModel(){locId = "MediumRectangle3", locName = "Medium Rectangle3 (300 X 250 (px))"},
             new AdsLocationOnPageModel(){locId = "Slider", locName = "Slider (1000 X 450 (px))"},
               new AdsLocationOnPageModel(){locId = "SquareBoard", locName = "Square Board (280 X 300 (px))"},
              
            };
        }
    }

    public class RegisterCareerViewModel
    {
        public int id { get; set; }
        [Required]
        [Display(Name = "First Name:")]
        public string FName { get; set; }
        [Required]
        [Display(Name = "Last Name:")]
        public string LName { get; set; }
        [Required]
        [Phone]
        [Display(Name = "Phone No.:")]
        public string Phone { get; set; }
        [Required]
        [EmailAddress]
        [Remote("valEmail", "Home", ErrorMessage = "Email Address Exist")]
        [Display(Name = "Email:")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; }
        [Display(Name = "Nationality")]
        public string Country { get; set; }
        [Display(Name = "State/Province:")]
        public string State { get; set; }
        [Display(Name = "City:")]
        public string City { get; set; }
        [Required]
        [Display(Name = "Address:")]
        public string Address { get; set; }
        [Display(Name = "Position")]
        public string JobTitle { get; set; }
        [Display(Name = "Specialization")]
        public string Specialization { get; set; }
    }
    //Depends on the RegisterCareerViewModel
    public class specializationsModel
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "Specialization")]
        public string Specialization { get; set; }
        public IEnumerable<specializationsModel> getSpecializations()
        {
            return new List<specializationsModel>{
                new specializationsModel(){Id = "HR", Specialization = "Human Resource"},
               new specializationsModel(){Id = "ICT", Specialization = "Information Technology"},
               new specializationsModel(){Id = "Marketing", Specialization = "Marketing"},
               new specializationsModel(){Id = "FA", Specialization = "Finance/Account"},
               new specializationsModel(){Id = "Legal", Specialization = "Legal"},
           
            };
        }
    }
    public class createJobViewModel
    {
        [Display(Name = "Job Position / Title")]
        [Required]
        public string JobTitle { get; set; }

        public specializationsModel specialization { get; set; }
        [Display(Name = "Job Description")]
        public string Descriptions { get; set; }


    }
    public class benefitclaimersDetails
    {
        public int RefCode { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        [Display(Name = "Acct. No.")]
        public string AccNo { get; set; }
        [Display(Name = "Acct. Name")]
        public string AccName { get; set; }
        public string BankName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        [Display(Name = "No. of Downlines")]
        public int Downline { get; set; }
    }
    public class create_blog_post_viewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Title")]
        public string post_Title { get; set; }
        [Display(Name = "Short Description")]
        public string short_description { get; set; }
        [Display(Name = "Content")]
        public string post_content { get; set; }
        [Display(Name = "Tags")]
        public string post_tags { get; set; }
        [Display(Name = "Video Url")]
        public string video_url { get; set; }
        [Display(Name = "Meta-Title")]
        public string meta_title { get; set; }
        [Display(Name = "Meta-Description")]
        public string meta_description { get; set; }
        [Display(Name = "Meta-Keyword")]
        public string meta_keyword { get; set; }
        [Display(Name = "Custom Url")]
        public string post_url { get; set; }
        [Display(Name = "Allow comment")]
        public bool allow_comment { get; set; }
    }
    public class Edit_blog_post_viewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Title")]
        public string post_Title { get; set; }
        [Display(Name = "Short Description")]
        public string short_description { get; set; }
        [Display(Name = "Content")]
        public string post_content { get; set; }
        [Display(Name = "Tags")]
        public string post_tags { get; set; }
        [Display(Name = "Video Url")]
        public string video_url { get; set; }
        [Display(Name = "Meta-Title")]
        public string meta_title { get; set; }
        [Display(Name = "Meta-Description")]
        public string meta_description { get; set; }
        [Display(Name = "Meta-Keyword")]
        public string meta_keyword { get; set; }
        [Display(Name = "Custom Url")]
        public string post_url { get; set; }
        [Display(Name = "Allow comment")]
        public bool allow_comment { get; set; }
        public DateTime? publish_date { get; set; }
        public bool use_video_as_cover_img { get; set; }
    }
    public class blog_post_details //this is the model that display list of blog post to site visitors
    {
        public int id { get; set; }
        public string post_title { get; set; }
        public string short_description { get; set; }
        public string post_content { get; set; }
        public string author { get; set; }
        public DateTime? publishedDate { get; set; }
        public byte[] featured_img { get; set; }
        public int no_comments { get; set; }
        public string post_url { get; set; }
        public string ImageUrl { get; set; }
        public string meta_title { get; set; }
        public string meta_description { get; set; }
        public string meta_keyword { get; set; }
        public IEnumerable<blog_post_comment> comments { get; set; }
    }
    public class reply_comment_viewModel
    {
        public int id { get; set; }
        public string comment_by_name { get; set; }
        [Display(Name = "Email")]
        public string comment_by_email { get; set; }
        [Display(Name = "Comment Content")]
        public string comment_content { get; set; }
        public string reply_content { get; set; }
        public string reply_by_name { get; set; }
        public string post_title { get; set; }

    }
    public class find_professionals_viewModel
    {
        public string Name { get; set; }
        public string profession { get; set; }
        public string summary { get; set; }
        public string fburl { get; set; }
        public string twUrl { get; set; }
        public string lnUrl { get; set; }
        public byte[] image { get; set; }
    }
    public class appBooking_ViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Phone]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "Type a valid valid Nigeria mobile number")]
        public string Phone { get; set; }
        //   public string Appointment_With { get; set; }
        // public string Service_Required { get; set; }
        public string Message { get; set; }
        // public DateTime? Appointment_Date { get; set; }
    }
    public class ApproveApptViewModel
    {
        public int id { get; set; }
        public int appid { get; set; }
        public string Name { get; set; }
        public DateTime? appointmentDate { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string apptwith { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string message { get; set; }
    }
    public class view_biz_profile
    {
        public int Id { get; set; }
        public string Business_name { get; set; }
        public string Overview { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string addr { get; set; }
        public string websit { get; set; }
        public string fb { get; set; }
        public string tw { get; set; }
        public string ln { get; set; }
        public string gPlus { get; set; }
        //public ICollection<BusinessImages> Bimg { get; set; }
    }
    public class referral_bonus_view_model
    {
        public int id { get; set; }
        public string downline_name { get; set; }
        public double subcription_fee { get; set; }
        public double my_bonus { get; set; }
        public string bonus_type { get; set; }
        public DateTime? created_date { get; set; }
    }
    public class Hospital_list_viewModel
    {
        public int id { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Hospital { get; set; }
        public string Address { get; set; }
        public string Plans { get; set; }
        public string Speciality { get; set; }
        public string reg_no { get; set; }
    }
    public class premium_user_Reg_ViewModel
    {

        [Required]
        [Display(Name = "First Name")]
        public string First_Name { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string Last_Name { get; set; }
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        //[System.Web.Mvc.Remote("Check_Puser_Email", "ING")]
        public string Email { get; set; }
        [Required]
        [Phone]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "Type a valid valid Nigeria mobile number")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]      
        public string Phone { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string job { get; set; }
        //[Required]
        public string dob { get; set; }
        //[Required]
        [Display(Name = "Country")]
        public string Nationality { get; set; }
        [Required]
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public premium_planModel premium_plan { get; set; }
        //[Required]
        public string Hospital_List { get; set; }
        [Required]
        public double Price { get; set; }
        public int referralcode { get; set; }
        [Display(Name = "Terms and Conditons")]
        public bool TermsAndConditions { get; set; }
    }
    public class premium_planModel
    {
        [Required]
        [Display(Name = "Plan*")]
        public string planID { get; set; }
        //[Required]
        //[Display(Name = "Plan*")]
        public string planName { get; set; }
        public IEnumerable<premium_planModel> GetPlan()
        {
            return new List<premium_planModel>{
                new premium_planModel(){planID= "FHLPA", planName = "FHLPA"},
                new premium_planModel(){planID= "FHLPB", planName = "FHLPB"},
                new premium_planModel(){planID= "SHLPA", planName = "SHLPA"},
               new premium_planModel(){planID = "SHLPB", planName = "SHLPB"},
                 new premium_planModel(){planID= "HCN1", planName = "HCN1"},
               new premium_planModel(){planID = "HCN2", planName = "HCN2"},
               new premium_planModel(){planID="HCN",planName="HCN"}
            };
        }
        public Premium_Package_Price plan_price { get; set; }
       // public premium_plan_HospitalListModel HospitalList { get; set; }
    }
    //Depends on the AdvertManagerViewModel
    //public class premium_plan_HospitalListModel
    //{
    //    //[Required]
    //    //[Display(Name = "Hospital List*")]
    //    public string Hosp_List_Id { get; set; }
    //    // [Required]
    //    //[Display(Name = "Hospital List*")]
    //    public string Hosp_List_Name { get; set; }
    //    public string planId { get; set; }
    //    public IEnumerable<premium_plan_HospitalListModel> Get_Hosp_List()
    //    {
    //        // char x = (char)8358;
    //        return new List<premium_plan_HospitalListModel>{
    //            new premium_plan_HospitalListModel(){planId = "FHLPA",Hosp_List_Id = "First_Hospital_ListA", Hosp_List_Name = "First Hospital List"},
    //           new premium_plan_HospitalListModel(){planId = "SHLPA",Hosp_List_Id = "Second_Hospital_ListA", Hosp_List_Name = "Second Hospital List"},
    //            new premium_plan_HospitalListModel(){planId = "FHLPB",Hosp_List_Id = "First_Hospital_ListB", Hosp_List_Name = "First Hospital List"},
    //           new premium_plan_HospitalListModel(){planId = "SHLPB",Hosp_List_Id = "Second_Hospital_ListB", Hosp_List_Name = "Second Hospital List"},
    //             new premium_plan_HospitalListModel(){planId = "HCN1",Hosp_List_Id = "HCN1", Hosp_List_Name = "High Class Networker"},
    //             new premium_plan_HospitalListModel(){planId = "HCN2",Hosp_List_Id = "HCN2", Hosp_List_Name = "High Class Networker"},
    //             new premium_plan_HospitalListModel(){planId = "HCN",Hosp_List_Id = "HCN", Hosp_List_Name = "High Class Networker"}
    //        };
    //    }
    //    public Premium_Package_Price Price { get; set; }
    //    //public virtual premium_planModel premium_plan { get; set; }
    //}
    public class Premium_Package_Price
    {
        //[Required]
        //[Display(Name = "Price*")]
        public string priceName { get; set; }
        //[Required]
        //[Display(Name = "Price*")]
        public double priceValue { get; set; }
       // public string Hosp_List_Id { get; set; }
       public string planID { get; set; }
        //public virtual premium_plan_HospitalListModel Hosp_List { get; set; }
        public IEnumerable<Premium_Package_Price> Get_Price_List()
        {
            return new List<Premium_Package_Price>{
        new Premium_Package_Price(){planID = "FHLPA", priceValue= 15000.00, priceName = (char)8358+"15,000.00"},
        new Premium_Package_Price(){planID = "FHLPB", priceValue= 25000.00, priceName = (char)8358+"25,000.00" },
        new Premium_Package_Price(){planID = "SHLPA", priceValue= 25000.00, priceName = (char)8358+"25,000.00" },
        new Premium_Package_Price(){planID = "SHLPB", priceValue= 50000.00, priceName = (char)8358+"50,000.00" },
        new Premium_Package_Price(){planID = "HCN1", priceValue= 60000.00, priceName = (char)8358+"60,000.00" },
        new Premium_Package_Price(){planID = "HCN2", priceValue= 90000.00, priceName = (char)8358+"90,000.00" },
        new Premium_Package_Price(){planID = "HCN", priceValue= 220000.00, priceName = (char)8358+"220,000.00" }
    };

        }
    }

    public class service_booster_client_VM
    {
        public string sbid { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }        
       
        [Display(Name = "Email")]
       
        public string Email { get; set; }
       
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }
      
        public string Gender { get; set; }
     
        public string Profession { get; set; }
    
        [Display(Name = "Country")]
        public string Nationality { get; set; }
      
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
      
      
    }
}
