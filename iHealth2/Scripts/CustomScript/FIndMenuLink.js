function Search_Hosp_subcatQuery(subcat) {

    if (subcat != "") {
        dpUI.loading.start("Loading ...");
        $("#partialView").load("/Hospital/find_Hosp", { sbValue: subcat }, function (responseTxt, statusTxt, xhr) {
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
function Search_Hosp_by_State(state) {
    if (state != "") {
        dpUI.loading.start("Loading ...");
        $("#partialView").load("/Hospital/search_by_state", { state: state }, function (responseTxt, statusTxt, xhr) {
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
function Search_Pharm_subcatQuery(subcat) {

    if (subcat != "") {
        dpUI.loading.start("Loading ...");
        $("#partialView").load("/Pharmacy/find_SubCat", { sbValue: subcat }, function (responseTxt, statusTxt, xhr) {
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
function Search_Pharm_by_State(state) {
    if (state != "") {
        dpUI.loading.start("Loading ...");
        $("#partialView").load("/Pharmacy/search_by_state", { state: state }, function (responseTxt, statusTxt, xhr) {
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
function Search_Lab_by_State(state) {
    if (state != "") {
        dpUI.loading.start("Loading ...");
        $("#partialView").load("/Lab/search_by_state", { state: state }, function (responseTxt, statusTxt, xhr) {
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
function Search_MedEquip_by_State(state) {
    if (state != "") {
        dpUI.loading.start("Loading ...");
        $("#partialView").load("/Med_Equipments/search_by_state", { state: state }, function (responseTxt, statusTxt, xhr) {
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
function search_equipt_by_manuf(manuf) {
    if (manuf != "") {
        //alert(manuf)
        dpUI.loading.start("Loading ...");
        $("#eq").load("/Med_Equipments/search_equipt_by_manuf",
           { m: manuf }, function (responseTxt, statusTxt, xhr) {
               if (statusTxt == "success") {
                   loadGrid()
                   dpUI.loading.stop();
                  
               }
               if (statusTxt == "error")

                   alert("Error: " + xhr.status + ": " + xhr.statusText);
               dpUI.loading.stop()
           });
    }
}
function Search_Vet_subcatQuery(subcat) {

    if (subcat != "") {
        dpUI.loading.start("Loading ...");
        $("#partialView").load("/Vet/find_Vet_Centr", { sbValue: subcat }, function (responseTxt, statusTxt, xhr) {
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
function Search_Vet_by_State(state) {
    if (state != "") {
        dpUI.loading.start("Loading ...");
        $("#partialView").load("/Vet/search_by_state", { state: state }, function (responseTxt, statusTxt, xhr) {
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

function Search_Herbal_by_State(state) {
    if (state != "") {
        dpUI.loading.start("Loading ...");
        $("#partialView").load("/Herbal/search_by_state", { state: state }, function (responseTxt, statusTxt, xhr) {
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
function Search_Hmos_by_State(state) {
    if (state != "") {
        dpUI.loading.start("Loading ...");
        $("#partialView").load("/home/search_hmos_by_state", { state: state }, function (responseTxt, statusTxt, xhr) {
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
function Search_Ngos_by_State(state) {
    if (state != "") {
        dpUI.loading.start("Loading ...");
        $("#partialView").load("/home/search_ngos_by_state", { state: state }, function (responseTxt, statusTxt, xhr) {
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

function search_client_boosters_category(category) {
    if (category != "") {
        dpUI.loading.start("Loading ...");
        $("#partialView").load("/ing/Client_Booster_List_by_category", { sbValue: category }, function (responseTxt, statusTxt, xhr) {
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
function Search_client_boosters_by_State(state) {
    if (state != "") {
        dpUI.loading.start("Loading ...");
        $("#partialView").load("/ing/search_by_state", { state: state }, function (responseTxt, statusTxt, xhr) {
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
$(document).ready(function () {
    $(window).load(function () {
        var options = { autoResize: true, container: $('#gridArea'), offset: 10 };
        var handler = $('#tiles li'); handler.wookmark(options);
        $('#tiles li').each(function () {
            var imgm = 0;
            if ($(this).find('img').length > 0) imgm = parseInt($(this).find('img').not('p img').css('margin-bottom'));
            var newHeight = $(this).find('img').height() + imgm + $(this).find('div').height() + $(this).find('h4').height() + $(this).find('p').not('blockquote p').height() + $(this).find('iframe').height() + $(this).find('blockquote').height() + 5;
            if ($(this).find('iframe').height()) newHeight = newHeight + 15; $(this).css('height', newHeight + 'px');
        }); handler.wookmark(options); handler.wookmark(options);
    });
    function initTb() {      
        $("#example").simplePagination({

            // the number of rows to show per page
            perPage: 20,

            // CSS classes to custom the pagination
            containerClass: '',
            previousButtonClass: 'btn btn-success',
            nextButtonClass: 'btn btn-success',

            // text for next and prev buttons
            previousButtonText: 'Previous',
            nextButtonText: 'Next',

            // initial page
            currentPage: 1

        });    
        
    }
})