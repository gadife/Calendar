$(function () {
    $("div#backgroundPopup").click(function () {
        disablePopup();  // function close pop up
    });

    $("div.close").click(function () {
        disablePopup();  // function close pop up
    });
});



function loading() {
    $("div.loader").show();
}
function closeloading() {
    $("div.loader").fadeOut('normal');
}

var popupStatus = 0; // set value

function loadPopup() {
    if (popupStatus == 0) { // if value is 0, show popup
        closeloading(); // fadeout loading
        $("#toPopup").fadeIn(0500); // fadein popup div
        $("#backgroundPopup").css("opacity", "0.7"); // css opacity, supports IE7, IE8
        $("#backgroundPopup").fadeIn(0001);
        popupStatus = 1; // and set value to 1
    }
}

function disablePopup() {
    if (popupStatus == 1) { // if value is 1, close popup
        $("#toPopup").fadeOut("normal");
        $("#backgroundPopup").fadeOut("normal");
        popupStatus = 0;  // and set value to 0
    }
}