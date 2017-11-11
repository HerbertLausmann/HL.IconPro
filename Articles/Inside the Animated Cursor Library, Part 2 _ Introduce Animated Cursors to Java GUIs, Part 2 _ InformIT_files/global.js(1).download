// the global.js include line MUST fall AFTER the jQSuery include for this logic to work
// the following logic ensures that flash objects appear below dropdown menus
$(document).ready(function () {

    mobileNav();

    // Sidebar Mobile Navigation
    sidebarToggle();

    //Article TOC toggle    
    $('.TOC').click(function () {
        $('.TOCcontent').toggleClass('active');
        return false;
    });

    $('.tocClose').click(function () {
        if ($(".TOCcontent").hasClass("active")) {
            $('.TOCcontent').removeClass('active');
            return false;
        };
    });
    //END article TOC toggle

    $('object').each(function () {
        if ($(this).attr('id')) {
            //jquery clone won't work due to IE bug returning innerHTML so use straight DOM
            var flash = document.getElementById($(this).attr('id'));
            var param = document.createElement('param');
            param.name = 'wmode';
            param.value = 'transparent';
            flash.appendChild(param);
            // Commented the following two lines out so the pushdown ads would work
            //var clone = flash.cloneNode(true);
            //flash.parentNode.replaceChild(clone, flash);
        }
    });
    //jQueryUI dialog box
    $('.dialogBox').dialog({
        autoOpen: false,
        resizable: false,
        width: 300,
        height: 'auto',
        maxHeight: 350
    });
    //jQueryUI dialog box opener
    $('.dialogOpener').click(function (event) {
        var dlg = $(this).attr('href');
        $(dlg).dialog('option', 'position', [(event.clientX - 250), (event.clientY + 15)]);
        $(dlg).css({ position: 'relative', 'margin-left': '0px', clip: 'auto' });
        $(dlg).dialog('open');
        // We want a different width for smaller displays
        var cw = ((document.documentElement.clientWidth) - 10);
        if (document.documentElement.clientWidth > 600) {
            $(dlg).dialog('option', 'width', 300);
        } else {
            $(dlg).dialog('option', 'width', cw);
        }
        // Remove annoying focus from first link in dialog
        $(dlg).find('a').first().blur();
        $(dlg).scrollTop(0);
        return false;
    });

    // Reload wishlist iframe in jQueryUI dialog
    $('.jsLoadFrame.dialogOpener').click(function () {
        $('#wishListSrc').attr('src', removeParameter($('#wishListSrc').attr('src'), 'initialLoad'));
   });

    function removeParameter(url, parameter) {
        var urlparts = url.split('?');

        if (urlparts.length >= 2) {
            var urlBase = urlparts.shift(); //get first part, and remove from array
            var queryString = urlparts.join("?"); //join it back up

            var prefix = encodeURIComponent(parameter) + '=';
            var pars = queryString.split(/[&;]/g);
            for (var i = pars.length; i-- > 0;) {
                //reverse iteration as may be destructive
                if (pars[i].lastIndexOf(prefix, 0) !== -1)   //idiom for string.startsWith
                {
                    pars.splice(i, 1);
                }
            }

            url = urlBase + '?' + pars.join('&');
        }

        return url;
    }

    //jQueryUI dialog fancyZoom replacement
    // Create dialog box in jQueryUI dialog modal 
    $('.dialogZoom').dialog({
        dialogClass: 'zoomy',
        autoOpen: false,
        resizable: false,
        draggable: false,
        width: 'auto',
        height: 'auto',
        show: 'fade',
        hide: 'fade',
        close: function (event, ui) {
            // Flush/reload content. Needed for IE issues with Flash
            var c = "#" + $(this).find('.flash-replaced').attr('id');
            var r = $(c).html();
            $(c).html('');
            $(c).html(r);
        }
    });
    //Open linked content in jQueryUI dialog modal					
    $('.zoomOpener').click(function (event) {
        var dlg = $(this).attr('href');
        $(dlg).css({ position: 'relative', 'margin-left': '0px', clip: 'auto' });
        $(dlg).dialog('open');
        // Set size after load, to accommodate IE6
        $(dlg).dialog('option', 'width', ($(dlg).children().width() + 20));
        // Remove annoying focus from first link in dialog
        $(dlg).find('a').first().blur();
        return false;
    });

    //Open cover image on product page in jQueryUI dialog modal           
    $('.coverZoomOpener').click(function (event) {
        if (document.documentElement.clientWidth > 750) {
            var dlg = $(this).attr('href');
            $(dlg).css({ position: 'relative', 'margin-left': '0px', clip: 'auto' });
            $(dlg).dialog('open');
            // Set size after load, to accommodate IE6
            $(dlg).dialog('option', 'width', ($(dlg).children().width() + 20));
            // Remove annoying focus from first link in dialog
            $(dlg).find('a').first().blur();
            return false;
        }
    });

    //Open video zoom on video product pages                   
    $('.videoZoomOpener').click(function (event) {
        var dlg = $(this).attr('href');
        //$(dlg).dialog('option','position',[event.clientX,event.clientY]);
        $(dlg).css({ position: 'relative', 'margin-left': '0px', clip: 'auto' });
        $(dlg).dialog('option', 'dialogClass', 'videoZoom');
        $(dlg).dialog('open');
        // Set size after load, to accommodate IE6
        var cw = ((document.documentElement.clientWidth) * .95);
        if (document.documentElement.clientWidth > 900) {
            $(dlg).dialog('option', 'width', 800);
        } else {
            $(dlg).dialog('option', 'width', cw);
        }
        // Remove annoying focus from first link in dialog
        $(dlg).find('a').first().blur();
        return false;
    });

    if ($('.layoutSearchColumn #columnTwo .results .advertisement').children().size() > 0) {
        $('.layoutSearchColumn #columnTwo .results .advertisement').css('margin-bottom', '1em');
    };

    // jquery.equalheights.js
    // Set all columns to the same height.
    if (!($("#content").hasClass("storeProduct") || $("#content").hasClass("authorsBiography"))) {
        $("#content").equalHeights();
    }
    // Set all items with class 'sib' to equal height
    $(".sib").equalHeights();
});

//SlideOut Menu - Mobile Nav
function mobileNav() {
    $('.menu-link').on('click touchstart', function () {
        $('.menu-link').toggleClass('active');
        $('.wrapper').toggleClass('active');
        return false;
    });

    $('.js #siteNav').siblings().on('click touchstart', function () {
        if ($(".wrapper").hasClass("active")) {
            $('.menu-link').toggleClass('active');
            $('.wrapper').toggleClass('active');
            return false;
        };
    });

    $('.js #rContent').on('click touchstart', function () {
        if ($(".wrapper").hasClass("active")) {
            $('.menu-link').toggleClass('active');
            $('.wrapper').toggleClass('active');
            return false;
        };
    });
}
function sidebarToggle() {
    var cw = $(window).width();
    $(window).resize(function () {
        cw = $(window).width();
    });
    $('.js-expander-header').on('click', function () {
        if (cw < 751) {
            $(this).next('.sidebar-tint').children('.sidebar-tint-list').slideToggle().toggleClass('js-open js-default');
            $(this).toggleClass('js-open');
            $('.sidebar-tint-list').toggleClass('js-open');
            $('.btb-sub').toggleClass('js-open');
            $('.see-all-titles').toggleClass('js-open');
        }
    });
}
function loadJWVideo(jwvid, holder, outwrapper, inwrapper) {
    if (Modernizr.flash || Modernizr.video) {
        // Either the user has Flash installed, or the client can handle JWPlayer's native video player
        var vidDiv = document.getElementById(jwvid);
        var vidFile = vidDiv.getAttribute('data-vid');
        var vidPoster = vidDiv.getAttribute('data-poster');
        jwplayer(vidDiv).setup({
            flashplayer: '//mediaplayer.pearsoncmg.com/static/jw5/player.swf',
            file: vidFile,
            image: vidPoster,
            controlbar: 'bottom'
        });
    } else if (!Modernizr.flash && !Modernizr.video) {
        var vidDiv = document.getElementById(jwvid);
        // Remove classes used for video clip resizing
        $(vidDiv).closest(holder).removeClass().addClass('jw-flash-missing-poster');
        $(vidDiv).closest(outwrapper).removeClass();
        $(vidDiv).closest(inwrapper).removeClass();
    }
}

// hook for individual pages to execute logic in the body onLoad event
function windowOnLoad() {
    if (window.pageOnLoad != null)
        pageOnLoad();
}

// Toggle text switch function
$.fn.toggleText = function (a, b) {
    return this.each(function () {
        $(this).text($(this).text() == a ? b : a);
    });
};

// SonOfASuckerfish Dropdown - IE6 and below
// http://www.htmldog.com/articles/suckerfish/dropdowns
sfHover = function () {
    var sfEls = document.getElementById("nav").getElementsByTagName("li");
    for (var i = 0; i < sfEls.length; i++) {
        sfEls[i].onmouseover = function () {
            this.className += " sfhover";
        }
        sfEls[i].onmouseout = function () {
            this.className = this.className.replace(new RegExp(" sfhover\\b"), "");
        }
    }
}
if (window.attachEvent) window.attachEvent("onload", sfHover);

// Global Nav search box clear
function checkClear(searchInput, defaultPhrase) {
    if (searchInput.value == defaultPhrase) searchInput.value = "";
    searchInput.className += " focus";
}

/* Open a Popup Window Based on MM_openBrWindow v2.0 */
/* ONLY for legacy links */
function openBrWindow(theURL, winName, features) {
    window.open(theURL, winName, features);
}

/* Open Popup Window */
/* ONLY for use in article & sample chpater content */
function popUp(pPage) {
    window.open(pPage, 'popWin', 'resizable=yes,scrollbars=yes,width=800,height=600,toolbar=no');
}

// Tab Widget
// Used for BSS, Product Widget, author info widget
function tabWidget(layer, myID) {
    var navRoot = $('#' + myID);

    // Toggle container visibility
    navRoot.children('div.container').each(function (index) {
        $(this).toggleClass('on', index === layer - 1);
    });

    // Toggle tab selection
    navRoot.children('ul.tabs').children().each(function (index) {
        $(this).toggleClass('selected', index === layer - 1);
    });
}

function loadWidget(containerId, childOrderId, requestUrl) {
    //Get a reference to the content div for the tab to load.
    var contentDiv = $('#' + containerId + ' > div:eq(' + childOrderId + ')');

    //Check to see if the tab has already been loaded.
    if (contentDiv.attr('isloaded') != 'true') {
        //If not loaded, perform a GET call for tab content.
        $.get(requestUrl, function (data) {
            //Populate tab content area with returned data.
            contentDiv.html(data);
        });

        //Set isloaded property to true to signify that the tab
        //has already been loaded.
        contentDiv.attr('isloaded', 'true');
    }
}

// Used on Blog Comments
// Strip HTML Tags (form) script - By JavaScriptKit.com (http://www.javascriptkit.com)
// For this and over 400+ free scripts, visit JavaScript Kit - http://www.javascriptkit.com/
// This notice must stay intact for use
function stripHTML() {
    var re = /<\S[^><]*>/g
    for (i = 0; i < arguments.length; i++)
        arguments[i].value = arguments[i].value.replace(re, "")
}

// Style switcher
// Use to show/hide layers
function showme(id, newClass) {
    identity = document.getElementById(id);
    identity.className = newClass;
}

function execSearch(searchTerm) {
    document.forms['homesearchform'].query.value = searchTerm;
    //fix for IE6, also works for Firefox, IE7
    setTimeout("document.forms['homesearchform'].submit()", 10);
}

function jumpMenu(targ, selObj, restore) { //v3.0
    eval(targ + ".location='" + selObj.options[selObj.selectedIndex].value + "'");
    if (restore) selObj.selectedIndex = 0;
}

/* EqualHeights jQuery Plugin http://blog.isilverlabs.com/2010/03/jquery-equal-column-height-plugin/ */
(function ($) { $.fn.equalHeights = function (minHeight, maxHeight) { tallest = (minHeight) ? minHeight : 0; this.each(function () { $(this).css("min-height", "") }); this.each(function () { if ($(this).height() > tallest) { tallest = $(this).height() } }); if ((maxHeight) && tallest > maxHeight) tallest = maxHeight; return this.each(function () { $(this).css("min-height", tallest + "px") }) } })(jQuery);

/* Alternate equalHeight jQuery plugin - only resizes on a per-row basis. Much smarter, better for grid views */
equalheight = function (b) { var a = 0, c = 0, f = new Array(), e, d = 0; $(b).each(function () { e = $(this); $(e).height("auto"); topPostion = e.position().top; if (c != topPostion) { for (currentDiv = 0; currentDiv < f.length; currentDiv++) { f[currentDiv].height(a) } f.length = 0; c = topPostion; a = e.height(); f.push(e) } else { f.push(e); a = (a < e.height()) ? (e.height()) : (a) } for (currentDiv = 0; currentDiv < f.length; currentDiv++) { f[currentDiv].height(a) } }) };

/* appendAround markup pattern. [c]2012, @scottjehl, Filament Group, Inc. MIT/GPL https://github.com/filamentgroup/AppendAround */
/* Added check to see if window.getComputedStyle is supported */
(function (e) { if (window.getComputedStyle) { e.fn.appendAround = function () { return this.each(function () { function u(e) { return window.getComputedStyle(e, null).getPropertyValue("display") === "none" } function a() { if (u(i)) { var e = 0; o.each(function () { if (!u(this) && !e) { t.appendTo(this); e++; i = this } }) } } var t = e(this), n = "data-set", r = t.parent(), i = r[0], s = r.attr(n), o = e("[" + n + "='" + s + "']"); a(); e(window).bind("resize", a) }) } } })(jQuery);

/* Tipr - jQuery Tooltip plugin - http://www.tipue.com/tipr/ (modified to include .touchStart, with 3 second delay until removing tooltip) */
(function (a) { a.fn.tipr = function (b) { var c = a.extend({ speed: 200, mode: "bottom" }, b); return this.each(function () { var d = ".tipr_container_" + c.mode; a(this).hover(function () { var e = '<div class="tipr_container_' + c.mode + '"><div class="tipr_point_' + c.mode + '"><div class="tipr_content">' + a(this).attr("data-tip") + "</div></div></div>"; a(this).append(e); var f = a(d).outerWidth(); var h = a(this).width(); var g = (h / 2) - (f / 2); a(d).css("margin-left", g + "px"); a(this).removeAttr("title"); a(d).fadeIn(c.speed) }, function () { a(d).remove() }); a(this).on("touchstart", function () { var e = '<div class="tipr_container_' + c.mode + '"><div class="tipr_point_' + c.mode + '"><div class="tipr_content">' + a(this).attr("data-tip") + "</div></div></div>"; a(this).append(e); var f = a(d).outerWidth(); var h = a(this).width(); var g = (h / 2) - (f / 2); a(d).css("margin-left", g + "px"); a(this).removeAttr("title"); a(d).fadeIn(c.speed); setTimeout(function () { a(d).remove() }, 3000) }) }) } })(jQuery);

// Function for adding thousands separators
function numberWithCommas(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}