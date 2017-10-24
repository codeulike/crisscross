// CrissCross - alternative user interface for running SSRS reports
// Copyright (C) 2011-2017 Ian Finch
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

function CrissCrossTour() {

    this.crissCrossClient = null;
    this.originalReportDescHeight = 200;
    this.welcomeHintAvailable = false;

    this.setupWelcomeHint = function(cookieProp) {
        $(".inlineHint button").button().addClass('ui-state-highlight');
        var self = this;
        $("#welcomeClose").click(function() {
            self.hideWelcomeHint();
            var tourInfo = self.getTourCookieInfo();
            tourInfo[cookieProp] = false;
            self.setTourCookieInfo(tourInfo);
        });
        $("#introLinkWrapper").show();
        $("#introLink").click(function(e) {
            e.preventDefault();
            self.showWelcomeHint();
            var tourInfo = self.getTourCookieInfo();
            tourInfo[cookieProp] = true;
            self.setTourCookieInfo(tourInfo);
        });
        this.welcomeHintAvailable = true;
        var tourInfo = this.getTourCookieInfo();
        if (tourInfo[cookieProp])
            this.showWelcomeHint(true);
    };

    this.showWelcomeHint = function(skipAnimation) {
        if (skipAnimation)
            $("#welcomeHint").show();
        else
            $("#welcomeHint").slideDown();
    };

    this.hideWelcomeHint = function(skipAnimation) {
        if (skipAnimation)
            $("#welcomeHint").hide();
        else
            $("#welcomeHint").slideUp();
            
        $(".tourStop").qtip("hide");
    };

    this.setupTour = function() {
        var self = this;
        $(".tourButton").button().addClass('ui-state-highlight');
        $(".tourButton").click(function() {
            var thisStop = this.getAttribute("data-crcThisTourStop");
            var nextStop = this.getAttribute("data-crcNextTourStop");
            if (thisStop)
                $("#" + thisStop).qtip("hide");
            if (nextStop) {
                $("#" + nextStop).qtip("show");
                var targetelem = $("#" + nextStop).get(0);
                if (targetelem) {
                    if (!self.isScrolledIntoView(targetelem, 200))
                        $("html, body").animate({ "scrollTop": $("#" + nextStop).offset().top - 200 }, 2000);
                }
            };

        });

        $('.tourStop').each(function() {
            var contentId = this.getAttribute("data-crcTourContent");
            // default to right
            var tipcorner = 'left center';
            var pos_my = 'left center';
            var pos_at = 'right center';
            var tourPos = this.getAttribute("data-crcTourPosition");
            if (tourPos == 'bottom') {
                tipcorner = 'top center';
                pos_my = 'top center';
                pos_at = 'bottom center';
            }
            if (tourPos == 'top') {
                tipcorner = 'bottom center';
                pos_my = 'bottom center';
                pos_at = 'top center';
            }
            $(this).qtip({
                content: $("#" + contentId),
                style: {
                    tip: {
                        corner: tipcorner,
                        height: 24,
                        width: 24
                    }
                },
                position: {
                    my: pos_my,
                    at: pos_at
                },
                show: {
                    event: false // never show unless explicitly called
                },
                hide: {
                    event: false
                }
            });
        });

    };

    this.setupReportDescription = function(cookieProp) {

        var self = this;
        $("#showReportDesc").click(function() {
            self.showReportDescription();
            var tourInfo = self.getTourCookieInfo();
            tourInfo[cookieProp] = true;
            self.setTourCookieInfo(tourInfo);
        });
        $("#hideReportDesc").click(function() {
            self.hideReportDescription();
            var tourInfo = self.getTourCookieInfo();
            tourInfo[cookieProp] = false;
            self.setTourCookieInfo(tourInfo);
        });

        var tourInfo = this.getTourCookieInfo();
        if (tourInfo[cookieProp])
            this.showReportDescription(true);
        else
            this.hideReportDescription(true);
    };

    this.showReportDescription = function(skipAnimation) {
        if (skipAnimation)
            $(".reportDescriptionText").height("auto");
        else
            $(".reportDescriptionText").animate({ height: this.originalReportDescHeight }, function() {
                $(".reportDescriptionText").height("auto");
            });
        $("#showReportDesc").hide();
        $("#hideReportDesc").show();
        $("#repDescEllipsis").hide();
    };

    this.hideReportDescription = function(skipAnimation) {
        this.originalReportDescHeight = $(".reportDescriptionText").css('height');
        if (skipAnimation)
            $(".reportDescriptionText").height("18");
        else
            $(".reportDescriptionText").animate({ height: "18px" });
        $("#showReportDesc").show();
        $("#hideReportDesc").hide();
        $("#repDescEllipsis").show();   
    };

    this.createReportPageTour = function() {
        // skip if welcome hint not available
        if (!this.welcomeHintAvailable)
            return;

        var dateBlock = null;
        var multiBlock = null;
        var multiBlockLight = null;
        $(".paramBlock").each(function(index, paramBlock) {
            var ctype = $(paramBlock).attr("crissCrossType");
            if (dateBlock == null && ctype == 'DatePick')
                dateBlock = paramBlock;
            if (multiBlock == null && ctype == 'MultiPick')
                multiBlock = paramBlock;
            if (multiBlockLight == null && ctype == 'MultiPickLight')
                multiBlockLight = paramBlock;
        });

        // add tour stops
        var startTour = $("#takeTour").get(0);
        var stops = new Array();
        //stops.push({ element: $("#reportTitle").get(0), contentId: 'tourContent1', tourPos: '' });
        stops.push({ element: $("#clientParamDiv").get(0), contentId: 'tourContent2', tourPos: 'top' });
        stops.push({ element: $("#extendParam").get(0), contentId: 'tourContentParamChooser', tourPos: 'bottom' });
        if (dateBlock)
            stops.push({ element: dateBlock, contentId: 'tourContent3', tourPos: '' });
        if (multiBlock)
            stops.push({ element: multiBlock, contentId: 'tourContent4', tourPos: '' });
        if (multiBlockLight)
            stops.push({ element: multiBlockLight, contentId: 'tourContent4b', tourPos: '' });
        stops.push({ element: $(".runButton").get(0), contentId: 'tourContent5', tourPos: '' });

        $(startTour).attr("data-crcNextTourStop", stops[0].element.id);
        $.each(stops, function(index, stop) {
            $(stop.element).addClass("tourStop");
            $(stop.element).attr("data-crcTourContent", stop.contentId);
            $(stop.element).attr("data-crcTourPosition", stop.tourPos);
            var nextStop = stops[index + 1];
            // set button attributes in content
            $("#" + stop.contentId + " .tourButton").attr("data-crcThisTourStop", stop.element.id);
            if (nextStop)
                $("#" + stop.contentId + " .tourButton").attr("data-crcNextTourStop", nextStop.element.id);

        });

        this.crissCrossClient.addLog("created tour");
        this.setupTour();
    };
    
    this.isScrolledIntoView = function(elem, overrideHeight) {
        var docViewTop = $(window).scrollTop();
        var docViewBottom = docViewTop + $(window).height();

        var elemTop = $(elem).offset().top - 100;
        var elemBottom = elemTop + $(elem).height();
        if (overrideHeight)
            elemBottom = elemTop + overrideHeight;

        return ((elemBottom >= docViewTop) && (elemTop <= docViewBottom)
      && (elemBottom <= docViewBottom) && (elemTop >= docViewTop));
    };

    this.getTourCookieInfo = function() {
        var tourCookieInfo = $.cookies.get('crissCrossTourInfo');
        if (!tourCookieInfo)
            tourCookieInfo = {
                showHomeHint: true,
                showReportHint: true,
                showReportDesc: true
            };
        return tourCookieInfo;
    };

    this.setTourCookieInfo = function(tourCookieInfo) {
        var expiredate = new Date();
        // set to expire in 30 days
        expiredate.setTime(expiredate.getTime() + (30 * 24 * 60 * 60 * 1000));
        
        $.cookies.set('crissCrossTourInfo', tourCookieInfo, { expiresAt: expiredate });
    };

}