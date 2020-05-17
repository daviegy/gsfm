//alert("comely");
var lgas = new Array(
					 new Array(),

                     new Array("Abia North", "Abia South", "Arochukwu", "bende", "Ikwuano", "Isiala-Ngwa North", "Isiala-Ngwa South", "Isuikwato", "Obi Nwa",
                               "Ohafia", "Osisioma", "Ngwa", "Ugwunagbo", "Ukwu-East", "Ukwu-West", "Umuahia-North", "Umuahia-South", "Umu-Neochi"),
                     new Array("Demsa", "Fufore", "Ganaye", "Gireri", "Gombi", "Guyuk", "Hong", "Jada", "Lamurde", "Madagali", "Maiha", "Mayo-Belwa", "Michika",
                               "Mubi-North", "Mubi-South", "Numan", "Song", "Toungo", "Yola-North", "Yola-South"),
                     new Array("Abak", "Eastern Obolo", "Eket", "Esit Eket", "Essien Udim", "Etim Ekpo", "Etinan", "Ibeno", "Ibesikpo Asutan", "Ibiono Ibom",
                               "Ika", "Ini", "Itu", "Mbo", "Mkpat Enin", "Nsit Atai", "Nsit Ibom", "Nsit Ibium", "Obot Akara", "Okobo", "Onna", "Oron", "Oruk Anam",
                               "Udung Uko", "Ukanagun", "Uruan", "Urue-Offong/Oruko", "Uyo"),
                     new Array("Aguata", "Anambra-East", "Anambra-west", "Anaocha", "Akwa-North", "Akwa-South", "Ayemelum", "Dunukofia", "Ekwusigo", "Idemili South",
                               "Idemili North", "Ihiala", "Njikoka", "Nnewi North", "Nnewi South", "Ogbaru", "Onitsha South", "Onitsha North", "Orumba North", "Orumba South",
                               "Oyi"),
                     new Array("Akaleri", "Bauchi", "Bogoro", "Damban", "Darazo", "Dazz", "Ganjuwa", "Giade", "Itas/Gadau", "Jamaare", "Katagun", "Kirfi", "Ningi", "Shira",
                               "Tafawa-Balewa", "Toro", "Warji", "Zafi"),
                     new Array("Brass", "Ekeremor", "Kolokuma/Opokuma", "Nembe", "Ogbia", "Sagbama", "Southern Jaw", "Yenogoa"),
                     new Array("Ado", "Agatu", "Apa", "Burukutu", "Gboko", "Guma", "Gwer East", "Gwer West", "Katsina Ala", "Koshisha", "Kwande", "Logo","Makurdi", "Obi", "Ogbadibo",
                               "Oju", "Okpokwu", "Ohimini", "Oturkpo", "Tarka", "Ukum", "Ushongo", "Vandeikya"),
                     new Array("Abadam", "Askira/Uba", "Bama", "Bayo", "Biu", "Chibok", "Damboa", "Gubio", "Guzamala", "Gwoza", "Hawul", "Jere", "Kaga", "Kala/Balga", "Kukawa",
                               "Kwaya Kusar", "Mafa", "magumeri", "Marte", "Mobbar", "Monguno", "Ngala", "Nganzai", "Shani"),
                     new Array("Akpabuyo", "Odukpani", "Akamkpa", "Biade", "Abi", "Ikom", "Yakur", "Odubra", "Boki", "Ogoja", "Yala", "Obianliku", "Obudu", "Calabar", "Etung",
                               "Bekwara", "Bakassi", "Calabar Municipality"),
                     new Array("Oshimili", "Aniocha", "Aniocha South", "Ika South", "Ika North-East", "Ndokwa West", "Ndokwa East", "Isoko South", "Isoko North",
                               "Bomadi", "Burutu", "Ugheli North", "Ugheli South", "Ethiope West", "Ethiope East", "Sapele", "Okpe", "Warri North", "Warri South",
                               "Uvwie", "Udu", "Warri Central","Ukwani", "Oshimili North", "Patani"),
                     new Array("Afikpo South", "Afikpo North", "Onicha", "Ohaozara", "Abakaliki", "Ishielu", "lkwo", "Ezza", "Ezza South", "Ohaukwu", "Ebonyi", "Ivo" ),
                     new Array("Esan North-East", "Esan Central", "Esan West", "Egor", "Ukpoba Central", "Etsako Central", "Igueben", "Oredo", "Ovia SouthWest",
                               "Ovia South-East", "Orhionwon", "Uhunmwonde", "Etsako East", "Esan South-East" ),
                     new Array("AdoEkiti-East", "Ekiti-West", "Emure/Ise/Orun", "Ekiti South-West", "Ikare", "Irepodun", "Ijero", "Ido/Osi", "Oye", "Ikole Moba", "Gbonyin",
                               "Efon", "Ise/Orun",  "Ilejemeje"),
                     new Array("Enugu South", "Igbo-Eze South", "Enugu North", "Nkanu-East", "Nkanu-West", "Udi Agwu", "Oji-River", "Ezeagu", "IgboEze North",
                               "Isi-Uzo", "Nsukka", "Igbo-Ekiti", "Uzo-Uwani", "Enugu East", "Aninri", "Udenu" ),
                     new Array("Akko", "Balanga", "Billiri", "Dukku", "Kaltungo", "Kwami", "Shomgom", "Funakaye", "Gombe", "Nafada/Bajoga", "Yamaltu/Delta" ),
                     new Array("Aboh-Mbaise", "Ahiazu-Mbaise", "Ehime-Mbano", "Ezinihitte", "Ideato North", "Ideato South", "Ihitte/Uboma", "Ikeduru", "Isiala Mbano",
                               "Isu", "Mbaitoli", "Mbaitoli", "Ngor-Okpala", "Njaba", "Nwangele", "Nkwerre", "Obowo", "Oguta", "Ohaji/Egbema", "Okigwe", "Orlu", "Orsu", "Oru East",
                               "Oru West", "Owerri-Municipal", "Owerri North", "Owerri West" ),
                     new Array("Auyo", "Babura", "Birni Kudu", "Biriniwa", "Buji", "Dutse", "Gagarawa", "Garki", "Gumel", "Guri", "Gwaram", "Gwiwa", "Hadejia", "Jahun",
                               "Kafin Hausa", "Kaugama Kazaure", "Kiri Kasamma", "Kiyawa", "Maigatari", "Malam Madori", "Miga", "Ringim", "Roni", "Sule-Tankarkar",
                               "Taura", "Yankwashi" ),
                     new Array("Birni-Gwari", "Chikun", "Giwa", "Igabi", "Ikara", "jaba", "Jema'a", "Kachia", "Kaduna North", "Kaduna South", "Kagarko", "Kajuru",
                               "Kaura", "Kauru", "Kubau", "Kudan", "Lere", "Makarfi", "Sabon-Gari", "Sanga", "Soba", "Zango-Kataf", "Zaria" ),
                     new Array("Ajingi", "Albasu", "Bagwai", "Bebeji", "Bichi", "Bunkure", "Dala", "Dambatta", "Dawakin Kudu", "Dawakin Tofa", "Doguwa", "Fagge", "Gabasawa",
                               "Garko", "Garum", "Mallam", "Gaya", "Gezawa", "Gwale", "Gwarzo", "Kabo", "Kano Municipal", "Karaye", "Kibiya", "Kiru", "kumbotso", "Kunchi",
                               "Kura", "Madobi", "Makoda", "Minjibir", "Nasarawa", "Rano", "Rimin Gado", "Rogo", "Shanono", "Sumaila", "Takali", "Tarauni", "Tofa", "Tsanyawa",
                               "Tudun Wada", "Ungogo", "Warawa", "Wudil"),
                     new Array("Bakori", "Batagarawa", "Batsari", "Baure", "Bindawa", "Charanchi", "Dandume", "Danja", "Dan Musa", "Daura", "Dutsi", "Dutsin-Ma", "Faskari",
                               "Funtua", "Ingawa", "Jibia", "Kafur", "Kaita", "Kankara", "Kankia", "Katsina", "Kurfi", "Kusada", "Mai'Adua", "Malumfashi", "Mani", "Mashi",
                               "Matazuu", "Musawa", "Rimi", "Sabuwa", "Safana", "Sandamu", "Zango" ),
                     new Array("Aleiro", "Arewa-Dandi", "Argungu", "Augie", "Bagudo", "Birnin Kebbi", "Bunza", "Dandi", "Fakai", "Gwandu", "Jega", "Kalgo", "Koko/Besse", "Maiyama",
                               "Ngaski", "Sakaba", "Shanga", "Suru", "Wasagu/Danko", "Yauri", "Zuru" ),
                     new Array("Adavi", "Ajaokuta", "Ankpa", "Bassa", "Dekina", "Ibaji", "Idah", "Igalamela-Odolu", "Ijumu", "Kabba/Bunu", "Kogi", "Lokoja", "Mopa-Muro",
                               "Ofu", "Ogori/Mangongo", "Okehi", "Okene", "Olamabolo", "Omala", "Yagba East", "Yagba West"),
                     new Array("Asa", "Baruten", "Edu", "Ekiti", "Ifelodun", "Ilorin East", "Ilorin West", "Irepodun", "Isin", "Kaiama", "Moro", "Offa", "Oke-Ero", "Oyun","Pategi" ),
                     new Array("Agege", "Ajeromi-Ifelodun", "Alimosho", "Amuwo-Odofin", "Apapa", "Badagry", "Epe", "Eti-Osa", "Ibeju/Lekki", "Ifako-Ijaye", "Ikeja", "Ikorodu", "Kosofe",
                               "Lagos Island", "Lagos Mainland", "Mushin", "Ojo", "Oshodi-Isolo", "Shomolu", "Surulere"),
                    new Array("Akwanga", "Awe", "Doma", "Karu", "Keana", "Keffi", "Kokona", "Lafia", "Nasarawa", "Nasarawa-Eggon", "Obi", "Toto", "Wamba" ),
                    new Array("Agaie", "Agwara", "Bida", "Borgu", "Bosso", "Chanchaga", "Edati", "Gbako", "Gurara", "Katcha", "Kontagora", "Lapai", "Lavun", "Magama", "Mariga", "Mashegu",
                              "Mokwa", "Muya", "Pailoro", "Rafi", "Rijau", "Shiroro", "Suleja", "Tafa", "Wushishi"),
                    new Array("Abeokuta North", "Abeokuta South", "Ado-Odo/Ota", "Egbado North", "Egbado South", "Ewekoro", "Ifo", "Ijebu East", "Ijebu North", "Ijebu North East", "Ijebu Ode",
                              "Ikenne", "Imeko-Afon", "Ipokia", "Obafemi-Owode", "Ogun Waterside", "Odeda", "Odogbolu", "Remo North", "Shagamu"),
                    new Array("Akoko North East", "Akoko North West", "Akoko South Akure East", "Akoko South West", "Akure North", "Akure South", "Ese-Odo", "Idanre", "Ifedore", "Ilaje", "Ile-Oluji",
                              "Okeigbo", "Irele", "Odigbo", "Okitipupa", "Ondo East", "Ondo West", "Ose", "Owo" ),
                    new Array("Aiyedade", "Aiyedire", "Atakumosa East", "Atakumosa West", "Boluwaduro", "Boripe", "Ede North","Ede South", "Egbedore", "Ejigbo", "Ife Central", "Ife East",
                              "Ife North", "Ife South", "Ifedayo", "Ifelodun", "Ila", "Ilesha East", "Ilesha West", "Irepodun", "Irewole", "Isokan", "Iwo", "Obokun", "Odo-Otin", "Ola-Oluwa",
                              "Olorunda", "Oriade", "Orolu", "Osogbo" ),
                    new Array("Afijio", "Akinyele", "Atiba", "Atigbo", "Egbeda", "IbadanCentral", "Ibadan North", "Ibadan North West", "Ibadan South East", "Ibadan South West", "Ibarapa Central",
                              "Ibarapa East", "Ibarapa North", "Ido", "Irepo", "Iseyin", "Itesiwaju", "Iwajowa", "Kajola", "Lagelu", "Ogbomosho North", "Ogbmosho South", "Ogo Oluwa", "Olorunsogo",
                              "Oluyole", "Ona-Ara", "Orelope", "Ori Ire", "Oyo East", "Oyo West", "Saki East", "Saki West", "Surulere"),
                new Array("Barikin Ladi", "Bassa", "Bokkos", "Jos East", "Jos North", "Jos South", "Kanam", "Kanke", "Langtang North", "Langtang South", "Mangu", "Mikang", "Pankshin","Qua'an Pan",
                          "Riyom", "Shendam", "Wase"),
                new Array("Abua/Odual", "Ahoada East", "Ahoada West", "Akuku Toru", "Andoni", "Asari-Toru", "Bonny", "Degema", "Emohua", "Eleme", "Etche", "Gokana", "Ikwerre", "Khana","Obia/Akpor",
                          "Ogba/Egbema/Ndoni", "Ogu/Bolo", "Okrika", "Omumma", "Opobo/Nkoro", "Oyigbo", "Port-Harcourt", "Tai" ),
                new Array("Binji", "Bodinga", "Dange-shnsi", "Gada", "Goronyo", "Gudu", "Gawabawa", "Illela", "Isa", "Kware", "kebbe", "Rabah", "Sabon birni", "Shagari", "Silame", "Sokoto North",
                          "Sokoto South", "Tambuwal", "Tongaza", "Tureta", "Wamako", "Wurno", "Yabo"),
                new Array("Ardo-kola","Bali","Donga","Gashaka", "Cassol", "Ibi", "Jalingo", "Karin-Lamido", "Kurmi", "Lau", "Sardauna", "Takum", "Ussa", "Wukari", "Yorro","Zing"),
                new Array("Bade", "Bursari", "Damaturu", "Fika", "Fune", "Geidam", "Gujba", "Gulani", "Jakusko", "Karasuwa", "Karawa", "Machina", "Nangere", "Nguru", "Potiskum","Tarmua","Yunusari","Yusufari"),
                new Array("Anka", "Bakura", "Birnin Magaji", "Bukkuyum", "Bungudu", "Gummi", "Gusau", "Kaura", "Namoda", "Maradun", "Maru", "Shinkafi", "Talata Mafara", "Tsafe", "Zurmi" ),
                new Array("Gwagwalada", "Kuje", "Abaji", "Abuja Municipal", "Bwari", "Kwali")
                     );

function populate(){
    //get the index of the selected state 
  
	 $('#state').attr('selected','')
    var selectedState = document.getElementById("state").selectedIndex;
    
    //get the value of the selelcted index
    var theState = document.getElementById("state").options[selectedState].value;
    if ($("#state").val() != "Select State") {
        if (theState != "") {//if the value is not empty, then proceed

            document.getElementById("lga").options.length = 0;//reset the lga options to zero i.e. so that it does not add cumulatively
            for (var i = 0; i < lgas[document.getElementById("state").selectedIndex].length; i++) {
                //create a new option
                document.getElementById("lga").options[i] = new Option(lgas[document.getElementById("state").selectedIndex][i]);
                //give the option a value
                document.getElementById("lga").options[i].value = lgas[document.getElementById("state").selectedIndex][i];
            }
        }
    }
    else {
        $('#lga').empty();
        $("#lga").prop("disabled", true);
    }
}



 function Rpopulate(){
    //get the index of the selected state 
	 $('#Rstate').attr('selected','')
    var selectedState = document.getElementById("Rstate").selectedIndex;
    
    //get the value of the selelcted index
    var theState = document.getElementById("Rstate").options[selectedState].value;
    if(theState!=""){//if the value is not empty, then proceed
        
        document.getElementById("Rlga").options.length = 0;//reset the lga options to zero i.e. so that it does not add cumulatively
        for(var i =0; i<lgas[document.getElementById("Rstate").selectedIndex].length; i++)
        {
            //create a new option
            document.getElementById("Rlga").options[i] = new Option(lgas[document.getElementById("Rstate").selectedIndex][i]);
            //give the option a value
            document.getElementById("Rlga").options[i].value = lgas[document.getElementById("Rstate").selectedIndex][i];
        }
    }
}





function Bpopulate(){
    //get the index of the selected state 
	 $('#Bstate').attr('selected','')
    var selectedState = document.getElementById("Bstate").selectedIndex;
    
    //get the value of the selelcted index
    var theState = document.getElementById("Bstate").options[selectedState].value;
    if(theState!=""){//if the value is not empty, then proceed
        
        document.getElementById("Blga").options.length = 0;//reset the lga options to zero i.e. so that it does not add cumulatively
        for(var i =0; i<lgas[document.getElementById("Bstate").selectedIndex].length; i++)
        {
            //create a new option
            document.getElementById("Blga").options[i] = new Option(lgas[document.getElementById("Bstate").selectedIndex][i]);
            //give the option a value
            document.getElementById("Blga").options[i].value = lgas[document.getElementById("Bstate").selectedIndex][i];
        }
    }
}




function Fpopulate(){
    //get the index of the selected state 
	 $('#Fstate').attr('selected','')
    var selectedState = document.getElementById("Fstate").selectedIndex;
    
    //get the value of the selelcted index
    var theState = document.getElementById("Fstate").options[selectedState].value;
    if(theState!=""){//if the value is not empty, then proceed
        
        document.getElementById("Flga").options.length = 0;//reset the lga options to zero i.e. so that it does not add cumulatively
        for(var i =0; i<lgas[document.getElementById("Fstate").selectedIndex].length; i++)
        {
            //create a new option
            document.getElementById("Flga").options[i] = new Option(lgas[document.getElementById("Fstate").selectedIndex][i]);
            //give the option a value
            document.getElementById("Flga").options[i].value = lgas[document.getElementById("Fstate").selectedIndex][i];
        }
    }
}

function Mpopulate() {
    //get the index of the selected state 
    $('#Mstate').attr('selected', '')
    var selectedState = document.getElementById("Mstate").selectedIndex;

    //get the value of the selelcted index
    var theState = document.getElementById("Mstate").options[selectedState].value;
    if (theState != "") {//if the value is not empty, then proceed

        document.getElementById("Mlga").options.length = 0;//reset the lga options to zero i.e. so that it does not add cumulatively
        for (var i = 0; i < lgas[document.getElementById("Mstate").selectedIndex].length; i++) {
            //create a new option
            document.getElementById("Mlga").options[i] = new Option(lgas[document.getElementById("Mstate").selectedIndex][i]);
            //give the option a value
            document.getElementById("Mlga").options[i].value = lgas[document.getElementById("Mstate").selectedIndex][i];
        }
    }
}

function Cpopulate() {
    //get the index of the selected state 
    $('#Cstate').attr('selected', '')
    var selectedState = document.getElementById("Cstate").selectedIndex;

    //get the value of the selelcted index
    var theState = document.getElementById("Cstate").options[selectedState].value;
    if (theState != "") {//if the value is not empty, then proceed

        document.getElementById("Clga").options.length = 0;//reset the lga options to zero i.e. so that it does not add cumulatively
        for (var i = 0; i < lgas[document.getElementById("Cstate").selectedIndex].length; i++) {
            //create a new option
            document.getElementById("Clga").options[i] = new Option(lgas[document.getElementById("Cstate").selectedIndex][i]);
            //give the option a value
            document.getElementById("Clga").options[i].value = lgas[document.getElementById("Cstate").selectedIndex][i];
        }
    }
}



function populate4(){
    //get the index of the selected state 
	 $('#state4').attr('selected','')
    var selectedState = document.getElementById("state4").selectedIndex;
    
    //get the value of the selelcted index
    var theState = document.getElementById("state4").options[selectedState].value;
    if(theState!=""){//if the value is not empty, then proceed
        
        document.getElementById("lga4").options.length = 0;//reset the lga options to zero i.e. so that it does not add cumulatively
        for(var i =0; i<lgas[document.getElementById("state4").selectedIndex].length; i++)
        {
            //create a new option
            document.getElementById("lga4").options[i] = new Option(lgas[document.getElementById("state4").selectedIndex][i]);
            //give the option a value
            document.getElementById("lga4").options[i].value = lgas[document.getElementById("state4").selectedIndex][i];
        }
    }
}




function populate5(){
    //get the index of the selected state 
	 $('#state5').attr('selected','')
    var selectedState = document.getElementById("state5").selectedIndex;
    
    //get the value of the selelcted index
    var theState = document.getElementById("state5").options[selectedState].value;
    if(theState!=""){//if the value is not empty, then proceed
        
        document.getElementById("lga5").options.length = 0;//reset the lga options to zero i.e. so that it does not add cumulatively
        for(var i =0; i<lgas[document.getElementById("state5").selectedIndex].length; i++)
        {
            //create a new option
            document.getElementById("lga5").options[i] = new Option(lgas[document.getElementById("state5").selectedIndex][i]);
            //give the option a value
            document.getElementById("lga5").options[i].value = lgas[document.getElementById("state5").selectedIndex][i];
        }
    }
}





function populate6(){
    //get the index of the selected state 
	 $('#state6').attr('selected','')
    var selectedState = document.getElementById("state6").selectedIndex;
    
    //get the value of the selelcted index
    var theState = document.getElementById("state6").options[selectedState].value;
    if(theState!=""){//if the value is not empty, then proceed
        
        document.getElementById("lga6").options.length = 0;//reset the lga options to zero i.e. so that it does not add cumulatively
        for(var i =0; i<lgas[document.getElementById("state6").selectedIndex].length; i++)
        {
            //create a new option
            document.getElementById("lga6").options[i] = new Option(lgas[document.getElementById("state6").selectedIndex][i]);
            //give the option a value
            document.getElementById("lga6").options[i].value = lgas[document.getElementById("state6").selectedIndex][i];
        }
    }
}