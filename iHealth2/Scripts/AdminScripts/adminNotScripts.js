
    $(document).ready(function () {
        //New User Notif Begins
        NewBuserNotif();
        NewINGuserNotif();
        NewPremuserNotif();
        getTUsersize();
        getNewCompanyProductSize();
        getNewProducts();
        getNewCompany();
        getFreshClaims();
        getContactus();
        getReportFraud();
        getFeedbackSize();
        getSuprtReqstSize();
        updateINGpromoPeriodStatus();
        //New User Notif Ends
    });
setInterval(getTUsersize, 300000);
setInterval(NewINGuserNotif, 300000);
setInterval(NewBuserNotif, 300000);
setInterval(NewPremuserNotif, 300000);
setInterval(getNewCompanyProductSize, 300000);
setInterval(getNewProducts, 300000);
setInterval(getNewCompany, 300000);
setInterval(getFreshClaims, 300000);
setInterval(getContactus, 300000);
setInterval(getReportFraud, 300000);
setInterval(getFeedbackSize, 300000)
setInterval(getSuprtReqstSize, 300000)
setInterval(updateINGpromoPeriodStatus, 100000)
function getTUsersize() {
                
    $.ajax({
        url: "/Admin/getTUsersize",
        cache: false,
        success: function (data) {
            $("#userSize,#userSize1").html(data);
            $("#userSizeHeader").html("You have " + data + " New Users/Subscibers");
        }
    })
}
function getNewCompanyProductSize() {
    $.ajax({
        url: "/Admin/getNewCompanyProductSize",
        cache: false,
        success: function (data) {
            $("#getNewCompanyProductSize").html(data);
            $("#getNewCompanyProductSizeHeader").html("You have " + data + " Newly Products/Companies");
        }
    })
}
function NewBuserNotif() {
    $.ajax({
        url: "/Admin/NewBuserNotif",
        cache: false,
        success: function (data) {
            if (data != ""){
                $("#NewBuserNotif1").html(data);
                $("#NewBuserNotif").empty();
                $("<a href=\"/Admin/NewBusers\"><i class=\"fa fa-user info\"></i>" + data + " New Normal User(s)</a>").appendTo('#NewBuserNotif')

            }
            else
            {
                $("#NewBuserNotif1").hide();
                $("#NewBuserNotif").empty();
            }
                        
        }
    })
}
function NewINGuserNotif()
{
    $.ajax({
        url: "/Admin/NewINGuserNotif",
        cache: false,
        success: function (data) {
            if (data !="") {
                $("#NewINGuserNotif1").html(data);
                $("#NewINGuserNotif").empty()
                $("<a href=\"/Admin/NewINIsubsciber\"><i class=\"fa fa-user info\"></i>" + data + " New ING Subscriber(s)</a>").appendTo('#NewINGuserNotif')
            }
            else {
                $("#NewINGuserNotif1").hide();
                $("#NewINGuserNotif").empty()
            }
        }
    })
}
function NewPremuserNotif() {
    $.ajax({
        url: "/Admin/NewPremuserNotif",
        cache: false,
        success: function (data) {
            if (data != "") {
                $("#NewPremuserNotif1").html(data);
                $("#NewPremuserNotif").empty()
                $("<a href=\"/Premium/NewPremiumUser\"><i class=\"fa fa-user info\"></i>" + data + " New Premium Subscriber(s)</a>").appendTo('#NewPremuserNotif')
            }
            else {
                $("#NewPremuserNotif1").hide();
                $("#NewPremuserNotif").empty()
            }
        }
    })
}

function getNewProducts(){
    $.ajax({
        url: "/Admin/getNewProducts",
        cache: false,
        success: function (data) {
            if (data != "") {
                $("#getNewProducts1,#getNewProducts2").html(data);
                $("#getNewProducts").empty();
                $("<a href=\"/Products/NewlyRegisteredProducts\"><i class=\"fa fa-circle info\"></i>" + data + " New Product(s)</a>").appendTo('#getNewProducts')

            }
            else {
                $("#getNewProducts1,#getNewProducts2").hide();
                $("#getNewProducts").empty();
            }
        }
    })
}
function getNewCompany(){
    $.ajax({
        url: "/Admin/getNewCompany",
        cache: false,
        success: function (data) {
            if (data != "") {
                $("#getNewCompany1,#getNewCompany2").html(data);
                $("#getNewCompany").empty();
                $("<a href=\"/Company/NewlyRegBusiness\"><i class=\"fa fa-building-o info\"></i>" + data + " New Company/Business(es)</a>").appendTo('#getNewCompany')

            }
            else {
                $("#getNewCompany1,#getNewCompany2").hide();
                $("#getNewCompany").empty();
            }
        }
    })
}
function getFreshClaims()
{
    $.ajax({
        url: "/Admin/getFreshClaims",
        cache: false,
        success: function (data) {
            if (data != "") {
                $("#getFreshClaimSize").html(data);
                $("#getFreshClaimSizeHeader").html("You have " + data + " New Benefit Claims Request");
                $("#getFreshClaims1,#getFreshClaims2").html(data);
                $("#getFreshClaims").empty();
                $("<a href=\"/Admin/FreshBenefitClaims\"><i class=\"glyphicon glyphicon-gift info\"></i>" + data + " New Benefit Claims Request</a>").appendTo('#getFreshClaims')
            }
            else {
                $("#getFreshClaims1,#getFreshClaims2").hide();
                $("#getFreshClaims").empty();
            }
        }
    })
}
function getFeedbackSize() {
    $.ajax({
        url: "/Admin/getFeedbackSize",
        cache: false,
        success: function (data) {
            if (data != "") {
                $("#getFeedbackSize,#getFeedbackSizeBadge").html(data);
                $("#getFeedbackSizeHeader").html("You have " + data + " new messsage(s)");
            }
            else {
                $("#getFeedbackSize,#getFeedbackSizeBadge").hide();
            }
           //
        }
    })
}
function getSuprtReqstSize() {
    $.ajax({
        url: "/Admin/getSupportRequestSize",
        cache: false,
        success: function (data) {
            if (data != "") {
                $("#getSuprtReqstSize1,#getSuprtReqstSize2").html(data);
                $("#getSuprtReqst").empty();
                $("<a href=\"/support/View_Support_Request/?type=F\"><div class=\"pull-left\"><img src=\"/Content/Logo/ihealthlogo.png\" class=\"img-circle\" alt=\"user image\" /></div>" + data + " new support request</a>").appendTo('#getSuprtReqst')
            }
        }
    })
}
function getContactus() {
    $.ajax({
        url: "/Admin/getContactusJson",
        cache: false,
        success: function (data) {
            if (data != "") {
                // alert(data)
                $("#getContactus1").html(data);
                $("#getContactus").empty();
                $("<a href=\"/Admin/GetContactUs\"><div class=\"pull-left\"><img src=\"/Content/Logo/ihealthlogo.png\" class=\"img-circle\" alt=\"user image\" /></div>" + data + " new contact us messsage(s)</a>").appendTo('#getContactus')
            }
            else {
                $("#getContactus1").hide();
                $("#getContactus").empty();
                //  alert("empty result")
            }
        }
    })
}
function getReportFraud() {
    $.ajax({
        url: "/Admin/getReportFraudJson",
        cache: false,
        success: function (data) {
            if (data != "") {
                // alert(data)
                $("#getReportFraud1").html(data);
                $("#getReportFraud").empty();
                $("<a href=\"/Admin/getFraudReport\"><div class=\"pull-left\"><img src=\"/Content/Logo/ihealthlogo.png\" class=\"img-circle\" alt=\"user image\" /></div>" + data + " new fraud report(s)</a>").appendTo('#getReportFraud')
            }
            else {
                $("#getReportFraud1").hide();
                $("#getReportFraud").empty();
                //  alert("empty result")
            }
        }
    })
}
function updateINGpromoPeriodStatus() {
    $.ajax({
        url: "/Home/UpdateINGsubscriberPromoStatus",
        cache: false,
        success: function (data) {
            console.log(data)
        },
        error: function(data){
            console.log(data)
    }
    })

}
