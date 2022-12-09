var gmap;
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
var nofuelAirportTemplate = new Template("<div id='fuelContainer'><div id='fuelTop'></div><div id='fuelAptIdentContainer'><span style=\"font-weight: bold;\">#{AptCode}</span>" + sHTML + sHTML + sHTML + sHTML + "<span style=\"font-weight: bold;\">#{Approach}</span></div><div style='padding:5px;'><b>#{AptName}</b><br><b>Longest Runway:</b> #{RunwayLength} ft<br></div><div style='clear: both;'><div style='padding: 5px;'>#{FuelContainer}<div style='clear: both; font-size: 10pt; font-weight: bold;'>No fuel prices reported at this time</div></div><div style='clear: both;'></div><div style='padding: 5px;'>#{FuelBrandLogos}</div><div id='fuelLinksContainer'><div id='fuelLinksLeft'><a target='_blank' href='#{AirportURL}'>Airport Information</a><br>#{ExtraLinksLeft}</div><div id='fuelLinksRight'>#{ExtraLinksRight}</div><div style='clear: both;'></div></div><div id='fuelBottom'></div></div>");
var hasfuelAirportTemplate = new Template("<div id='fuelContainer'><div id='fuelTop'></div><div id='fuelAptIdentContainer'><span style=\"font-weight: bold;\">#{AptCode}</span>" + sHTML + sHTML + sHTML + sHTML + "<span style=\"font-weight: bold;\">#{Approach}</span></div><div style=''><div style='width: 100px; padding: 2px; float: left;'>#{FBOLogo}</div><div id='fuelFBOInfo'><b>Longest Runway:</b> #{RunwayLength} ft</div><div style='clear: both;'></div></div><div style='clear: both;'><div style='padding: 5px;'>#{FuelContainer}<div style='clear: both; font-size: 8pt;'><br>All Prices Include Taxes</div></div><div style='padding: 5px;'>#{FuelBrandLogos}</div><div id='fuelLinksContainer'><div id='fuelLinksLeft'><a target='_blank' href='#{AirportURL}'>Airport Information</a><br>#{ExtraLinksLeft}</div><div id='fuelLinksRight'>#{ExtraLinksRight}</div><br>#{ExtraLinkFBO}<div style='clear: both;'></div></div></div><div id='fuelBottom'></div></div>");
var fuelFBOTemplate = new Template("<div style=\"background-color:#ffffff;\" id='fuelTypeContainer'><div style=\"background-color:#ffffff;\" id='fuelType'><span style=\"background-color:#ffffff;\"><center><font color='black' style=\"font-size:8pt; font-weight: bold;\"><strong>#{FuelName}</strong></font></center></span></div><div id='fuelPrice'><center>#{FuelPrice}</center></div></div>");
var AptsInRadius;

function GetAptsAroundApt(AptCode, Range, Private, Public, Fac, Rwy, Approach, Jeta, ll100, Mogas, gUL94, FuelBrand, Saf) {
    var first_apt;
    var temp_1 = new Ajax.Request("/airport/_includes/ajax/getapt.aspx?", {
        method: "get",
        parameters: {
            aptcode: AptCode
        },
        onSuccess: function (e) {
            e.responseText = "[" + e.responseText + "]";
            first_apt = eval(e.responseText);
            if (first_apt[0] == null) {
                alert("Your fuel mapping search did not return any matching airports. Please try different search parameters.");
                HideLoadingIcon();
                hideSearchingIcon();
            }
        }
    });
    var temp_2 = new Ajax.Request("/airport/_includes/ajax/getaptinrange.aspx?", {
        method: "get",
        parameters: {
            aptcode: AptCode,
            range: Range,
            pr: Private,
            pu: Public,
            fac: Fac,
            rwy: Rwy,
            approach: Approach,
            jeta: Jeta,
            "100ll": ll100,
            mogas: Mogas,
            "UL94": gUL94,
            fuelbrand: FuelBrand,
            saf: Saf
        },
        onSuccess: function (e) {
            AptsInRadius = eval(e.responseText);
            if (AptsInRadius[0] != undefined) {
                AptsInRadius.unshift(first_apt[0]);
                DisplayAirports(AptsInRadius)
            } else {
                if (first_apt[0] != null) {
                    DisplayAirports(first_apt)
                }
            }
        }
    });
    PointDoubleClick = null
}

function GetAptInRadius(Lat, Lng, Range, Private, Public, Fac, Rwy, Approach, Jeta, ll100, Mogas, gUL94, FuelBrand, Saf) {
    PointDoubleClick = new GLatLng(Lat, Lng);
    var temp = new Ajax.Request("/airport/_includes/ajax/getaptinrange.aspx?", {
        method: "get",
        parameters: {
            lat: Lat,
            lng: Lng,
            range: Range,
            pr: Private,
            pu: Public,
            fac: Fac,
            rwy: Rwy,
            approach: Approach,
            jeta: Jeta,
            "100ll": ll100,
            mogas: Mogas,
            "UL94": gUL94,
            fuelbrand: FuelBrand,
            saf: Saf
        },
        onSuccess: function (e) {
            var AptsInRadius = eval(e.responseText);
            if (AptsInRadius[0] == null) {
                alert("Your fuel mapping search did not return any matching airports. Please try different search parameters.");
                HideLoadingIcon();
                hideSearchingIcon();
            } else {
                DisplayAirports(AptsInRadius)
            }
        }
    })
}

function DoubleClick(a) {
    var g = getUrlVars();
    var l = "false";
    var h = "false";
    if (g.av == "pu") {
        h = "true"
    } else {
        if (g.av == "pr") {
            l = "true"
        } else {
            h = "true";
            l = "true"
        }
    }
    var j = "";
    if (g.rad != null && g.rad != "") {
        j = g.rad
    }
    var b = "";
    if (g.FacType != null && g.FacType != "") {
        b = g.FacType
    }
    var c = "";
    if (g.rwy != null && g.rwy != "") {
        c = g.rwy
    }
    var m = "";
    if (g.app != null && g.app != "") {
        m = g.app
    }
    var k = "false";
    if (g.fueljeta != null && g.fueljeta != "") {
        k = "true"
    }
    var f = "false";
    if (g.fuel100ll != null && g.fuel100ll != "") {
        f = "true"
    }
    var n = "false";
    if (g.fuelUL94 != null && g.fuelUL94 != "") {
        n = "true"
    }
    var d = "false";
    if (g.fuelmogas != null && g.fuelmogas != "") {
        d = "true"
    }
    var e = "";
    if (g.fuelbrand != null && g.fuelbrand != "") {
        e = g.fuelbrand
    }
    var saf = "false";
    if (g.fuelsaf != null && g.fuelsaf != "") {
        saf = "true"
    }
    GetAptInRadius(a.lat(), a.lng(), j, l, h, b, c, m, k, f, d, n, e, saf)
}

function FindAirports() {
    var g = getUrlVars();
    var a = g.aptcode;
    var l = "false";
    var h = "false";
    if (g.av == "pu") {
        h = "true"
    } else {
        if (g.av == "pr") {
            l = "true"
        } else {
            h = "true";
            l = "true"
        }
    }
    var j = "";
    if (g.rad != null && g.rad != "") {
        j = g.rad
    }
    var b = "";
    if (g.FacType != null && g.FacType != "") {
        b = g.FacType
    }
    var c = "";
    if (g.rwy != null && g.rwy != "") {
        c = g.rwy
    }
    var m = "";
    if (g.app != null && g.app != "") {
        m = g.app
    }
    var k = "false";
    if (g.fueljeta != null && g.fueljeta != "") {
        k = "true"
    }
    var f = "false";
    if (g.fuel100ll != null && g.fuel100ll != "") {
        f = "true"
    }
    var n = "false";
    if (g.fuelUL94 != null && g.fuelUL94 != "") {
        n = "true"
    }
    var d = "false";
    if (g.fuelmogas != null && g.fuelmogas != "") {
        d = "true"
    }
    var e = "";
    if (g.fuelbrand != null && g.fuelbrand != "") {
        e = g.fuelbrand
    }
    var saf = "false";
    if (g.fuelsaf != null && g.fuelsaf != "") {
        saf = "true"
    }
    if (a != "") {
        GetAptsAroundApt(a, j, l, h, b, c, m, k, f, d, n, e, saf)
    } else {
        GetAptInRadius(gmap.getCenter().lat(), gmap.getCenter().lng(), j, l, h, b, c, m, k, f, d, n, e, saf)
    }
}

function SetZoomLevel() {
    var a = getUrlVars();
    var b = "50";
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
    HideLoadingIcon();
    //alert(B);
    var D = (document.URL.indexOf("marty") > -1);
    var a = getUrlVars();
    var c = "false";
    if (a.fueljeta != null && a.fueljeta != "") {
        c = "true"
    }
    var e = "false";
    if (a.fuel100ll != null && a.fuel100ll != "") {
        e = "true"
    }
    var m = "false";
    if (a.fuelUL94 != null && a.fuelUL94 != "") {
        m = "true"
    }
    var n = "false";
    if (a.fuelmogas != null && a.fuelmogas != "") {
        n = "true"
    }
    Airports = [];
    Airports = B;
    //$("FuelAptList").replace('<div id="FuelAptList"></div>');
    $("divFuelAptList").replace('<div id="divFuelAptList"><table id="fuelresults" class="table table-striped table-responsive"><thead><tr><th>Name</th><th>Identifier</th><th>Distance</th><th>FBO</th><th>Price</th><th>Longest Runway</th></tr></thead><tbody id="FuelAptList"><tbody></table></div>');
    while (overlays[0]) {
        overlays.pop().setMap(null)
    }
    Markers = [];
    var a = getUrlVars();
    if (PointDoubleClick == null) {
        gmap.setCenter(new google.maps.LatLng(B[0].lat, B[0].lng))
    } else {
        gmap.setCenter(PointDoubleClick)
    }
    SetZoomLevel();
    var y = "50";
    if (a.rad != null && a.rad != "") {
        y = a.rad
    }
    var l = new google.maps.LatLngBounds();
    drawCircle(new google.maps.LatLng(B[0].lat, B[0].lng), y);
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
        var j = B[i].lat;
        var F = B[i].lng;
        var u;
        var o;
        var b;
        if (B[i].fbo[0] == undefined) {
            o = false
        } else {
            o = true
        }
        var E;
        if (o) {
            var g = "/airport/marker.aspx?aptcode=" + B[i].apt + "&image=hasfuel";
            E = new google.maps.MarkerImage(g, new google.maps.Size(45, 13), new google.maps.Point(0, 0), new google.maps.Point(20, 12))
        } else {
            var g = "/airport/marker.aspx?aptcode=" + B[i].apt + "&image=nofuel";
            E = new google.maps.MarkerImage(g, new google.maps.Size(45, 13), new google.maps.Point(0, 0), new google.maps.Point(20, 12))
        }
        fuelMarker = DisplayMarker(j, F, E, i, B[i].apt);
        overlays.push(fuelMarker);
        fuelMarker.setMap(gmap);
        p = "";
        z = B[i].LowestSS100LL;
        f = B[i].Lowest100LL;
        t = B[i].LowestJETA;
        s = B[i].LowestSSJETA;
        q = B[i].LowestJETAPRIST;
        r = B[i].LowestSSJETAPRIST;
        k = B[i].LowestMOGAS;
        C = B[i].LowestUL94;
        saf = B[i].LowestSAF;
        sssaf = B[i].LowestSSSAF;
        safprist = B[i].LowestSAFPRIST;
        sssafprist = B[i].LowestSSSAFPRIST;
        overall100LL = B[i].Overall100LL;
        overallJETA = B[i].OverallJETA;
        overallSaf = B[i].OverallSAF;
        A = B[i].LowestPrice;
        if (v == "100.0000") {
            v = "N/A"
        } else {
            v = "$" + v
        }
        if (z == "100.0000") {
            z = "N/A"
        } else {
            z = "$" + z
        }
        if (t == "100.0000") {
            t = "N/A"
        } else {
            t = "$" + t
        }
        if (s == "100.0000") {
            s = "N/A"
        } else {
            s = "$" + s
        }
        if (q == "100.0000") {
            q = "N/A"
        } else {
            q = "$" + q
        }
        if (r == "100.0000") {
            r = "N/A"
        } else {
            r = "$" + r
        }
        if (k == "100.0000") {
            k = "N/A"
        } else {
            k = "$" + k
        }
        if (C == "100.0000") {
            C = "N/A"
        } else {
            C = "$" + C
        }
        if (f == "100.0000") {
            f = "N/A"
        } else {
            f = "$" + f
        }
        if (saf == "100.0000") {
            saf = "N/A"
        } else {
            saf = "$" + saf
        }
        if (sssaf == "100.0000") {
            sssaf = "N/A"
        } else {
            sssaf = "$" + sssaf
        }
        if (safprist == "100.0000") {
            safprist = "N/A"
        } else {
            safprist = "$" + safprist
        }
        if (sssafprist == "100.0000") {
            sssafprist = "N/A"
        } else {
            sssafprist = "$" + sssafprist
        }
        if (overall100LL == "100.0000") {
            overall100LL = "N/A"
        } else {
            overall100LL = "$" + overall100LL
        }
        if (overallJETA == "100.0000") {
            overallJETA = "N/A"
        } else {
            overallJETA = "$" + overallJETA
        }
        if (overallSaf == "100.0000") {
            overallSaf = "N/A"
        } else {
            overallSaf = "$" + overallSaf
        }
        if (A == "100.0000") {
            A = "N/A"
        } else {
            A = "$" + A
        }
        if (c == "true") {
            p = overallJETA
        } else {
            if (e == "true") {
                p = overall100LL
            } else {
                if (m == "true") {
                    p = C
                } else {
                    if (n == "true") {
                        p = k
                    } else {
                        p = A
                    }
                }
            }
        }
        if (p.length > 5) {
            p = p.substring(0, 5)
        }
        if (p.trim() == "$") {
            p = "";
        }
        if (B[i].dist == "|distance|") { } else {
            if (B[i].fbo[0] == undefined) { } else {
                var aurl = BuildAirportURL(B[i].apt, B[i].fac);
                //$("FuelAptList").insert('<span style="line-height:170%;"><a href="javascript:openInfoWindowLinked(\'' + B[i].apt + "', '" + i + "')\">(" + B[i].apt + ") " + B[i].fac + "</a> - " + p + " - " + parseInt(B[i].dist).toFixed(0) + " nm</span><br />");
                if (h > 0) {
                    $("FuelAptList").insert('<tr><td><a href="javascript:openInfoWindowLinked(\'' + B[i].apt + "', '" + i + "')\">" + B[i].fac + "</a>" + "</td><td><a href='" + aurl + "'>" + B[i].apt +"</a></td><td>" + parseInt(B[i].dist).toFixed(0) + " nm</td><td>" + B[i].fbo[0].comp + "</td><td>" + p + "</td><td>" + B[i].rwy.length + " ft</td></tr>");
                }
                else {
                    $("FuelAptList").insert('<tr><td><a href="javascript:openInfoWindowLinked(\'' + B[i].apt + "', '" + i + "')\">" + B[i].fac + "</a><br><span id='tbInfo'>Click for map<span>" + "</td><td><a href='" + aurl + "'>" + B[i].apt+"</a><br><span id='tbInfo'>Click for Airport<span></td><td>" + parseInt(B[i].dist).toFixed(0) + " nm</td><td>" + B[i].fbo[0].comp + "</td><td>" + p + "</td><td>" + B[i].rwy.length + " ft</td></tr>");
                }
                h = h + 1
            }
        }
    }
    if (h > 0) {
        var ma = document.all("MatchingAirports");
        if (ma != null) ma.innerHTML = h;
        $("FuelAptHeader").insert('<h2><strong>Results are listed by Lowest Fuel Price:</strong></h2>')
    }
    var w = "";
    if (h == 0) {
        w = "<strong style='color:#b2d743'>No Matching Results</strong><br>"
    } else {
        if (h == 1) {
            w = "<strong style='color:#b2d743'>1 Matching Result</strong><br>"
        } else {
            if (h > 1) {
                w = "<strong style='color:#b2d743'>" + h.toString() + " Matching Results</strong><br>"
            }
        }
    }
    $("FuelAptHeader").insert(w);
    HideLoadingIcon();
    hideSearchingIcon();
    //$("FuelAptList").insert("<br />" + ad);
    var d = "";
    if (a.aptcode != null && a.aptcode != "") {
        d = a.aptcode
    }
    if (d != "") {
        openInfoWindowLinked(B[0].apt, 0)
    }
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

function DisplayMarker(e, d, f, a, c) {
    var b = new google.maps.Marker({
        position: new google.maps.LatLng(e, d),
        map: gmap,
        icon: f
    });
    Markers[c] = b;
    google.maps.event.addListener(b, "click", function () {
        openInfoWindow(b, a, c)
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

function openInfoWindowLinked(b, a) {
    if (Markers[b] != undefined) {
        openInfoWindow(Markers[b], a, b)
    } else { }
}

function openInfoWindow(G, b, v) {
    if (999 != b) {
        var s;
        var C = 0;
        var r = Airports[b].Lowest100LL;
        var x = Airports[b].LowestSS100LL;
        var m = Airports[b].LowestJETA;
        var j = Airports[b].LowestSSJETA;
        var g = Airports[b].LowestJETAPRIST;
        var h = Airports[b].LowestSSJETAPRIST;
        var e = Airports[b].LowestMOGAS;
        var B = Airports[b].LowestUL94;
        var Saf = Airports[b].LowestSAF;
        var SSSaf = Airports[b].LowestSSSAF;
        var SafPRIST = Airports[b].LowestSAFPRIST;
        var SSSafPRIST = Airports[b].LowestSSSAFPRIST;
        if (Saf == null || Saf == "100.0000") {
            Saf = "N/A"
        } else {
            Saf = "$" + Saf.substring(0, 4)
        }
        if (SSSaf == null || SSSaf == "100.0000") {
            SSSaf = "N/A"
        } else {
            SSSaf = "$" + SSSaf.substring(0, 4)
        }
        if (SafPRIST == null || SafPRIST == "100.0000") {
            SafPRIST = "N/A"
        } else {
            SafPRIST = "$" + SafPRIST.substring(0, 4)
        }
        if (SSSafPRIST == null || SSSafPRIST == "100.0000") {
            SSSafPRIST = "N/A"
        } else {
            SSSafPRIST = "$" + SSSafPRIST.substring(0, 4)
        }
        if (r == null || r == "100.0000") {
            r = "N/A"
        } else {
            r = "$" + r.substring(0, 4)
        }
        if (x == null || x == "100.0000") {
            x = "N/A"
        } else {
            x = "$" + x.substring(0, 4)
        }
        if (m == null || m == "100.0000") {
            m = "N/A"
        } else {
            m = "$" + m.substring(0, 4)
        }
        if (j == null || j == "100.0000") {
            j = "N/A"
        } else {
            j = "$" + j.substring(0, 4)
        }
        if (g == null || g == "100.0000") {
            g = "N/A"
        } else {
            g = "$" + g.substring(0, 4)
        }
        if (h == null || h == "100.0000") {
            h = "N/A"
        } else {
            h = "$" + h.substring(0, 4)
        }
        if (e == null || e == "100.0000") {
            e = "N/A"
        } else {
            e = "$" + e.substring(0, 4)
        }
        if (B == null || B == "100.0000") {
            B = "N/A"
        } else {
            B = "$" + B.substring(0, 4)
        }
        if (r != "N/A") {
            var D = {
                FuelName: "100LL",
                FuelPrice: r
            };
            var q = fuelFBOTemplate.evaluate(D)
        } else {
            var q = ""
        }
        if (x != "N/A") {
            var D = {
                FuelName: "SS100LL",
                FuelPrice: x
            };
            var E = fuelFBOTemplate.evaluate(D)
        } else {
            var E = ""
        }
        if (m != "N/A") {
            var D = {
                FuelName: "JETA",
                FuelPrice: m
            };
            var l = fuelFBOTemplate.evaluate(D)
        } else {
            var l = ""
        }
        if (Saf != "N/A") {
            var D = {
                FuelName: "SAF",
                FuelPrice: Saf
            };
            var ss = fuelFBOTemplate.evaluate(D)
        } else {
            var ss = ""
        }
        if (SSSaf != "N/A") {
            var D = {
                FuelName: "SSSAF",
                FuelPrice: SSSaf
            };
            var sss = fuelFBOTemplate.evaluate(D)
        } else {
            var sss = ""
        }
        if (SafPRIST != "N/A") {
            var D = {
                FuelName: "SAF+",
                FuelPrice: SafPRIST
            };
            var ssprist = fuelFBOTemplate.evaluate(D)
        } else {
            var ssprist = ""
        }
        if (SSSafPRIST != "N/A") {
            var D = {
                FuelName: "SSSAF+",
                FuelPrice: SSSafPRIST
            };
            var sssprist = fuelFBOTemplate.evaluate(D)
        } else {
            var sssprist = ""
        }
        if (j != "N/A") {
            var D = {
                FuelName: "SSJETA",
                FuelPrice: j
            };
            var w = fuelFBOTemplate.evaluate(D)
        } else {
            var w = ""
        }
        if (g != "N/A") {
            var D = {
                FuelName: "JETA+",
                FuelPrice: g
            };
            var f = fuelFBOTemplate.evaluate(D)
        } else {
            var f = ""
        }
        if (h != "N/A") {
            var D = {
                FuelName: "SSJETA+",
                FuelPrice: h
            };
            var o = fuelFBOTemplate.evaluate(D)
        } else {
            var o = ""
        }
        if (B != "N/A") {
            var D = {
                FuelName: "UL94",
                FuelPrice: B
            };
            var z = fuelFBOTemplate.evaluate(D)
        } else {
            var z = ""
        }
        if (e != "N/A") {
            var D = {
                FuelName: "MOGAS",
                FuelPrice: e
            };
            var t = fuelFBOTemplate.evaluate(D)
        } else {
            var t = ""
        }
        if (Airports[b].fbo[0] == undefined) {
            var a = BuildAirportURL(Airports[b].apt, Airports[b].fac);
            var k = {
                AptCode: Airports[b].apt,
                Approach: Airports[b].approach,
                AptName: Airports[b].fac,
                RunwayLength: Airports[b].rwy.length,
                StopNum: 0,
                FuelBrandLogos: Airports[b].fuelbrandlogos,
                ExtraLinksLeft: "",
                ExtraLinksRight: "",
                AirportURL: a
            };
            s = nofuelAirportTemplate.evaluate(k);
            var n = new google.maps.InfoWindow({
                content: s,
                pixelOffset: new google.maps.Size(0, 0)
            });
            CloseAllInfoWindows();
            SaveInfoWindow(n);
            n.open(gmap, G)
        } else {
            var p = Airports[b].fbo.length;
            var c = "";
            if (p == 1) {
                c = " (1 FBO)";

            } else {
                if (p > 1) {
                    c = " (" + p.toString() + " FBOs)";
                }
            }
            var d = '<span style="font-weight:bold;">Lowest prices on the field' + c + ":</span><br><br>" + q + E + l + w + '<div style="clear:left;"></div>' + f + o + z + t + '<div style="clear:left;"></div>' + ss + sss + ssprist + sssprist;
            var y = BuildAirportURL(Airports[b].apt, Airports[b].fac);
            var F = y + "#fuel";
            var u = Airports[b].CompLogo;
            if (u != "N/A" && u != "" && u != "n/a") {
                u = "https://resources.globalair.com/airport/images/complogo/" + u;
                u = "<a target='_blank' href='" + y + "'><img class='bubbleLogo' src='" + u + "' alt='" + Airports[b].fac + "' border='0' style='float: left; width: 100px;'/></a>"
            } else {
                u = ""
            }
            var A;
            if (p > 1) {
                A = {
                    AptCode: Airports[b].apt.toUpperCase(),
                    Approach: Airports[b].approach,
                    AptName: Airports[b].fac,
                    RunwayLength: Airports[b].rwy.length,
                    StopNum: 0,
                    FBOName: "",
                    FBOPhone: "",
                    FBOLogo: u,
                    FBOArinc: "",
                    FuelContainer: d,
                    FuelBrandLogos: Airports[b].fuelbrandlogos,
                    ExtraLinksLeft: "",
                    ExtraLinksRight: '<div style="border-left: 1px #555555 solid;"><div style="margin-left:5px;"><a href="' + F + '" target="_blank">Fuel Prices</a></div></div>',
                    AirportURL: y,
                    ExtraLinkFBO: '<br><a target="_blank" href="/airport/apt.compare.aspx?aptcode=' + Airports[b].apt + '">Compare Fbo</a>'
                };
            } else {
                A = {
                    AptCode: Airports[b].apt.toUpperCase(),
                    Approach: Airports[b].approach,
                    AptName: Airports[b].fac,
                    RunwayLength: Airports[b].rwy.length,
                    StopNum: 0,
                    FBOName: "",
                    FBOPhone: "",
                    FBOLogo: u,
                    FBOArinc: "",
                    FuelContainer: d,
                    FuelBrandLogos: Airports[b].fuelbrandlogos,
                    ExtraLinksLeft: "",
                    ExtraLinksRight: '<div style="border-left: 1px #555555 solid;"><div style="margin-left:5px;"><a href="' + F + '" target="_blank">Fuel Prices</a></div></div>',
                    AirportURL: y,
                    ExtraLinkFBO: ""
                };
            }
            s = hasfuelAirportTemplate.evaluate(A);
            var n = new google.maps.InfoWindow({
                content: s,
                pixelOffset: new google.maps.Size(0, 0)
            });
            document.getElementById("map").scrollIntoView();
            CloseAllInfoWindows();
            SaveInfoWindow(n);
            n.open(gmap, G)
        }
        openedInfoWindow = b
    }
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

function Init_Map() {
    gmap = new google.maps.Map(document.getElementById("map"), {
        center: { lat: 37.71859, lng: -98.701172 },
        mapTypeId: 'roadmap',
        zoom: 4,
        disableDoubleClickZoom: true,
        scrollwheel: true,
        mapTypeControl: true,
        scaleControl: true,
        navigationControl: true,
        navigationControlOptions: {
            style: google.maps.NavigationControlStyle.ZOOM_PAN
        },
        mapTypeControlOptions: {
            mapTypeIds: [google.maps.MapTypeId.ROADMAP, google.maps.MapTypeId.HYBRID, google.maps.MapTypeId.SATELLITE, google.maps.MapTypeId.TERRAIN],
            position: google.maps.ControlPosition.TOP_LEFT
        }
    });
    google.maps.event.addListener(gmap, "dblclick", function (n) {
        DoubleClick(n.latLng)
    });
    newBounds = new google.maps.LatLngBounds();
    noFuelIconLabeled = new google.maps.MarkerImage("/airport/images/fuelmap/no_fuel_png.png", new google.maps.Size(45, 13), new google.maps.Point(0, 0), new google.maps.Point(20, 12));
    FuelIconLabeled = new google.maps.MarkerImage("/airport/images/fuelmap/has_fuel_png.png", new google.maps.Size(45, 13), new google.maps.Point(0, 0), new google.maps.Point(20, 12));
    var c = getUrlVars();
    if (c.aptcode != null && c.aptcode != "") {
        var k;
        var d;
        var b;
        var f;
        var h;
        var g;
        var e;
        var m;
        var l;
        var j;
        var a;
        var saf;
        if (c.av == "all") {
            k = true;
            d = true
        } else {
            if (c.av == "pu") {
                k = false;
                d = true
            } else {
                if (c.av == "pr") {
                    k = true;
                    d = false
                } else {
                    k = false;
                    d = true
                }
            }
        }
        if (c.rad == null) {
            f = 50
        } else {
            f = getUrlRad(c.rad)
        }
        if (c.FacType == null) {
            b = "airport"
        } else {
            b = c.FacType
        }
        if (c.rwy == null) {
            h = "any"
        }
        if (c.rwy == "lt5000") {
            h = "lt5000"
        } else {
            if (c.rwy == "gt5000") {
                h = "gt5000"
            } else {
                h = "any"
            }
        }
        if (c.app == null) {
            g = ""
        }
        if (c.app == "ils") {
            g = "ils"
        } else {
            if (c.app == "instrument") {
                g = "instrument"
            } else {
                g = ""
            }
        }
        if (c.fueljeta == "true") {
            e = "true"
        } else {
            e = "false"
        }
        if (c.fuel100ll == "true") {
            m = "true"
        } else {
            m = "false"
        }
        if (c.fuelmogas == "true") {
            l = "true"
        } else {
            l = "false"
        }
        if (c.fuelUL94 == "true") {
            j = "true"
        } else {
            j = "false"
        }
        if (c.fuelsaf == "true") {
            saf = "true"
        } else {
            saf = "false"
        }
        if (c.fuelbrand == "airbp") {
            a = "airbp"
        } else {
            if (c.fuelbrand == "avfuel") {
                a = "avfuel"
            } else {
                if (c.fuelbrand == "chevron") {
                    a = "chevron"
                } else {
                    if (c.fuelbrand == "exxonmobil") {
                        a = "exxonmobil"
                    } else {
                        if (c.fuelbrand == "phillips66") {
                            a = "phillips66"
                        } else {
                            if (c.fuelbrand == "shell") {
                                a = "shell"
                            } else {
                                if (c.fuelbrand == "texaco") {
                                    a = "texaco"
                                } else {
                                    if (c.fuelbrand == "independent") {
                                        a = "independent"
                                    } else {
                                        a = ""
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        GetAptsAroundApt(c.aptcode, f, k, d, b, h, g, e, m, l, j, a, saf)
    }
    if (c.city != null && c.state != null) {
        geocoder = new google.maps.Geocoder();
        geocoder.geocode(c.city + ", " + c.state, function (v, r) {
            var x;
            var q;
            var o;
            var s;
            var u;
            var t;
            var p;
            var z;
            var y;
            var w;
            var n;
            var saf;
            if (c.nearest != null && c.nearest != "") { }
            if (c.av == "all") {
                x = true;
                q = true
            } else {
                if (c.av == "pu") {
                    x = false;
                    q = true
                } else {
                    if (c.av == "pr") {
                        x = true;
                        q = false
                    } else {
                        x = false;
                        q = false
                    }
                }
            }
            s = getUrlRad(c.rad);
            if (c.FacType == null) {
                o = "airport"
            } else {
                o = c.FacType
            }
            if (c.FacType == "airport") { } else {
                if (c.FacType == "balloonport") { } else {
                    if (c.FacType == "heliport") { } else {
                        if (c.FacType == "seaplane") { } else {
                            if (c.FacType == "stolport") { } else { }
                        }
                    }
                }
            }
            if (c.rwy == null) {
                u = "any"
            }
            if (c.rwy == "lt5000") {
                u = "lt5000"
            } else {
                if (c.rwy == "gt5000") {
                    u = "gt5000"
                } else {
                    u = "any"
                }
            }
            if (c.app == null) {
                t = "all"
            }
            if (c.app == "ils") {
                t = "ils"
            } else {
                if (c.app == "instrument") {
                    t = "instrument"
                } else {
                    t = "all"
                }
            }
            if (c.fueljeta == "true") {
                p = "true"
            } else {
                p = "false"
            }
            if (c.saf == "true") {
                saf = "true"
            } else {
                saf = "false"
            }
            if (c.fuel100ll == "true") {
                z = "true"
            } else {
                z = "false"
            }
            if (c.fuelmogas == "true") {
                y = "true"
            } else {
                y = "false"
            }
            if (c.fuelUL94 == "true") {
                w = "true"
            } else {
                w = "false"
            }
            if (c.fuelsaf == "true") {
                saf = "true"
            } else {
                saf = "false"
            }
            if (c.fuelbrand == "airbp") {
                n = "airbp"
            } else {
                if (c.fuelbrand == "avfuel") {
                    n = "avfuel"
                } else {
                    if (c.fuelbrand == "chevron") {
                        n = "chevron"
                    } else {
                        if (c.fuelbrand == "exxonmobil") {
                            n = "exxonmobil"
                        } else {
                            if (c.fuelbrand == "phillips66") {
                                n = "phillips66"
                            } else {
                                if (c.fuelbrand == "shell") {
                                    n = "shell"
                                } else {
                                    if (c.fuelbrand == "texaco") {
                                        n = "texaco"
                                    } else {
                                        if (c.fuelbrand == "independent") {
                                            n = "independent"
                                        } else {
                                            n = ""
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (r == google.maps.GeocoderStatus.OK) {
                GetAptInRadius(v[0].geometry.location.lat(), v[0].geometry.location.lng(), s, x, q, o, u, t, p, z, y, w, n, saf)
            } else { }
        })
    }
    //$("FuelAptList").insert("<br />" + ad)
}

function weatherToggle_Click() {
    if (weatherLayer.getMap() != null) {
        weatherLayer.setMap(null);
        document.getElementById("weatherToggle").innerText = "Show Weather"
    } else {
        weatherLayer.setMap(gmap);
        document.getElementById("weatherToggle").innerText = "Hide Weather"
    }
}

function banmanPro() {
    var d = "";
    var a = navigator.appName;
    var f = (new Date()).getTime();
    var e = parseInt(navigator.appVersion);
    var b = navigator.userAgent.toLowerCase();
    var c = "";
    if (a == "Netscape") {
        if (e >= 5) {
            d = '<iframe src="/banmanpro/banman.asp?ZoneID=34&Task=Get&Browser=NETSCAPE6&X=' + f + '" width=241 height=263 Marginwidth=0 Marginheight=0 Hspace=0 Vspace=0 Frameborder=0 Scrolling=No></iframe>'
        } else {
            if ((e >= 4) && (b.indexOf("mac") == -1)) {
                d = '<SCRIPT src="/banmanpro/banman.asp?ZoneID=34&Task=Get&Browser=NETSCAPE4"><\/script>'
            } else {
                if (e >= 3) {
                    d = '<A HREF="/banmanpro/banman.asp?ZoneID=34&Task=Click&Mode=HTML&PageID=' + f + "&RandomNumber=" + f + '" target="_new"><IMG SRC="/banmanpro/banman.asp?ZoneID=34&Task=Get&Mode=HTML&PageID=' + f + "&RandomNumber=" + f + '" width="241" height="263" border="0"></A>'
                }
            }
        }
    }
    if (a == "Microsoft Internet Explorer") {
        d = '<iframe src="/banmanpro/banman.asp?ZoneID=34&Task=Get&X=' + f + '" width=241 height=263 Marginwidth=0 Marginheight=0 Hspace=0 Vspace=0 Frameborder=0 Scrolling=No></iframe>'
    }
    return d
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

function showSearchingIcon(a) {
    searchingIcon = true;
    Modalbox.show(getSearchingIconBox(a), {
        title: "GlobalAir.com",
        width: 300,
        overlayClose: false,
        slideDownDuration: 0,
        slideUpDuration: 0,
        resizeDuration: 0
    })
}

function hideSearchingIcon() {
    if (searchingIcon) {
        Modalbox.hide();
        searchingIcon = false
    }
}

function getSearchingIconBox(a) {
    return "<div align='center'><img src='/airport/images/loading.gif' width='100' height='100' /><br /><br /><b>" + a + "</b></div>"
};

function HideLoadingIcon() {
    document.getElementById('loadingContainer').style.display = "none";
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