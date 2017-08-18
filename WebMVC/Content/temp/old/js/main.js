var imgSrc;
$(window).scroll(function() {
  if ($(document).scrollTop() > 150) {
    $('.navbar').addClass('shrink');
  } else {
    $('.navbar').removeClass('shrink');
  }
});


$(function () {  
      /*Return to top*/
      var offset = 220;
      var duration = 500;
      var button = $('<a href="#" class="back-to-top"><i class="fa fa-angle-up"></i></a>');
      button.appendTo("body");
      
      jQuery(window).scroll(function() {
        if (jQuery(this).scrollTop() > offset) {
            jQuery('.back-to-top').fadeIn(duration);
        } else {
            jQuery('.back-to-top').fadeOut(duration);
        }
      });
    
      jQuery('.back-to-top').click(function(event) {
          event.preventDefault();
          jQuery('html, body').animate({scrollTop: 0}, duration);
          return false;
      });
	  
});

( function( $ ) {
	
	// Setup variables
	$window = $(window);
	$slide = $('.homeSlide');
	$body = $('body');
	
    //FadeIn all sections   
	$body.imagesLoaded( function() {
		setTimeout(function() {
		      
		      // Resize sections
		      adjustWindow();
		      
		      // Fade in sections
			  $body.removeClass('loading').addClass('loaded');
			  
		}, 800);
	});
	
	function adjustWindow(){

	    // Get window size
	    winH = $window.height();
	    winW = $window.width();

	    // Keep minimum height 550
	    if(winH <= 550) {
	        winH = 550;
	    }

	    // Init Skrollr for 768 and up
	    if( winW >= 768) {

	        // Init Skrollr
	        var s = skrollr.init({
	            forceHeight: false
	        });

	        // Resize our slides
	        $slide.height(winH);

	        s.refresh($('.homeSlide'));

	    } else {

	        // Init Skrollr
	        var s = skrollr.init();
	        s.destroy();
	    }
	
		// Check for touch
	   	if(Modernizr.touch) {

			// Init Skrollr
			var s = skrollr.init();
			s.destroy();
	   	}

	}
	
	function initAdjustWindow() {
	    return {
	        match : function() {
	            adjustWindow();
	        },
	        unmatch : function() {
	            adjustWindow();
	        }
	    };
	}

	/*enquire.register("screen and (min-width : 768px)", initAdjustWindow(), false)
	        .listen(100);*/
		
} )( jQuery );
jQuery(document).ready(function(){
 
	var in_height = window.innerHeight-197;
	$( '.main-div' ).height(in_height);
  $('.main-div-full-height').height(window.innerHeight)
	var next=in_height;
	$( '#slideshow' ).height(in_height);
  $( '.transparent-move-in' ).height(in_height);
  $( '.transparent-move-left-to-right' ).height(in_height);
	$( ".move-btn").click(function(event) {
		event.preventDefault();
	   $('html, body').animate({scrollTop: $('#slide-2').offset().top+in_height}, 1000);
	   if($( document ).height()-next<in_height)
	   {
		   $('html, body').animate({scrollTop: $('#slide-2').offset().top}, 1000);
		   in_height = 0;
	   }
	   in_height=next+in_height
	});

  $( '.loading-section' ).delay( 5000 ).addClass( "end-loading" );
  $( '.loading-section' ).addClass( "display-none" );

  
   
  $('.light-blue-filter').height(window.innerHeight+20);
  $('.dark-blue-filter').height(window.innerHeight+20);
});
$( window ).resize(function() {
  var in_height = window.innerHeight-40;
   $( '.image-wrapper' ).height(in_height/2);  
   var in_height = window.innerHeight-197;
  $( '.main-div' ).height(in_height); 
  $('.main-div-full-height').height(window.innerHeight)
  $( '.transparent-move-in' ).height(in_height+20);
  $( '.transparent-move-left-to-right' ).height(in_height+20);
  $('.light-blue-filter').height(window.innerHeight+20);
  $('.dark-blue-filter').height(window.innerHeight+20);
 
});
function showCurrentTab(w){
  $('.tabPane').hide();
  $('.arrow-down').remove();
  $('.tab-btn').removeClass('active-tab')
  $(w).addClass('active-tab')
  $(w).closest('.form-section-input').append('<div class="arrow-down"></div>')
  if($(w).attr("tabId")=="1"){

    $('#AirDiv').show();
  }
  else{
    $('#ChaufDiv').show();
  }
}
function showResMenu(){
  $('.top-menu-container-v3').addClass('top-menu-container-v3-move-in');
}
$(document).mouseup(function (e) {
    var container = $(".top-menu-container-v3");
    
    if (!container.is(e.target) && container.has(e.target).length === 0) // ... nor a descendant of the container
    {
        $(".top-menu-container-v3").removeClass("top-menu-container-v3-move-in");
    }
});