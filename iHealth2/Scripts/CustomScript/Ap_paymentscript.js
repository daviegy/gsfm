$(document).ready(function (e) {

    //Enable iCheck plugin for checkboxes
    //iCheck for checkbox and radio inputs
    var $b = $('input[type = "checkbox"]');
    $('.mailbox-messages input[type="checkbox"]').iCheck({
        checkboxClass: 'icheckbox_flat-blue',
        radioClass: 'iradio_flat-blue'
    });
    //Enable check and uncheck all functionality
    $(".checkbox-toggle").click(function () {
        var clicks = $(this).data('clicks');
        if (clicks) {
            //Uncheck all checkboxes
            //  alert("i just got clicked")
            $(".mailbox-messages input[type='checkbox']").iCheck("uncheck");
            $(".fa", this).removeClass("fa-check-square-o").addClass('fa-square-o');

        } else {
            //Check all checkboxes
            $(".mailbox-messages input[type='checkbox']").iCheck("check", function () {
                $("#submitbtn").prop('disabled', false)
            });
            $(".fa", this).removeClass("fa-square-o").addClass('fa-check-square-o');
        }
        $(this).data("clicks", !clicks);
    });
    //enable check/unchecked for each chechkbox functionally
    $('.mailbox-messages input[type="checkbox"]').on('ifToggled', function (e) {
        if ($('#markAsPaidbyId:checked').length > 0) {
            $("#submitbtn").prop('disabled', false)
        }
        else {
            $("#submitbtn").prop('disabled', true)

        }

    })
});
$('#table').bootstrapTable({

    striped: true,
    // cardView: true,
    pagination: true,
    pageSize: 15,
    pageList: [15, 30, 50, 100, 200],
    smartDisplay: false,
    showToggle: true,
    search: true,
    showColumns: true,
    showRefresh: true,
    //    border : true
    //align: 'center',
    //valign: 'bottom'

});
//Approve pmt for ing
function app_pmt_for_ING_user() {
   
    swal({
        title: "Are you sure?",
        text: "You want to confirm this payment(s), once payment(s) is approved it can not be reverse.",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn btn-warning",
        confirmButtonText: "Yes, approve it!",
        cancelButtonText: "No",
        closeOnConfirm: false,
        closeOnCancel: false,
        showLoaderOnConfirm: true
    },
function (isConfirm) {
    if (isConfirm) {
       
        var id = [];
        $("input[name = 'ItemId']:checked").each(function () {
            id.push($(this).val())
        });

        $.ajax({
            url: '/Admin/MarkAsPaid',
            data: {
                markAsPaidbyId: id
            },
            cache: false,
            type: "POST",
           // timeout: 10000,
            dataType: "json",
            success: function (result) {
                if (result == true) {

                    swal("Confirmed!", "Payment is confirmed successfully", "success");
                    window.location.reload();
                }
                else {
                    swal("Error", "There is an error confirming payment", "error")
                    window.location.reload();
                }
            },
            error: function (xhr, result) {

                swal("Error", "There is an error confirming payment", "error")
                window.location.reload();
            }
        });

    } else {
        swal("Cancelled", "Payment not approved", "error");
    }
}

    );
}
//approve pmt for prem
function app_pmt_for_Premium_user() {
    swal({
        title: "Are you sure?",
        text: "You want to confirm this payment(s), once payment(s) is approved it can not be reverse.",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Yes, approve it!",
        cancelButtonText: "No",
        closeOnConfirm: false,
        closeOnCancel: false,
        showLoaderOnConfirm: true
    },
function (isConfirm) {
    if (isConfirm) {
        //alert("ss")
        var id = [];
        $("input[name = 'ItemId']:checked").each(function () {
            id.push($(this).val())
        });
        //alert(id)
        $.ajax({
            url: '/Premium/ApprovePayment',
            data: {
                markAsPaidbyId: id
            },
            cache: false,
            type: "POST",
           // timeout: 10000,
            dataType: "json",
            success: function (result) {
                if (result == true) {

                    swal("Confirmed!", "Payment is confirmed successfully", "success");
                    location.reload('#mytable');
                }
                else {
                    swal("Error", "There is an error confirming payment", "error")
                    location.reload('#mytable');
                }
            },
            error: function (xhr, result) {

                swal("Error", "", "error")
                location.reload('#mytable');
            }
        });

    } else {
        swal("Cancelled", "Payment not approved", "error");
    }
}

    );

}
