window.onresize = function() {
    refreshNavPosition();
};

var refreshNavPosition = function() {
    var $root = $("div#nav ul.root");
    var rootPos = $root.position();

    var subs = $("div#nav ul.root > li > ul");
    subs.each(function() {
        $(this).css({ left: rootPos.left + "px", right: rootPos.left + "px" });
    });
};

jQuery.fn.reverse = [].reverse;

$(document).ready(function() {
    refreshNavPosition();

//    $("div#nav > ul > li").hoverIntent(function() {
//        refreshNavPosition();
//        $(this).children("ul").stop().delay(300).animate({ height: 'toggle', opacity: 'toggle' }, { duration: 'normal', easing: 'easeInQuart' });
//    }, function() {
//        $(this).children("ul").stop().delay(100).animate({ opacity: 'toggle', height: 'toggle' }, { duration: 'normal', easing: 'easeOutCubic' });
//    });
    $("div#nav > ul > li").hoverIntent(function() {
        refreshNavPosition();
        $(this).children("ul").show();
    }, function() {
        $(this).children("ul").hide();
    });

    $(".more").each(function() {
        $(this).click(function() {
            var elem = $("#" + $(this).attr("clip-id"));
            var step = 1;

            if ($(this).attr("expanded")) {
                elem.children("li.hide, li.-hide, div.hide, div.-hide").reverse().each(function() {
                    $(this).delay(25 * step++).animate({ height: 'toggle' }, { duration: 'fast', easing: 'easeInElastic' });
                });
                $(this).removeAttr("expanded");
                $(this).children("i").removeClass("icon-circle-arrow-up").addClass("icon-circle-arrow-down");
            } else {
                elem.children("li.hide, li.-hide, div.hide, div.-hide").each(function() {
                    $(this).delay(25 * step++).animate({ height: 'toggle' }, { duration: 'fast', easing: 'easeOutElastic' });
                });
                $(this).attr("expanded", "true");
                $(this).children("i").removeClass("icon-circle-arrow-down").addClass("icon-circle-arrow-up");
            }
        });
    });

    var prepareSearchBox = function() {
        var searchQuery = $("#ssq");

        if (searchQuery.length) {
            var defaultWidth = searchQuery.width();

            searchQuery.focus(function() {
                $(this).animate({ width: defaultWidth * 1.5 + "px" });
            }).blur(function() {
                $(this).animate({ width: defaultWidth + "px" });
            });
        }
    };

    prepareSearchBox();

    var prepareLargeSearchBox = function() {
        var largeSearchQuery = $("#lsq");

        if (largeSearchQuery.length) {
            largeSearchQuery.focus();

            var defaultWidth = largeSearchQuery.width();

            var parentWidth =
                largeSearchQuery.parent().width() -
                    parseInt(largeSearchQuery.parent().css("padding-left").replace("px", "")) -
                        parseInt(largeSearchQuery.parent().css("padding-right").replace("px", ""));

            largeSearchQuery.focus(function() {
                $(this).animate({ width: parentWidth + "px" });
            }).blur(function() {
                $(this).animate({ width: defaultWidth + "px" });
            });
        }
    };

    prepareLargeSearchBox();

    var $camera = $("#camera");
    if ($camera.length) {
        $camera.camera();
    }

    $('.newest > .inside').slideIt({
        prevButtonSelector: "#newest-buttons button.prev",
        nextButtonSelector: "#newest-buttons button.next"
    });

    $('.toggle-tree-menu').click(function() {
        var target = $(this).next().next();
        target.toggle('slow');

        if ($(this).hasClass("icon-minus-sign"))
            $(this).removeClass("icon-minus-sign").addClass("icon-plus-sign");
        else
            $(this).removeClass("icon-plus-sign").addClass("icon-minus-sign");
    });

    $('.highlights > .inside').slideIt({
        prevButtonSelector: "#highlights-buttons button.prev",
        nextButtonSelector: "#highlights-buttons button.next",
        elementsPerPage: 4
    });

    $(".ckeditor").each(function() {
        CKEDITOR.replace($(this).attr("id"));
    });

    $('a[trap="true"]').click(function(event) {
        event.preventDefault();
    });

    $("a[data-delete-handle='true']").click(function() {
        var deleteLink = $("a[data-delete='true']");

        if (deleteLink.length) {
            deleteLink.attr("href", $(this).attr("href"));
        }
    });
});


var $lockStatus = $('#lockStatus');
var $securityToken = $('#SecurityToken');
var $process = $('#process');

var checkLock = function() {
    try {
        var novinLock = new ActiveXObject("NLLIB.clsNovinLock");

        var securityToken = novinLock.GetSerial();

        if (novinLock.ErrNo == 0) {
            $lockStatus.attr("class", 'btn btn-info').html('شناسه متصل می باشد!');
            $securityToken.val(securityToken);
            $process.removeAttr("disabled");
        } else {
            $lockStatus.attr("class", 'btn btn-warning').html('شناسه متصل نمی باشد.');
            $securityToken.val('');
            $process.attr("disabled", "disabled");
        }
    } catch(ex) {
        $lockStatus.attr("class", 'btn btn-danger').html('درایور شناسه نصب نمی باشد.');
        $securityToken.val('');
        $process.attr("disabled", "disabled");
    }

    setTimeout('checkLock()', 1000);
};