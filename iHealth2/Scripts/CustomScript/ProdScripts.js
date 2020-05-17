function approvebiz() {
    dpUI.loading.start("Approving...");
    var array = $.map($('input[name="processId"]:checked'), function (c) { return c.value; })
    var option = {};
    option.url = "/Products/Approve";
    option.type = "Post";
    option.dataType = "json";
    option.data = { Id: array };
    option.success = function () {
        dpUI.loading.stop();
        swal("", "Approved", "success");
        //  $('#mytable').load('_NewlyRegBizParital')
        location.reload('#mytable');

    }
    option.error = function (data) {
        dpUI.loading.stop();
        swal("", "Error", "error")
    }
    $.ajax(option);
}

function deletebiz() {
    swal({
        title: "Are you sure?",
        text: "You will not be able to recover this data if deleted",
        type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes, delete it!", cancelButtonText: "No, cancel it!",
        closeOnConfirm: false, closeOnCancel: false
    }, function (isConfirm) {
        if (isConfirm) {
            dpUI.loading.start("Deleting...");
            var array = $.map($('input[name="processId"]:checked'), function (c) { return c.value; })
            var option = {};
            option.url = "/Products/Delete";
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

function VerifyBiz() {
    var array = $.map($('input[name="processId"]:checked'), function (c) { return c.value; })
    dpUI.loading.start("Verifying...");
    var option = {};
    option.url = "/Products/VerifyProduct";
    option.type = "Post";
    option.dataType = "json";
    option.data = { Id: array };
    option.success = function () {
        dpUI.loading.stop();
        swal("", "Verified", "success");
        //  $('#mytable').load('_NewlyRegBizParital')
        location.reload();

    }
    option.error = function () {
        dpUI.loading.stop();
        swal("", "Error", "error")
    }
    $.ajax(option);
}
