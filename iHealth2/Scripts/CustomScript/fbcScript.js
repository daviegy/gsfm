$('#table').bootstrapTable({

    striped: true,
    // cardView: true,
    pagination: true,
    pageSize: 15,
    pageList: [15, 30, 50, 100, 200],
    smartDisplay: false,
    showToggle: false,
    search: true,
    showColumns: true,
    showRefresh: true,
    //    border : true
    //align: 'center',
    //valign: 'bottom'

});
function getDetails(val1, val2) { 
    dpUI.loading.start("Loading...");
    var options = {};
    options.cache = false;
    options.type = "GET";
    options.url = "/admin/getDetails";
    //options.dataType = "json";
    options.data = { Id: val1 , pid: val2};
    options.success = function (data) {
        dpUI.loading.stop();
        $('#dt').html('')
        var trHTML = "<table style='text-wrap:inherit' class='table table-striped  table-hover'>"

        $.each(data, function (i, o) {
            trHTML += '<tr><th>Name</th><td>' + o.Name + '</td></tr>' +
           '<tr><th>Phone</th><td>' + o.Phone + '</td></tr>' +
            '<tr><th>Email</th><td>' + o.Email + '</td></tr>' +
            '<tr><th>Acct Name</th><td>' + o.AccName + '</td></tr>' +
            '<tr><th>Acct. No.</th><td>' + o.AccNo + '</td></tr>' +
             '<tr><th>Bank Name</th><td>' + o.BankName + '</td></tr>' +
            '<tr><th>No.of Downlines</th><td>' + o.Downline + '</td></tr>' +
            '<tr><th colspan="2"><a href="#"onclick=getModal() class="btn btn-success">Send Email</a></th></tr>'
            $('#emailto').val(o.Email);


        });
        trHTML += "</table>"
        $('#dt').append(trHTML);
        $('#dtpanel').show();
    }; $.ajax(options);
}
function getModal() {
    $('#sndMail').modal('toggle')


}