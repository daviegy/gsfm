$(document).ready(function () {
              var url = "/Home/getState";
           
           $("#Country").change(function () {
               var CnVal;
               if ($("#Country").val() != "")
               {
                   CnVal = $("#Country").val();
               }
               else
               {
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
                  
                   if (State.length>0)
                   {
                   //    alert('data available')
                       $("#state").prop("required", true)
                       $("#state").append('<option value="">Select State</option>')
                       $.each(State, function (i, State) {
                          // alert()
                           $("#state").append('<option value="' + State.Value + '">' +
               State.Text + '</option>')
                       });
                       $("#state").prop("disabled", false)
                   }
                   else {
                      // alert('no data')
                       $("#state").append('<option value="0">Select State</option>')
                      
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
               if ($("#cat").val() != "")
               {
                   ctValue = $("#cat").val();
               }
               else {
                   ctValue = 0;
               }

               var opt = {};
               opt.url = "/Home/getSubcat1";
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
                           $("#subCat1").append('<option value=""> Select </option>')
                           $("#subCat1").prop("required", false)
                        
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
               if ($("#subCat1").val() != "")
               {
                   sbCatVal = $("#subCat1").val();
               }
               else
               {
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

       });


//AutoComplete Search For HomePage
   
        $('#SearchBox').autoComplete({
            minChars:1,
            source: function (term, response) {
                $.getJSON('/Search/HomePageAutocomplete', { term: term }, function (data) { response(data); });
            }
        });
   

