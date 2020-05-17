    function approvebiz() {
        dpUI.loading.start("Approving...");
        var array = $.map($('input[name="processId"]:checked'), function (c) { return c.value; })
        var option = {};
        option.url = "/Company/Approve";
        option.type = "Post";
        option.dataType = "json";
        option.data = { Id: array };
        option.success = function () {
            dpUI.loading.stop();
            swal({ title: "Alert", text: "Business Approved succesfully", timer: 3000, showConfirmButton: false });
            //  $('#mytable').load('_NewlyRegBizParital')
            location.reload('#mytable');

        }
        option.error = function (xhr,statusText) {
            dpUI.loading.stop();
            alert("Error: " + xhr.status + ": " + xhr.statusText);
        }
        $.ajax(option);
      

    }

    function deletebiz(id) {
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
                option.url = "/Company/Delete";
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
    function VerifyBiz(id) {
        dpUI.loading.start("Verifying...");
        var array = $.map($('input[name="processId"]:checked'), function (c) { return c.value; })
        var option = {};
        option.url = "/Company/VerifyCom";
        option.type = "Post";
        option.dataType = "json";
        option.data = { Id: array };
        option.success = function () {
            dpUI.loading.stop();
            //swal("", "", "success");
            swal({ title: "Alert", text: "Business verified succesfully", timer: 3000, showConfirmButton: false },"success");
            //  $('#mytable').load('_NewlyRegBizParital')
            location.reload();

        }
        option.error = function () {
            dpUI.loading.stop();
            swal("", "Error", "error")
        }
        $.ajax(option);
    }
    function RecommendBiz() {
        dpUI.loading.start("Recommending...");
        var array = $.map($('input[name="processId"]:checked'), function (c) { return c.value; })
        var option = {};
        option.url = "/Company/Mark_as_recommended";
        option.type = "Post";
        option.dataType = "json";
        option.data = { Id: array };
        option.success = function () {
            dpUI.loading.stop();
            //swal("", "", "success");
            swal({ title: "Alert", text: "Business or service has been marked as recommended succesfully", timer: 3000, showConfirmButton: false }, "success");
            //  $('#mytable').load('_NewlyRegBizParital')
            location.reload();

        }
        option.error = function () {
            dpUI.loading.stop();
            swal("", "Error", "error")
        }
        $.ajax(option);
    }

    function initTb() {
        $('#table').bootstrapTable({

            striped: true,
            // cardView: true,
            pagination: true,
            pageSize: 10,
            pageList: [10, 25, 50, 100, 200],
            smartDisplay: false,
            showToggle: true,
            search: true,
            showColumns: true,
            showRefresh: true,
            border: true,
            align: 'center',
            valign: 'bottom'

        });
    }

