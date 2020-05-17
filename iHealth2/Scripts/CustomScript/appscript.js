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
               // $("#btnappr").prop('disabled', false)
                $("#btnPend").prop('disabled', false)
                $("#btnDel").prop('disabled', false)
            });
            $(".fa", this).removeClass("fa-square-o").addClass('fa-check-square-o');
        }
        $(this).data("clicks", !clicks);
    });
    //enable check/unchecked for each chechkbox functionally
    $('.mailbox-messages input[type="checkbox"]').on('ifToggled', function (e) {
        if ($('#markedId:checked').length > 0) {
          //  $("#btnappr").prop('disabled', false)
            $("#btnPend").prop('disabled', false)
            $("#btnDel").prop('disabled', false)
        }
        else {
           // $("#btnappr").prop('disabled', true)
            $("#btnPend").prop('disabled', true)
            $("#btnDel").prop('disabled', true)
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
function pendAppointment() {
    dpUI.loading.start("pending Appointment...");
    var array = $.map($('input[name="markedId"]:checked'), function (c) { return c.value; })
    var option = {};
    option.url = "/Appointments/pendapp";
    option.type = "Post";
    option.dataType = "json";
    option.data = { Id: array };
    option.success = function () {
        dpUI.loading.stop();
        //  $('#mytable').load('_NewlyRegBizParital')
        location.reload('#mytable');
    }
    option.error = function () {
        dpUI.loading.stop();
        swal("", "Error", "error")
    }
    $.ajax(option);
}
//function approveAppointment() {
//    dpUI.loading.start("Approving Appointment...");
//    var array = $.map($('input[name="markedId"]:checked'), function (c) { return c.value; })
//    var option = {};
//    option.url = "/Appointments/approveapp";
//    option.type = "Post";
//    option.dataType = "json";
//    option.data = { Id: array };
//    option.success = function () {
//        dpUI.loading.stop();
//        //  $('#mytable').load('_NewlyRegBizParital')
//        location.reload('#mytable');
//    }
//    option.error = function () {
//        dpUI.loading.stop();
//        swal("", "Error", "error")
//    }
//    $.ajax(option);
//}
function delAppointment() {
    swal({
        title: "Are you sure?",
        text: "You will not be able to recover this comment if deleted",
        type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes, delete it!", cancelButtonText: "No, cancel it!",
        closeOnConfirm: false, closeOnCancel: false
    }, function (isConfirm) {
        if (isConfirm) {
            dpUI.loading.start("Deleting Appointment...");
            var array = $.map($('input[name="markedId"]:checked'), function (c) { return c.value; })
            var option = {};
            option.url = "/Appointments/delapp";
            option.type = "Post";
            option.dataType = "json";
            option.data = { id: array };
            option.success = function () {
                dpUI.loading.stop();
                swal("Deleted!", "Business has been successfully deleted.", "success");
                location.reload();
            }
            option.error = function () {
                dpUI.loading.stop();
                swal("Error", "There is an error deleting the selected business", "error")
            }
            $.ajax(option);

        }
        else {
            swal("Cancelled", "Delete Operation has been Cancel)", "error");
        }
    });
}