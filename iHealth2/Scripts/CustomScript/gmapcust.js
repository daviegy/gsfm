var lat;
var long;
var city;
var latlng;
var map;
var geocoder;
var city;
var currentloc = document.getElementById('CitySearch');
lat = 10.0000;
long = 8.0000;
latlng = new google.maps.LatLng(lat, long);
function init() {
    geocoder = new google.maps.Geocoder();
    var option = {
        center: latlng,
        zoom: 10,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        mapTypeControl: true,
        mapTypeControlOptions:
        {
            style: google.maps.MapTypeControlStyle.DROPDOWN_MENU,
            poistion: google.maps.ControlPosition.TOP_RIGHT,
            mapTypeIds: [google.maps.MapTypeId.ROADMAP,
              google.maps.MapTypeId.TERRAIN,
              google.maps.MapTypeId.HYBRID,
              google.maps.MapTypeId.SATELLITE]
        },
        navigationControl: true,
        navigationControlOptions:
        {
            style: google.maps.NavigationControlStyle.ZOOM_PAN
        },
        scaleControl: true,
        disableDoubleClickZoom: false,
        draggable: true,
        streetViewControl: true,
        draggableCursor: 'hand'

    };
    map = new google.maps.Map(document.getElementById('map_canvas'), option);
    SetMarker(map,lat,long)
   
  
    }
google.maps.event.addDomListener(window, 'load', init);
addEventListener('click', function searchGeocode()
{
    var option = {
        types: ['geocode'], 
        
        componentRestrictions: { country: "NG" }

    };
    
    var autocomplete1 = new google.maps.places.Autocomplete(currentloc,option); 
    google.maps.event.addListener(autocomplete1, 'place_changed', function () {
        place = autocomplete1.getPlace();
        lat = document.getElementById('CityLat')
        long = document.getElementById('CityLong')
        lat.value = place.geometry.location.lat()
        long.value = place.geometry.location.lng()

    });
  
    if (currentloc.value == null)
    {
        currentloc.value = city
    }
    
    geocoder.geocode({ 'address': currentloc.value }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            lat.value = results[0].geometry.location.lat();
            long.value = results[0].geometry.location.lng();
            map.setCenter(results[0].geometry.location);
            SetMarker(map, results[0].geometry.location.lat(), results[0].geometry.location.lng())

        } //else {
        //    alert("Geocode was not successful for the following reason: <strong> No valid City </strong> ");
        //}
    });

})
//google.maps.event.addDomListener(window, 'load', searchGeocode);

function SetMarker(map,lat,long)
{
    var maker = new google.maps.Marker({
        position: new google.maps.LatLng(lat, long),
        map: map,
        title: 'Click me'

    });
    var infowindow = new google.maps.InfoWindow({
        content: "<b>Your Address</b><br/> Latitude:" + lat + "<br /> Longitude:" + long + ""
    });
    infowindow.open(map, maker);
    //google.maps.event.addListener(marker, 'click', function () {
    //    // Calling the open method of the infoWindow 
    //    infowindow.open(map, marker);
    //});
}

function ContactUsMap()
{
    geocoder = new google.maps.Geocoder();
    var option = {
        center: new google.maps.LatLng(9.07648, 7.39857),
        zoom: 10,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        mapTypeControl: true,
        mapTypeControlOptions:
        {
            style: google.maps.MapTypeControlStyle.DROPDOWN_MENU,
            poistion: google.maps.ControlPosition.TOP_RIGHT,
            mapTypeIds: [google.maps.MapTypeId.ROADMAP,
              google.maps.MapTypeId.TERRAIN,
              google.maps.MapTypeId.HYBRID,
              google.maps.MapTypeId.SATELLITE]
        },
        navigationControl: true,
        navigationControlOptions:
        {
            style: google.maps.NavigationControlStyle.ZOOM_PAN
        },
        scaleControl: true,
        disableDoubleClickZoom: false,
        draggable: true,
        streetViewControl: true,
        draggableCursor: 'hand'

    };
    map = new google.maps.Map(document.getElementById('map_canvasContactUs'), option);
    var maker = new google.maps.Marker({
        position: new google.maps.LatLng(9.07648, 7.39857),
        map: map,
        title: 'Click me'

    });
    var infowindow = new google.maps.InfoWindow({
        content: "<b>Ihealth Nigeria GSFM Head Office </b><br/> Address: ABUJA, NIGERIA <br/> Latitude:" + 9.07648 + "<br /> Longitude:" + 7.39857 + ""
    });
    infowindow.open(map, maker);
} google.maps.event.addDomListener(window, 'load', ContactUsMap);
