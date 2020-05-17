$(document).ready(function () {
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

    var subtr1 = document.getElementById('subTr1')
    //  var space1 = document.getElementById('space1')
    var subtr2 = document.getElementById('subTr2')
    //var space2 = document.getElementById('space2')
    subtr1.style.display = 'none'
    subtr2.style.display = 'none'
    // space1.style.display = 'none'
    // space2.style.display = 'none'

    $("#cat").change(function () {
        var ctValue;
        if ($("#cat").val() != "") {
            ctValue = $("#cat").val();
        }
        else {
            ctValue = 0;
        }
        var opt = {};
        opt.url = "/Business/getSubcat1";
        opt.type = "Post";
        opt.dataType = "json";
        opt.data = JSON.stringify({ catId: ctValue });
        opt.contentType = "application/json";
        opt.success = function (subcat1) {

            //alert('success');
            if (subcat1 == null) {
                subtr1.style.display = 'none';
                subtr2.style.display = 'none';
                //  space1.style.display = 'none'

            }
            else {

                if (subcat1.length > 0) {
                    $("#subCat1").empty();
                    $("#subCat1").prop("required", true)
                    $("#subCat1").append('<option value=""> Select </option>')
                    $.each(subcat1, function (i, subcat1) {
                        $("#subCat1").append('<option value="' + subcat1.Value + '">' +
                               subcat1.Text + '</option>')
                    })

                    subtr1.style.display = '';
                    subtr2.style.display = 'none';
                    //  space1.style.display = ''
                }
                else {
                    $("#subCat1").empty();
                    subtr1.style.display = 'none'
                    // space1.style.display = 'none'
                }


            }
        }
        $.ajax(opt);
    })

    $("#subCat1").change(function () {
        var sbCatVal;
        if ($("#subCat1").val() != "") {
            sbCatVal = $("#subCat1").val();
        }
        else {
            sbCatVal = 0;
        }
        var option = {};
        option.url = "/Business/getSubcat2";
        option.type = "Post";
        option.dataType = "json";
        option.data = JSON.stringify({ subcat1Id: sbCatVal });
        option.contentType = "application/json";
        option.success = function (subcat2) {
            if (subcat2 == null) {

                subtr2.style.display = 'none'
                //  space2.style.display = 'none'
            }
            else {
                if (subcat2.length > 0) {
                    $("#subCat2").empty();
                    $("#subCat2").prop("required", true)
                    $("#subCat2").append('<option value=""> Select </option>')
                    $.each(subcat2, function (i, subcat2) {
                        $("#subCat2").append('<option value="' + subcat2.Value + '">' +
                               subcat2.Text + '</option>')
                    })

                    subtr2.style.display = ''
                    //  space2.style.display = ''
                }
                else {
                    $("#subCat2").empty();
                    subtr2.style.display = 'none'
                    // space2.style.display = 'none'
                }


            }
        }
        $.ajax(option);
    })

    $("#logo").fileinput({
        uploadUrl: "/",
        autoReplace: true,
        maxFileCount: 1,
        allowedFileExtensions: ["jpg", "png", "gif"],
        removeIcon: '<i class="icon icon icon-trash text-danger"></i>',
        removeClass: 'btn  btn-danger',
        removeTitle: 'Remove file',
        //uploadIcon: '<i class="icon icon-upload text-info"></i>',
        //uploadClass: 'btn btn-xs btn-success',
        //uploadTitle: 'Upload file',
        //maxFileSize: 1000,
        showCaption: false,
        showUpload: false,


    });

    $().ready(function () {

        $('input.myClass').prettyCheckable({
            color: 'red',
            label: document.getElementById("chkAccept").innerHtml = 'I accept the <a href=\"/home/terms\" target=\"_blank\">Terms and Conditions</a> of using this service' //document.write("I accept the <a href=\"/home/terms\" target=\"_blank\">Terms and Conditions</a> of using this service")
        });

    });
    //Enable iCheck plugin for checkboxes
    //iCheck for checkbox and radio inputs
   
    $('#summary').summernote();
});
$(document).ready(function () {
    $('#chkAccept').iCheck({
        checkboxClass: 'icheckbox_flat-green',
        radioClass: 'iradio_flat-greens'
    });

});
$("#logo").fileinput({
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
function submitEntry() {
    if ($("#state").val() == "") {
        stateError.textContent = "Select a state";
        return
        false;
    }
    else
        if ($("#CitySearch").val() == "") {
            cityError.textContent = "City field is required!";
            return false;
        }


            //else if ($("#subCat1").val() == "") {
            //    sub1Error.textContent = "Required!"
            //    return false;
            //}
            //else if ("#subCat2") {
            //    sub2Error.textContent = "Required!"
            //    return false;
            //}
        else if (this.chkAccept.checked == false) {
            swal({
                title: "Error!", text: "<b>You must Accept Terms and Condition</b>",
                type: "error",
                confirmButtonText: "OK",
                html: true
            });

            return false;
        }
}
