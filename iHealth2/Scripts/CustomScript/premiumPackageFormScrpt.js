$(document).ready(function () {
    var rd_yes = document.getElementById('rd_yes')
    var rd_no = document.getElementById('rd_no')

    $('input[type="checkbox"],input[type="radio"]').iCheck({
        checkboxClass: 'icheckbox_flat-green',
        radioClass: 'iradio_flat-green'

    });
    $("#dob").datepicker({
        format: "dd-mm-yyyy",
        clearBtn: true,
        autoclose: true,
    })

    $("#ddlCountry").change(function (e) {
        var CnVal;
        if ($("#ddlCountry").val() != "") {
            CnVal = $("#ddlCountry").val();
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
    $("#plan").change(function (e) {
        var planVal = ($("#plan").val() != "" || $("#plan").val() != null) ? $("#plan").val() : null
        var stVal = ($("#state").val() != "" || $("#state").val() != null) ? $("#state").val() : 0
        $.ajax({
            url: "/premium/get_Plan_Price_List",
            type: "Post",
            dataType: "json",
            data: JSON.stringify({ Id: planVal }),
            contentType: "application/json",
            success: function (data) {
                $("#price_list").empty()
                if (data.length > 0) {

                    $("#price_list").append('<option value="">Select Price*</option>')
                    $.each(data, function (i, data) {
                        $("#price_list").append('<option value="' + data.Value + '"selected>' +
            data.Text + '</option>')

                    });
                    $("#price_list").prop("disabled", false)

                }
                else {
                    $("#price_list").append('<option value="">Select List*</option>')

                }

            }
        });
        $.ajax({
            url: "/premium/get_Hosp_List",
            type: "Post",
            dataType: "json",
            data: JSON.stringify({ PlanId: planVal, loc: stVal }),
            contentType: "application/json",
            success: function (data) {
                $("#Hosp_list").empty()
                if (data.length > 0) {

                    $("#Hosp_list").append('<option value="">Select Provider*</option>')
                    $.each(data, function (i, data) {
                        $("#Hosp_list").append('<option value="' + data.Value + '"selected>' +
            data.Text + '</option>')

                    });
                    $("#Hosp_list").prop("disabled", false)

                }
                else {
                    $("#Hosp_list").append('<option value="">Select Provider*</option>')

                }

            }
        });       
    });
    
    $('#state').change(function (e) {
        var planVal = ($("#plan").val() != "" || $("#plan").val() != null)?$("#plan").val(): null 
        var stVal = ($("#state").val() != "" || $("#state").val() != null) ? $("#state").val() : 0
        $.ajax({
            url: "/premium/get_Hosp_List",
            type: "Post",
            dataType: "json",
            data: JSON.stringify({ PlanId: planVal, loc: stVal }),
            contentType: "application/json",
            success: function (data) {
                $("#Hosp_list").empty()
                if (data.length > 0) {

                    $("#Hosp_list").append('<option value="">Select Provider*</option>')
                    $.each(data, function (i, data) {
                        $("#Hosp_list").append('<option value="' + data.Value + '"selected>' +
            data.Text + '</option>')

                    });
                    $("#Hosp_list").prop("disabled", false)

                }
                else {
                    $("#Hosp_list").append('<option value="">Select Provider*</option>')

                }

            }
        });
    })


})

window.addEventListener('load', function () {
    var CnVal;
    if ($("#plan").val() != "" || $('plan').val() != null) {
        CnVal = $("#plan").val();
        $.ajax({
            url: "/premium/get_Hosp_List",
            type: "Post",
            dataType: "json",
            data: JSON.stringify({ PlanId: CnVal , loc:null}),
            contentType: "application/json",
            success: function (data) {
                $("#Hosp_list").empty()
                if (data.length > 0) {

                    $("#Hosp_list").append('<option value="">Select Provider*</option>')
                    $.each(data, function (i, data) {
                        $("#Hosp_list").append('<option value="' + data.Value + '"selected>' +
            data.Text + '</option>')

                    });
                    $("#Hosp_list").prop("disabled", false)

                }
                else {
                    $("#Hosp_list").append('<option value="">Select Provider*</option>')

                }

            }
        });
        $.ajax({
            url: "/premium/get_Plan_Price_List",
            type: "Post",
            dataType: "json",
            data: JSON.stringify({ Id: CnVal }),
            contentType: "application/json",
            success: function (data) {
                $("#price_list").empty()
                if (data.length > 0) {

                    $("#price_list").append('<option value="">Select Price*</option>')
                    $.each(data, function (i, data) {
                        $("#price_list").append('<option value="' + data.Value + '"selected>' +
            data.Text + '</option>')

                    });
                    $("#price_list").prop("disabled", false)

                }
                else {
                    $("#price_list").append('<option value="">Select Price*</option>')

                }

            }
        });
    }

})