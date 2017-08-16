/**
 *   Source File:		script.js
 *   Property of Hava All rights reserved.
 *
 *   Description:        main script file
 *
 *   Date		  Author/(Reviewer)		Description
 *   -------------------------------------------------------
 *   18 May 2017    Chamara Bandara	    Creation
 *
 */
$(function() {
//data picker
 $( ".datepicker" ).datepicker({
 	beforeShow: function(){    
           $(".ui-datepicker").css('width', $('#main_page_transfer_at_date').width()) 
    }
 });
//toggle search 
    $('.toggle-switch-buttons').on('click', '.btn-booking-type', function() {
        $(this).addClass('active').siblings().removeClass('active');
    });

    //time picker
   $('#main_page_transfer_at_time').timepicker();

   //toggle in mobile view
    $('.navi-mobile button').click(function(){
		 $('.navi-mobile-dropdown').toggleClass('mobile_menu_toggle');
		});
     
     //set current date / time now
	document.getElementById("main_page_transfer_at_date").value = getCurrentDate();
	document.getElementById("main_page_transfer_at_time").value = getCurrentTime();

	//toggle login button
	$('.btn-login').click(function(){
		$('.login-entry').toggleClass('open');
	});
  
  //close login popup when click outside the popup
	  $(document).on('click', function (event) {
		  if (!$(event.target).closest('.login-entry').length) {
		   $('.login-entry').removeClass('open');  }
	});
 
});

//get currnet Date in dd/mm/yyyy format
function getCurrentDate(){
	  var today = new Date();
	var dd = today.getDate();
	var mm = today.getMonth()+1; //January is 0!

	var yyyy = today.getFullYear();
	if(dd<10){
	    dd='0'+dd;
	} 
	if(mm<10){
	    mm='0'+mm;
	} 
	var today = dd+'/'+mm+'/'+yyyy;

	return today;
}

//get current time form 24 hrs format
function getCurrentTime(){

	var d = new Date(); // for now
d.getHours(); // => 9
d.getMinutes(); // =>  30
d.getSeconds(); // => 51

return d.getHours() + ':' + d.getMinutes()+ '('+formatAMPM(d) +')';
}


//get current time form 12 hrs format
function formatAMPM(date) {
  var hours = date.getHours();
  var minutes = date.getMinutes();
  var ampm = hours >= 12 ? 'PM' : 'AM';
  hours = hours % 12;
  hours = hours ? hours : 12; // the hour '0' should be '12'
  minutes = minutes < 10 ? '0'+minutes : minutes;
   var strTime = hours + ':' + minutes + ' ' + ampm;
  return strTime;
}

var Slider;

$(document).ready(function() {
  Slider = $('.slider').bxSlider({
    pager: true
  });
  $('.number').on('click', function(e) {
    e.preventDefault();
    var index = $(this).attr('data-slider');
    Slider.goToSlide(index);
  });
});