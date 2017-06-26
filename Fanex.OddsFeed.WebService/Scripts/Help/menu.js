$(document).ready(function () {

    //SetScrollHeader();
    SetStartMenu();

    $("ul.topnav li").hover(function () {
        $(this).find("ul.subNav1").slideDown('fast').show();
        $(this).hover(function () {
        }, function () {
            $(this).find("ul.subNav1").slideUp('fast');
        });
    });

    $("ul.topnav li ul.subNav1 li").hover(function () {
        $(this).parent().parent().find("a").addClass("hoverItem");
        $(this).find("ul.subNav2").slideDown('fast').show();
        $(this).hover(function () {
        }, function () {
            $(this).parent().parent().find("a").removeClass("hoverItem");
            $(this).find("ul.subNav2").slideUp('fast');
        });
    });

    $("ul.topnav li").click(function () {        
        $("ul.topnav li").find("a").removeClass("actived");
        $(this).find("a").addClass("actived");        
    });


});

function SetStartMenu() {
    $(".section").hide();
    $(div_get_started).show();
    $("#getStared").addClass("actived");
};

function SetScrollHeader() {
    var headerTop = $('#header').offset().top;
    $(window).scroll(function () {        
        if ($(window).scrollTop() > headerTop) {
            $('#header').css({ position: 'fixed', top: '0px' });
            $('#header').css('z-index', '1');
        }
        else {
            $('#header').css({ position: 'static', top: '0px' });
        }
    });
};

function ActiveMenu(obj, classNameHide, level) {
    $(".section").hide();

    if (classNameHide != undefined) {
        $(classNameHide).hide();
    }

    if (classNameHide == '.sub-commands') {
        $(".commands").hide();
    }

    if (obj.id == 'div_commands') {
        $(".sub-commands").show();
    }

    $(obj).show();
    $(obj).children().show();
    $(obj).parent().show();

    if (level == 2) {    
        $(obj).parent().parent().show();
    }
}