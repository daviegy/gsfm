var proff = document.getElementById('prof')
var subtr1 = document.getElementById('subTr1')
var subtr2 = document.getElementById('subTr2')
subtr1.style.display = 'none'
subtr2.style.display = 'none'
proff.style.display = 'none'
var prof = document.getElementById('other_Profession');
prof.style.display = 'none';
var selectPro = document.getElementById('Profession')
$(document).ready(function () {
   // var prof = document.getElementById('prof')
    var rd_yes = document.getElementById('rd_yes')
    var rd_no = document.getElementById('rd_no')
   // prof.style.display = 'none'
    $('input[type="radio"]').iCheck({
        checkboxClass: 'icheckbox_flat-green',
        radioClass: 'iradio_flat-green'
    });
    $('input[type="radio"]').iCheck('check', function (event) {
        $('input[type="radio"]').on('ifToggled', function (event) {
            if (rd_yes.checked) {
                proff.style.display = ''
                //subtr1.style.display = ''
                //subtr2.style.display = ''
                //prof.style.display = '';

            }
            else if (rd_no.checked)
            {
             
                proff.style.display = 'none';
                subtr1.style.display = 'none';
                subtr2.style.display = 'none';
                prof.style.display = 'none';
                var subCat1 = document.getElementById('subCat1')
                var subCat2 = document.getElementById('subCat2')
                selectPro.selectedIndex = ''
                subCat1.selectedIndex = ''
                subCat2.selectedIndex = ''

            }
        
        })
       
    });
})
//$(document).ready(function () {
//var subtr1 = document.getElementById('subTr1')
////  var space1 = document.getElementById('space1')
//var subtr2 = document.getElementById('subTr2')
////var space2 = document.getElementById('space2')
//subtr1.style.display = 'none'
//subtr2.style.display = 'none'
//var prof = document.getElementById('other_Profession');
//prof.style.display = 'none';
$('#Profession').change(function () {
    if ($("#Profession").val() == 1) {
        prof.style.display = 'none';
        $('#other_Profession').prop("required", false)
        var ctValue;
        if ($("#Profession").val() != "") {
            ctValue = $("#Profession").val();
        }
        else {
            ctValue = 0;
        }

        var opt = {};
        opt.url = "/Home/getSubPro1";
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
                    $("#subCat1").append('<option value=""> Select Category </option>')
                    $.each(subcat1, function (i, subcat1) {
                        $("#subCat1").append('<option value="' + subcat1.Value + '">' +
                               subcat1.Text + '</option>')
                    })

                    subtr1.style.display = '';
                    subtr2.style.display = 'none';
                    //  space1.style.display = ''
                }
                else {
                    $("#subCat1").append('<option value="">Select Category</option>')
                    $("#subCat1").prop("required", false)

                    $("#subCat1").empty();
                    subtr1.style.display = 'none'
                    // space1.style.display = 'none'
                }


            }
        }
        $.ajax(opt);
    }
    else if ($("#Profession").val() == 9) {
        subtr1.style.display = 'none';
        subtr2.style.display = 'none';
        prof.style.display = '';
        $('#other_Profession').prop("required", true)
    }
    else {
        prof.style.display = 'none';
        $('#other_Profession').prop("required", false)
        $("#subCat1").empty();
        subtr1.style.display = 'none'
        subtr2.style.display = 'none';
    }
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
    option.url = "/Home/getSubcat2";
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
                $("#subCat2").append('<option value=""> Select Area of Specialization </option>')
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
});