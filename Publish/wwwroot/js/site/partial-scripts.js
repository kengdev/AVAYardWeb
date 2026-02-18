$(function () {
    $('.form-number').keypress(function (e) {
        //return (e.which != 8 && e.which != 0 && (e.which == 46)) ? false : true;
        if (e.which != 8 && e.which != 0) {
            if (e.which == 46) {
                return true;
            }
            else if (e.which < 48 || e.which > 57) {
                return false;
            }
        }
        else {
            if (e.which == 8 || e.which == 0) {
                return true;
            }
        }
    });

    $('.form-decimal').keypress(function (e) {
        //return (e.which != 8 && e.which != 0 && (e.which == 46)) ? false : true;
        if (e.which != 8 && e.which != 0) {
            if (e.which == 46) {
                return true;
            }
            else if (e.which < 48 || e.which > 57) {
                return false;
            }
        }
        else {
            if (e.which == 8 || e.which == 0) {
                return true;
            }
        }
    });

    $('.form-geo, .form-license').keypress(function (e) {
        if (e.which != 8 && e.which != 0) {
            if (e.which == 46 || e.which == 45) {
                return true;
            }
            else if (e.which < 48 || e.which > 57) {
                return false;
            }
        }
        else {
            if (e.which == 8 || e.which == 0) {
                return true;
            }
        }
    });

    $('.form-eng').keypress(function (e) {
        if (e.which != 8 && e.which != 0) {
            if (e.which >= 48 && e.which <= 90) {
                if (e.which >= 58 && e.which <= 64) {
                    return false;
                }
                else {
                    //ตัวอักษรใหญ่
                    return true;
                }
            }
            else {
                //ตัวอักษรเล็ก97-122
                if (e.which >= 97 && e.which <= 122) {
                    return true;
                }
                else if (e.which == 45) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        else {
            if (e.which == 8 || e.which == 0) {
                return true;
            }
        }
    });

    $('.form-username').keypress(function (e) {
        if (e.which != 8 && e.which != 0) {
            if (e.which < 97 || e.which > 122) {
                if (e.which < 48 || e.which > 57) {
                    return false;
                }
                else {
                    return true;
                }
            }
        }
        else {
            if (e.which == 8 || e.which == 0) {
                return true;
            }
        }
    });

    $('.datepicker').datetimepicker({
        datepicker: true,
        timepicker: false,
        lang: 'th',
        format: 'Y-m-d',
        inline: false
    });

    $('.datetimepicker').datetimepicker({
        datepicker: true,
        timepicker: true,
        lang: 'th',
        step: 30,
        format: 'Y-m-d H:i',
        inline: false
    });
});

function overLayPage() {
    $("<div id='overlay-form'></div><div class='spin-overlay'><div class='loader10'></div></div>").css({
        position: "absolute",
        width: "100%",
        height: "100%",
        opacity: 0.7,
        top: 0,
        left: 0,
        background: "#fff",
        'z-index': "9999"
    }).appendTo($('.portlet').css("position", "relative"));

    timeout = setTimeout(killOverlayPage, 7000);
}

function overLayPage() {
    $("<div id='overlay-form'></div><div class='spin-overlay'><div class='loader10'></div></div>").css({
        position: "absolute",
        width: "100%",
        height: "100%",
        opacity: 0.7,
        top: 0,
        left: 0,
        background: "#fff",
        'z-index': "9999"
    }).appendTo($('.partial-body-container').css("position", "relative"));

    timeout = setTimeout(killOverlayPage, 7000);
}

function overLayForm() {
    $("<div id='overlay-form'></div><div class='spin-overlay'><div class='loader10'></div></div>").css({
        position: "absolute",
        width: "100%",
        height: "100%",
        opacity: 0.7,
        top: 0,
        left: 0,
        background: "#fff",
        'z-index': "9999"
    }).appendTo($('.portlet').css("position", "relative"));
    $('.tools-action').hide();
    $('<div class="tools-loading"><div class="spin-item"><div class="loader09"></div></div></div>').appendTo($('.form-actions'));

    timeout = setTimeout(killOverlayForm, 7000);
}

function overLayDiv(element) {
    $("<div id='overlay-form'></div>").css({
        position: "absolute",
        width: "100%",
        height: "100%",
        opacity: 0.7,
        top: 0,
        left: 0,
        background: "#fff",
        'z-index': "9999"
    }).appendTo($(element).css("position", "relative"));

    timeout = setTimeout(killOverlayDiv, 7000);
}

function killOverlayForm() {
    $('.tools-action').show();
    $('.tools-loading').hide();
    $('#overlay-form').remove();
    $('.spin-overlay').remove();
    clearTimeout(timeout);
}

function killOverlayPage() {
    $('#overlay-form').remove();
    $('.spin-overlay').remove();
    clearTimeout(timeout);
}

function killOverlayDiv() {
    $('#overlay-form').remove();
    $('#wrapper-action-modal').show();
    $('#spin-item').hide();
    clearTimeout(timeout);
}