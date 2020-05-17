$(document).ready(function () {
    $('#example').on('init.dt', function () {
        //Enable iCheck plugin for checkboxes
        //iCheck for checkbox and radio inputs
        var $b = $('input[type = "checkbox"]');
        $('.mailbox-messages input[type="checkbox"]').iCheck({
            checkboxClass: 'icheckbox_flat-green',
            radioClass: 'iradio_flat-green'
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
            if ($('#markImgId:checked').length > 0) {
                $("#btnDel").prop('disabled', false)
                $("#btnFeatured").prop('disabled', false)
                $("#btnDel1").prop('disabled', false)
                $("#btnFeatured1").prop('disabled', false)
                $("#btnUnFeatured1").prop('disabled', false)
                $("#btnUnFeatured").prop('disabled', false)
            }
            else {
                $("#btnFeatured").prop('disabled', true)
                $("#btnDel").prop('disabled', true)
                $("#btnFeatured1").prop('disabled', true)
                $("#btnDel1").prop('disabled', true)
                $("#btnUnFeatured").prop('disabled', true)
                $("#btnUnFeatured1").prop('disabled', true)
            }

        })
    }).DataTable({
        "ordering": false,
        "info": false,
        "searching": false,
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        "stateSave": true
    });
});
$(document).ready(function () {
    $('#example').on('page.dt', function () {
        //Enable iCheck plugin for checkboxes
        //iCheck for checkbox and radio inputs
        var $b = $('input[type = "checkbox"]');
        $('.mailbox-messages input[type="checkbox"]').iCheck({
            checkboxClass: 'icheckbox_flat-green',
            radioClass: 'iradio_flat-green'
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
            if ($('#markImgId:checked').length > 0) {
                $("#btnDel").prop('disabled', false)
                $("#btnFeatured").prop('disabled', false)
                $("#btnDel1").prop('disabled', false)
                $("#btnFeatured1").prop('disabled', false)
                $("#btnUnFeatured").prop('disabled', false)
                $("#btnUnFeatured1").prop('disabled', false)
            }
            else {
                $("#btnFeatured").prop('disabled', true)
                $("#btnDel").prop('disabled', true)
                $("#btnFeatured1").prop('disabled', true)
                $("#btnDel1").prop('disabled', true)
                $("#btnUnFeatured").prop('disabled', true)
                $("#btnUnFeatured1").prop('disabled', true)
            }

        })

    });
    $('#example').on('length.dt', function (e, settings, len) {
        //Enable iCheck plugin for checkboxes
        //iCheck for checkbox and radio inputs
        var $b = $('input[type = "checkbox"]');
        $('.mailbox-messages input[type="checkbox"]').iCheck({
            checkboxClass: 'icheckbox_flat-green',
            radioClass: 'iradio_flat-green'
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
            if ($('#markImgId:checked').length > 0) {
                $("#btnDel").prop('disabled', false)
                $("#btnFeatured").prop('disabled', false)
                $("#btnDel1").prop('disabled', false)
                $("#btnFeatured1").prop('disabled', false)
                $("#btnUnFeatured").prop('disabled', false)
                $("#btnUnFeatured1").prop('disabled', false)
            }
            else {
                $("#btnFeatured").prop('disabled', true)
                $("#btnDel").prop('disabled', true)
                $("#btnFeatured1").prop('disabled', true)
                $("#btnDel1").prop('disabled', true)
                $("#btnUnFeatured").prop('disabled', true)
                $("#btnUnFeatured1").prop('disabled', true)
            }

        })
    });
});
$("#galleryImg").fileinput({
    uploadUrl: "/",
    autoReplace: true,
    //uploadAsync: true,
    allowedFileExtensions: ["jpg", "png", "gif"],
    removeIcon: '<i class="icon icon icon-trash text-danger"></i>',
    removeClass: 'btn btn-danger',
    removeTitle: 'Remove file',
    uploadIcon: '<i class="icon icon-upload text-info"></i>',
    uploadClass: 'btn btn-xs btn-success',
    uploadTitle: 'Upload Image',
    maxFileSize: 2000,
    showCaption: false,
    showUpload: false,
});
var url = "/Business/getState";
$("#Country").change(function () {
    var CnVal;
    if ($("#Country").val() != "") {
        CnVal = $("#Country").val();
    }
    else {
        CnVal = 0;
    }
    var options = {};
    options.url = "/Business/getState",
    options.type = "Post";
    options.dataType = "json";
    options.data = JSON.stringify({ Id: CnVal });
    options.contentType = "application/json";
    options.success = function (State) {
        $("#state").empty()

        if (State.length > 0) {
            $("#state").prop("required", true)
            $("#state").append('<option value="">Select State</option>')
            $.each(State, function (i, State) {
                $("#state").append('<option value="' + State.Value + '">' +
    State.Text + '</option>')
            });
            $("#state").prop("disabled", false)
        }
        else {
            $("#state").append('<option value="">Select State</option>')

        }

    }

    $.ajax(options);
});


function delImg(id) {
    swal({
        title: "Are you sure?",
        text: "You will not be able to recover this data if deleted",
        type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes, delete it!", cancelButtonText: "No, cancel it!",
        closeOnConfirm: false, closeOnCancel: false
    }, function (isConfirm) {
        if (isConfirm) {
            dpUI.loading.start("Deleting...");
            var array = $.map($('input[name="markImgId"]:checked'), function (c) { return c.value; })
            var option = {};
            option.url = "/Business/delImg";
            option.type = "Post";
            option.dataType = "json";
            option.data = { Id: array };
            option.success = function () {
                dpUI.loading.stop();
                swal("Deleted!", "Image has been successfully deleted.", "success");
                location.reload();
            }
            option.error = function () {
                dpUI.loading.stop();
                swal("Error", "There is an error deleting the selected Image(s)", "error")
            }
            $.ajax(option);

        }
        else {
            swal("Cancelled", "Delete Operation has been Cancel)", "error");
        }
    });
}
function featuredImg(id) {
    dpUI.loading.start("Loading...");
    var array = $.map($('input[name="markImgId"]:checked'), function (c) { return c.value; })
    if (array.length <= 4) {
        var option = {};
        option.url = "/Business/featuredImg";
        option.type = "Post";
        option.dataType = "json";
        option.data = { Id: array };
        option.success = function (data) {
            if (data == true) {
                dpUI.loading.stop();
                swal({ title: "Alert", text: "Featured Image set succesfully", timer: 3000, showConfirmButton: false });
                //  $('#mytable').load('_NewlyRegBizParital')
                location.reload();
            }
            else {
                dpUI.loading.stop();
                swal(
'Oops...',
'Maximum four(4) Images can be set as featured image',
'error'
)
                //  $('#mytable').load('_NewlyRegBizParital')
                //location.reload();
            }


        }
        option.error = function (xhr, statusText) {
            dpUI.loading.stop();
            alert("Error: " + xhr.status + ": " + xhr.statusText);
        }
        $.ajax(option);
    }
    else {
        swal(
'Oops...',
'Maximum four(4) Images can be set as featured image',
'error'
)

        dpUI.loading.stop();
    }



}
function unfeaturedImg(id) {
    dpUI.loading.start("Loading...");
    var array = $.map($('input[name="markImgId"]:checked'), function (c) { return c.value; })
    var option = {};
    option.url = "/Business/unfeaturedImg";
    option.type = "Post";
    option.dataType = "json";
    option.data = { Id: array };
    option.success = function () {
        dpUI.loading.stop();
        swal({ title: "Alert", text: "Featured Image set succesfully", timer: 3000, showConfirmButton: false });
        //  $('#mytable').load('_NewlyRegBizParital')
        location.reload();

    }
    option.error = function (xhr, statusText) {
        dpUI.loading.stop();
        alert("Error: " + xhr.status + ": " + xhr.statusText);
    }
    $.ajax(option);

}
