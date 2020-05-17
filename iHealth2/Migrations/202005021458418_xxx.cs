namespace iHealth2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class xxx : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AppBookingTbs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        app_id = c.Int(nullable: false),
                        Name = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        Appointment_With = c.String(),
                        Service_Required = c.String(),
                        Message = c.String(),
                        Appointment_Date = c.DateTime(),
                        notifyStatus = c.Int(nullable: false),
                        ApprovedStatus = c.String(),
                        AssignedTo = c.String(),
                        RescheduledDate = c.DateTime(),
                        ApproveDate = c.DateTime(),
                        ApprovedBy = c.String(),
                        state = c.String(),
                        city = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Bank_Account_Info",
                c => new
                    {
                        Account_holder_id = c.String(nullable: false, maxLength: 128),
                        BankName = c.String(),
                        AccountName = c.String(),
                        AccountNumber = c.String(),
                        CreatedDate = c.DateTime(),
                        UpdatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Account_holder_id)
                .ForeignKey("dbo.AspNetUsers", t => t.Account_holder_id)
                .Index(t => t.Account_holder_id);
            
            CreateTable(
                "dbo.blog_Post",
                c => new
                    {
                        post_id = c.Int(nullable: false, identity: true),
                        published_by_Id = c.String(maxLength: 128),
                        post_Title = c.String(),
                        short_description = c.String(),
                        post_content = c.String(),
                        post_tags = c.String(),
                        Feature_Image = c.Binary(),
                        Image_url = c.String(),
                        video_url = c.String(),
                        meta_title = c.String(),
                        meta_description = c.String(),
                        meta_keyword = c.String(),
                        post_url = c.String(),
                        number_of_comment = c.Int(nullable: false),
                        allow_comment = c.Boolean(nullable: false),
                        use_video_as_cover_img = c.Boolean(nullable: false),
                        publish_date = c.DateTime(),
                    })
                .PrimaryKey(t => t.post_id)
                .ForeignKey("dbo.AspNetUsers", t => t.published_by_Id)
                .Index(t => t.published_by_Id);
            
            CreateTable(
                "dbo.blog_post_comment",
                c => new
                    {
                        comment_id = c.Int(nullable: false, identity: true),
                        post_id = c.Int(nullable: false),
                        publish_by_name = c.String(),
                        publish_by_email = c.String(),
                        content = c.String(),
                        publish_date = c.DateTime(),
                    })
                .PrimaryKey(t => t.comment_id)
                .ForeignKey("dbo.blog_Post", t => t.post_id, cascadeDelete: true)
                .Index(t => t.post_id);
            
            CreateTable(
                "dbo.Reply_Comments_tb",
                c => new
                    {
                        reply_id = c.Int(nullable: false, identity: true),
                        comment_id = c.Int(nullable: false),
                        content = c.String(),
                        Reply_date = c.DateTime(),
                        reply_from = c.String(),
                    })
                .PrimaryKey(t => t.reply_id)
                .ForeignKey("dbo.blog_post_comment", t => t.comment_id, cascadeDelete: true)
                .Index(t => t.comment_id);
            
            CreateTable(
                "dbo.BusinessImages",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BizId = c.Int(nullable: false),
                        galleryImage = c.Binary(),
                        isSliderImage = c.String(),
                        imageUrl = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.BusinessInfoes", t => t.BizId, cascadeDelete: true)
                .Index(t => t.BizId);
            
            CreateTable(
                "dbo.referral_BonusTb",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        user_ID = c.String(maxLength: 128),
                        Downline_Id = c.Int(nullable: false),
                        Downline_Name = c.String(),
                        Subscription_Fee = c.Double(nullable: false),
                        Bonus = c.Double(nullable: false),
                        totalBonus = c.Double(nullable: false),
                        Bonus_Type = c.String(),
                        Bonus_created_date = c.DateTime(),
                        Bonus_Used_Date = c.DateTime(),
                        promo_period_bonus = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.INISubcriberExtraDetails", t => t.user_ID)
                .Index(t => t.user_ID);
            
            CreateTable(
                "dbo.Careers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FName = c.String(),
                        LName = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                        Gender = c.String(),
                        Nationality = c.String(),
                        State = c.String(),
                        City = c.String(),
                        Address = c.String(),
                        CV = c.Binary(),
                        CV_name = c.String(),
                        CV_size = c.Int(nullable: false),
                        CV_contentType = c.String(),
                        JobTitle = c.String(),
                        Specialization = c.String(),
                        SubmittedDate = c.DateTime(),
                        notificationStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ING_Hospital_List",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        State = c.String(),
                        City = c.String(),
                        Hospital = c.String(),
                        Address = c.String(),
                        Plans = c.String(),
                        Speciality = c.String(),
                        category = c.String(),
                        reg_no = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.InterswitchTransactionsTBs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Transaction_Id = c.String(),
                        Client_Name = c.String(),
                        Client_Email = c.String(),
                        Transaction_Amount = c.String(),
                        Transaction_ResponseCode = c.String(),
                        Transaction_Response = c.String(),
                        Transaction_date = c.DateTime(),
                        Transaction_purpose = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Jobs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        JobTitle = c.String(),
                        Jobdescription = c.String(),
                        Specialization = c.String(),
                        Min_Experience = c.Int(nullable: false),
                        CreatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.premium_user",
                c => new
                    {
                        premiumUserId = c.String(nullable: false, maxLength: 128),
                        referrence_Id = c.String(),
                        First_Name = c.String(),
                        Last_Name = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        is_ING_Member = c.Boolean(nullable: false),
                        Gender = c.String(),
                        Occupation = c.String(),
                        Nationality = c.String(),
                        State = c.String(),
                        City = c.String(),
                        Address = c.String(),
                        D_O_B = c.DateTime(),
                        sponsorID = c.Int(nullable: false),
                        plan = c.String(),
                        Hospital_List = c.String(),
                        Register_Date = c.DateTime(),
                        transaction_Id = c.String(),
                        paymentMethod = c.String(),
                        PaymentStatus = c.String(),
                        paymentGateway = c.String(),
                        Amount = c.Double(nullable: false),
                        Transaction_date = c.DateTime(),
                        NotifyStatus = c.Int(nullable: false),
                        SignedBy = c.String(),
                    })
                .PrimaryKey(t => t.premiumUserId);
            
            CreateTable(
                "dbo.reviewTbs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        product_or_company_id = c.Int(nullable: false),
                        reviewerId = c.String(),
                        reviewerName = c.String(),
                        reviewTitle = c.String(),
                        review_content = c.String(),
                        ratingValue = c.Int(nullable: false),
                        reviewedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Supports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TicketId = c.Int(nullable: false),
                        Subject = c.String(),
                        Content = c.String(),
                        support_status = c.String(),
                        Support_Requestor_Email = c.String(),
                        Support_Requestor_Name = c.String(),
                        Support_Processed_By_Name = c.String(),
                        Support_createdDate = c.DateTime(),
                        Support_ProcessedDate = c.DateTime(),
                        Support_Reply_Text_From_Admin = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AdvertManagerTbs", "AdsURL", c => c.String());
            AddColumn("dbo.BenefitClaimersTbs", "HealthBenefits", c => c.String());
            AddColumn("dbo.BenefitClaimersTbs", "CashBenefits", c => c.Double(nullable: false));
            AddColumn("dbo.BenefitClaimersTbs", "cashBonus", c => c.Double(nullable: false));
            AddColumn("dbo.BenefitClaimersTbs", "totalCashBenefits", c => c.Double(nullable: false));
            AddColumn("dbo.BenefitClaimersTbs", "Meet_promo_target", c => c.Boolean(nullable: false));
            AddColumn("dbo.BenefitClaimersTbs", "username", c => c.String());
            AddColumn("dbo.BusinessInfoes", "Facebook", c => c.String());
            AddColumn("dbo.BusinessInfoes", "Twitter", c => c.String());
            AddColumn("dbo.BusinessInfoes", "LinkedIn", c => c.String());
            AddColumn("dbo.BusinessInfoes", "Google_Plus", c => c.String());
            AddColumn("dbo.BusinessInfoes", "Custom_Url", c => c.String());
            AddColumn("dbo.BusinessInfoes", "Recommended_Status", c => c.String());
            AddColumn("dbo.BusinessInfoes", "RecommendedDate", c => c.DateTime());
            AddColumn("dbo.BusinessInfoes", "isServiceBooster", c => c.String());
            AddColumn("dbo.AspNetUsers", "subscriptionType", c => c.String());
            AddColumn("dbo.AspNetUsers", "Health_Service_Provider", c => c.String());
            AddColumn("dbo.AspNetUsers", "isClientBooster", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "newsletterStatus", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "backgroundinfo", c => c.String());
            AddColumn("dbo.AspNetUsers", "facebookUrl", c => c.String());
            AddColumn("dbo.AspNetUsers", "twitterUrl", c => c.String());
            AddColumn("dbo.AspNetUsers", "linkedinUrl", c => c.String());
            AddColumn("dbo.INISubcriberExtraDetails", "Promotional_Target_Subscription_Status", c => c.String());
            AddColumn("dbo.INISubcriberExtraDetails", "Promo_start_date", c => c.DateTime());
            AddColumn("dbo.INISubcriberExtraDetails", "Promo_end_date", c => c.DateTime());
            AddColumn("dbo.INISubcriberExtraDetails", "promo_dl_size", c => c.Int(nullable: false));
            AddColumn("dbo.INISubcriberExtraDetails", "Non_promo_dl_size", c => c.Int(nullable: false));
            AddColumn("dbo.INISubcriberExtraDetails", "Total_dl_Size", c => c.Int(nullable: false));
            AddColumn("dbo.INISubcriberExtraDetails", "Most_recent_DL_Reg_date", c => c.DateTime());
            AddColumn("dbo.INISubcriberExtraDetails", "My_Sponsor_Referral_Code", c => c.String());
            AddColumn("dbo.INISubcriberExtraDetails", "CurrentBonus", c => c.Double(nullable: false));
            AddColumn("dbo.INISubcriberExtraDetails", "transaction_Id", c => c.String());
            AddColumn("dbo.INISubcriberExtraDetails", "paymentGateway", c => c.String());
            AddColumn("dbo.INISubcriberExtraDetails", "Amounts", c => c.Int(nullable: false));
            AddColumn("dbo.INISubcriberExtraDetails", "Transaction_date", c => c.DateTime());
            AddColumn("dbo.countries", "sortname", c => c.String());
            AddColumn("dbo.countries", "phonecode", c => c.Int(nullable: false));
            AddColumn("dbo.FeedBacks", "subject", c => c.String(nullable: false));
            AddColumn("dbo.ProductsInfoes", "ProductCode", c => c.Int(nullable: false));
            AddColumn("dbo.ProductsInfoes", "Manufacturer", c => c.String());
            AddColumn("dbo.ProductsInfoes", "Pharmaceutic_Drug_Category", c => c.String());
            AddColumn("dbo.ProductsInfoes", "Product_Summary", c => c.String());
            AddColumn("dbo.UrgentNeedForDrugsTbs", "dosageForm", c => c.String());
            AddColumn("dbo.UrgentNeedForDrugsTbs", "drugStrength", c => c.String());
            AddColumn("dbo.UrgentNeedForDrugsTbs", "manufacturerCountry", c => c.String());
            AddColumn("dbo.UrgentNeedForDrugsTbs", "MoreInformation", c => c.String());
            AddColumn("dbo.UrgentNeedForDrugsTbs", "landMark", c => c.String());
            AddColumn("dbo.UrgentNeedForDrugsTbs", "prescription", c => c.Binary());
            AlterColumn("dbo.UrgentNeedForDrugsTbs", "Manufacturer", c => c.String());
            AlterColumn("dbo.UrgentNeedForDrugsTbs", "Email", c => c.String(nullable: false));
            DropColumn("dbo.BenefitClaimersTbs", "CurrentBenefit");
            DropColumn("dbo.AspNetUsers", "How_Do_Hear_About");
            DropColumn("dbo.INISubcriberExtraDetails", "MaxDT2MtTarget");
            DropColumn("dbo.INISubcriberExtraDetails", "MaxDT2MtTarget2");
            DropColumn("dbo.INISubcriberExtraDetails", "MaxDT2MtTarget3");
            DropColumn("dbo.INISubcriberExtraDetails", "MaxDT2MtTarget4");
            DropColumn("dbo.INISubcriberExtraDetails", "MaxDT2MtTarget5");
            DropColumn("dbo.INISubcriberExtraDetails", "SDowlineSize");
            DropColumn("dbo.INISubcriberExtraDetails", "DIncrementDate");
            DropColumn("dbo.INISubcriberExtraDetails", "SSponsorCode");
            DropColumn("dbo.INISubcriberExtraDetails", "CurrentStage");
            DropColumn("dbo.INISubcriberExtraDetails", "BankName");
            DropColumn("dbo.INISubcriberExtraDetails", "AccountName");
            DropColumn("dbo.INISubcriberExtraDetails", "AccountNumber");
            DropColumn("dbo.INISubcriberExtraDetails", "Stage1Benefit");
            DropColumn("dbo.INISubcriberExtraDetails", "Stage2Benefit");
            DropColumn("dbo.INISubcriberExtraDetails", "Stage3Benefit");
            DropColumn("dbo.INISubcriberExtraDetails", "Stage4Benefit");
            DropColumn("dbo.INISubcriberExtraDetails", "Stage5Benefit");
            DropTable("dbo.AdvertManagerViewModels");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.AdvertManagerViewModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AdsOwner = c.String(),
                        AdsName = c.String(),
                        AdsImage = c.Binary(),
                        Adspage = c.String(),
                        AdsLocOnPage = c.String(),
                        AdsSize = c.String(),
                        AdsStatus = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.INISubcriberExtraDetails", "Stage5Benefit", c => c.String());
            AddColumn("dbo.INISubcriberExtraDetails", "Stage4Benefit", c => c.String());
            AddColumn("dbo.INISubcriberExtraDetails", "Stage3Benefit", c => c.String());
            AddColumn("dbo.INISubcriberExtraDetails", "Stage2Benefit", c => c.String());
            AddColumn("dbo.INISubcriberExtraDetails", "Stage1Benefit", c => c.String());
            AddColumn("dbo.INISubcriberExtraDetails", "AccountNumber", c => c.String());
            AddColumn("dbo.INISubcriberExtraDetails", "AccountName", c => c.String());
            AddColumn("dbo.INISubcriberExtraDetails", "BankName", c => c.String());
            AddColumn("dbo.INISubcriberExtraDetails", "CurrentStage", c => c.String());
            AddColumn("dbo.INISubcriberExtraDetails", "SSponsorCode", c => c.String());
            AddColumn("dbo.INISubcriberExtraDetails", "DIncrementDate", c => c.DateTime());
            AddColumn("dbo.INISubcriberExtraDetails", "SDowlineSize", c => c.Int(nullable: false));
            AddColumn("dbo.INISubcriberExtraDetails", "MaxDT2MtTarget5", c => c.DateTime());
            AddColumn("dbo.INISubcriberExtraDetails", "MaxDT2MtTarget4", c => c.DateTime());
            AddColumn("dbo.INISubcriberExtraDetails", "MaxDT2MtTarget3", c => c.DateTime());
            AddColumn("dbo.INISubcriberExtraDetails", "MaxDT2MtTarget2", c => c.DateTime());
            AddColumn("dbo.INISubcriberExtraDetails", "MaxDT2MtTarget", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "How_Do_Hear_About", c => c.String());
            AddColumn("dbo.BenefitClaimersTbs", "CurrentBenefit", c => c.String());
            DropForeignKey("dbo.Bank_Account_Info", "Account_holder_id", "dbo.AspNetUsers");
            DropForeignKey("dbo.referral_BonusTb", "user_ID", "dbo.INISubcriberExtraDetails");
            DropForeignKey("dbo.BusinessImages", "BizId", "dbo.BusinessInfoes");
            DropForeignKey("dbo.Reply_Comments_tb", "comment_id", "dbo.blog_post_comment");
            DropForeignKey("dbo.blog_post_comment", "post_id", "dbo.blog_Post");
            DropForeignKey("dbo.blog_Post", "published_by_Id", "dbo.AspNetUsers");
            DropIndex("dbo.referral_BonusTb", new[] { "user_ID" });
            DropIndex("dbo.BusinessImages", new[] { "BizId" });
            DropIndex("dbo.Reply_Comments_tb", new[] { "comment_id" });
            DropIndex("dbo.blog_post_comment", new[] { "post_id" });
            DropIndex("dbo.blog_Post", new[] { "published_by_Id" });
            DropIndex("dbo.Bank_Account_Info", new[] { "Account_holder_id" });
            AlterColumn("dbo.UrgentNeedForDrugsTbs", "Email", c => c.String());
            AlterColumn("dbo.UrgentNeedForDrugsTbs", "Manufacturer", c => c.String(nullable: false));
            DropColumn("dbo.UrgentNeedForDrugsTbs", "prescription");
            DropColumn("dbo.UrgentNeedForDrugsTbs", "landMark");
            DropColumn("dbo.UrgentNeedForDrugsTbs", "MoreInformation");
            DropColumn("dbo.UrgentNeedForDrugsTbs", "manufacturerCountry");
            DropColumn("dbo.UrgentNeedForDrugsTbs", "drugStrength");
            DropColumn("dbo.UrgentNeedForDrugsTbs", "dosageForm");
            DropColumn("dbo.ProductsInfoes", "Product_Summary");
            DropColumn("dbo.ProductsInfoes", "Pharmaceutic_Drug_Category");
            DropColumn("dbo.ProductsInfoes", "Manufacturer");
            DropColumn("dbo.ProductsInfoes", "ProductCode");
            DropColumn("dbo.FeedBacks", "subject");
            DropColumn("dbo.countries", "phonecode");
            DropColumn("dbo.countries", "sortname");
            DropColumn("dbo.INISubcriberExtraDetails", "Transaction_date");
            DropColumn("dbo.INISubcriberExtraDetails", "Amounts");
            DropColumn("dbo.INISubcriberExtraDetails", "paymentGateway");
            DropColumn("dbo.INISubcriberExtraDetails", "transaction_Id");
            DropColumn("dbo.INISubcriberExtraDetails", "CurrentBonus");
            DropColumn("dbo.INISubcriberExtraDetails", "My_Sponsor_Referral_Code");
            DropColumn("dbo.INISubcriberExtraDetails", "Most_recent_DL_Reg_date");
            DropColumn("dbo.INISubcriberExtraDetails", "Total_dl_Size");
            DropColumn("dbo.INISubcriberExtraDetails", "Non_promo_dl_size");
            DropColumn("dbo.INISubcriberExtraDetails", "promo_dl_size");
            DropColumn("dbo.INISubcriberExtraDetails", "Promo_end_date");
            DropColumn("dbo.INISubcriberExtraDetails", "Promo_start_date");
            DropColumn("dbo.INISubcriberExtraDetails", "Promotional_Target_Subscription_Status");
            DropColumn("dbo.AspNetUsers", "linkedinUrl");
            DropColumn("dbo.AspNetUsers", "twitterUrl");
            DropColumn("dbo.AspNetUsers", "facebookUrl");
            DropColumn("dbo.AspNetUsers", "backgroundinfo");
            DropColumn("dbo.AspNetUsers", "newsletterStatus");
            DropColumn("dbo.AspNetUsers", "isClientBooster");
            DropColumn("dbo.AspNetUsers", "Health_Service_Provider");
            DropColumn("dbo.AspNetUsers", "subscriptionType");
            DropColumn("dbo.BusinessInfoes", "isServiceBooster");
            DropColumn("dbo.BusinessInfoes", "RecommendedDate");
            DropColumn("dbo.BusinessInfoes", "Recommended_Status");
            DropColumn("dbo.BusinessInfoes", "Custom_Url");
            DropColumn("dbo.BusinessInfoes", "Google_Plus");
            DropColumn("dbo.BusinessInfoes", "LinkedIn");
            DropColumn("dbo.BusinessInfoes", "Twitter");
            DropColumn("dbo.BusinessInfoes", "Facebook");
            DropColumn("dbo.BenefitClaimersTbs", "username");
            DropColumn("dbo.BenefitClaimersTbs", "Meet_promo_target");
            DropColumn("dbo.BenefitClaimersTbs", "totalCashBenefits");
            DropColumn("dbo.BenefitClaimersTbs", "cashBonus");
            DropColumn("dbo.BenefitClaimersTbs", "CashBenefits");
            DropColumn("dbo.BenefitClaimersTbs", "HealthBenefits");
            DropColumn("dbo.AdvertManagerTbs", "AdsURL");
            DropTable("dbo.Supports");
            DropTable("dbo.reviewTbs");
            DropTable("dbo.premium_user");
            DropTable("dbo.Jobs");
            DropTable("dbo.InterswitchTransactionsTBs");
            DropTable("dbo.ING_Hospital_List");
            DropTable("dbo.Careers");
            DropTable("dbo.referral_BonusTb");
            DropTable("dbo.BusinessImages");
            DropTable("dbo.Reply_Comments_tb");
            DropTable("dbo.blog_post_comment");
            DropTable("dbo.blog_Post");
            DropTable("dbo.Bank_Account_Info");
            DropTable("dbo.AppBookingTbs");
        }
    }
}
