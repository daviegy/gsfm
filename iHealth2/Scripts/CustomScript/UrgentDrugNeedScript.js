$(document).ready(function (e) {
   
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
})