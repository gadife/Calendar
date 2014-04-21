$(function () {
    $('#gmtOffset').val(get_gmt_offset());
}); 

function show_result(data) {
    $('#meetings_content').html('');
    var cal = $('#meetings_content').fullCalendar({
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek,agendaDay'
        },
        defaultView: 'agendaWeek',
        selectable: true,
        selectHelper: true,
        select: function(start, end, allDay) {
            //var title = prompt('Event Title:');
            show_add_window(start, end, allDay);
        },
        editable: true,
        events: data
    });

    finish_claculate();
}

function finish_claculate() {
    $('#loading_gif').hide();
    $('#calc_btn').attr("disabled", false);
}

function start_claculate() {
    $('#loading_gif').show();
    $('#calc_btn').attr("disabled",true);
}

function get_gmt_offset() {
    var current_date = new Date();
    return -current_date.getTimezoneOffset() / 60;
}

function show_add_window(start, end, allDay) {
    $('#popup_content').html('Loading ... Please wait');
    loadPopup();

    $.ajax({
        type: "POST",
        url: "GetAddView",
        data: {
            start : start,
            end: end,
            allDay : allDay
        }
    }).done(function (data) {
        $('#popup_content').html(data);
    });
}

function add_event()
{
    start = new Date($('#start_datetimepicker').data("DateTimePicker").getDate())
    end = new Date($('#end_datetimepicker').data("DateTimePicker").getDate())
    allDay = $('#allDay').is(':checked');
    title = $('#title').val();

    console.log(start);
    console.log(end);
    console.log(allDay);

    start_claculate();

    $.ajax({
        type: "POST",
        url: "AddData",
        data: {
            users: $("#users").val(),
            start: date_to_string(start),
            end: date_to_string(end),
            gmtOffset: get_gmt_offset(),
            isAllDay: allDay,
            title: title,
            location: $('#location').val(),
            desc: $('#description').val(),
        }
    })
    .done(function () {
        add_to_calendar(start, end, title, allDay);
        finish_claculate();
        disablePopup();
    });
}

function disable_event()
{
    disablePopup();
    $('#meetings_content').fullCalendar('unselect');
}

function add_to_calendar(start,end,title,allDay) {
    $('#meetings_content').fullCalendar('renderEvent',
                    {
                        title: title,
                        start: start,
                        end: end,
                        allDay: allDay
                    },
                    true // make the event "stick"
                );
    console.log('after');
}