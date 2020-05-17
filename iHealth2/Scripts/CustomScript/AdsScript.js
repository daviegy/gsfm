
    $("#AdsImage").fileinput({
        uploadUrl: "/",
        autoReplace: true,
        maxFileCount: 1,
        allowedFileExtensions: ["jpg", "png", "gif"],
        removeIcon: '<i class="icon icon icon-trash text-danger"></i>',
        removeClass: 'btn  btn-danger',
        removeTitle: 'Remove file',
        uploadIcon: '<i class="icon icon-upload text-info"></i>',
        uploadClass: 'btn btn-xs btn-success',
        uploadTitle: 'Upload file',
        maxFileSize: 1000,
        showCaption: false,
        showUpload: false,
        resizeImage: false,
        showPreview:true
    
    })
    $(function () {
        $('#btnSubmit').bind("click", function () {
            var p = document.getElementById('AdsImage').value;

            if (p.length < 1) {
                swal({
                    title: "Required!!!", text: "<b>You must Upload an Advert Image</b>",
                    type: "error",
                    confirmButtonText: "OK",
                    html: true
                });
                //alert("error");
                return false;

            }

        });
    });



function ReactivateAds() {
    var id = [];
    $("input[name = 'ItemId']:checked").each(function () {
        id.push($(this).val())
    });
    dpUI.loading.start("Reactivating...");
    $.ajax({
        url: '/SuperAdm/ReactivateAds',
        data: {
            MarkAsReactivateId: id.join(",")
        },
        cache: false,
        type: "POST",
        timeout: 10000,
        dataType: "json",
        success: function (result) {
            if (result == true) {
                dpUI.loading.stop();
                //   swal("Reactivated", "Advert has been successfully reactivated.", "success");
                window.location.reload();
            }
            else {
                dpUI.loading.stop();
                //  swal("Reactivated", "Advert has been successfully reactivated.", "error");
                window.location.reload();
            }

        },
        error: function (xhr, result) {
            dpui.loading.stop();
            alert("error: " + xhr.status + ": " + xhr.result);
        }
    });
}

function DeleteAds() {
    swal({
        title: "Are you sure?",
        text: "You will not be able to recover this data if deleted",
        type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes, delete it!", cancelButtonText: "No, cancel it!",
        closeOnConfirm: false, closeOnCancel: false
    },
     function (isConfirm) {
         if (isConfirm) {
             var id = [];
             $("input[name = 'ItemId']:checked").each(function () {
                 id.push($(this).val())
             });
             dpUI.loading.start("Deleting...");
             $.ajax({
                 url: '/SuperAdm/DeleteAds',
                 data: {
                     MarkAsDeleteId: id
                 },
                 cache: false,
                 type: "POST",
                 timeout: 10000,
                 dataType: "json",
                 success: function (result) {
                     if (result == true) {
                         dpUI.loading.stop();
                         //   swal("Reactivated", "Advert has been successfully reactivated.", "success");
                         window.location.reload();
                     }
                     else {
                         dpUI.loading.stop();
                         //  swal("Reactivated", "Advert has been successfully reactivated.", "error");
                         window.location.reload();
                     }

                 },
                 error: function (xhr, result) {
                     dpUI.loading.stop();
                     swal("Error", "There is an error deleting the selected Advert", "error")
                 }
             });
         }
         else {
             swal("Cancelled", "Delete Operation has been Cancel)", "error");
         }
     })
}
