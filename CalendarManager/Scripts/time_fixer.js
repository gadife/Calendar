$(function () {
    $(".time").each(function (index) {
        gmt_offset = get_gmt_offset()

        //alert($(this).text())
        var date = parseDate($(this).text())
        date.setHours(date.getHours() + gmt_offset);

        $(this).text(date.toString())
    });
});

function parseDate(input) {
    var parts = input.split(':');
    // new Date(year, month [, date [, hours[, minutes[, seconds[, ms]]]]])
    return new Date(parts[2], parts[1] - 1, parts[0], parts[3], parts[4], parts[5]); // months are 0-based
}

function date_to_string(date) {
    var str = "";
    
    str += add_zero_if_needed(date.getDate()) + ":";
    str += add_zero_if_needed(date.getMonth()+1) + ":";
    str += date.getFullYear() + ":";
    str += add_zero_if_needed(date.getHours()) + ":";
    str += add_zero_if_needed(date.getMinutes()) + ":";
    str += add_zero_if_needed(date.getSeconds());

    return str;
}

function get_gmt_offset() {
    var current_date = new Date();
    return -current_date.getTimezoneOffset() / 60;
}

function add_zero_if_needed(value) {
    return (String(value).length) == 1 ? "0" + String(value) : value;
}