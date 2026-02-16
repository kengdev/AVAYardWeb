var timeout;

$('#header_notification_bar').click(function () {
    $('#append-notification').html('');
    $('#render-notification').addClass('loader');
    $.post("/app/layout/GetUnbillDetail",
        {

        }, function (result) {
            $.each(result.data, function (data) {
                alert(data.hours)
            });
        });


    //for (i = 0; i < 5; i++) {
    //    var notif = '<li>' +
    //        '<a href="javascript:;">' +
    //        '<span class="time">9 days</span>'+
    //        '<span class="details">' +
    //        'RWLU8302575 <div style="padding-top:1px;">บริษัท เจทีเค โลจิสติกส์ จำกัด</div></span>'+
    //        '</a>'+
    //        '</li>';
    //    $('#render-notification').removeClass('loader');
    //    $('#append-notification').append(notif);
    //}
});

$('.filterText').keyup(function () {
    reloadRow();
});

$('.filterDropdown').change(function () {
    reloadRow();
});

$('#profile-edit').click(function () {
    var url = '/account/editprofile';
    var setting = 'width=1000,height=540';

    window.open(url, 'editprofile account', setting);
});

$('.tooltips').click(function () {
    var name = $(this).attr('data-style');
    $.post("/layout/SetTheme",
        {
            name: name
        }, function (result) {

        });
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

$('.form-int').keypress(function (e) {
    //return (e.which != 8 && e.which != 0 && (e.which == 46)) ? false : true;
    if (e.which != 8 && e.which != 0) {
        if (e.which < 48 || e.which > 57) {
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

$('#userAttendance').click(function () {
    var url = '/account/_attendance';
    var setting = 'width=900,height=800';

    window.open(url, 'single workattendance', setting);
});

function DetailUnbill() {
    var url = '/app/job/unbill';
    var setting = 'width=1200,height=650';

    window.open(url, 'unbill job', setting);
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
    }).appendTo($('.portlet').css("position", "relative"));

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