$(document).ready(function () {
    //$(".txtPdescription").jqte();
    var pcondition = document.getElementById('pcondd');
    pcondition.style.display = 'none';
    $('#drugCat').hide()
        $("#Addr").jqte();
    var url = "/Home/getState";
    //$("#state").prop("disabled",true)
    $("#Country").change(function (e) {
        var CnVal;
        if ($("#Country").val() != "") {
            CnVal = $("#Country").val();
        }
        else {
            CnVal = 0;
        }
        var options = {};
        options.url = "/Home/getState",
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

    $('#ProductCategory').change(function () {
        if ($('#ProductCategory').val() == 4) {
            $('#drugCat').show()
            $('#drugct').attr("required", true).css('border-color', 'red')
            pcondition.style.display = 'none';
            $('#pcond option:selected').removeAttr('selected')
            $('#pcond').removeAttr("required")
        }
       else if ($("#ProductCategory").val() == 5 ) {
            pcondition.style.display = '';
            $('#pcond').attr("required", true).css('border-color', 'red')
            $('#drugCat').hide()
            $('#drugct option:selected').removeAttr('selected')
            $('#drugct').removeAttr("required")
        }
       else {
           $('#drugCat').hide()
           $('#drugct option:selected').removeAttr('selected')
            pcondition.style.display = 'none';
           // $('#pcond').prop("required", true)
            $('#pcond option:selected').removeAttr('selected')
        }
    })

    $("#ProductImage").fileinput({
        uploadUrl: "/file-upload-batch/2",
        autoReplace: false,
        maxFileCount: 4,
        allowedFileExtensions: ["jpg", "png", "gif"],
        removeIcon: '<i class="icon icon icon-trash text-danger"></i>',
        removeClass: 'btn  btn-danger',
        removeTitle: 'Remove file',
        uploadIcon: '<i class="icon icon-upload text-info"></i>',
        uploadClass: 'btn btn-xs btn-success',
        uploadTitle: 'Upload file',
        previewFileIcon: '<i class="icon icon-file"></i>',
        showCaption: false,
        showUpload: false,
        resizeImage: true,
        //maxImageWidth: 350,
        //maxImageHeight: 263,
       // maxFileSize: 1000,
    });
    $('#ProductImage').on('change', function (event) {
      
        $('#ProductImage').fileinput('reset');
    });
    $().ready(function () {

        $('input.myClass').prettyCheckable({
            color: 'red',
            label: document.getElementById("chkAccept").innerHtml = 'I accept the <a href=\"/home/terms\" target=\"_blank\">Terms and Conditions</a> of using this service' //document.write("I accept the <a href=\"/home/terms\" target=\"_blank\">Terms and Conditions</a> of using this service")
        });

    });


})

function submitEntry() {
   var p = document.getElementById('ProductImage').value;
   if (p.length<1) {
        swal({
            title: "Required!!!", text: "<b>You must Upload atleast one Image</b>",
            type: "error",
            confirmButtonText: "OK",
            html: true
        });
        return false;

   }
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

//function trimfield(str) {
//    return str.replace(/^\s+|\s+$/g, '');
//}