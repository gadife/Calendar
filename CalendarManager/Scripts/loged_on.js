$(function () {
    get_end_set_requests();
});

function get_end_set_requests() {
    $.ajax({
        type: "POST",
        url: "../User/GetRequestsCount",
    })
   .success(function (count) {
       set_requests_count(count);
   })
   .fail(function (data) {
       console.log(data);
   });
}

function subscribe_one_request() {
    var current_count = Number($('#requests_count .badge').html());
    set_requests_count(current_count-1);
}

function set_requests_count(count) {
    $('#requests_count').html('Requests&nbsp<span class="badge pull-right">' + count + '</span>');
    if (count > 0) {
        $('#requests_count .badge').addClass('red');
    }
}