// Camera slideshow v1.1.2 - a jQuery slideshow with many effects, transitions, easy to customize, using canvas and mobile ready, based on jQuery 1.4+
// Copyright (c) 2012 by Manuel Masia - www.pixedelic.com
// Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
(function($) {
    $.fn.camera = function(opts, callback) {

        var defaults = {
            alignment: 'center', //topLeft, topCenter, topRight, centerLeft, center, centerRight, bottomLeft, bottomCenter, bottomRight

            autoAdvance: true,	//true, false

            mobileAutoAdvance: true, //true, false. Auto-advancing for mobile devices

            barDirection: 'rightToLeft',	//'leftToRight', 'rightToLeft', 'topToBottom', 'bottomToTop'

            barPosition: 'top',	//'bottom', 'left', 'top', 'right'

            cols: 6,

            easing: 'easeInOutExpo',	//for the complete list http://jqueryui.com/demos/effect/easing.html

            mobileEasing: '',	//leave empty if you want to display the same easing on mobile devices and on desktop etc.

            fx: 'random', //'random','simpleFade', 'curtainTopLeft', 'curtainTopRight', 'curtainBottomLeft', 'curtainBottomRight', 'curtainSliceLeft', 'curtainSliceRight', 'blindCurtainTopLeft', 'blindCurtainTopRight', 'blindCurtainBottomLeft', 'blindCurtainBottomRight', 'blindCurtainSliceBottom', 'blindCurtainSliceTop', 'stampede', 'mosaic', 'mosaicReverse', 'mosaicRandom', 'mosaicSpiral', 'mosaicSpiralReverse', 'topLeftBottomRight', 'bottomRightTopLeft', 'bottomLeftTopRight', 'bottomLeftTopRight'
            //you can also use more than one effect, just separate them with commas: 'simpleFade, scrollRight, scrollBottom'

            mobileFx: '',	//leave empty if you want to display the same effect on mobile devices and on desktop etc.

            gridDifference: 250,	//to make the grid blocks slower than the slices, this value must be smaller than transPeriod

            height: '25%',	//here you can type pixels (for instance '300px'), a percentage (relative to the width of the slideshow, for instance '50%') or 'auto'

            imagePath: 'images/',	//he path to the image folder (it serves for the blank.gif, when you want to display videos)

            hover: true,	//true, false. Puase on state hover. Not available for mobile devices

            loader: 'bar',	//pie, bar, none (even if you choose "pie", old browsers like IE8- can't display it... they will display always a loading bar)

            loaderColor: 'rgba(42, 56, 117, 0.75)',

            loaderBgColor: 'transparent',

            loaderOpacity: 1,	//0, .1, .2, .3, .4, .5, .6, .7, .8, .9, 1

            loaderPadding: 0,	//how many empty pixels you want to display between the loader and its background

            loaderStroke: 7,	//the thickness both of the pie loader and of the bar loader. Remember: for the pie, the loader thickness must be less than a half of the pie diameter

            minHeight: '',	//you can also leave it blank

            navigation: false,	//true or false, to display or not the navigation buttons

            navigationHover: true,	//if true the navigation button (prev, next and play/stop buttons) will be visible on hover state only, if false they will be visible always

            mobileNavHover: true,	//same as above, but only for mobile devices

            opacityOnGrid: true,	//true, false. Decide to apply a fade effect to blocks and slices: if your slideshow is fullscreen or simply big, I recommend to set it false to have a smoother effect 

            overlayer: true,	//a layer on the images to prevent the users grab them simply by clicking the right button of their mouse (.camera_overlayer)

            pagination: false,

            playPause: false,	//true or false, to display or not the play/pause buttons

            pauseOnClick: false,	//true, false. It stops the slideshow when you click the sliders.

            pieDiameter: 38,

            piePosition: 'rightTop',	//'rightTop', 'leftTop', 'leftBottom', 'rightBottom'

            portrait: true, //true, false. Select true if you don't want that your images are cropped

            rows: 4,

            slicedCols: 12,	//if 0 the same value of cols

            slicedRows: 8,	//if 0 the same value of rows

            slideOn: 'random',	//next, prev, random: decide if the transition effect will be applied to the current (prev) or the next slide

            thumbnails: false,

            time: 7000,	//milliseconds between the end of the sliding effect and the start of the nex one

            transPeriod: 1500,	//lenght of the sliding effect in milliseconds
		
////////callbacks

            onEndTransition: function() {
            },	//this callback is invoked when the transition effect ends

            onLoaded: function() {
            },	//this callback is invoked when the image on a slide has completely loaded

            onStartLoading: function() {
            },	//this callback is invoked when the image on a slide start loading

            onStartTransition: function() {
            }	//this callback is invoked when the transition effect starts
        };


        function isMobile() {
            if (navigator.userAgent.match( /Android/i ) ||
                navigator.userAgent.match( /webOS/i ) ||
                    navigator.userAgent.match( /iPad/i ) ||
                        navigator.userAgent.match( /iPhone/i ) ||
                            navigator.userAgent.match( /iPod/i )) {
                return true;
            }
        }

        var opts = $.extend({ }, defaults, opts);

        var wrap = $(this).addClass('camera-wrapper');

        wrap.wrapInner('<div class="camera-src" />')
            .wrapInner('<div class="camera-fake-hover" />');

        var fakeHover = $('.camera-fake-hover', wrap);

        fakeHover.append('<div class="camera-target"></div>');
        if (opts.overlayer == true) {
            fakeHover.append('<div class="camera-overlayer"></div>');
        }
        fakeHover.append('<div class="camera-target-content"></div>');

        var loader;

        if (opts.loader == 'pie' && $.browser.msie && $.browser.version < 9) {
            loader = 'bar';
        } else {
            loader = opts.loader;
        }

        if (loader == 'pie') {
            fakeHover.append('<div class="camera-pie"></div>');
        } else if (loader == 'bar') {
            fakeHover.append('<div class="camera-bar"></div>');
        } else {
            fakeHover.append('<div class="camera-bar" style="display:none"></div>');
        }

        if (opts.playPause == true) {
            fakeHover.append('<div class="camera-commands"></div>');
        }

        if (opts.navigation == true) {
            fakeHover.append('<div class="camera-prev"><span></span></div>')
                .append('<div class="camera-next"><span></span></div>');
        }

        if (opts.thumbnails == true) {
            wrap.append('<div class="camera-thumbs-cont" />');
        }

        if (opts.thumbnails == true && opts.pagination != true) {
            $('.camera-thumbs-cont', wrap).wrap('<div />')
                .wrap('<div class="camera-thumbs" />')
                .wrap('<div />')
                .wrap('<div class="camera-command-wrap" />');
        }

        if (opts.pagination == true) {
            wrap.append('<div class="camera-page"></div>');
        }

        wrap.append('<div class="camera-loader"></div>');

        $('.camera-caption', wrap).each(function() {
            $(this).wrapInner('<div />');
        });


        var pieID = 'pie-' + wrap.index(),
            elem = $('.camera-src', wrap),
            target = $('.camera-target', wrap),
            content = $('.camera-target-content', wrap),
            pieContainer = $('.camera-pie', wrap),
            barContainer = $('.camera-bar', wrap),
            prevNav = $('.camera-prev', wrap),
            nextNav = $('.camera-next', wrap),
            commands = $('.camera-commands', wrap),
            pagination = $('.camera-page', wrap),
            thumbs = $('.camera-thumbs-cont', wrap);


        var w,
            h;


        var allImg = new Array();
        $('> div', elem).each(function() {
            allImg.push($(this).attr('data-src'));
        });

        var allLinks = new Array();
        $('> div', elem).each(function() {
            if ($(this).attr('data-link')) {
                allLinks.push($(this).attr('data-link'));
            } else {
                allLinks.push('');
            }
        });

        var allTargets = new Array();
        $('> div', elem).each(function() {
            if ($(this).attr('data-target')) {
                allTargets.push($(this).attr('data-target'));
            } else {
                allTargets.push('');
            }
        });

        var allPor = new Array();
        $('> div', elem).each(function() {
            if ($(this).attr('data-portrait')) {
                allPor.push($(this).attr('data-portrait'));
            } else {
                allPor.push('');
            }
        });

        var allAlign = new Array();
        $('> div', elem).each(function() {
            if ($(this).attr('data-alignment')) {
                allAlign.push($(this).attr('data-alignment'));
            } else {
                allAlign.push('');
            }
        });


        var allThumbs = new Array();
        $('> div', elem).each(function() {
            if ($(this).attr('data-thumb')) {
                allThumbs.push($(this).attr('data-thumb'));
            } else {
                allThumbs.push('');
            }
        });

        var amountSlide = allImg.length;

        $(content).append('<div class="camera-contents" />');
        var loopMove;
        for (loopMove = 0; loopMove < amountSlide; loopMove++) {
            $('.camera-contents', content).append('<div class="camera-content" />');
            if (allLinks[loopMove] != '') {
                //only for Wordpress plugin
                var dataBox = $('> div ', elem).eq(loopMove).attr('data-box');
                if (typeof dataBox !== 'undefined' && dataBox !== false && dataBox != '') {
                    dataBox = 'data-box="' + $('> div ', elem).eq(loopMove).attr('data-box') + '"';
                } else {
                    dataBox = '';
                }
                //
                $('.camera-target-content .camera-content:eq(' + loopMove + ')', wrap).append('<a class="camera-link" href="' + allLinks[loopMove] + '" ' + dataBox + ' target="' + allTargets[loopMove] + '"></a>');
            }

        }
        $('.camera-caption', wrap).each(function() {
            var ind = $(this).parent().index(),
                cont = wrap.find('.camera-content').eq(ind);
            $(this).appendTo(cont);
        });

        target.append('<div class="camera-cont" />');
        var cameraCont = $('.camera-cont', wrap);

        var loop;
        for (loop = 0; loop < amountSlide; loop++) {
            cameraCont.append('<div class="camera-slide camera-slide-' + loop + '" />');
            var div = $('> div:eq(' + loop + ')', elem);
            target.find('.camera-slide-' + loop).clone(div);
        }


        function thumbnailVisible() {
            var wTh = $(thumbs).width();
            $('li', thumbs).removeClass('camera-vis-thumb');
            $('li', thumbs).each(function() {
                var pos = $(this).position(),
                    ulW = $('ul', thumbs).outerWidth(),
                    offUl = $('ul', thumbs).offset().left,
                    offDiv = $('> div', thumbs).offset().left,
                    ulLeft = offDiv - offUl;
                if (ulLeft > 0) {
                    $('.camera-prev-thumbs', camera_thumbs_wrap).removeClass('hide-nav');
                } else {
                    $('.camera-prev-thumbs', camera_thumbs_wrap).addClass('hide-nav');
                }
                if ((ulW - ulLeft) > wTh) {
                    $('.camera-next-thumbs', camera_thumbs_wrap).removeClass('hide-nav');
                } else {
                    $('.camera-next-thumbs', camera_thumbs_wrap).addClass('hide-nav');
                }
                var left = pos.left,
                    right = pos.left + ($(this).width());
                if (right - ulLeft <= wTh && left - ulLeft >= 0) {
                    $(this).addClass('camera-vis-thumb');
                }
            });
        }

        $(window).bind('load resize', function() {
            thumbnailPos();
            thumbnailVisible();
        });


        cameraCont.append('<div class="camera-slide camera-slide-' + loop + '" />');


        var started;

        wrap.show();
        var w = target.width();
        var h = target.height();

        var setPause;

        $(window).bind('resize', function() {
            if (started == true) {
                resizeImage();
            }
            $('ul', thumbs).animate({ 'margin-top': 0 }, 0, thumbnailPos);
            if (!elem.hasClass('paused')) {
                elem.addClass('paused');
                if ($('.camera-stop', camera_thumbs_wrap).length) {
                    $('.camera-stop', camera_thumbs_wrap).hide();
                    $('.camera-play', camera_thumbs_wrap).show();
                    if (loader != 'none') {
                        $('#' + pieID).hide();
                    }
                } else {
                    if (loader != 'none') {
                        $('#' + pieID).hide();
                    }
                }
                clearTimeout(setPause);
                setPause = setTimeout(function() {
                    elem.removeClass('paused');
                    if ($('.camera-play', camera_thumbs_wrap).length) {
                        $('.camera-play', camera_thumbs_wrap).hide();
                        $('.camera-stop', camera_thumbs_wrap).show();
                        if (loader != 'none') {
                            $('#' + pieID).fadeIn();
                        }
                    } else {
                        if (loader != 'none') {
                            $('#' + pieID).fadeIn();
                        }
                    }
                }, 1500);
            }
        });

        function resizeImage() {
            var res;

            function resizeImageWork() {
                w = wrap.width();
                if (opts.height.indexOf('%') != -1) {
                    var startH = Math.round(w / (100 / parseFloat(opts.height)));
                    if (opts.minHeight != '' && startH < parseFloat(opts.minHeight)) {
                        h = parseFloat(opts.minHeight);
                    } else {
                        h = startH;
                    }
                    wrap.css({ height: h });
                } else if (opts.height == 'auto') {
                    h = wrap.height();
                } else {
                    h = parseFloat(opts.height);
                    wrap.css({ height: h });
                }
                $('.camera-relative', target).css({ 'width': w, 'height': h });
                $('.img-loaded', target).each(function() {
                    var t = $(this),
                        wT = t.attr('width'),
                        hT = t.attr('height'),
                        imgLoadIn = t.index(),
                        mTop,
                        mLeft,
                        alignment = t.attr('data-alignment'),
                        portrait = t.attr('data-portrait');

                    if (typeof alignment === 'undefined' || alignment === false || alignment === '') {
                        alignment = opts.alignment;
                    }

                    if (typeof portrait === 'undefined' || portrait === false || portrait === '') {
                        portrait = opts.portrait;
                    }

                    if (portrait == false || portrait == 'false') {
                        if ((wT / hT) < (w / h)) {
                            var r = w / wT;
                            var d = (Math.abs(h - (hT * r))) * 0.5;
                            switch (alignment) {
                            case 'topLeft':
                                mTop = 0;
                                break;
                            case 'topCenter':
                                mTop = 0;
                                break;
                            case 'topRight':
                                mTop = 0;
                                break;
                            case 'centerLeft':
                                mTop = '-' + d + 'px';
                                break;
                            case 'center':
                                mTop = '-' + d + 'px';
                                break;
                            case 'centerRight':
                                mTop = '-' + d + 'px';
                                break;
                            case 'bottomLeft':
                                mTop = '-' + d * 2 + 'px';
                                break;
                            case 'bottomCenter':
                                mTop = '-' + d * 2 + 'px';
                                break;
                            case 'bottomRight':
                                mTop = '-' + d * 2 + 'px';
                                break;
                            }
                            t.css({
                                'height': hT * r,
                                'margin-left': 0,
                                'margin-top': mTop,
                                'position': 'absolute',
                                'visibility': 'visible',
                                'width': w
                            });
                        } else {
                            var r = h / hT;
                            var d = (Math.abs(w - (wT * r))) * 0.5;
                            switch (alignment) {
                            case 'topLeft':
                                mLeft = 0;
                                break;
                            case 'topCenter':
                                mLeft = '-' + d + 'px';
                                break;
                            case 'topRight':
                                mLeft = '-' + d * 2 + 'px';
                                break;
                            case 'centerLeft':
                                mLeft = 0;
                                break;
                            case 'center':
                                mLeft = '-' + d + 'px';
                                break;
                            case 'centerRight':
                                mLeft = '-' + d * 2 + 'px';
                                break;
                            case 'bottomLeft':
                                mLeft = 0;
                                break;
                            case 'bottomCenter':
                                mLeft = '-' + d + 'px';
                                break;
                            case 'bottomRight':
                                mLeft = '-' + d * 2 + 'px';
                                break;
                            }
                            t.css({
                                'height': h,
                                'margin-left': mLeft,
                                'margin-top': 0,
                                'position': 'absolute',
                                'visibility': 'visible',
                                'width': wT * r
                            });
                        }
                    } else {
                        if ((wT / hT) < (w / h)) {
                            var r = h / hT;
                            var d = (Math.abs(w - (wT * r))) * 0.5;
                            switch (alignment) {
                            case 'topLeft':
                                mLeft = 0;
                                break;
                            case 'topCenter':
                                mLeft = d + 'px';
                                break;
                            case 'topRight':
                                mLeft = d * 2 + 'px';
                                break;
                            case 'centerLeft':
                                mLeft = 0;
                                break;
                            case 'center':
                                mLeft = d + 'px';
                                break;
                            case 'centerRight':
                                mLeft = d * 2 + 'px';
                                break;
                            case 'bottomLeft':
                                mLeft = 0;
                                break;
                            case 'bottomCenter':
                                mLeft = d + 'px';
                                break;
                            case 'bottomRight':
                                mLeft = d * 2 + 'px';
                                break;
                            }
                            t.css({
                                'height': h,
                                'margin-left': mLeft,
                                'margin-top': 0,
                                'position': 'absolute',
                                'visibility': 'visible',
                                'width': wT * r
                            });
                        } else {
                            var r = w / wT;
                            var d = (Math.abs(h - (hT * r))) * 0.5;
                            switch (alignment) {
                            case 'topLeft':
                                mTop = 0;
                                break;
                            case 'topCenter':
                                mTop = 0;
                                break;
                            case 'topRight':
                                mTop = 0;
                                break;
                            case 'centerLeft':
                                mTop = d + 'px';
                                break;
                            case 'center':
                                mTop = d + 'px';
                                break;
                            case 'centerRight':
                                mTop = d + 'px';
                                break;
                            case 'bottomLeft':
                                mTop = d * 2 + 'px';
                                break;
                            case 'bottomCenter':
                                mTop = d * 2 + 'px';
                                break;
                            case 'bottomRight':
                                mTop = d * 2 + 'px';
                                break;
                            }
                            t.css({
                                'height': hT * r,
                                'margin-left': 0,
                                'margin-top': mTop,
                                'position': 'absolute',
                                'visibility': 'visible',
                                'width': w
                            });
                        }
                    }
                });
            }

            if (started == true) {
                clearTimeout(res);
                res = setTimeout(resizeImageWork, 200);
            } else {
                resizeImageWork();
            }

            started = true;
        }


        var u,
            setT;

        var clickEv,
            autoAdv,
            navHover,
            commands,
            pagination;

        var videoHover,
            videoPresent;

        if (isMobile() && opts.mobileAutoAdvance != '') {
            autoAdv = opts.mobileAutoAdvance;
        } else {
            autoAdv = opts.autoAdvance;
        }

        if (autoAdv == false) {
            elem.addClass('paused');
        }

        if (isMobile() && opts.mobileNavHover != '') {
            navHover = opts.mobileNavHover;
        } else {
            navHover = opts.navigationHover;
        }

        if (elem.length != 0) {

            var selector = $('.camera-slide', target);
            selector.wrapInner('<div class="camera-relative" />');

            var navSlide;

            var barDirection = opts.barDirection;

            var camera_thumbs_wrap = wrap;


            $('iframe', fakeHover).each(function() {
                var t = $(this);
                var src = t.attr('src');
                t.attr('data-src', src);
                var divInd = t.parent().index('.camera-src > div');
                $('.camera-target-content .camera-content:eq(' + divInd + ')', wrap).append(t);
            });

            function imgFake() {
                $('iframe', fakeHover).each(function() {
                    $('.camera-caption', fakeHover).show();
                    var t = $(this);
                    var cloneSrc = t.attr('data-src');
                    t.attr('src', cloneSrc);
                    var imgFakeUrl = opts.imagePath + 'blank.gif';
                    var imgFake = new Image();
                    imgFake.src = imgFakeUrl;
                    if (opts.height.indexOf('%') != -1) {
                        var startH = Math.round(w / (100 / parseFloat(opts.height)));
                        if (opts.minHeight != '' && startH < parseFloat(opts.minHeight)) {
                            h = parseFloat(opts.minHeight);
                        } else {
                            h = startH;
                        }
                    } else if (opts.height == 'auto') {
                        h = wrap.height();
                    } else {
                        h = parseFloat(opts.height);
                    }
                    t.after($(imgFake).attr({ 'class': 'img-fake', 'width': w, 'height': h }));
                    var clone = t.clone();
                    t.remove();
                    $(imgFake).bind('click', function() {
                        if ($(this).css('position') == 'absolute') {
                            $(this).remove();
                            if (cloneSrc.indexOf('vimeo') != -1 || cloneSrc.indexOf('youtube') != -1) {
                                if (cloneSrc.indexOf('?') != -1) {
                                    autoplay = '&autoplay=1';
                                } else {
                                    autoplay = '?autoplay=1';
                                }
                            } else if (cloneSrc.indexOf('dailymotion') != -1) {
                                if (cloneSrc.indexOf('?') != -1) {
                                    autoplay = '&autoPlay=1';
                                } else {
                                    autoplay = '?autoPlay=1';
                                }
                            }
                            clone.attr('src', cloneSrc + autoplay);
                            videoPresent = true;
                        } else {
                            $(this).css({ position: 'absolute', top: 0, left: 0, zIndex: 10 }).after(clone);
                        }
                    });
                });
            }

            imgFake();


            if (opts.hover == true) {
                if (!isMobile()) {
                    fakeHover.hover(function() {
                        elem.addClass('hovered');
                    }, function() {
                        elem.removeClass('hovered');
                    });
                }
            }

            if (navHover == true) {
                $(prevNav, wrap).animate({ opacity: 0 }, 0);
                $(nextNav, wrap).animate({ opacity: 0 }, 0);
                $(commands, wrap).animate({ opacity: 0 }, 0);
                if (isMobile()) {
                    fakeHover.live('vmouseover', function() {
                        $(prevNav, wrap).animate({ opacity: 1 }, 200);
                        $(nextNav, wrap).animate({ opacity: 1 }, 200);
                        $(commands, wrap).animate({ opacity: 1 }, 200);
                    });
                    fakeHover.live('vmouseout', function() {
                        $(prevNav, wrap).delay(500).animate({ opacity: 0 }, 200);
                        $(nextNav, wrap).delay(500).animate({ opacity: 0 }, 200);
                        $(commands, wrap).delay(500).animate({ opacity: 0 }, 200);
                    });
                } else {
                    fakeHover.hover(function() {
                        $(prevNav, wrap).animate({ opacity: 1 }, 200);
                        $(nextNav, wrap).animate({ opacity: 1 }, 200);
                        $(commands, wrap).animate({ opacity: 1 }, 200);
                    }, function() {
                        $(prevNav, wrap).animate({ opacity: 0 }, 200);
                        $(nextNav, wrap).animate({ opacity: 0 }, 200);
                        $(commands, wrap).animate({ opacity: 0 }, 200);
                    });
                }
            }


            $('.camera-stop', camera_thumbs_wrap).live('click', function() {
                autoAdv = false;
                elem.addClass('paused');
                if ($('.camera-stop', camera_thumbs_wrap).length) {
                    $('.camera-stop', camera_thumbs_wrap).hide();
                    $('.camera-play', camera_thumbs_wrap).show();
                    if (loader != 'none') {
                        $('#' + pieID).hide();
                    }
                } else {
                    if (loader != 'none') {
                        $('#' + pieID).hide();
                    }
                }
            });

            $('.camera-play', camera_thumbs_wrap).live('click', function() {
                autoAdv = true;
                elem.removeClass('paused');
                if ($('.camera-play', camera_thumbs_wrap).length) {
                    $('.camera-play', camera_thumbs_wrap).hide();
                    $('.camera-stop', camera_thumbs_wrap).show();
                    if (loader != 'none') {
                        $('#' + pieID).show();
                    }
                } else {
                    if (loader != 'none') {
                        $('#' + pieID).show();
                    }
                }
            });

            if (opts.pauseOnClick == true) {
                $('.camera-target-content', fakeHover).mouseup(function() {
                    autoAdv = false;
                    elem.addClass('paused');
                    $('.camera-stop', camera_thumbs_wrap).hide();
                    $('.camera-play', camera_thumbs_wrap).show();
                    $('#' + pieID).hide();
                });
            }
            $('.camera-content, .img-fake', fakeHover).hover(function() {
                videoHover = true;
            }, function() {
                videoHover = false;
            });

            $('.camera-content, .img-fake', fakeHover).bind('click', function() {
                if (videoPresent == true && videoHover == true) {
                    autoAdv = false;
                    $('.camera-caption', fakeHover).hide();
                    elem.addClass('paused');
                    $('.camera-stop', camera_thumbs_wrap).hide();
                    $('.camera-play', camera_thumbs_wrap).show();
                    $('#' + pieID).hide();
                }
            });


        }


        function shuffle(arr) {
            for (
                var j, x, i = arr.length; i;
                j = parseInt(Math.random() * i),
                x = arr[--i], arr[i] = arr[j], arr[j] = x
                ) ;
            return arr;
        }

        function isInteger(s) {
            return Math.ceil(s) == Math.floor(s);
        }

        if (loader != 'pie') {
            barContainer.append('<span class="camera-bar-cont" />');
            $('.camera-bar-cont', barContainer)
                .animate({ opacity: opts.loaderOpacity }, 0)
                .css({ 'position': 'absolute', 'left': 0, 'right': 0, 'top': 0, 'bottom': 0, 'background-color': opts.loaderBgColor })
                .append('<span id="' + pieID + '" />');
            $('#' + pieID).animate({ opacity: 0 }, 0);
            var canvas = $('#' + pieID);
            canvas.css({ 'position': 'absolute', 'background-color': opts.loaderColor });
            switch (opts.barPosition) {
            case 'left':
                barContainer.css({ right: 'auto', width: opts.loaderStroke });
                break;
            case 'right':
                barContainer.css({ left: 'auto', width: opts.loaderStroke });
                break;
            case 'top':
                barContainer.css({ bottom: 'auto', height: opts.loaderStroke });
                break;
            case 'bottom':
                barContainer.css({ top: 'auto', height: opts.loaderStroke });
                break;
            }
            switch (barDirection) {
            case 'leftToRight':
                canvas.css({ 'left': 0, 'right': 0, 'top': opts.loaderPadding, 'bottom': opts.loaderPadding });
                break;
            case 'rightToLeft':
                canvas.css({ 'left': 0, 'right': 0, 'top': opts.loaderPadding, 'bottom': opts.loaderPadding });
                break;
            case 'topToBottom':
                canvas.css({ 'left': opts.loaderPadding, 'right': opts.loaderPadding, 'top': 0, 'bottom': 0 });
                break;
            case 'bottomToTop':
                canvas.css({ 'left': opts.loaderPadding, 'right': opts.loaderPadding, 'top': 0, 'bottom': 0 });
                break;
            }
        } else {
            pieContainer.append('<canvas id="' + pieID + '"></canvas>');
            var G_vmlCanvasManager;
            var canvas = document.getElementById(pieID);
            canvas.setAttribute("width", opts.pieDiameter);
            canvas.setAttribute("height", opts.pieDiameter);
            var piePosition;
            switch (opts.piePosition) {
            case 'leftTop':
                piePosition = 'left:0; top:0;';
                break;
            case 'rightTop':
                piePosition = 'right:0; top:0;';
                break;
            case 'leftBottom':
                piePosition = 'left:0; bottom:0;';
                break;
            case 'rightBottom':
                piePosition = 'right:0; bottom:0;';
                break;
            }
            canvas.setAttribute("style", "position:absolute; z-index:1002; " + piePosition);
            var rad;
            var radNew;

            if (canvas && canvas.getContext) {
                var ctx = canvas.getContext("2d");
                ctx.rotate(Math.PI * (3 / 2));
                ctx.translate(-opts.pieDiameter, 0);
            }

        }
        if (loader == 'none' || autoAdv == false) {
            $('#' + pieID).hide();
            $('.camera-canvas-wrap', camera_thumbs_wrap).hide();
        }

        if ($(pagination).length) {
            $(pagination).append('<ul class="camera-page-ul" />');
            var li;
            for (li = 0; li < amountSlide; li++) {
                $('.camera-page-ul', wrap).append('<li class="page-nav-' + li + '" style="position:relative; z-index:1002"><span><span>' + li + '</span></span></li>');
            }
            $('.camera-page-ul li', wrap).hover(function() {
                $(this).addClass('camera-hover');
                if ($('.camera-thumb', this).length) {
                    var wTh = $('.camera-thumb', this).outerWidth(),
                        hTh = $('.camera-thumb', this).outerHeight(),
                        wTt = $(this).outerWidth();
                    $('.camera-thumb', this).show().css({ 'top': '-' + hTh + 'px', 'left': '-' + (wTh - wTt) / 2 + 'px' }).animate({ 'opacity': 1, 'margin-top': '-3px' }, 200);
                    $('.thumb-arrow', this).show().animate({ 'opacity': 1, 'margin-top': '-3px' }, 200);
                }
            }, function() {
                $(this).removeClass('camera-hover');
                $('.camera-thumb', this).animate({ 'margin-top': '-20px', 'opacity': 0 }, 200, function() {
                    $(this).css({ marginTop: '5px' }).hide();
                });
                $('.thumb-arrow', this).animate({ 'margin-top': '-20px', 'opacity': 0 }, 200, function() {
                    $(this).css({ marginTop: '5px' }).hide();
                });
            });
        }


        if ($(thumbs).length) {
            var thumbUrl;
            if (!$(pagination).length) {
                $(thumbs).append('<div />');
                $(thumbs).before('<div class="camera-prev-thumbs hide-nav"><div></div></div>').before('<div class="camera-next-thumbs hide-nav"><div></div></div>');
                $('> div', thumbs).append('<ul />');
                $.each(allThumbs, function(i, val) {
                    if ($('> div', elem).eq(i).attr('data-thumb') != '') {
                        var thumbUrl = $('> div', elem).eq(i).attr('data-thumb'),
                            newImg = new Image();
                        newImg.src = thumbUrl;
                        $('ul', thumbs).append('<li class="pix-thumb pix-thumb-' + i + '" />');
                        $('li.pix-thumb-' + i, thumbs).append($(newImg).attr('class', 'camera-thumb'));
                    }
                });
            } else {
                $.each(allThumbs, function(i, val) {
                    if ($('> div', elem).eq(i).attr('data-thumb') != '') {
                        var thumbUrl = $('> div', elem).eq(i).attr('data-thumb'),
                            newImg = new Image();
                        newImg.src = thumbUrl;
                        $('li.page-nav-' + i, pagination).append($(newImg).attr('class', 'camera-thumb').css({ 'position': 'absolute' }).animate({ opacity: 0 }, 0));
                        $('li.page-nav-' + i + ' > img', pagination).after('<div class="thumb-arrow" />');
                        $('li.page-nav-' + i + ' > .thumb-arrow', pagination).animate({ opacity: 0 }, 0);
                    }
                });
                wrap.css({ marginBottom: $(pagination).outerHeight() });
            }
        } else if (!$(thumbs).length && $(pagination).length) {
            wrap.css({ marginBottom: $(pagination).outerHeight() });
        }


        var firstPos = true;

        function thumbnailPos() {
            if ($(thumbs).length && !$(pagination).length) {
                var wTh = $(thumbs).outerWidth(),
                    owTh = $('ul > li', thumbs).outerWidth(),
                    pos = $('li.camera-current', thumbs).position(),
                    ulW = ($('ul > li', thumbs).length * $('ul > li', thumbs).outerWidth()),
                    offUl = $('ul', thumbs).offset().left,
                    offDiv = $('> div', thumbs).offset().left,
                    ulLeft;

                if (offUl < 0) {
                    ulLeft = '-' + (offDiv - offUl);
                } else {
                    ulLeft = offDiv - offUl;
                }


                if (firstPos == true) {
                    $('ul', thumbs).width($('ul > li', thumbs).length * $('ul > li', thumbs).outerWidth());
                    if ($(thumbs).length && !$(pagination).lenght) {
                        wrap.css({ marginBottom: $(thumbs).outerHeight() });
                    }
                    thumbnailVisible();
                    /*I repeat this two lines because of a problem with iPhones*/
                    $('ul', thumbs).width($('ul > li', thumbs).length * $('ul > li', thumbs).outerWidth());
                    if ($(thumbs).length && !$(pagination).lenght) {
                        wrap.css({ marginBottom: $(thumbs).outerHeight() });
                    }
                    /*...*/
                }
                firstPos = false;

                $('.camera-prev-thumbs', camera_thumbs_wrap).css('visibility', 'visible');
                $('.camera-next-thumbs', camera_thumbs_wrap).css('visibility', 'visible');
                var left = pos.left,
                    right = pos.left + ($('li.camera-current', thumbs).outerWidth());
                if (left < $('li.camera-current', thumbs).outerWidth()) {
                    left = 0;
                }
                if (right - ulLeft > wTh) {
                    if ((left + wTh) < ulW) {
                        $('ul', thumbs).animate({ 'margin-left': '-' + (left) + 'px' }, 500, thumbnailVisible);
                    } else {
                        $('ul', thumbs).animate({ 'margin-left': '-' + ($('ul', thumbs).outerWidth() - wTh) + 'px' }, 500, thumbnailVisible);
                    }
                } else if (left - ulLeft < 0) {
                    $('ul', thumbs).animate({ 'margin-left': '-' + (left) + 'px' }, 500, thumbnailVisible);
                } else {
                    $('ul', thumbs).css({ 'margin-left': 'auto', 'margin-right': 'auto' });
                    setTimeout(thumbnailVisible, 100);
                }

            }
        }

        if ($(commands).length) {
            $(commands).append('<div class="camera-play"></div>').append('<div class="camera-stop"></div>');
            if (autoAdv == true) {
                $('.camera-play', camera_thumbs_wrap).hide();
                $('.camera-stop', camera_thumbs_wrap).show();
            } else {
                $('.camera-stop', camera_thumbs_wrap).hide();
                $('.camera-play', camera_thumbs_wrap).show();
            }

        }


        function canvasLoader() {
            rad = 0;
            var barWidth = $('.camera-bar-cont', camera_thumbs_wrap).width(),
                barHeight = $('.camera-bar-cont', camera_thumbs_wrap).height();

            if (loader != 'pie') {
                switch (barDirection) {
                case 'leftToRight':
                    $('#' + pieID).css({ 'right': barWidth });
                    break;
                case 'rightToLeft':
                    $('#' + pieID).css({ 'left': barWidth });
                    break;
                case 'topToBottom':
                    $('#' + pieID).css({ 'bottom': barHeight });
                    break;
                case 'bottomToTop':
                    $('#' + pieID).css({ 'top': barHeight });
                    break;
                }
            } else {
                ctx.clearRect(0, 0, opts.pieDiameter, opts.pieDiameter);
            }
        }


        canvasLoader();


        $('.moveFromLeft, .moveFromRight, .moveFromTop, .moveFromBottom, .fadeIn, .fadeFromLeft, .fadeFromRight, .fadeFromTop, .fadeFromBottom', fakeHover).each(function() {
            $(this).css('visibility', 'hidden');
        });

        opts.onStartLoading.call(this);

        nextSlide();


        /*************************** FUNCTION nextSlide() ***************************/

        function nextSlide(navSlide) {
            elem.addClass('camera-sliding');

            videoPresent = false;
            var vis = parseFloat($('div.camera-slide.camera-current', target).index());

            if (navSlide > 0) {
                var slideI = navSlide - 1;
            } else if (vis == amountSlide - 1) {
                var slideI = 0;
            } else {
                var slideI = vis + 1;
            }


            var slide = $('.camera-slide:eq(' + slideI + ')', target);
            $('.camera-content', fakeHover).fadeOut(600);
            $('.camera-caption', fakeHover).show();

            $('.camera-relative', slide).append($('> div ', elem).eq(slideI).find('> div.camera-effected'));

            $('.camera-target-content .camera-content:eq(' + slideI + ')', wrap).append($('> div ', elem).eq(slideI).find('> div'));

            if (!$('.img-loaded', slide).length) {
                var imgUrl = allImg[slideI];
                var imgLoaded = new Image();
                imgLoaded.src = imgUrl + "?" + new Date().getTime();
                slide.css('visibility', 'hidden');
                slide.prepend($(imgLoaded).attr('class', 'img-loaded').css('visibility', 'hidden'));
                var wT, hT;
                if (!$(imgLoaded).get(0).complete || wT == '0' || hT == '0' || typeof wT === 'undefined' || wT === false || typeof hT === 'undefined' || hT === false) {
                    $('.camera-loader', wrap).delay(500).fadeIn(400);
                    imgLoaded.onload = function() {
                        wT = imgLoaded.naturalWidth;
                        hT = imgLoaded.naturalHeight;
                        $(imgLoaded).attr('data-alignment', allAlign[slideI]).attr('data-portrait', allPor[slideI]);
                        $(imgLoaded).attr('width', wT);
                        $(imgLoaded).attr('height', hT);
                        target.find('.camera-slide-' + slideI).hide().css('visibility', 'visible');
                        resizeImage();
                        nextSlide(slideI + 1);
                    };
                } else {
                    wT = imgLoaded.naturalWidth;
                    hT = imgLoaded.naturalHeight;
                    $(imgLoaded).attr('width', wT);
                    $(imgLoaded).attr('height', hT);
                    $(imgLoaded).attr('data-alignment', allAlign[slideI]);
                    $(imgLoaded).attr('data-portrait', allPor[slideI]);
                    target.find('.camera-slide-' + slideI).hide().css('visibility', 'visible');
                    resizeImage();
                    imgLoaded.onload = function() {
                        nextSlide(slideI + 1);
                    };
                }
            } else {
                opts.onLoaded.call(this);
                if ($('.camera-loader', wrap).is(':visible')) {
                    $('.camera-loader', wrap).fadeOut(400);
                } else {
                    $('.camera-loader', wrap).css({ 'visibility': 'hidden' });
                    $('.camera-loader', wrap).fadeOut(400, function() {
                        $('.camera-loader', wrap).css({ 'visibility': 'visible' });
                    });
                }
                var rows = opts.rows,
                    cols = opts.cols,
                    couples = 1,
                    difference = 0,
                    dataSlideOn,
                    time,
                    transPeriod,
                    fx,
                    easing,
                    randomFx = new Array('simpleFade', 'curtainTopLeft', 'curtainTopRight', 'curtainBottomLeft', 'curtainBottomRight', 'curtainSliceLeft', 'curtainSliceRight', 'blindCurtainTopLeft', 'blindCurtainTopRight', 'blindCurtainBottomLeft', 'blindCurtainBottomRight', 'blindCurtainSliceBottom', 'blindCurtainSliceTop', 'stampede', 'mosaic', 'mosaicReverse', 'mosaicRandom', 'mosaicSpiral', 'mosaicSpiralReverse', 'topLeftBottomRight', 'bottomRightTopLeft', 'bottomLeftTopRight', 'topRightBottomLeft', 'scrollLeft', 'scrollRight', 'scrollTop', 'scrollBottom', 'scrollHorz');
                marginLeft = 0,
                marginTop = 0,
                opacityOnGrid = 0;

                if (opts.opacityOnGrid == true) {
                    opacityOnGrid = 0;
                } else {
                    opacityOnGrid = 1;
                }


                var dataFx = $(' > div', elem).eq(slideI).attr('data-fx');

                if (isMobile() && opts.mobileFx != '' && opts.mobileFx != 'default') {
                    fx = opts.mobileFx;
                } else {
                    if (typeof dataFx !== 'undefined' && dataFx !== false && dataFx !== 'default') {
                        fx = dataFx;
                    } else {
                        fx = opts.fx;
                    }
                }

                if (fx == 'random') {
                    fx = shuffle(randomFx);
                    fx = fx[0];
                } else {
                    fx = fx;
                    if (fx.indexOf(',') > 0) {
                        fx = fx.replace( / /g , '');
                        fx = fx.split(',');
                        fx = shuffle(fx);
                        fx = fx[0];
                    }
                }

                dataEasing = $(' > div', elem).eq(slideI).attr('data-easing');
                mobileEasing = $(' > div', elem).eq(slideI).attr('data-mobileEasing');

                if (isMobile() && opts.mobileEasing != '' && opts.mobileEasing != 'default') {
                    if (typeof mobileEasing !== 'undefined' && mobileEasing !== false && mobileEasing !== 'default') {
                        easing = mobileEasing;
                    } else {
                        easing = opts.mobileEasing;
                    }
                } else {
                    if (typeof dataEasing !== 'undefined' && dataEasing !== false && dataEasing !== 'default') {
                        easing = dataEasing;
                    } else {
                        easing = opts.easing;
                    }
                }

                dataSlideOn = $(' > div', elem).eq(slideI).attr('data-slideOn');
                if (typeof dataSlideOn !== 'undefined' && dataSlideOn !== false) {
                    slideOn = dataSlideOn;
                } else {
                    if (opts.slideOn == 'random') {
                        var slideOn = new Array('next', 'prev');
                        slideOn = shuffle(slideOn);
                        slideOn = slideOn[0];
                    } else {
                        slideOn = opts.slideOn;
                    }
                }

                var dataTime = $(' > div', elem).eq(slideI).attr('data-time');
                if (typeof dataTime !== 'undefined' && dataTime !== false && dataTime !== '') {
                    time = parseFloat(dataTime);
                } else {
                    time = opts.time;
                }

                var dataTransPeriod = $(' > div', elem).eq(slideI).attr('data-transPeriod');
                if (typeof dataTransPeriod !== 'undefined' && dataTransPeriod !== false && dataTransPeriod !== '') {
                    transPeriod = parseFloat(dataTransPeriod);
                } else {
                    transPeriod = opts.transPeriod;
                }

                if (!$(elem).hasClass('camera-started')) {
                    fx = 'simpleFade';
                    slideOn = 'next';
                    easing = '';
                    transPeriod = 400;
                    $(elem).addClass('camera-started');
                }

                switch (fx) {
                case 'simpleFade':
                    cols = 1;
                    rows = 1;
                    break;
                case 'curtainTopLeft':
                    if (opts.slicedCols == 0) {
                        cols = opts.cols;
                    } else {
                        cols = opts.slicedCols;
                    }
                    rows = 1;
                    break;
                case 'curtainTopRight':
                    if (opts.slicedCols == 0) {
                        cols = opts.cols;
                    } else {
                        cols = opts.slicedCols;
                    }
                    rows = 1;
                    break;
                case 'curtainBottomLeft':
                    if (opts.slicedCols == 0) {
                        cols = opts.cols;
                    } else {
                        cols = opts.slicedCols;
                    }
                    rows = 1;
                    break;
                case 'curtainBottomRight':
                    if (opts.slicedCols == 0) {
                        cols = opts.cols;
                    } else {
                        cols = opts.slicedCols;
                    }
                    rows = 1;
                    break;
                case 'curtainSliceLeft':
                    if (opts.slicedCols == 0) {
                        cols = opts.cols;
                    } else {
                        cols = opts.slicedCols;
                    }
                    rows = 1;
                    break;
                case 'curtainSliceRight':
                    if (opts.slicedCols == 0) {
                        cols = opts.cols;
                    } else {
                        cols = opts.slicedCols;
                    }
                    rows = 1;
                    break;
                case 'blindCurtainTopLeft':
                    if (opts.slicedRows == 0) {
                        rows = opts.rows;
                    } else {
                        rows = opts.slicedRows;
                    }
                    cols = 1;
                    break;
                case 'blindCurtainTopRight':
                    if (opts.slicedRows == 0) {
                        rows = opts.rows;
                    } else {
                        rows = opts.slicedRows;
                    }
                    cols = 1;
                    break;
                case 'blindCurtainBottomLeft':
                    if (opts.slicedRows == 0) {
                        rows = opts.rows;
                    } else {
                        rows = opts.slicedRows;
                    }
                    cols = 1;
                    break;
                case 'blindCurtainBottomRight':
                    if (opts.slicedRows == 0) {
                        rows = opts.rows;
                    } else {
                        rows = opts.slicedRows;
                    }
                    cols = 1;
                    break;
                case 'blindCurtainSliceTop':
                    if (opts.slicedRows == 0) {
                        rows = opts.rows;
                    } else {
                        rows = opts.slicedRows;
                    }
                    cols = 1;
                    break;
                case 'blindCurtainSliceBottom':
                    if (opts.slicedRows == 0) {
                        rows = opts.rows;
                    } else {
                        rows = opts.slicedRows;
                    }
                    cols = 1;
                    break;
                case 'stampede':
                    difference = '-' + transPeriod;
                    break;
                case 'mosaic':
                    difference = opts.gridDifference;
                    break;
                case 'mosaicReverse':
                    difference = opts.gridDifference;
                    break;
                case 'mosaicRandom':
                    break;
                case 'mosaicSpiral':
                    difference = opts.gridDifference;
                    couples = 1.7;
                    break;
                case 'mosaicSpiralReverse':
                    difference = opts.gridDifference;
                    couples = 1.7;
                    break;
                case 'topLeftBottomRight':
                    difference = opts.gridDifference;
                    couples = 6;
                    break;
                case 'bottomRightTopLeft':
                    difference = opts.gridDifference;
                    couples = 6;
                    break;
                case 'bottomLeftTopRight':
                    difference = opts.gridDifference;
                    couples = 6;
                    break;
                case 'topRightBottomLeft':
                    difference = opts.gridDifference;
                    couples = 6;
                    break;
                case 'scrollLeft':
                    cols = 1;
                    rows = 1;
                    break;
                case 'scrollRight':
                    cols = 1;
                    rows = 1;
                    break;
                case 'scrollTop':
                    cols = 1;
                    rows = 1;
                    break;
                case 'scrollBottom':
                    cols = 1;
                    rows = 1;
                    break;
                case 'scrollHorz':
                    cols = 1;
                    rows = 1;
                    break;
                }

                var cycle = 0;
                var blocks = rows * cols;
                var leftScrap = w - (Math.floor(w / cols) * cols);
                var topScrap = h - (Math.floor(h / rows) * rows);
                var addLeft;
                var addTop;
                var tAppW = 0;
                var tAppH = 0;
                var arr = new Array();
                var delay = new Array();
                var order = new Array();
                while (cycle < blocks) {
                    arr.push(cycle);
                    delay.push(cycle);
                    cameraCont.append('<div class="camera-appended" style="display:none; overflow:hidden; position:absolute; z-index:1000" />');
                    var tApp = $('.camera-appended:eq(' + cycle + ')', target);
                    if (fx == 'scrollLeft' || fx == 'scrollRight' || fx == 'scrollTop' || fx == 'scrollBottom' || fx == 'scrollHorz') {
                        selector.eq(slideI).clone().show().appendTo(tApp);
                    } else {
                        if (slideOn == 'next') {
                            selector.eq(slideI).clone().show().appendTo(tApp);
                        } else {
                            selector.eq(vis).clone().show().appendTo(tApp);
                        }
                    }

                    if (cycle % cols < leftScrap) {
                        addLeft = 1;
                    } else {
                        addLeft = 0;
                    }
                    if (cycle % cols == 0) {
                        tAppW = 0;
                    }
                    if (Math.floor(cycle / cols) < topScrap) {
                        addTop = 1;
                    } else {
                        addTop = 0;
                    }
                    tApp.css({
                        'height': Math.floor((h / rows) + addTop + 1),
                        'left': tAppW,
                        'top': tAppH,
                        'width': Math.floor((w / cols) + addLeft + 1)
                    });
                    $('> .camera-slide', tApp).css({
                        'height': h,
                        'margin-left': '-' + tAppW + 'px',
                        'margin-top': '-' + tAppH + 'px',
                        'width': w
                    });
                    tAppW = tAppW + tApp.width() - 1;
                    if (cycle % cols == cols - 1) {
                        tAppH = tAppH + tApp.height() - 1;
                    }
                    cycle++;
                }


                switch (fx) {
                case 'curtainTopLeft':
                    break;
                case 'curtainBottomLeft':
                    break;
                case 'curtainSliceLeft':
                    break;
                case 'curtainTopRight':
                    arr = arr.reverse();
                    break;
                case 'curtainBottomRight':
                    arr = arr.reverse();
                    break;
                case 'curtainSliceRight':
                    arr = arr.reverse();
                    break;
                case 'blindCurtainTopLeft':
                    break;
                case 'blindCurtainBottomLeft':
                    arr = arr.reverse();
                    break;
                case 'blindCurtainSliceTop':
                    break;
                case 'blindCurtainTopRight':
                    break;
                case 'blindCurtainBottomRight':
                    arr = arr.reverse();
                    break;
                case 'blindCurtainSliceBottom':
                    arr = arr.reverse();
                    break;
                case 'stampede':
                    arr = shuffle(arr);
                    break;
                case 'mosaic':
                    break;
                case 'mosaicReverse':
                    arr = arr.reverse();
                    break;
                case 'mosaicRandom':
                    arr = shuffle(arr);
                    break;
                case 'mosaicSpiral':
                    var rows2 = rows / 2, x, y, z, n = 0;
                    for (z = 0; z < rows2; z++) {
                        y = z;
                        for (x = z; x < cols - z - 1; x++) {
                            order[n++] = y * cols + x;
                        }
                        x = cols - z - 1;
                        for (y = z; y < rows - z - 1; y++) {
                            order[n++] = y * cols + x;
                        }
                        y = rows - z - 1;
                        for (x = cols - z - 1; x > z; x--) {
                            order[n++] = y * cols + x;
                        }
                        x = z;
                        for (y = rows - z - 1; y > z; y--) {
                            order[n++] = y * cols + x;
                        }
                    }

                    arr = order;

                    break;
                case 'mosaicSpiralReverse':
                    var rows2 = rows / 2, x, y, z, n = blocks - 1;
                    for (z = 0; z < rows2; z++) {
                        y = z;
                        for (x = z; x < cols - z - 1; x++) {
                            order[n--] = y * cols + x;
                        }
                        x = cols - z - 1;
                        for (y = z; y < rows - z - 1; y++) {
                            order[n--] = y * cols + x;
                        }
                        y = rows - z - 1;
                        for (x = cols - z - 1; x > z; x--) {
                            order[n--] = y * cols + x;
                        }
                        x = z;
                        for (y = rows - z - 1; y > z; y--) {
                            order[n--] = y * cols + x;
                        }
                    }

                    arr = order;

                    break;
                case 'topLeftBottomRight':
                    for (var y = 0; y < rows; y++)
                        for (var x = 0; x < cols; x++) {
                            order.push(x + y);
                        }
                    delay = order;
                    break;
                case 'bottomRightTopLeft':
                    for (var y = 0; y < rows; y++)
                        for (var x = 0; x < cols; x++) {
                            order.push(x + y);
                        }
                    delay = order.reverse();
                    break;
                case 'bottomLeftTopRight':
                    for (var y = rows; y > 0; y--)
                        for (var x = 0; x < cols; x++) {
                            order.push(x + y);
                        }
                    delay = order;
                    break;
                case 'topRightBottomLeft':
                    for (var y = 0; y < rows; y++)
                        for (var x = cols; x > 0; x--) {
                            order.push(x + y);
                        }
                    delay = order;
                    break;
                }


                $.each(arr, function(index, value) {

                    if (value % cols < leftScrap) {
                        addLeft = 1;
                    } else {
                        addLeft = 0;
                    }
                    if (value % cols == 0) {
                        tAppW = 0;
                    }
                    if (Math.floor(value / cols) < topScrap) {
                        addTop = 1;
                    } else {
                        addTop = 0;
                    }

                    switch (fx) {
                    case 'simpleFade':
                        height = h;
                        width = w;
                        opacityOnGrid = 0;
                        break;
                    case 'curtainTopLeft':
                        height = 0,
                        width = Math.floor((w / cols) + addLeft + 1),
                        marginTop = '-' + Math.floor((h / rows) + addTop + 1) + 'px';
                        break;
                    case 'curtainTopRight':
                        height = 0,
                        width = Math.floor((w / cols) + addLeft + 1),
                        marginTop = '-' + Math.floor((h / rows) + addTop + 1) + 'px';
                        break;
                    case 'curtainBottomLeft':
                        height = 0,
                        width = Math.floor((w / cols) + addLeft + 1),
                        marginTop = Math.floor((h / rows) + addTop + 1) + 'px';
                        break;
                    case 'curtainBottomRight':
                        height = 0,
                        width = Math.floor((w / cols) + addLeft + 1),
                        marginTop = Math.floor((h / rows) + addTop + 1) + 'px';
                        break;
                    case 'curtainSliceLeft':
                        height = 0,
                        width = Math.floor((w / cols) + addLeft + 1);
                        if (value % 2 == 0) {
                            marginTop = Math.floor((h / rows) + addTop + 1) + 'px';
                        } else {
                            marginTop = '-' + Math.floor((h / rows) + addTop + 1) + 'px';
                        }
                        break;
                    case 'curtainSliceRight':
                        height = 0,
                        width = Math.floor((w / cols) + addLeft + 1);
                        if (value % 2 == 0) {
                            marginTop = Math.floor((h / rows) + addTop + 1) + 'px';
                        } else {
                            marginTop = '-' + Math.floor((h / rows) + addTop + 1) + 'px';
                        }
                        break;
                    case 'blindCurtainTopLeft':
                        height = Math.floor((h / rows) + addTop + 1),
                        width = 0,
                        marginLeft = '-' + Math.floor((w / cols) + addLeft + 1) + 'px';
                        break;
                    case 'blindCurtainTopRight':
                        height = Math.floor((h / rows) + addTop + 1),
                        width = 0,
                        marginLeft = Math.floor((w / cols) + addLeft + 1) + 'px';
                        break;
                    case 'blindCurtainBottomLeft':
                        height = Math.floor((h / rows) + addTop + 1),
                        width = 0,
                        marginLeft = '-' + Math.floor((w / cols) + addLeft + 1) + 'px';
                        break;
                    case 'blindCurtainBottomRight':
                        height = Math.floor((h / rows) + addTop + 1),
                        width = 0,
                        marginLeft = Math.floor((w / cols) + addLeft + 1) + 'px';
                        break;
                    case 'blindCurtainSliceBottom':
                        height = Math.floor((h / rows) + addTop + 1),
                        width = 0;
                        if (value % 2 == 0) {
                            marginLeft = '-' + Math.floor((w / cols) + addLeft + 1) + 'px';
                        } else {
                            marginLeft = Math.floor((w / cols) + addLeft + 1) + 'px';
                        }
                        break;
                    case 'blindCurtainSliceTop':
                        height = Math.floor((h / rows) + addTop + 1),
                        width = 0;
                        if (value % 2 == 0) {
                            marginLeft = '-' + Math.floor((w / cols) + addLeft + 1) + 'px';
                        } else {
                            marginLeft = Math.floor((w / cols) + addLeft + 1) + 'px';
                        }
                        break;
                    case 'stampede':
                        height = 0;
                        width = 0;
                        marginLeft = (w * 0.2) * (((index) % cols) - (cols - (Math.floor(cols / 2)))) + 'px';
                        marginTop = (h * 0.2) * ((Math.floor(index / cols) + 1) - (rows - (Math.floor(rows / 2)))) + 'px';
                        break;
                    case 'mosaic':
                        height = 0;
                        width = 0;
                        break;
                    case 'mosaicReverse':
                        height = 0;
                        width = 0;
                        marginLeft = Math.floor((w / cols) + addLeft + 1) + 'px';
                        marginTop = Math.floor((h / rows) + addTop + 1) + 'px';
                        break;
                    case 'mosaicRandom':
                        height = 0;
                        width = 0;
                        marginLeft = Math.floor((w / cols) + addLeft + 1) * 0.5 + 'px';
                        marginTop = Math.floor((h / rows) + addTop + 1) * 0.5 + 'px';
                        break;
                    case 'mosaicSpiral':
                        height = 0;
                        width = 0;
                        marginLeft = Math.floor((w / cols) + addLeft + 1) * 0.5 + 'px';
                        marginTop = Math.floor((h / rows) + addTop + 1) * 0.5 + 'px';
                        break;
                    case 'mosaicSpiralReverse':
                        height = 0;
                        width = 0;
                        marginLeft = Math.floor((w / cols) + addLeft + 1) * 0.5 + 'px';
                        marginTop = Math.floor((h / rows) + addTop + 1) * 0.5 + 'px';
                        break;
                    case 'topLeftBottomRight':
                        height = 0;
                        width = 0;
                        break;
                    case 'bottomRightTopLeft':
                        height = 0;
                        width = 0;
                        marginLeft = Math.floor((w / cols) + addLeft + 1) + 'px';
                        marginTop = Math.floor((h / rows) + addTop + 1) + 'px';
                        break;
                    case 'bottomLeftTopRight':
                        height = 0;
                        width = 0;
                        marginLeft = 0;
                        marginTop = Math.floor((h / rows) + addTop + 1) + 'px';
                        break;
                    case 'topRightBottomLeft':
                        height = 0;
                        width = 0;
                        marginLeft = Math.floor((w / cols) + addLeft + 1) + 'px';
                        marginTop = 0;
                        break;
                    case 'scrollRight':
                        height = h;
                        width = w;
                        marginLeft = -w;
                        break;
                    case 'scrollLeft':
                        height = h;
                        width = w;
                        marginLeft = w;
                        break;
                    case 'scrollTop':
                        height = h;
                        width = w;
                        marginTop = h;
                        break;
                    case 'scrollBottom':
                        height = h;
                        width = w;
                        marginTop = -h;
                        break;
                    case 'scrollHorz':
                        height = h;
                        width = w;
                        if (vis == 0 && slideI == amountSlide - 1) {
                            marginLeft = -w;
                        } else if (vis < slideI || (vis == amountSlide - 1 && slideI == 0)) {
                            marginLeft = w;
                        } else {
                            marginLeft = -w;
                        }
                        break;
                    }


                    var tApp = $('.camera-appended:eq(' + value + ')', target);

                    if (typeof u !== 'undefined') {
                        clearInterval(u);
                        clearTimeout(setT);
                        setT = setTimeout(canvasLoader, transPeriod + difference);
                    }


                    if ($(pagination).length) {
                        $('.camera-page li', wrap).removeClass('camera-current');
                        $('.camera-page li', wrap).eq(slideI).addClass('camera-current');
                    }

                    if ($(thumbs).length) {
                        $('li', thumbs).removeClass('camera-current');
                        $('li', thumbs).eq(slideI).addClass('camera-current');
                        $('li', thumbs).not('.camera-current').find('img').animate({ opacity: .5 }, 0);
                        $('li.camera-current img', thumbs).animate({ opacity: 1 }, 0);
                        $('li', thumbs).hover(function() {
                            $('img', this).stop(true, false).animate({ opacity: 1 }, 150);
                        }, function() {
                            if (!$(this).hasClass('camera-current')) {
                                $('img', this).stop(true, false).animate({ opacity: .5 }, 150);
                            }
                        });
                    }


                    var easedTime = parseFloat(transPeriod) + parseFloat(difference);

                    function cameraeased() {
                        opts.onEndTransition.call(this);

                        $(this).addClass('camera-eased');
                        if ($('.camera-eased', target).length >= 0) {
                            $(thumbs).css({ visibility: 'visible' });
                        }
                        if ($('.camera-eased', target).length == blocks) {

                            thumbnailPos();

                            $('.moveFromLeft, .moveFromRight, .moveFromTop, .moveFromBottom, .fadeIn, .fadeFromLeft, .fadeFromRight, .fadeFromTop, .fadeFromBottom', fakeHover).each(function() {
                                $(this).css('visibility', 'hidden');
                            });

                            selector.eq(slideI).show().css('z-index', '999').addClass('camera-current');
                            selector.eq(vis).css('z-index', '1').removeClass('camera-current');
                            $('.camera-content', fakeHover).eq(slideI).addClass('camera-current');
                            if (vis >= 0) {
                                $('.camera-content', fakeHover).eq(vis).removeClass('camera-current');
                            }

                            if ($('> div', elem).eq(slideI).attr('data-video') != 'hide' && $('.camera-content.camera-current .img-fake', fakeHover).length) {
                                $('.camera-content.camera-current .img-fake', fakeHover).click();
                            }


                            var lMoveIn = selector.eq(slideI).find('.fadeIn').length;
                            var lMoveInContent = $('.camera-content', fakeHover).eq(slideI).find('.moveFromLeft, .moveFromRight, .moveFromTop, .moveFromBottom, .fadeIn, .fadeFromLeft, .fadeFromRight, .fadeFromTop, .fadeFromBottom').length;

                            if (lMoveIn != 0) {
                                $('.camera-slide.camera-current .fadeIn', fakeHover).each(function() {
                                    if ($(this).attr('data-easing') != '') {
                                        var easeMove = $(this).attr('data-easing');
                                    } else {
                                        var easeMove = easing;
                                    }
                                    var t = $(this);
                                    if (typeof t.attr('data-outerWidth') === 'undefined' || t.attr('data-outerWidth') === false || t.attr('data-outerWidth') === '') {
                                        var wMoveIn = t.outerWidth();
                                        t.attr('data-outerWidth', wMoveIn);
                                    } else {
                                        var wMoveIn = t.attr('data-outerWidth');
                                    }
                                    if (typeof t.attr('data-outerHeight') === 'undefined' || t.attr('data-outerHeight') === false || t.attr('data-outerHeight') === '') {
                                        var hMoveIn = t.outerHeight();
                                        t.attr('data-outerHeight', hMoveIn);
                                    } else {
                                        var hMoveIn = t.attr('data-outerHeight');
                                    }
                                    //t.css('width',wMoveIn);
                                    var pos = t.position();
                                    var left = pos.left;
                                    var top = pos.top;
                                    var tClass = t.attr('class');
                                    var ind = t.index();
                                    var hRel = t.parents('.camera-relative').outerHeight();
                                    var wRel = t.parents('.camera-relative').outerWidth();
                                    if (tClass.indexOf("fadeIn") != -1) {
                                        t.animate({ opacity: 0 }, 0).css('visibility', 'visible').delay((time / lMoveIn) * (0.1 * (ind - 1))).animate({ opacity: 1 }, (time / lMoveIn) * 0.15, easeMove);
                                    } else {
                                        t.css('visibility', 'visible');
                                    }
                                });
                            }

                            $('.camera-content.camera-current', fakeHover).show();
                            if (lMoveInContent != 0) {

                                $('.camera-content.camera-current .moveFromLeft, .camera-content.camera-current .moveFromRight, .camera-content.camera-current .moveFromTop, .camera-content.camera-current .moveFromBottom, .camera-content.camera-current .fadeIn, .camera-content.camera-current .fadeFromLeft, .camera-content.camera-current .fadeFromRight, .camera-content.camera-current .fadeFromTop, .camera-content.camera-current .fadeFromBottom', fakeHover).each(function() {
                                    if ($(this).attr('data-easing') != '') {
                                        var easeMove = $(this).attr('data-easing');
                                    } else {
                                        var easeMove = easing;
                                    }
                                    var t = $(this);
                                    var pos = t.position();
                                    var left = pos.left;
                                    var top = pos.top;
                                    var tClass = t.attr('class');
                                    var ind = t.index();
                                    var thisH = t.outerHeight();
                                    if (tClass.indexOf("moveFromLeft") != -1) {
                                        t.css({ 'left': '-' + (w) + 'px', 'right': 'auto' });
                                        t.css('visibility', 'visible').delay((time / lMoveInContent) * (0.1 * (ind - 1))).animate({ 'left': pos.left }, (time / lMoveInContent) * 0.15, easeMove);
                                    } else if (tClass.indexOf("moveFromRight") != -1) {
                                        t.css({ 'left': w + 'px', 'right': 'auto' });
                                        t.css('visibility', 'visible').delay((time / lMoveInContent) * (0.1 * (ind - 1))).animate({ 'left': pos.left }, (time / lMoveInContent) * 0.15, easeMove);
                                    } else if (tClass.indexOf("moveFromTop") != -1) {
                                        t.css({ 'top': '-' + h + 'px', 'bottom': 'auto' });
                                        t.css('visibility', 'visible').delay((time / lMoveInContent) * (0.1 * (ind - 1))).animate({ 'top': pos.top }, (time / lMoveInContent) * 0.15, easeMove, function() {
                                            t.css({ top: 'auto', bottom: 0 });
                                        });
                                    } else if (tClass.indexOf("moveFromBottom") != -1) {
                                        t.css({ 'top': h + 'px', 'bottom': 'auto' });
                                        t.css('visibility', 'visible').delay((time / lMoveInContent) * (0.1 * (ind - 1))).animate({ 'top': pos.top }, (time / lMoveInContent) * 0.15, easeMove);
                                    } else if (tClass.indexOf("fadeFromLeft") != -1) {
                                        t.animate({ opacity: 0 }, 0).css({ 'left': '-' + (w) + 'px', 'right': 'auto' });
                                        t.css('visibility', 'visible').delay((time / lMoveInContent) * (0.1 * (ind - 1))).animate({ 'left': pos.left, opacity: 1 }, (time / lMoveInContent) * 0.15, easeMove);
                                    } else if (tClass.indexOf("fadeFromRight") != -1) {
                                        t.animate({ opacity: 0 }, 0).css({ 'left': (w) + 'px', 'right': 'auto' });
                                        t.css('visibility', 'visible').delay((time / lMoveInContent) * (0.1 * (ind - 1))).animate({ 'left': pos.left, opacity: 1 }, (time / lMoveInContent) * 0.15, easeMove);
                                    } else if (tClass.indexOf("fadeFromTop") != -1) {
                                        t.animate({ opacity: 0 }, 0).css({ 'top': '-' + (h) + 'px', 'bottom': 'auto' });
                                        t.css('visibility', 'visible').delay((time / lMoveInContent) * (0.1 * (ind - 1))).animate({ 'top': pos.top, opacity: 1 }, (time / lMoveInContent) * 0.15, easeMove, function() {
                                            t.css({ top: 'auto', bottom: 0 });
                                        });
                                    } else if (tClass.indexOf("fadeFromBottom") != -1) {
                                        t.animate({ opacity: 0 }, 0).css({ 'bottom': '-' + thisH + 'px' });
                                        t.css('visibility', 'visible').delay((time / lMoveInContent) * (0.1 * (ind - 1))).animate({ 'bottom': '0', opacity: 1 }, (time / lMoveInContent) * 0.15, easeMove);
                                    } else if (tClass.indexOf("fadeIn") != -1) {
                                        t.animate({ opacity: 0 }, 0).css('visibility', 'visible').delay((time / lMoveInContent) * (0.1 * (ind - 1))).animate({ opacity: 1 }, (time / lMoveInContent) * 0.15, easeMove);
                                    } else {
                                        t.css('visibility', 'visible');
                                    }
                                });
                            }


                            $('.camera-appended', target).remove();
                            elem.removeClass('camera-sliding');
                            selector.eq(vis).hide();
                            var barWidth = $('.camera-bar-cont', camera_thumbs_wrap).width(),
                                barHeight = $('.camera-bar-cont', camera_thumbs_wrap).height(),
                                radSum;
                            if (loader != 'pie') {
                                radSum = 0.05;
                            } else {
                                radSum = 0.005;
                            }
                            $('#' + pieID).animate({ opacity: opts.loaderOpacity }, 200);
                            u = setInterval(
                                function() {
                                    if (elem.hasClass('stopped')) {
                                        clearInterval(u);
                                    }
                                    if (loader != 'pie') {
                                        if (rad <= 1.002 && !elem.hasClass('stopped') && !elem.hasClass('paused') && !elem.hasClass('hovered')) {
                                            rad = (rad + radSum);
                                        } else if (rad <= 1 && (elem.hasClass('stopped') || elem.hasClass('paused') || elem.hasClass('stopped') || elem.hasClass('hovered'))) {
                                            rad = rad;
                                        } else {
                                            if (!elem.hasClass('stopped') && !elem.hasClass('paused') && !elem.hasClass('hovered')) {
                                                clearInterval(u);
                                                imgFake();
                                                $('#' + pieID).animate({ opacity: 0 }, 200, function() {
                                                    clearTimeout(setT);
                                                    setT = setTimeout(canvasLoader, easedTime);
                                                    nextSlide();
                                                    opts.onStartLoading.call(this);
                                                });
                                            }
                                        }
                                        switch (barDirection) {
                                        case 'leftToRight':
                                            $('#' + pieID).animate({ 'right': barWidth - (barWidth * rad) }, (time * radSum), 'linear');
                                            break;
                                        case 'rightToLeft':
                                            $('#' + pieID).animate({ 'left': barWidth - (barWidth * rad) }, (time * radSum), 'linear');
                                            break;
                                        case 'topToBottom':
                                            $('#' + pieID).animate({ 'bottom': barHeight - (barHeight * rad) }, (time * radSum), 'linear');
                                            break;
                                        case 'bottomToTop':
                                            $('#' + pieID).animate({ 'bottom': barHeight - (barHeight * rad) }, (time * radSum), 'linear');
                                            break;
                                        }

                                    } else {
                                        radNew = rad;
                                        ctx.clearRect(0, 0, opts.pieDiameter, opts.pieDiameter);
                                        ctx.globalCompositeOperation = 'destination-over';
                                        ctx.beginPath();
                                        ctx.arc((opts.pieDiameter) / 2, (opts.pieDiameter) / 2, (opts.pieDiameter) / 2 - opts.loaderStroke, 0, Math.PI * 2, false);
                                        ctx.lineWidth = opts.loaderStroke;
                                        ctx.strokeStyle = opts.loaderBgColor;
                                        ctx.stroke();
                                        ctx.closePath();
                                        ctx.globalCompositeOperation = 'source-over';
                                        ctx.beginPath();
                                        ctx.arc((opts.pieDiameter) / 2, (opts.pieDiameter) / 2, (opts.pieDiameter) / 2 - opts.loaderStroke, 0, Math.PI * 2 * radNew, false);
                                        ctx.lineWidth = opts.loaderStroke - (opts.loaderPadding * 2);
                                        ctx.strokeStyle = opts.loaderColor;
                                        ctx.stroke();
                                        ctx.closePath();

                                        if (rad <= 1.002 && !elem.hasClass('stopped') && !elem.hasClass('paused') && !elem.hasClass('hovered')) {
                                            rad = (rad + radSum);
                                        } else if (rad <= 1 && (elem.hasClass('stopped') || elem.hasClass('paused') || elem.hasClass('hovered'))) {
                                            rad = rad;
                                        } else {
                                            if (!elem.hasClass('stopped') && !elem.hasClass('paused') && !elem.hasClass('hovered')) {
                                                clearInterval(u);
                                                imgFake();
                                                $('#' + pieID + ', .camera-canvas-wrap', camera_thumbs_wrap).animate({ opacity: 0 }, 200, function() {
                                                    clearTimeout(setT);
                                                    setT = setTimeout(canvasLoader, easedTime);
                                                    nextSlide();
                                                    opts.onStartLoading.call(this);
                                                });
                                            }
                                        }
                                    }
                                }, time * radSum
                            );
                        }

                    }


                    if (fx == 'scrollLeft' || fx == 'scrollRight' || fx == 'scrollTop' || fx == 'scrollBottom' || fx == 'scrollHorz') {
                        opts.onStartTransition.call(this);
                        easedTime = 0;
                        tApp.delay((((transPeriod + difference) / blocks) * delay[index] * couples) * 0.5).css({
                            'display': 'block',
                            'height': height,
                            'margin-left': marginLeft,
                            'margin-top': marginTop,
                            'width': width
                        }).animate({
                            'height': Math.floor((h / rows) + addTop + 1),
                            'margin-top': 0,
                            'margin-left': 0,
                            'width': Math.floor((w / cols) + addLeft + 1)
                        }, (transPeriod - difference), easing, cameraeased);
                        selector.eq(vis).delay((((transPeriod + difference) / blocks) * delay[index] * couples) * 0.5).animate({
                                'margin-left': marginLeft * (-1),
                                'margin-top': marginTop * (-1)
                            }, (transPeriod - difference), easing, function() {
                                $(this).css({ 'margin-top': 0, 'margin-left': 0 });
                            });
                    } else {
                        opts.onStartTransition.call(this);
                        easedTime = parseFloat(transPeriod) + parseFloat(difference);
                        if (slideOn == 'next') {
                            tApp.delay((((transPeriod + difference) / blocks) * delay[index] * couples) * 0.5).css({
                                'display': 'block',
                                'height': height,
                                'margin-left': marginLeft,
                                'margin-top': marginTop,
                                'width': width,
                                'opacity': opacityOnGrid
                            }).animate({
                                'height': Math.floor((h / rows) + addTop + 1),
                                'margin-top': 0,
                                'margin-left': 0,
                                'opacity': 1,
                                'width': Math.floor((w / cols) + addLeft + 1)
                            }, (transPeriod - difference), easing, cameraeased);
                        } else {
                            selector.eq(slideI).show().css('z-index', '999').addClass('camera-current');
                            selector.eq(vis).css('z-index', '1').removeClass('camera-current');
                            $('.camera-content', fakeHover).eq(slideI).addClass('camera-current');
                            $('.camera-content', fakeHover).eq(vis).removeClass('camera-current');
                            tApp.delay((((transPeriod + difference) / blocks) * delay[index] * couples) * 0.5).css({
                                'display': 'block',
                                'height': Math.floor((h / rows) + addTop + 1),
                                'margin-top': 0,
                                'margin-left': 0,
                                'opacity': 1,
                                'width': Math.floor((w / cols) + addLeft + 1)
                            }).animate({
                                'height': height,
                                'margin-left': marginLeft,
                                'margin-top': marginTop,
                                'width': width,
                                'opacity': opacityOnGrid
                            }, (transPeriod - difference), easing, cameraeased);
                        }
                    }


                });


            }
        }


        if ($(prevNav).length) {
            $(prevNav).click(function() {
                if (!elem.hasClass('camera-sliding')) {
                    var idNum = parseFloat($('.camera-slide.camera-current', target).index());
                    clearInterval(u);
                    imgFake();
                    $('#' + pieID + ', .camera-canvas-wrap', wrap).animate({ opacity: 0 }, 0);
                    canvasLoader();
                    if (idNum != 0) {
                        nextSlide(idNum);
                    } else {
                        nextSlide(amountSlide);
                    }
                    opts.onStartLoading.call(this);
                }
            });
        }

        if ($(nextNav).length) {
            $(nextNav).click(function() {
                if (!elem.hasClass('camera-sliding')) {
                    var idNum = parseFloat($('.camera-slide.camera-current', target).index());
                    clearInterval(u);
                    imgFake();
                    $('#' + pieID + ', .camera-canvas-wrap', camera_thumbs_wrap).animate({ opacity: 0 }, 0);
                    canvasLoader();
                    if (idNum == amountSlide - 1) {
                        nextSlide(1);
                    } else {
                        nextSlide(idNum + 2);
                    }
                    opts.onStartLoading.call(this);
                }
            });
        }


        if (isMobile()) {
            fakeHover.bind('swipeleft', function(event) {
                if (!elem.hasClass('camera-sliding')) {
                    var idNum = parseFloat($('.camera-slide.camera-current', target).index());
                    clearInterval(u);
                    imgFake();
                    $('#' + pieID + ', .camera-canvas-wrap', camera_thumbs_wrap).animate({ opacity: 0 }, 0);
                    canvasLoader();
                    if (idNum == amountSlide - 1) {
                        nextSlide(1);
                    } else {
                        nextSlide(idNum + 2);
                    }
                    opts.onStartLoading.call(this);
                }
            });
            fakeHover.bind('swiperight', function(event) {
                if (!elem.hasClass('camera-sliding')) {
                    var idNum = parseFloat($('.camera-slide.camera-current', target).index());
                    clearInterval(u);
                    imgFake();
                    $('#' + pieID + ', .camera-canvas-wrap', camera_thumbs_wrap).animate({ opacity: 0 }, 0);
                    canvasLoader();
                    if (idNum != 0) {
                        nextSlide(idNum);
                    } else {
                        nextSlide(amountSlide);
                    }
                    opts.onStartLoading.call(this);
                }
            });
        }

        if ($(pagination).length) {
            $('.camera-page li', wrap).click(function() {
                if (!elem.hasClass('camera-sliding')) {
                    var idNum = parseFloat($(this).index());
                    var curNum = parseFloat($('.camera-slide.camera-current', target).index());
                    if (idNum != curNum) {
                        clearInterval(u);
                        imgFake();
                        $('#' + pieID + ', .camera-canvas-wrap', camera_thumbs_wrap).animate({ opacity: 0 }, 0);
                        canvasLoader();
                        nextSlide(idNum + 1);
                        opts.onStartLoading.call(this);
                    }
                }
            });
        }

        if ($(thumbs).length) {

            $('.pix-thumb img', thumbs).click(function() {
                if (!elem.hasClass('camera-sliding')) {
                    var idNum = parseFloat($(this).parents('li').index());
                    var curNum = parseFloat($('.camera-current', target).index());
                    if (idNum != curNum) {
                        clearInterval(u);
                        imgFake();
                        $('#' + pieID + ', .camera-canvas-wrap', camera_thumbs_wrap).animate({ opacity: 0 }, 0);
                        $('.pix-thumb', thumbs).removeClass('camera-current');
                        $(this).parents('li').addClass('camera-current');
                        canvasLoader();
                        nextSlide(idNum + 1);
                        thumbnailPos();
                        opts.onStartLoading.call(this);
                    }
                }
            });

            $('.camera-thumbs-cont .camera-prev-thumbs', camera_thumbs_wrap).hover(function() {
                $(this).stop(true, false).animate({ opacity: 1 }, 250);
            }, function() {
                $(this).stop(true, false).animate({ opacity: .7 }, 250);
            });
            $('.camera-prev-thumbs', camera_thumbs_wrap).click(function() {
                var sum = 0,
                    wTh = $(thumbs).outerWidth(),
                    offUl = $('ul', thumbs).offset().left,
                    offDiv = $('> div', thumbs).offset().left,
                    ulLeft = offDiv - offUl;
                $('.camera-vis-thumb', thumbs).each(function() {
                    var tW = $(this).outerWidth();
                    sum = sum + tW;
                });
                if (ulLeft - sum > 0) {
                    $('ul', thumbs).animate({ 'margin-left': '-' + (ulLeft - sum) + 'px' }, 500, thumbnailVisible);
                } else {
                    $('ul', thumbs).animate({ 'margin-left': 0 }, 500, thumbnailVisible);
                }
            });

            $('.camera-thumbs-cont .camera-next-thumbs', camera_thumbs_wrap).hover(function() {
                $(this).stop(true, false).animate({ opacity: 1 }, 250);
            }, function() {
                $(this).stop(true, false).animate({ opacity: .7 }, 250);
            });
            $('.camera-next-thumbs', camera_thumbs_wrap).click(function() {
                var sum = 0,
                    wTh = $(thumbs).outerWidth(),
                    ulW = $('ul', thumbs).outerWidth(),
                    offUl = $('ul', thumbs).offset().left,
                    offDiv = $('> div', thumbs).offset().left,
                    ulLeft = offDiv - offUl;
                $('.camera-vis-thumb', thumbs).each(function() {
                    var tW = $(this).outerWidth();
                    sum = sum + tW;
                });
                if (ulLeft + sum + sum < ulW) {
                    $('ul', thumbs).animate({ 'margin-left': '-' + (ulLeft + sum) + 'px' }, 500, thumbnailVisible);
                } else {
                    $('ul', thumbs).animate({ 'margin-left': '-' + (ulW - wTh) + 'px' }, 500, thumbnailVisible);
                }
            });

        }


    };

})(jQuery);

(function($) {
    $.fn.cameraStop = function() {
        var wrap = $(this),
            elem = $('.camera-src', wrap),
            pieID = 'pie-' + wrap.index();
        elem.addClass('stopped');
        if ($('.camera-show-commands').length) {
            var camera_thumbs_wrap = $('.camera-thumbs-wrap', wrap);
        } else {
            var camera_thumbs_wrap = wrap;
        }
    };
})(jQuery);

;
(function($) {
    $.fn.cameraPause = function() {
        var wrap = $(this);
        var elem = $('.camera-src', wrap);
        elem.addClass('paused');
    };
})(jQuery);

;
(function($) {
    $.fn.cameraResume = function() {
        var wrap = $(this);
        var elem = $('.camera-src', wrap);
        if (typeof autoAdv === 'undefined' || autoAdv !== true) {
            elem.removeClass('paused');
        }
    };
})(jQuery);