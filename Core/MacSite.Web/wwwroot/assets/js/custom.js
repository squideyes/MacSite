$(document).ready(function(){
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
      {
        breakpoint: 1200,
        settings: {
          slidesToShow: 2
        }
      },
      {
        breakpoint: 992,
        settings: {
          slidesToShow: 1
        }
      }
    ]
  });

  $('.portfolio-slider').slick({
    infinite: true,
    slidesToShow: 2,
    slidesToScroll: 1,
    centerMode: false,
    dots: false,
    arrows: false,
    focusOnSelect: true,
    // autoPlay: true,
    // autoPlaySpeed: 2000,
    responsive: [
      {
        breakpoint: 1200,
        settings: {
          slidesToShow: 2
        }
      },
      {
        breakpoint: 992,
        settings: {
          slidesToShow: 1
        }
      }
    ]
  });

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

  // Mobile Menu Toggle
  $('.menu-icon').on('click', function() {
    $('.main-nav').addClass('open');
    $('.menu-overlay').addClass('active');
    $('body').addClass('menu-open');
  });

  $('.close-menu, .menu-overlay, .main-nav a').on('click', function() {
    $('.main-nav').removeClass('open');
    $('.menu-overlay').removeClass('active');
    $('body').removeClass('menu-open');
  });

  // Conditional Press Slider
  function initPressSlider() {
    if ($(window).width() < 992) {
      if (!$('.press-slider').hasClass('slick-initialized')) {
        $('.press-slider').slick({
          infinite: true,
          slidesToShow: 1,
          slidesToScroll: 1,
          dots: false,
          arrows: false
        });
      }
    } else {
      if ($('.press-slider').hasClass('slick-initialized')) {
        $('.press-slider').slick('unslick');
      }
    }
  }

  initPressSlider();
  $(window).on('resize', function() {
    initPressSlider();
  });

  // Portfolio Tabs Switching and Filtering
  if ($('.tabs').length > 0) {
    $('.tabs a').on('click', function(e) {
      e.preventDefault();
      
      $('.tabs a').removeClass('active');
      $(this).addClass('active');
      
      var category = $(this).data('category');
      var allItems = $('.portfolio-grid .item');
      var matchingItems = allItems.filter(function() {
        return $(this).data('category') === category;
      });
      
      if (matchingItems.length > 0) {
        allItems.hide();
        matchingItems.show();
        $('.portfolio-grid').show();
        $('.no-data-view').hide();
        $('.button-row').show();
      } else {
        allItems.hide();
        $('.portfolio-grid').hide();
        
        var categoryName = $(this).text().trim();
        $('.no-data-view .no-data-title').text('NO ' + categoryName + ' YET');
        $('.no-data-view .no-data-text').text('No productions in this category yet. Check back soon — new work is added regularly.');
        $('.no-data-view').show();
        $('.button-row').hide();
      }
    });
  }

  // Smooth scroll for hash links
  $('a[href^="#"], a[href*="#"]').on('click', function(e) {
    var href = $(this).attr('href');
    
    // Check if the link contains a hash
    if (href.indexOf('#') !== -1) {
      var targetId = href.substring(href.indexOf('#'));
      var $target = $(targetId);
      
      if ($target.length > 0) {
        e.preventDefault();
        
        // Close mobile menu if open
        $('.main-nav').removeClass('open');
        $('.menu-overlay').removeClass('active');
        $('body').removeClass('menu-open');
        
        $('html, body').animate({
          scrollTop: $target.offset().top
        }, 800);
        
        // Update hash in URL
        window.location.hash = targetId;
      }
    }
  });
});
