(function ($) {
    $.fn.slideIt = function (options) {
        var settings = $.extend({
            'prevButtonSelector': '',
            'nextButtonSelector': '',
            delayBetweenAnimations: 100,
            elementsPerPage: 4
        }, options);
        
        var elements = this.find("> div[data-element]");
        var elementHeight = elements.first().outerHeight(true);
        var dummyElement = $('<div id="dummy"></div>');
        dummyElement.css({ "min-height": elementHeight });

        this.find("> div[data-dummy]").before(dummyElement);

        var prevButton = $(settings.prevButtonSelector);
        var nextButton = $(settings.nextButtonSelector);

        var totalElements = elements.length;
        var totalPages = Math.ceil(totalElements / settings.elementsPerPage);
        var currentPage = 0;
        var overallDelayAmount = 0;

        invalidateVisibilities(true);
        invalidatePagers();

        prevButton.click(function () {
            if (currentPage - 1 >= 0) {
                currentPage -= 1;

                invalidateVisibilities();
            }
            invalidatePagers();
        });

        nextButton.click(function () {
            if (currentPage + 1 < totalPages) {
                currentPage += 1;

                invalidateVisibilities();
            }
            invalidatePagers();
        });

        function invalidatePagers() {
            if (currentPage - 1 < 0) {
                prevButton.addClass("disabled");
            } else {
                prevButton.removeClass("disabled");
            }

            if (currentPage + 1 >= totalPages) {
                nextButton.addClass("disabled");
            } else {
                nextButton.removeClass("disabled");
            }
        }

        function invalidateVisibilities(noEffect) {
            var startItem = currentPage * settings.elementsPerPage;
            var endItem = (currentPage + 1) * settings.elementsPerPage;
            overallDelayAmount = 0;

            elements.each(function (i) {
                if (i >= startItem && i < endItem) {
                    $(this).delay(overallDelayAmount).slideDown();
                    overallDelayAmount += noEffect ? 0 : settings.delayBetweenAnimations;
                } else
                    $(this).hide();
            });
        }
    };
})(jQuery);