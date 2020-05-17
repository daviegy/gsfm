$(document).ready(function (e) {

    //Enable iCheck plugin for checkboxes
    //iCheck for checkbox and radio inputs
    var $b = $('input[type = "checkbox"]');
    $('.mailbox-messages input[type="checkbox"]').iCheck({
        checkboxClass: 'icheckbox_flat-blue',
        radioClass: 'iradio_flat-blue'
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
        if ($('#markAsSeenbyId:checked').length > 0) {
            $("#btnappr").prop('disabled', false)

        }
        else {
            $("#btnappr").prop('disabled', true)

        }

    })
});

$('#table').bootstrapTable({

    striped: true,
    // cardView: true,
    pagination: true,
    pageSize: 15,
    pageList: [15, 30, 50, 100, 200],
    smartDisplay: false,
    showToggle: true,
    search: true,
    showColumns: true,
    showRefresh: true,
    //    border : true
    //align: 'center',
    //valign: 'bottom'

});
function approve() {
    dpUI.loading.start("Approving...");
    var array = $.map($('input[name="markAsSeenbyId"]:checked'), function (c) { return c.value; })

    var option = {};
    option.url = "/Admin/ApproveUser";
    option.type = "Post";
    option.dataType = "json";
    option.data = { Id: array };
    option.success = function () {
        dpUI.loading.stop();
        //  $('#mytable').load('_NewlyRegBizParital')
        location.reload('#mytable');

    }
    option.error = function () {
        dpUI.loading.stop();
        swal("", "Error", "error")
    }
    $.ajax(option);
}

//    var option = {};
//    option.url = "";
//    option.type = "Post";
//    option.dataType = "json";
//    option.data = { : };
//    option.success = function () {
//        dpUI.loading.stop();
//        //  $('#mytable').load('_NewlyRegBizParital')


//    }
//    option.error = function () {
//        dpUI.loading.stop();
//        swal("", "Error", "error")
//    }
//    $.ajax(option);
//}



