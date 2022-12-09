////var gmap;
var newBounds = null;
var PointDoubleClick;
var Markers = [];
//var ad = "<div align='center'>" + banmanPro() + "</div>";
var Airports = [];
var openedInfoWindow;
var searchingIcon = false;
var sHTML = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
var overlays = new Array();
var infoWindows = new Array();
var fuelMarker;
var weatherLayer;
var AptsInRadius;


function GetAptInRadius(Lat, Lng, Range) {
    //console.log("GetAptInRadius " + Lat + " - " + Lng + " - " + Range);
    //PointDoubleClick = new GLatLng(Lat, Lng);

    //var temp = new Ajax.Request("/airport/_includes/ajax/getaptinrange.aspx?", {
    //    method: "get",
    //    parameters: {
    //        lat: Lat,
    //        lng: Lng,
    //        range: Range,
    //        //pr: Private,
    //        //pu: Public,
    //        fac: 'Airport',
    //        //rwy: Rwy,
    //        //approach: Approach,
    //        //jeta: Jeta,
    //        //"100ll": ll100,
    //        //mogas: Mogas,
    //        //"UL94": gUL94,
    //        //fuelbrand: FuelBrand,
    //        //saf: Saf
    //    },
    //    onSuccess: function (e) {
    //        var AptsInRadius = eval(e.responseText);

    //        console.log("Range: " + Range);
    //        console.log(AptsInRadius);

    //        if (AptsInRadius.length <= 0 && Range <= 250) {
    //            Range = Range + 50;
    //            GetAptInRadius(Lat, Lng, Range);
    //        }
    //        else if (AptsInRadius[0] == null) {
    //            console.log("Your fuel mapping search did not return any matching airports. Please try different search parameters.");
    //            //HideLoadingIcon();
    //            //hideSearchingIcon();
    //        } else {
    //            DisplayAirports(AptsInRadius)
    //        }
    //    }
    //})

    $.ajax({
        url: '/airport/_includes/ajax/getaptinrange.aspx?',
        type: 'GET',
        data: {
            lat: Lat,
            lng: Lng,
            range: Range,
            //pr: Private,
            pu: 'true',
            fac: 'Airport',
            //rwy: Rwy,
            //approach: Approach,
            //jeta: Jeta,
            //"100ll": ll100,
            //mogas: Mogas,
            //"UL94": gUL94,
            //fuelbrand: FuelBrand,
            //saf: Saf
        },
        success: function (e) {
            var AptsInRadius = eval(e);

            //console.log("Range: " + Range);
            //console.log(AptsInRadius);

            if (AptsInRadius.length <= 0 && Range <= 250) {
                Range = Range + 50;
                GetAptInRadius(Lat, Lng, Range);
            }
            else if (AptsInRadius[0] == null) {
                console.log("Your fuel mapping search did not return any matching airports. Please try different search parameters.");
                //HideLoadingIcon();
                //hideSearchingIcon();
            } else {
                DisplayAirports(AptsInRadius)
            }

        },
        error: function (xhr, exception) {
            console.log("error");
        }
    });
}



function SetZoomLevel() {
    var a = getUrlVars();
    var b = "30";
    if (a.rad != null && a.rad != "") {
        b = a.rad
    }
    ZoomDistance = b;
    switch (parseInt(ZoomDistance)) {
        case 10:
            gmap.setZoom(11);
            break;
        case 20:
            gmap.setZoom(10);
            break;
        case 30:
            gmap.setZoom(9);
            break;
        case 40:
            gmap.setZoom(9);
            break;
        case 50:
            gmap.setZoom(8);
            break;
        case 60:
            gmap.setZoom(8);
            break;
        case 70:
            gmap.setZoom(8);
            break;
        case 80:
            gmap.setZoom(8);
            break;
        case 90:
            gmap.setZoom(8);
            break;
        case 100:
            gmap.setZoom(7);
            break;
        default:
            gmap.setZoom(7);
            break
    }
}

function DisplayAirports(B) {
    //HideLoadingIcon();
    //alert(B);
    var D = (document.URL.indexOf("marty") > -1);
    var a = getUrlVars();
    
    Airports = [];
    Airports = B;
    //$("FuelAptList").replace('<div id="FuelAptList"></div>');
    //$("divFuelAptList").replace('<div id="divFuelAptList"><table id="fuelresults" class="table table-striped table-responsive"><thead><tr><th>Name</th><th>Identifier</th><th>Distance</th><th>FBO</th><th>Price</th><th>Longest Runway</th></tr></thead><tbody id="FuelAptList"><tbody></table></div>');
    while (overlays[0]) {
        overlays.pop().setMap(null)
    }
    Markers = [];
    var a = getUrlVars();
    //if (PointDoubleClick == null) {
    //    gmap.setCenter(new google.maps.LatLng(B[0].lat, B[0].lng))
    //} else {
    //    gmap.setCenter(PointDoubleClick)
    //}
    SetZoomLevel();//3516
    var y = "50";
    if (a.rad != null && a.rad != "") {
        y = a.rad
    }
    var l = new google.maps.LatLngBounds();

    //drawCircle(new google.maps.LatLng(B[0].lat, B[0].lng), y);//3516
    var h = 0;
    var v = "";
    var z;
    var t;
    var s;
    var q;
    var r;
    var k;
    var C;
    var p = "";
    var f = "";
    var overallJETA;
    var A;
    var saf;
    var sssaf;
    var safprist;
    var sssafprist;
    var overall100LL;
    var overallSaf;
    for (i = 0; i < B.length; i++) {
        //if (i > 30) return;//3516
        var j = B[i].lat;
        var F = B[i].lng;
        var u;
        var o;
        var b;
        //if (B[i].fbo[0] == undefined) {
        //    o = false
        //} else {
        //    o = true
        //}
        var E;
        //if (o) {
            var g = "images/aeroplane.png";
            //E = new google.maps.MarkerImage(g, new google.maps.Size(45, 13), new google.maps.Point(0, 0), new google.maps.Point(20, 12))
            E = new google.maps.MarkerImage(g, new google.maps.Size(48, 48), new google.maps.Point(0, 0), new google.maps.Point(20, 12))
        //} else {
        //    var g = "/airport/marker.aspx?aptcode=" + B[i].apt + "&image=nofuel";
        //    E = new google.maps.MarkerImage(g, new google.maps.Size(45, 13), new google.maps.Point(0, 0), new google.maps.Point(20, 12))
        //}
            fuelMarker = DisplayMarker(j, F, E, i, B[i].apt, B[i].fac);
        overlays.push(fuelMarker);
        fuelMarker.setMap(gmap);
        //p = "";
        //console.log("done");
        
    }
    //HideLoadingIcon();
    //hideSearchingIcon();
    
}

function CloseSingleInfoWindow(a) {
    a.close()
}

function SaveInfoWindow(a) {
    infoWindows.push(a)
}

function CloseAllInfoWindows() {
    while (infoWindows[0]) {
        CloseSingleInfoWindow(infoWindows.pop())
    }
    openedInfoWindow = null
}

function DisplayMarker(e, d, f, a, c, title) {
    var b = new google.maps.Marker({
        position: new google.maps.LatLng(e, d),
        map: gmap,
        icon: f,
        title: title
    });
    Markers[c] = b;
    google.maps.event.addListener(b, "click", function () {
        window.location.href = BuildAirportURL(c, title);
    });
    return b
}

function BuildAirportURL(b, c) {
    var a = "";
    c = c.toLowerCase();
    b = b.toLowerCase();
    c = c.split(" ").join("-");
    c = c.split("/").join("-");
    c = c.split("&").join("and");
    c = c.split(".").join("");
    c = c.split(",").join("");
    c = c.split("'").join("");
    c = c.split("-intl").join("-international");
    c = c.split("-muni").join("-municipal");
    c = c.split("-municipalcipal").join("-municipal");
    c = c.split("-rgnl").join("-regional");
    a = "/airport/";
    a = a + c;
    a = a + "-";
    a = a + b + ".aspx";
    a = a.toLowerCase();
    return a
}




function drawCircle(o, m) {
    var l = "#3366ff";
    var g = 5;
    var q = m;
    var h = Math.PI / 180;
    var b = 180 / Math.PI;
    var d = (q / 3444) * b;
    var e = d / Math.cos(o.lat() * h);
    var f = [];
    for (var j = 0; j < 33; j++) {
        var c = Math.PI * (j / 16);
        var a = o.lng() + (e * Math.cos(c));
        var p = o.lat() + (d * Math.sin(c));
        var k = new google.maps.LatLng(p, a);
        newBounds.extend(k);
        f.push(k)
    }
    var n = new google.maps.Polyline({
        path: f,
        strokeColor: l,
        strokeWeight: g
    });
    overlays.push(n);
    n.setMap(gmap)
}




function getUrlRad(a) {
    if (a == null) {
        range = 10
    } else {
        var b;
        switch (parseInt(a)) {
            case 10:
                b = 0;
                break;
            case 20:
                b = 1;
                break;
            case 30:
                b = 2;
                break;
            case 40:
                b = 3;
                break;
            case 50:
                b = 4;
                break;
            case 60:
                b = 5;
                break;
            case 70:
                b = 6;
                break;
            case 80:
                b = 7;
                break;
            case 90:
                b = 8;
                break;
            case 100:
                b = 9;
                break
        }
        range = a
    }
    return range
}

function getUrlVars() {
    var e = [],
        d;
    var b;
    var a = window.location.href.slice(window.location.href.indexOf("?") + 1).split("&");
    if (a.length > 0) {
        for (var c = 0; c < a.length; c++) {
            d = a[c].split("=");
            e.push(d[0]);
            if (d[1] != null) {
                b = d[1];
                b = b.replace("%20", " ");
                e[d[0]] = b
            }
        }
    }
    return e
}


var ganinit = false;
var ganhist = false;

jQuery(function () {
    jQuery("#map").bind("loadmap", function () {
        $(this).removeClass("mapready").addClass("maploaded");
        var a = new google.maps.Map(this, myOptions)
    }).bind("unloadmap", function () {
        $(this).empty().removeClass("maploaded").addClass("mapready")
    });

    // Init_Map();
})