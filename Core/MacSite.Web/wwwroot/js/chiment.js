(function () {
    function resetSlick($el) {
        if (!$el || !$el.length) return;

        try {
            if ($el.hasClass('slick-initialized')) {
                $el.slick('unslick');
            }
        } catch (err) {
            console.warn('chiment: slick unslick skipped', err);
        }

        $el.removeClass('slick-initialized slick-slider');
        $el.find('.slide').css({
            display: '',
            opacity: '',
            position: '',
            width: '',
            left: '',
            top: '',
            zIndex: ''
        });
    }

    function destroyAllSlicks() {
        if (typeof jQuery === 'undefined' || typeof $.fn.slick === 'undefined') return;
        $('.slick-initialized').each(function () {
            resetSlick($(this));
        });
    }

    function initSliders() {
        resetSlick($('.testimonials-slider'));
        if ($('.testimonials-slider').length) {
            $('.testimonials-slider').slick({
                infinite: true,
                slidesToShow: 2,
                slidesToScroll: 1,
                centerMode: false,
                dots: true,
                arrows: false,
                autoplay: true,
                autoplaySpeed: 3000,
                speed: 600,
                focusOnSelect: true,
                responsive: [
                    { breakpoint: 1200, settings: { slidesToShow: 2 } },
                    { breakpoint: 992, settings: { slidesToShow: 1 } }
                ]
            });
        }

        resetSlick($('.portfolio-slider'));
        if ($('.portfolio-slider').length) {
            $('.portfolio-slider').slick({
                infinite: true,
                slidesToShow: 2,
                slidesToScroll: 1,
                centerMode: false,
                dots: false,
                arrows: false,
                focusOnSelect: true,
                responsive: [
                    { breakpoint: 1200, settings: { slidesToShow: 2 } },
                    { breakpoint: 992, settings: { slidesToShow: 1 } }
                ]
            });
        }

        resetSlick($('.home-banner .slider'));
        if ($('.home-banner .slider').length) {
            $('.home-banner .slider').slick({
                infinite: true,
                slidesToShow: 1,
                slidesToScroll: 1,
                dots: true,
                arrows: false,
                autoplay: true,
                autoplaySpeed: 3000,
                speed: 600,
                fade: true,
                cssEase: 'linear'
            });
        }
    }

    function initPressSlider() {
        if ($(window).width() < 992) {
            if ($('.press-slider').length && !$('.press-slider').hasClass('slick-initialized')) {
                $('.press-slider').slick({
                    infinite: true,
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    dots: false,
                    arrows: false
                });
            }
        } else {
            resetSlick($('.press-slider'));
        }
    }

    function bindMobileMenu() {
        $('.menu-icon').off('click.chiment').on('click.chiment', function () {
            $('.main-nav').addClass('open');
            $('.menu-overlay').addClass('active');
            $('body').addClass('menu-open');
        });

        $('.close-menu, .menu-overlay, .main-nav a').off('click.chiment').on('click.chiment', function () {
            $('.main-nav').removeClass('open');
            $('.menu-overlay').removeClass('active');
            $('body').removeClass('menu-open');
        });
    }

    function filterPortfolioCategory(category, categoryName) {
        var allItems = $('.portfolio-grid .item');
        var matchingItems = allItems.filter(function () {
            return $(this).attr('data-category') === category;
        });

        if (matchingItems.length > 0) {
            allItems.hide();
            matchingItems.show();
            $('.portfolio-grid').show();
            $('.no-data-view').hide();
            $('.portfolio .button-row').show();
        } else {
            allItems.hide();
            $('.portfolio-grid').hide();
            $('.no-data-view .no-data-title').text('NO ' + categoryName + ' YET');
            $('.no-data-view .no-data-text').text('No productions in this category yet. Check back soon — new work is added regularly.');
            $('.no-data-view').show();
            $('.portfolio .button-row').hide();
        }
    }

    function bindPortfolioTabs() {
        if ($('.tabs').length === 0) return;

        $(document).off('click.chiment', '.tabs a').on('click.chiment', '.tabs a', function (e) {
            e.preventDefault();
            e.stopPropagation();

            $('.tabs a').removeClass('active');
            $(this).addClass('active');

            var category = $(this).attr('data-category');
            var categoryName = $(this).text().trim();
            filterPortfolioCategory(category, categoryName);
        });
    }

    function bindSmoothScroll() {
        $(document).off('click.chiment', 'a[href*="#"]:not(.tabs a)').on('click.chiment', 'a[href*="#"]:not(.tabs a)', function (e) {
            var href = $(this).attr('href');
            if (!href || href === '#') return;

            var hashIndex = href.indexOf('#');
            if (hashIndex === -1) return;

            var path = hashIndex === 0 ? window.location.pathname : href.substring(0, hashIndex);
            var targetId = href.substring(hashIndex);
            if (targetId.length <= 1) return;

            if (path && path !== window.location.pathname) return;

            var $target = $(targetId);
            if ($target.length > 0) {
                e.preventDefault();

                $('.main-nav').removeClass('open');
                $('.menu-overlay').removeClass('active');
                $('body').removeClass('menu-open');

                $('html, body').animate({
                    scrollTop: $target.offset().top
                }, 800);

                window.location.hash = targetId;
            }
        });
    }

    window.chimentInit = function () {
        if (typeof jQuery === 'undefined') return;

        try {
            if (typeof $.fn.slick !== 'undefined') {
                initSliders();
                initPressSlider();
            }
        } catch (err) {
            console.warn('chimentInit: slider init skipped', err);
        }

        bindMobileMenu();
        bindPortfolioTabs();
        bindSmoothScroll();
    };

    function scheduleChimentInit() {
        requestAnimationFrame(function () {
            requestAnimationFrame(function () {
                window.chimentInit();
            });
        });
    }

    function hookBlazorNavigation() {
        if (window.__chimentEnhancedHooked) return true;
        if (typeof Blazor === 'undefined' || typeof Blazor.addEventListener !== 'function') return false;

        Blazor.addEventListener('enhancednavigationstart', function () {
            destroyAllSlicks();
        });

        Blazor.addEventListener('enhancedload', function () {
            scheduleChimentInit();
        });

        window.__chimentEnhancedHooked = true;
        return true;
    }

    function boot() {
        window.chimentInit();
        hookBlazorNavigation();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', boot);
    } else {
        boot();
    }

    if (!hookBlazorNavigation()) {
        var attempts = 0;
        var timer = setInterval(function () {
            if (hookBlazorNavigation() || ++attempts > 100) {
                clearInterval(timer);
            }
        }, 50);
    }

    $(window).off('resize.chiment').on('resize.chiment', function () {
        if (typeof $.fn.slick !== 'undefined') {
            initPressSlider();
        }
    });
})();
