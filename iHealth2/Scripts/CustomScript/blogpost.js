$(document).ready(function () {
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
                $("#btndel1").prop('disabled', false)
                $("#btndel2").prop('disabled', false)
            });
            $(".fa", this).removeClass("fa-square-o").addClass('fa-check-square-o');
        }
        $(this).data("clicks", !clicks);
    });
    //enable check/unchecked for each chechkbox functionally
    $('.mailbox-messages input[type="checkbox"]').on('ifToggled', function (e) {
        if ($('#delid:checked').length > 0) {
            $("#btndel1").prop('disabled', false)
            $("#btndel2").prop('disabled', false)
        }
        else {
            $("#btndel1").prop('disabled', true)
            $("#btndel2").prop('disabled', true)
        }

    })

});

$('#table').bootstrapTable({

    striped: true,
    // cardView: true,
    pagination: true,
    pageSize: 15,
    pageList: [15, 25, 50, 100, 200],
    smartDisplay: false,
    showToggle: true,
    search: true,
    showColumns: true,
    showRefresh: true,
    //    border : true
    //align: 'center',
    //valign: 'bottom'

});
function delete_blog_post() {
    swal({
        title: "Are you sure?",
        text: "You will not be able to recover this post if deleted",
        type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes, delete it!", cancelButtonText: "No, cancel it!",
        closeOnConfirm: false, closeOnCancel: false
    }, function (isConfirm) {
        if (isConfirm) {
            dpUI.loading.start("Deleting...");
            var array = $.map($('input[name="delid"]:checked'), function (c) { return c.value; })
            var option = {};
            option.url = "/blog/Delete_Post";
            option.type = "Post";
            option.dataType = "json";
            option.data = { Id: array };
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
function delete_blog_comment() {
    swal({
        title: "Are you sure?",
        text: "You will not be able to recover this comment if deleted",
        type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes, delete it!", cancelButtonText: "No, cancel it!",
        closeOnConfirm: false, closeOnCancel: false
    }, function (isConfirm) {
        if (isConfirm) {
            dpUI.loading.start("Deleting...");
            var array = $.map($('input[name="delid"]:checked'), function (c) { return c.value; })
            var option = {};
            option.url = "/blog/Delete_Comment";
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