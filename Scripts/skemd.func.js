// hide/show accordion menu
function f_hide_show_accordion() {
    if ($('.demo')[0].clientWidth > 0) {
        $('.demo').hide();
        document.getElementById('btn_hide_show_accordion').src = '../Content/img/accordion_22_show.png';
    }
    else {
        $('.demo').show();
        document.getElementById('btn_hide_show_accordion').src = '../Content/img/accordion_17_hide.png';
    }
}

function full_screen(enabled) {
    if (enabled == true) {
        $('#btn_fullscreen').show();
        $('#Div_Map_Menu').hide();
    }
    else {
        $('#btn_fullscreen').hide();
        $('#Div_Map_Menu').show();
    }
}

$(document).ready(function () {
    $('#btn_fullscreen').hide();
    var comp = document.getElementsByClassName('info_content');
    for (var i = 0; i < comp.length; i++)
        comp[i].style.maxHeight = $(window).height() / 2.5 + "px";
    $(".content")[0].style.height = $(window).height() - 54 - 60 + "px";
    comp = document.getElementById('graphParamsFieldsetDate');
    if (comp != null) {
        $('#graphParamsFieldsetDate').width(300);
        $("#graphParamsEditors")[0].style.width = ($(window).width() - $('#graphParamsFieldsetDate').width() - 80) + 'px';
    }
});

$(window).resize(function () {
    var comp = document.getElementsByClassName('info_content');
    for (var i = 0; i < comp.length; i++)
        comp[i].style.maxHeight = $(window).height() / 2.5 + "px";
    $(".content")[0].style.height = $(window).height() - 54 - 60 + "px";
    comp = document.getElementById('graphParamsEditors');
    if (comp != null) {
        comp.style.width = ($(window).width() - $("#graphParamsFieldsetDate").width() - 80) + 'px';
    }
});