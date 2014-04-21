function disable_request_buttons(request_id) {
    $('button.' + request_id).each(function (index) {
        $(this).attr("disabled", true);
    });
}

function clear_reuqests(request_id) {
    $('div.request.' + request_id).fadeOut(300, function () {
        $(this).remove();
    });

    subscribe_one_request();
}