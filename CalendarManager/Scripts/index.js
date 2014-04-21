var user_mail;

function add_new_date(start, end, allDay, email) {
    user_mail = email;
    $('#dates tbody').append('<tr><td>' + get_remove_button_td() + '</td><td>' + to_nice_string(start) + '</td><td>' + to_nice_string(end) + '</td>'+add_json_dates(start,end)+'</tr>')
}

function add_json_dates(start, end) {
    var start = '<td class="start_time">' + start.getTime() + '</td>';
    var end = '<td class="end_time">' + end.getTime() + '</td>';
    return start + end;
}

function to_nice_string(date) {
    return date.toLocaleTimeString() + ' ' + date.toLocaleDateString();
}

function get_remove_button_td() {
    return '<button type="submit" class="btn btn-danger" onclick="remove_date(this)"><span class="glyphicon glyphicon-remove"></span></button>'
}

function remove_date(btn) {
    var tr = $(btn).closest('tr');
    remove_tr(tr);
}

function remove_tr(row) {
    var start = $(row).children('.start_time').html();
    var end = $(row).children('.end_time').html();

    // remove from calendar
    $('#calendar').fullCalendar('removeEvents', function (event) {
        if (event.start.getTime() == start && event.end.getTime() == end) {
            return true;
        }
        return false;
    });

    // remove from dates 
    $(row).fadeOut(300, function () {
        $(this).remove();
    });
}

function send_new_events() {
    disable_or_unable_form(true);

    $.ajax({
        type: "POST",
        url: "SendUserEventOptions",
        //contentType: "application/json",
        //dataType: 'json',
        data: {
            json: JSON.stringify(create_temps_object()),
            gmtOffset: get_gmt_offset(),
            email: user_mail
        }
    })
   .success(function (d) {
       //$('#dates tbody').remove();
       clear_form();
       disable_or_unable_form(false);
   })
   .fail(function (data) {
       alert('error');
       console.log(data);
       disable_or_unable_form(false);
   });;
}

function disable_or_unable_form(action) {
    $('#loading_gif').css('display', action ? 'inline' : 'none')
    $('#add_event').attr("disabled", action);
    $('#title').attr("disabled", action);
    $('#location').attr("disabled", action);
    $('#mail').attr("disabled", action);
    $('#name').attr("disabled", action);
    $('#description').attr("disabled", action);

    if (action) {
        var dates = $('#dates tbody').children('tr');
        for (var i = 0; i < dates.length; i++) {
            $(dates[i]).find('button').attr("disabled", action);
        }
    }
}

function create_temps_object() {
    var title = $('#title').val();
    var location = $('#location').val();
    var from_mail = $('#mail').val();
    var from_name = $('#name').val();
    var desc = $('#description').val();

    var dates = $('#dates tbody').children('tr');

    var temps = [];
    for (var i = 0; i < dates.length; i++) { 
        var s = $(dates[i]).children('.start_time').html();
        var e = $(dates[i]).children('.end_time').html();

        var temp_date = {
            Title: title,
            Location: location,
            ReturnMail: from_mail,
            ReturnName: from_name,
            Desc: desc,
            StartDateString: date_to_string(new Date(Number(s))),
            EndDateString: date_to_string(new Date(Number(e))),
        }
        temps.push(temp_date);
    }

    return temps;
}

function clear_form() {
    $('#title').val('');
    $('#location').val('');
    $('#mail').val('');
    $('#name').val('');
    $('#description').val('');
    
    var dates = $('#dates tbody').children('tr');
    for (var i = 0; i < dates.length; i++) {
        remove_tr(dates[i]);
    }
}