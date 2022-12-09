function createCookie(name, value, days) {
    var expires;

    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toGMTString();
    } else {
        expires = "";
    }
    //document.cookie = encodeURIComponent(name) + "=" + value + expires + ";path=/";
    document.cookie = encodeURIComponent(name) + "=" + value + expires + ";domain=www.globalair.com;path=/;samesite=lax";
}

function readCookie(name) {
    //if (name == "AccountTypes" || name == "GlobalAir") {

    //    var value = getSecure(name);
    //    //console.log(value);
    //    if (value == "")
    //        return null;
    //    else
    //        return value;
    //}
    //else {
        name += '=';
        for (var ca = document.cookie.split(/;\s*/), i = ca.length - 1; i >= 0; i--)
            if (!ca[i].indexOf(name))
                return ca[i].replace(name, '');
    //}

    return null;
}

function getSecure(name) {
    var value = "";
    jQuery.ajax({
        async: false,
        type: 'POST',
        data: { name },
        url: '/Home/GetSecure',
        cache: false,
        dataType: 'json',
        success: function (response) {
            value = response.value;

        },
        error: function (response) {    // if unhandled error on server

        }
    });

    return value;
}

function eraseSecureCookie(name) {
    //var value = "";
    jQuery.ajax({
        //async: false,
        type: 'POST',
        data: { name },
        url: '/Home/EraseSecureCookie',
        cache: false,
        dataType: 'json',
        success: function (response) {
            //value = response.value;

        },
        error: function (response) {    // if unhandled error on server

        }
    });

    //return value;
}

function eraseCookie(name) {
    date = new Date();
    date.setTime(date.getTime() + (-1 * 24 * 60 * 60 * 1000));
    expires = "; expires=" + date.toGMTString();
    document.cookie = encodeURIComponent(name) + "=;" + expires + ";path=/";
    document.cookie = encodeURIComponent(name) + "=;" + expires + ";domain=www.globalair.com;path=/";
}

// Cookie notification banner
var jQuerycookieBanner = jQuery('#cookie-notification');
if (readCookie('cookie-notification') == null) {
    jQuerycookieBanner.css('display', 'flex');
}
jQuerycookieBanner.find('#cookie-notification-btn').on('click', function (e) {
    e.preventDefault();
    jQuerycookieBanner.hide();
    createCookie('cookie-notification', 'true', 365);
});

// Check Login
lc = readCookie("AccountTypes");
if (lc != null) {
    jQuery("#logged-in").css("display", "inline-block");
    lp = lc.split("|");
    a4sid = lp[0];
    fboid = lp[1];
    if (lp.length >= 3) jQuery("#first-name").html(lp[2]);
    else {
        uid = readCookie("GlobalAir");
        jQuery("#first-name").load("/GetLoggedInName", { userId: parseInt(uid) }, function () {
            createCookie("AccountTypes", lc + "|" + jQuery("#first-name").text(), 30);
        });
    }
    if (a4sid == "" || a4sid == "0") jQuery("#dboard-link").parent().hide();
    if (fboid != "" && fboid != "0") jQuery("#fbo-link").show();
    if ((a4sid == "" || a4sid == "0") && (fboid == "" || fboid == "0")) jQuery("#dboard-link").attr("href", "/login.aspx").parent().show();
}
else jQuery("#logged-out").css("display", "inline-block");

// Create cookie if coming from email so we don't show subscribe modal
//if (location.href.indexOf("utm_source=") > -1) {
//    createCookie("subscribe-box", "true", 365);
//}

// Subscribe Modal
//setTimeout(showSubscribeModal, 10000);

//jQuery('#subscribeModal').on('hidden.bs.modal', function (e) {
//    createCookie('subscribe-box-session', 'true', 1/48); // 1/2 hour
//});

// Modal
function gamodal(obj, arg) {
    if (arg == "show") {
        obj.addClass("show");
        jQuery(".modal-backdrop").addClass("show");
    }
    else if (arg == "hide") {
        obj.removeClass("show");
        jQuery(".modal-backdrop").removeClass("show");
    }

    return obj;
}
jQuery.fn.gamodal = function gamodal(arg) {
    if (arg == "show") {
        jQuery(this).addClass("show");
        jQuery(".modal-backdrop").addClass("show");
    }
    else if (arg == "hide") {
        jQuery(this).removeClass("show");
        jQuery(".modal-backdrop").removeClass("show");
    }

    return jQuery(this);
};

jQuery(document).on("click", "[data-toggler]", function () {
    event.preventDefault();
    var toggle = jQuery(this).data("toggler");
    var target = jQuery(this).data("target");
    var targetItem = jQuery(target);

    if (toggle == "modal") {
        gamodal(targetItem, "show");
        targetItem.trigger("show", [jQuery(this)]);
        toggle = "show";
    }
    else {
        targetItem.toggleClass(toggle);
    }
});

jQuery(document).on("click", ".modal, [data-dismiss='modal']", function () {
    if (jQuery(event.target).hasClass("modal"))
        jQuery(".modal-backdrop, .modal").removeClass("show");
});

jQuery(document).on("click", "[data-dismiss='modal']", function () {
    jQuery(".modal-backdrop, .modal").removeClass("show");
});

jQuery('#btnReg,#btnRegSm').click(function () {
    window.location.href = '/register?URL=' + window.location.pathname + window.location.search;
})

function logoutActionGA() {
    //logout
    eraseCookie("GlobalAir");
    eraseCookie("AccountTypes");
    //eraseSecureCookie("GlobalAir");
    //eraseSecureCookie("AccountTypes");

    console.log(readCookie("GlobalAir"));
    console.log(readCookie("AccountTypes"));
    //eraseCookieFromDomainFrame("GlobalAir");
    //eraseCookieFromDomainFrame("AccountTypes");

    jQuery('.logged-in-user-context').css('display', 'none');
    jQuery('.logged-out-user-context').css('display', 'inline-block');

    location.href = '/';
}

// Alert Boxes
function showMessage(msg, isError, title, callback, hideImage, hideButtons) {
    var t = title || (isError ? "Error!" : "Success!");
    hide = hideImage || false;
    hideB = hideButtons || false;
    var imageHtml = "";
    var imageURL = "";
    var imageButton = "";
    if (isError == true) {
        imageURL = "error.png";
    }
    else {
        imageURL = "success.png";
    }
    if (hide == false) {
        imageHtml = "<div align='center' class='p-2'><img width='90' src='/images/showMessage-" + imageURL + "'></div>";
    }
    if (hideB == false) {
        imageButton = "<div class='pt-4 pb-0' align='center'><button type='button' id='showMessageClose' data-dismiss='modal' aria-label='Close' class='btn btn-lg btn-primary showMessageClose'>OK</button></div>";
    }

    const successBox =
        "<div style='z-index:10000' id='showMessage' class='modal fade show' tabindex='-1' role='dialog' aria-hidden='true'>" +
        "<div class='gamodal modal-dialog modal-dialog-centered' role='document'>" +
        "<div class='modal-content p-4'>" +
        "<div class='modal-body'>" + imageHtml +
        "<h2 class='text-center p-2'>" + t + "</h2>" +
        "<div class='text-center text-muted lead'>" + msg + "</div>" +
        imageButton +
        "</div>" +
        "</div>" +
        "</div>" +
        "</div>";

    jQuery("body").prepend(successBox);
    jQuery(".modal-backdrop").addClass("show");

    if (callback != null) {
        jQuery(document).on("click", ".showMessageClose", callback);
    }
}

// Social Icons
function winOpen(url, width, height) {
    var win, left, top, opts;
    if (width && height) {
        left = (document.documentElement.clientWidth / 2 - width / 2);
        top = (document.documentElement.clientHeight - height) / 2;
        opts = 'status=1,resizable=yes' +
            ',width=' + width + ',height=' + height +
            ',top=' + top + ',left=' + left;
        win = window.open(url, '', opts);
    } else {
        win = window.open(url);
    }
    win.focus();
    return win;
}

function getTitle(network) {
    var title;
    if (network == 'twitter')
        title = getMetaContent('twitter:title');
    return title || document.title;
}

function getText(network) {
    var text;
    if (network == 'twitter')
        text = getMetaContent('twitter:description');
    return text || getMetaContent('description');
}

function getMetaContent(tagName, attr) {
    var text,
        tag = jQuery('meta[' + (attr ? attr : tagName.indexOf('og:') === 0 ? 'property' : 'name') + '="' + tagName + '"]');
    if (tag.length) {
        text = tag[0].getAttribute('content') || '';
    }
    return text || ''
}

function socialIconClick(network) {
    var shareUrl = encodeURIComponent(window.location.href),
        title = getTitle(network),
        text = getText(network);

    switch (network) {
        case 'facebook':
            url = 'https://www.facebook.com/share.php?u=' + shareUrl;
            break;
        case 'twitter':
            url = 'https://twitter.com/intent/tweet?url=' + shareUrl +
                '&text=' + encodeURIComponent(title + (text && title ? ' - ' : '') + text);
            break;
        case 'pinterest':
            url = 'https://pinterest.com/pin/create/button/?url=' + shareUrl +
                '&description=' + encodeURIComponent(text);
            break;
        case 'linkedin':
            url = 'https://www.linkedin.com/shareArticle?mini=true&url=' + shareUrl +
                '&title=' + encodeURIComponent(title) +
                '&summary=' + encodeURIComponent(text);
            break;
    }

    winOpen(url, 575, 400);
}
// End Social Icons

//function showSubscribeModal() {
//    if (readCookie('subscribe-box') == null && readCookie('subscribe-box-session') == null) {
//        jQuery('#subscribeModal').gamodal("show");
//        jQuery(".modal-backdrop").css("opacity", ".7");
//        jQuery(".modal-backdrop").css("background-color", "white");

//        jQuery('#subscribeModal .close').click(function () {
//            createCookie('subscribe-box', 'true', (islogged == "Logout" ? 90 : 20)); // Logged in users don't get prompted as often
//        });
//    }
//}
// Share with Friend BEGIN
function showSendToFriend(title, cls, adid) {
    var el = jQuery('#ShareFriend');
    el.find('#email-subject').val(document.URL);
    el.find('#plane-title').val(title);
    el.find('#plane-class').val(cls);
    el.find('#adid').val(adid);

    el.gamodal("show");
}

jQuery('.share-friend, .ssk-email').on('click', function (e, d) {
    e.preventDefault();
    var btn = jQuery(this);
    var adid = btn.data("adid");
    var cls = btn.data("classification");
    var title = btn.data("planetitle");

    var el = jQuery('#ShareFriend');
    if (el.length == 0) {
        jQuery.post("/aircraft-for-sale/EmailFriend", function (data) {
            jQuery("body").append(data);
            showSendToFriend(title, cls, adid);
        });
    }
    else {
        showSendToFriend(title, cls, adid);
    }
});

function EmailSubmit(frm) {
    event.preventDefault();
    var isplanemail = frm.find("input[name='PlaneMail']").prop('checked');
    var isairmail = frm.find("input[name='AirMail']").prop('checked');
    var ispistonmail = frm.find("input[name='PistonMail']").prop('checked');

    if (isplanemail == false && isairmail == false && ispistonmail == false) {
        alert("You must select at least one checkbox in order to subscribe.");
        return false;
    }

    frm.find("button").attr("disabled", "true");
    var emailaddr = frm.find("input[name='Email']").val();

    jQuery.ajax({
        type: 'POST',
        url: '/Home/EmailSubmit',
        data: {
            emailAddr: emailaddr,
            isPlaneMail: isplanemail,
            isAirMail: isairmail,
            isPistonMail: ispistonmail
        },
        cache: false,
        dataType: 'json',
        success: function (response) {
            if (response.isAdded) {
                jQuery("#subscribeModal").removeClass("show");

                showMessage("We added you to our list. Watch your inbox for upcoming mailings.", false, "Congratulations!", function () {
                    createCookie('subscribe-box', 'true', 365);
                    jQuery("#signupform").hide();
                });
            } else {
                showMessage("Sorry. Something went wrong. Please check your email and try again.", true);
            }
        },
        error: function (response) {
            showMessage("Sorry. Something went wrong. Please check your email and try again.", true);
        }
    });
    return false;
}

jQuery(function () {
    jQuery("#frmSignIn").validate({
        ignore: "",
        rules: {
            "tbUsername": {
                required: true
            },
            "tbPassword": {
                required: true
            }
        },
        errorPlacement: function (error, element) {
            if (element.attr("type") == "checkbox") {
                error.insertBefore(jQuery('.checkboxerrors'));
            } else {
                error.insertAfter(element);
            }
        },
        submitHandler: function (form) {
            event.preventDefault();

            jQuery.ajax({
                type: 'POST',
                data: jQuery("#frmSignIn").serialize(),
                url: '/Home/SignInUser',
                cache: false,
                dataType: 'json',
                success: function (response) {
                    if (response.status == "Success") {
                        if (typeof isFBORating !== 'undefined' && isFBORating == "yes") {
                            window.location.href = window.location.pathname + window.location.search + "?rating=true";
                        }
                        else {
                            window.location.href = response.msg;
                        }
                    } else {
                        //not successful
                        jQuery('#SigninError').html("<div class='alert alert-danger' role='alert'>" + response.msg + "</div>");
                    };
                },
                error: function (response) {    // if unhandled error on server
                    jQuery('#loginModal').removeClass('show');
                    showMessage("Sorry. Something went wrong.", true);
                }
            });

            return false;
        }
    });

    jQuery("#signupform").validate({
        rules: {
            "Email": {
                required: true,
                email: true
            }
        },
        messages: {
            "Email": {
                required: "Email is required",
                email: "Email format is incorrect"
            },
            "AirMail": {
                required: ""
            },
            "PistonMail": {
                required: ""
            },
            "PlaneMail": {
                required: ""
            }
        },
        submitHandler: function (form) {
            EmailSubmit(jQuery("#signupform"));
        }
    });
})