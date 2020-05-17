using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace iHealth2.Models
{
    public class country
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string sortname { get; set; }
        public string CountryName { get; set; }
        public int phonecode { get; set; }

        public virtual ICollection<state> States { get; set; }
        //   public ICollection<BusinessInfo> Businessinfo { get; set; }
    }
    public class state
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int CountryID { get; set; }
        public string StateName { get; set; }
        public virtual country country { get; set; }
        public virtual ICollection<city> City { get; set; }
        //public ICollection<BusinessInfo> Businessinfo { get; set; }

    }
    public class city
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int stateID { get; set; }
        public string cityName { get; set; }
        public virtual state State { get; set; }
        //   public ICollection<BusinessInfo> Businessinfo { get; set; }
    }

    public class Category
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string CatName { get; set; }
        public virtual ICollection<SubCategory1> Subcategory1 { get; set; }
        //  public ICollection<BusinessInfo> Businessinfo { get; set; }
    }
    public class SubCategory1
    {
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int CategoryID { get; set; }
        public string SubCat1Name { get; set; }
        public virtual Category category { get; set; }
        public virtual ICollection<SubCategory2> subCategory2 { get; set; }
        //   public ICollection<BusinessInfo> Businessinfo { get; set; }
    }
    public class SubCategory2
    {
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int SubCategory1ID { get; set; }
        public string SubCat2name { get; set; }
        public virtual SubCategory1 subCategory1 { get; set; }
        // public ICollection<BusinessInfo> Businessinfo { get; set; }
    }
    public class BusinessInfo
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        [Required]
        public string businessName { get; set; }
        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }
        [Required]
        [Display(Name = "State")]
        public string State { get; set; }
        [Required]
        public string City { get; set; }
        public string ZipCode { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "Type a valid valid Nigeria mobile number")]
        [DataType(DataType.PhoneNumber)]
        public String Phone { get; set; }
        [Required]
        [Display(Name = "Business Type")]
        public string Category { get; set; }
        public string subCategory1 { get; set; }
        public string subCategory2 { get; set; }
        public string Website { get; set; }
        //social media handle
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string LinkedIn { get; set; }
        public string Google_Plus { get; set; }
        public string Custom_Url { get; set; } // this is the URL to the business profile on ihealth Website
        // this will make the company easily searchable by site user
        public string Description { get; set; }
        public string keyword { get; set; }
        [Required]
        public string Address { get; set; }
        public byte[] logo { get; set; }
        public int NotifyStatus { get; set; }
        public DateTime? regDate { get; set; }
        public DbGeography mapLocation { get; set; }
        // public DbGeography latitude { get; set; }
        /// <summary>
        /// approved, verified, and recommended status show the order with which business registered with ihealth 
        /// are displayed
        /// the business with recommended status are displayed first follow by verified and then approved
        /// </summary>
        public string ApprovedStatus { get; set; }
        public string VerifiedStatus { get; set; }
        public string Recommended_Status { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? VerifyDate { get; set; }
        public DateTime? RecommendedDate { get; set; }
        public string SignedBy { get; set; }// this display the name of the Ihealth Staff that attended to this request
        [ForeignKey("UserID")]
        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }
        public ICollection<BusinessImages> BusinessImages { get; set; }
        public string isServiceBooster { get; set; }

    }
    public class BusinessImages
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int BizId { get; set; }
        public byte[] galleryImage { get; set; }
        public string isSliderImage { get; set; } // this will either be true or false
        public string imageUrl { get; set; }
        [ForeignKey("BizId")]
        public virtual BusinessInfo bizinfo { get; set; }
    }
    public class status
    {

        //[DataType(dataType: char)]
        [Key]
        [MaxLength(4)]
        public string ID { get; set; }
        public string Name { get; set; }


    }

    public class IHealthUsersMLM  //Business subscribers referral table 
    // this contain business user other information. 
    // from the data in this column Business user can track  upliner and downlines
    {
        //[DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        // public int ID { get; set; }
        [Key, ForeignKey("User")]
        public string UserID { get; set; }
        [Required]
        public int MyRefferalCode { get; set; }

        public string MySponsorRefCode { get; set; }
        public int MyDownlineCount { get; set; }

        // [JsonIgnore]
        public virtual ApplicationUser User { get; set; }
        //public DateTime? referredRegdate { get; set; }

    }

    public class LatestNews
    {
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public string Url { get; set; }
        public byte[] Image { get; set; }

        public string ImageUrl { get; set; }
        public DateTime? EntryDate { get; set; }
    }

    public class ProductsInfo
    {
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserID { get; set; }
        public int ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Manufacturer { get; set; }
        public string ProductCategory { get; set; }
        public string Pharmaceutic_Drug_Category { get; set; }//This will hold the category of drugs if pharmaceutical drug is selected
        public string Product_Summary { get; set; }
        public String ProductDescription { get; set; }
        public Decimal price { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Address { get; set; }
        public string location { get; set; }
        public byte[] ProductImage { get; set; }
        public byte[] ProductImage2 { get; set; }
        public byte[] ProductImage3 { get; set; }
        public byte[] ProductImage4 { get; set; }
        public string ApprovedStatus { get; set; }
        public string VerifiedStatus { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? VerifyDate { get; set; }
        [ForeignKey("UserID")]
        public virtual ApplicationUser OwnerInfo { get; set; }
        public int NotifyStatus { get; set; }
        public DateTime? regDate { get; set; }
        public string ProductCondition { get; set; }
        public string Signedby { get; set; }// this display the name of the Ihealth Staff that attended to this request

    }
    public class productCategory
    {
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string PCatName { get; set; }
    }

    public class autoSearchTerm
    {
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public string searchTerm { get; set; }
    }

    public class FeedBack
    {
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string FBType { get; set; }
        [Required]
        public string FRName { get; set; }
        [Required]
        public string FREmail { get; set; }
        public string FRPhone { get; set; }
         [Required]
        public string subject { get; set; }

        [Required]
        public string FBMessage { get; set; }
        public string FBStatus { get; set; }
        public DateTime? SentDate { get; set; }
        public string ReportType { get; set; }
        public string RepliedMsg { get; set; }
        public DateTime? ReplyDate { get; set; }

    }

    public class Professions
    {
        public int ID { get; set; }
        public string Nomenclature { get; set; }
    }
    //NOTE: INI means Ihealth Networking insurance
    public class INISubcriberExtraDetail
    {
        // this give access to subscriber basic info
        [Key, ForeignKey("User")]
        public string UserID { get; set; }
        // public int MyRefferalCode { get; set; }

        //this is set to check active Subscribers
        public string INI_Status { get; set; }
        //the main purpose of the Promotional_Target_Subscription_Status is to
        //enable subscriber choose to participate in the ihealth promotion or not
        //if members subscribe for the promo and meet the required target then they
        //are entitle to the health insurance benefits or additional 10% on referral bonus
        //the status of the Promotional_Target_Subscription_Status can either be "ACTIVE" OR "IN_ACTIVE"
        public string Promotional_Target_Subscription_Status { get; set; }
        // this date is required for ING members, who subscribe for the promotional target keep track of 
        // their promotion.
        // 
        public DateTime? Promo_start_date { get; set; }
        // the period is not more than 30days from the starting date.
        public DateTime? Promo_end_date { get; set; }
        //this is the date required for only Health Benefit Subcribers to meet
        // required target
        //public DateTime? MaxDT2MtTarget2 { get; set; }// 6 month
        //public DateTime? MaxDT2MtTarget3 { get; set; }// 9 month
        //public DateTime? MaxDT2MtTarget4 { get; set; }// 12 month
        //public DateTime? MaxDT2MtTarget5 { get; set; }// 15 month 
        //this give information about subcriber benefit eligibility
        public string BEstatus { get; set; }

        //this  give information about subscriber's Downline Size (SDownline) and sponsor code (if any is availble)
        //this is the total number of downline that registered under a subscriber within the promo period
        public int promo_dl_size { get; set; }
        //this is the total number of downline that registered under a subscriber outside the promo period
        public int Non_promo_dl_size { get; set; }
        //this is the total number of downline under a subscriber
        public int Total_dl_Size { get; set; }
        //this record the date that a subscriber's most recent downline registered.       
        // the main purpose of this is to keep track of whether the subcriber is able to meet the target within the 
        // originally stipulated date .
        public DateTime? Most_recent_DL_Reg_date { get; set; }
        // 
        public string My_Sponsor_Referral_Code { get; set; }

        // This give Info about subscriber's benefit once he/she qualify for or meet the required target set.
        // this session will also be manage/updated by the system admin once the subscriber benefit has been processed.

        public DateTime? BenefitStartDate { get; set; }
        public DateTime? BenefitEndDate { get; set; }
        public double CurrentBonus { get; set; }
        public string CurrentBenefit { get; set; }// this will be use to set subscriber's benefit whenever he/she                                                 
        // has one. and it will be reset to default value once he claims benefit
        public string BenefitStatus { get; set; }
        public string BenefitCategory { get; set; }
        //  public string CurrentStage { get; set; }// this show the current stage a Health Benefit subscriber is in
        //Bank Information


        //// the columns below will be used to keep track of health subscriber's benefits per stage
        //public string Stage1Benefit { get; set; }
        //public string Stage2Benefit { get; set; }
        //public string Stage3Benefit { get; set; }
        //public string Stage4Benefit { get; set; }
        //public string Stage5Benefit { get; set; }

        public DateTime? UpgradeDate { get; set; }// this is the date that a normal user upgraded to become an ING subscriber
        //payment informations begins
        public string transaction_Id { get; set; }
        public string paymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string paymentGateway { get; set; }
        public int Amounts { get; set; }
        public DateTime? Transaction_date { get; set; }
        //payment information ends 
        public virtual ApplicationUser User { get; set; }
        public ICollection<referral_BonusTb> referralBonus { get; set; }
    }
    public class Bank_Account_Info
    {
        [Key, ForeignKey("Account_Owner")]
        public string Account_holder_id { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public String AccountNumber { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        //[Required]

        public virtual ApplicationUser Account_Owner { get; set; }
    }
    public class BenefitClaimersTb
    {
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }        //
        public String ProcessingId { get; set; }
        public int SubRefCode { get; set; }
        public string Name { get; set; }
        //the number of people reffered by benefit claimer
        public int DownlineSize { get; set; }
        public string BenefitCategory { get; set; }
        //this date required for the benefit Claimer to meet target
        public DateTime? MaxDateToMeetTarget { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        //Benefit Application Date
        public DateTime? B_applicationDate { get; set; }
        public string BenefitStage { get; set; }
        //  public string CurrentBenefit { get; set; }
        public string HealthBenefits { get; set; }
        public double CashBenefits { get; set; }
        public double cashBonus { get; set; }
        public double totalCashBenefits { get; set; }
        public Boolean Meet_promo_target { get; set; }
        public string username { get; set; }
        // public DateTime? BenefitClaimabilityDate { get; set; }
        // this will be use to check if claimer's benefit is been process or not
        /* Values includes 
         * 1. Fresh = when a new claim is submitted to admin for processing
         * 2. Pending = when admin mark it for processing later
         * 3. processed = when admin finally process it
         */
        public string BenefitProcessStatus { get; set; }
        public DateTime? ProcessedDate { get; set; }// this the date that the Admin worked on it

        public string SignedBy { get; set; }// this display the name of the Ihealth Staff that attended to this request


    }
    public class UrgentNeedForDrugsTb
    {
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public string DrugName { get; set; }
        public String Manufacturer { get; set; }
        //Drug Buyer/Requestor information
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Phone]
        public string Phone { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public DateTime? RequestedDate { get; set; }
        public int notifyStatus { get; set; }
        public string RequestStatus { get; set; }
        public DateTime? TreatedDate { get; set; }
        public string dosageForm { get; set; }
        public string drugStrength { get; set; }
        public string manufacturerCountry { get; set; }
        public string MoreInformation { get; set; }
        public string landMark { get; set; }
        public byte[] prescription { get; set; }

    }
    public class AdvertManagerTb
    {
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "Advert Owner")]
        public string AdsOwner { get; set; }
        [Display(Name = "Advert Name")]
        public string AdsName { get; set; }
        public byte[] AdsImage { get; set; }
        public string UniqueID { get; set; }
        // this will contain information on which page 
        // the Ads will be displayed
        [Display(Name = "Advert Page")]
        public string Adspage { get; set; }
        //this will contain information on the particular location where the Ads 
        // will be displayed on a spefic page
        [Display(Name = "Advert Location on page")]
        public string AdsLocOnPage { get; set; }
        [Display(Name = "Advert Size")]
        public string AdsSize { get; set; }
        [Display(Name = "Advert Status")]
        public string AdsStatus { get; set; }
        [Display(Name = "Advert Url")]
        public string AdsURL { get; set; }
        [Display(Name = "Date Created")]
        public DateTime? CreatedDate { get; set; }
    }

    public class Career
    {
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "First Name")]
        public string FName { get; set; }
        [Display(Name = "Last Name")]
        public string LName { get; set; }
        [Display(Name = "Name")]
        public string FullName
        {
            get { return FName + " " + LName; }
        }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public byte[] CV { get; set; }
        public string CV_name { get; set; }
        public int CV_size { get; set; }
        public string CV_contentType { get; set; }
        [Display(Name = "Job Position")]
        public string JobTitle { get; set; }
        [Display(Name = "Specialization")]
        public string Specialization { get; set; }
        [Display(Name = "Submited Date")]
        public DateTime? SubmittedDate { get; set; }
        public int notificationStatus { get; set; }
    }
    public class Job
    {
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string JobTitle { get; set; }
        public string Jobdescription { get; set; }
        public string Specialization { get; set; }
        public int Min_Experience { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class Support
    {
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string support_status { get; set; } // this can either be "Fresh" or "Pending" or "Processed"
        public string Support_Requestor_Email { get; set; }
        public string Support_Requestor_Name { get; set; }
        public string Support_Processed_By_Name { get; set; }
        public DateTime? Support_createdDate { get; set; }
        public DateTime? Support_ProcessedDate { get; set; }

        public string Support_Reply_Text_From_Admin { get; set; }

    }
    //Blog Post
    public class blog_Post
    {
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        [Key]
        public int post_id { get; set; }
        public string published_by_Id { get; set; }
        public string post_Title { get; set; }
        public string short_description { get; set; }
        public string post_content { get; set; }
        public string post_tags { get; set; }
        public byte[] Feature_Image { get; set; }
        public string Image_url { get; set; }
        public string video_url { get; set; }
        public string meta_title { get; set; }
        public string meta_description { get; set; }
        public string meta_keyword { get; set; }
        public string post_url { get; set; }
        public int number_of_comment { get; set; }
        public bool allow_comment { get; set; }
        public bool use_video_as_cover_img { get; set; }
        public DateTime? publish_date { get; set; }
        public virtual ICollection<blog_post_comment> blog_post_comment { get; set; }
        [ForeignKey("published_by_Id")]
        public virtual ApplicationUser blog_creator { get; set; }
    }
    public class blog_post_comment
    {
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        [Key]
        public int comment_id { get; set; }
        public int post_id { get; set; }
        public string publish_by_name { get; set; }
        public string publish_by_email { get; set; }
        public string content { get; set; }
        public DateTime? publish_date { get; set; }
        [ForeignKey("post_id")]
        public virtual blog_Post blog_post { get; set; }
        public virtual ICollection<Reply_Comments_tb> Reply_Comments_tb { get; set; }

    }
    public class Reply_Comments_tb
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int reply_id { get; set; }
        public int comment_id { get; set; }
        public string content { get; set; }
        public DateTime? Reply_date { get; set; }
        public string reply_from { get; set; }
        [ForeignKey("comment_id")]
        public virtual blog_post_comment blog_post_comment { get; set; }
    }
    public class AppBookingTb //Appointment Booking table
    {
        public int id { get; set; }
        public int app_id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Appointment_With { get; set; }
        public string Service_Required { get; set; }
        public string Message { get; set; }
        public DateTime? Appointment_Date { get; set; }
        public int notifyStatus { get; set; }
        public string ApprovedStatus { get; set; } //Status: Pending, Approved
        public string AssignedTo { get; set; }
        public DateTime? RescheduledDate { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string ApprovedBy { get; set; }
        public string state { get; set; }
        public string city { get; set; }
    }

    public class InterswitchTransactionsTB
    {
        public int ID { get; set; }
        public string Transaction_Id { get; set; }
        public string Client_Name { get; set; }
        public string Client_Email { get; set; }
        public string Transaction_Amount { get; set; }
        public string Transaction_ResponseCode { get; set; }
        public string Transaction_Response { get; set; }
        public DateTime? Transaction_date { get; set; }
        public string Transaction_purpose { get; set; }
    }
    public class referral_BonusTb
    {
        public int id { get; set; }
        [ForeignKey("ing_user_details")]
        public string user_ID { get; set; }
        public int Downline_Id { get; set; }
        public string Downline_Name { get; set; }
        public double Subscription_Fee { get; set; }
        public double Bonus { get; set; }
        public double totalBonus { get; set; }
        public string Bonus_Type { get; set; }
        public DateTime? Bonus_created_date { get; set; }
        public DateTime? Bonus_Used_Date { get; set; }
        //if the bonus alotted to this member is the bonus acquire during promo period 
        // this column is set to TRUE, else FALSE
        public bool promo_period_bonus { get; set; }

        public virtual INISubcriberExtraDetail ing_user_details { get; set; }
    }
    public class ING_Hospital_List
    {
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Hospital { get; set; }
        public string Address { get; set; }
        public string Plans { get; set; }       
        public string Speciality { get; set; }        
        public string category { get; set; }
        public string reg_no { get; set; }
    }
    public class premium_user
    {
        //Basic info begins
       // [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
       
        [Key]
        public string premiumUserId { get; set; }
        public string referrence_Id { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool is_ING_Member { get; set; }
        public string Gender { get; set; }
        public string Occupation { get; set; }
        public string Nationality { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public DateTime? D_O_B { get; set; }
        public int sponsorID { get; set; }
        //Package Type Begins
        public string plan { get; set; }
        public string Hospital_List { get; set; }
        public DateTime? Register_Date { get; set; }
        //payment informations begins
        public string transaction_Id { get; set; }
        public string paymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string paymentGateway { get; set; }
        public double Amount { get; set; }
        public DateTime? Transaction_date { get; set; }
        public int NotifyStatus { get; set; }
        //iHealth Admin
        public string SignedBy { get; set; }// this display the name of the Ihealth Staff that attended to this request

    }
    public class reviewTb
    {
        public int id { get; set; }
        public int product_or_company_id { get; set; } 
        public string reviewerId { get; set; }
        public string reviewerName { get; set; }
        public string reviewTitle { get; set; }
        public string review_content { get; set; }
        public int ratingValue { get; set; }
        public DateTime? reviewedDate { get; set; }
    }
}
