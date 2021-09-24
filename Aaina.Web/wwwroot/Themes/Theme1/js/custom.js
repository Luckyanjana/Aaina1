! function($) {
    "use strict";
    var a = {
        accordionOn: ["xs"]
    };
    $.fn.responsiveTabs = function(e) {
        var t = $.extend({}, a, e),
            s = "";
        return $.each(t.accordionOn, function(a, e) {
            s += " accordion-" + e
        }), this.each(function() {
            var a = $(this),
                e = a.find("> li > a"),
                t = $(e.first().attr("href")).parent(".tab-content"),
                i = t.children(".tab-pane");
            a.add(t).wrapAll('<div class="responsive-tabs-container" />');
            var n = a.parent(".responsive-tabs-container");
            n.addClass(s), e.each(function(a) {
                var t = $(this),
                    s = t.attr("href"),
                    i = "",
                    n = "",
                    r = "";
                t.parent("li").hasClass("active") && (i = " active"), 0 === a && (n = " first"), a === e.length - 1 && (r = " last"), t.clone(!1).addClass("accordion-link" + i + n + r).insertBefore(s)
            });
            var r = t.children(".accordion-link");
            e.on("click", function(a) {
                a.preventDefault();
                var e = $(this),
                    s = e.parent("li"),
                    n = s.siblings("li"),
                    c = e.attr("href"),
                    l = t.children('a[href="' + c + '"]');
                s.hasClass("active") || (s.addClass("active"), n.removeClass("active"), i.removeClass("active"), $(c).addClass("active"), r.removeClass("active"), l.addClass("active"))
            }), r.on("click", function(t) {
                t.preventDefault();
                var s = $(this),
                    n = s.attr("href"),
                    c = a.find('li > a[href="' + n + '"]').parent("li");
                s.hasClass("active") || (r.removeClass("active"), s.addClass("active"), i.removeClass("active"), $(n).addClass("active"), e.parent("li").removeClass("active"), c.addClass("active"))
            })
        })
    }
}(jQuery);


$('.responsive-tabs').responsiveTabs({
    								     accordionOn: ['xs', 'sm']
									});
 

$('.naleftbtn').click(function() {
    if($('.top-boxL').hasClass('current')) {       
        $('.top-boxL').removeClass('current');
    }
    else{
        $('.top-boxL').addClass('current');
    }
});

$('.naleftbtn').click(function() {
    if($(this).hasClass('current')) {       
        $(this).removeClass('current');
    }
    else{
        $(this).addClass('current');
    }
});

$('.naleftbtn').click(function() {
    if($('.overlap').hasClass('current')) {       
        $('.overlap').removeClass('current');
    }
    else{
        $('.overlap').addClass('current');
    }
});

$('.slidemob').click(function() {
    if($('.mobfoot').hasClass('current')) {       
        $('.mobfoot').removeClass('current');
    }
    else{
        $('.mobfoot').addClass('current');
    }
});


$( ".top-boxL nav ul li .caret" ).click(function() {
  $(this).toggleClass( "highlight" );
});


$( ".nav > li ul li i" ).click(function() {
  $(this).toggleClass( "highlight" );
});








// Code goes here
'use strict'
window.onload = function(){
    var tableCont = document.querySelector('#table-cont');
    
  /**
   * scroll handle
   * @param {event} e -- scroll event
   */
    if (tableCont != null) {
        function scrollHandle(e) {
            var scrollTop = this.scrollTop;
            this.querySelector('thead').style.transform = 'translateY(' + scrollTop + 'px)';
        }

        tableCont.addEventListener('scroll', scrollHandle);
    }
    
}