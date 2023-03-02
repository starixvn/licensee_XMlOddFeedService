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
    $('.section').hide();

    if (classNameHide != undefined) {
        $(classNameHide).hide();
    }

    if (classNameHide === '.sub-general-concepts') {
        $('.general_concepts').hide();
    } else {
        if (classNameHide === '.sub-commands') {
            $('.commands').hide();
        }
    }

    if (obj.id === 'div_general_concepts') {
        $('.sub-general-concepts').show();
    } else {
        if (obj.id === 'div_commands') {
            $('.sub-commands').show();
        }
    }

    $(obj).show();
    $(obj).children().show();
    $(obj).parent().show();

    $('.term_definition_result_feed').hide();
    if (obj.id === 'div_general_concepts' || obj.id === 'div_general_concepts_result') {
        $('#div_result_feed_general').show();
    }

    if (level === 2) {
        $(obj).parent().parent().show();
    }
}

function activeTermAndDefinitionResultFeed(obj) {
    $('#div_term_definition').hide();
    $('#div_general_concepts_errors').hide();
    $('#div_result_feed_general').hide();
    $('.term_definition_result_feed').hide();

    $(obj).show();
}