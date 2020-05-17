$(document).keypress(function (e) {
    var keycode = (e.keyCode ? e.keyCode : e.which);
    if (keycode == '13') {
        var d = document.getElementById("searchvalue").value;
        if (d != "") {
          //  alert(input)
            dpUI.loading.start("Loading ...");
            $("#partialView").load("/ing/Search_Health_Providers", {searchTerm: d }, function (responseTxt, statusTxt, xhr) {
                if (statusTxt == "success") {
                    dpUI.loading.stop();
                    initTb();
                }
                if (statusTxt == "error")

                    alert("Error: " + xhr.status + ": " + xhr.statusText);
                dpUI.loading.stop();
            });

        }
        // alert(input.value)
    }
})

function search_Health_Providers(hpList) {
    var input = document.getElementById("searchvalue").value;
    if (input != "") {
     //   alert(input)
        dpUI.loading.start("Loading ...");        
        $("#partialView").load("/ing/Search_Health_Providers", { ct: hpList, searchTerm: input }, function (responseTxt, statusTxt, xhr) {
            if (statusTxt == "success") {
                dpUI.loading.stop();
                initTb();
            }
            if (statusTxt == "error")

                alert("Error: " + xhr.status + ": " + xhr.statusText);
            dpUI.loading.stop();
        });

    }
}

