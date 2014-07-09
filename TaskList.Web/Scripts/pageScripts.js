
(function($) {
	$(function (){

		$('.sidebar-toggle').on('click', function (){
			$('.loaded').toggleClass('sidebar-visible');
		});
		$('.user-nav').on('click',function(){
			$('.user-nav > li').toggle();
		});
	});
})(jQuery);