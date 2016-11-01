
$(document).ready(function () {
    //checkbox
    $('input.myCheckBox').prettyCheckable();

    //accordion
    $("#accordion").accordion({
        heightStyle: "content"
    });
});

$(window).load(function () {
    if ($('#Div_Map').height() < $('#div_demo').height()) {
        $('#Div_Map').height($('#div_demo').height());
    }
});