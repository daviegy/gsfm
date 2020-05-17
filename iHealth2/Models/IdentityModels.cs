using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;

namespace iHealth2.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;

        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName {
            get { return FirstName + " " + LastName; }  
        }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Profession { get; set; }
        
        public Nullable<DateTime> RegDate { get; set; }
        public int NotifyStatus { get; set; }
        public int MyRefferalCode { get; set; }
        public byte[] Photo { get; set; }
       // public string How_Do_Hear_About { get; set; }
        public string Refferal_Url { get; set; }
        public string AreaOfSpecialization { get; set; }// Area of specializations
        public string UserType { get; set; }// this will help know which type of user
                                            // registered on IHealth {e.g. Normal Business User or INI Subcriber}
        public string subscriptionType { get; set; }
        public string Health_Service_Provider { get; set; }
        public bool isClientBooster { get; set; }
        public int newsletterStatus { get; set; } 
        public virtual ICollection<BusinessInfo> Bussinessinfo { get; set; }
        public virtual ICollection<blog_Post> blog_post { get; set; }
        public virtual Bank_Account_Info bank_acct_info { get; set; }
        //Normal Business User Additional Info
        public virtual IHealthUsersMLM IHealthUsersMLM { get; set; }
        // Ihealth Networking Group User Additional information
        public virtual INISubcriberExtraDetail INISubcriberExtraDetails { get; set; }
        
        public string backgroundinfo { get; set; }

        public string facebookUrl { get; set; }

        public string twitterUrl { get; set; }

        public string linkedinUrl { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("ihealthCon", throwIfV1Schema: false)
        {
            var ojectTimeOut = (this as IObjectContextAdapter).ObjectContext;
            ojectTimeOut.CommandTimeout = 180;

        }
        public DbSet<country> country { get; set; }
        public DbSet<state> State { get; set; }
        public DbSet<city> City { get; set; }
        public DbSet<Category> category { get; set; }
        public DbSet<SubCategory1> SubCategory1 { get; set; }
        public DbSet<SubCategory2> SubCategory2 { get; set; }
        public DbSet<BusinessInfo> BusinessInfoes { get; set; }
        public DbSet<status> Status { get; set; }
        public DbSet<IHealthUsersMLM> IHealthUsersMLM { get; set; }
        public DbSet<LatestNews> News { get; set; }
        public DbSet<ProductsInfo> ProductTb { get; set; }
        public DbSet<productCategory> productCategory { get; set; }
        public DbSet<autoSearchTerm> SearchTermModel { get; set; }
        public DbSet<FeedBack> Feedback { get; set; }
        public DbSet<Professions> Profession { get; set; }
        public DbSet<INISubcriberExtraDetail> INISubcriberExtraDetails { get; set; }
        public DbSet<BenefitClaimersTb> BenefitClaimersTb { get; set; }
        public DbSet<UrgentNeedForDrugsTb> urgentNeedforDrugsTb { get; set; }
        public DbSet<AdvertManagerTb> AdvertManagerTb { get; set; }
        public DbSet<Career> career{ get; set; }
        public DbSet<Job> jobs { get; set; }
        public DbSet<Support> supports { get; set; }
        public DbSet<blog_Post> blog_posts { get; set; }
        public DbSet<blog_post_comment> blog_post_comments { get; set; }
        public DbSet<Reply_Comments_tb> reply_comments_tb { get; set; }
        public DbSet<BusinessImages> businessImage { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public DbSet<AppBookingTb> AppBookingTb { get; set; }
        public DbSet<InterswitchTransactionsTB> InterswtichTransactionsTables { get; set; }
        public DbSet<referral_BonusTb> referral_bonus_tb { get; set; }
        public DbSet<Bank_Account_Info> bank_info { get; set; }
        public DbSet<ING_Hospital_List> ING_Hospital_List { get; set; }
        public DbSet<premium_user> premium_user { get; set; }
        public DbSet<reviewTb> reviewTb { get; set; }
        //public System.Data.Entity.DbSet<iHealth2.Models.ApplicationUser> ApplicationUsers { get; set; }
        // public DbSet<Health_Insurance_Benefit> Health_Insurance_Benefit { get; set; }
        //    public System.Data.Entity.DbSet<iHealth2.Models.Hospital_list_viewModel> Hospital_list_viewModel { get; set; }
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<ApplicationUser>()
        //        .HasOptional(u => u.INISubcriberExtraDetails)
        //        .WithMany()
        //        .WillCascadeOnDelete(true);
        //    base.OnModelCreating(modelBuilder);
        //}
    }
}