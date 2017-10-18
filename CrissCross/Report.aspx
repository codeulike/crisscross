<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="CrissCross.Report" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
    var crissCross = new CrissCrossClient();
    crissCross.reportPath = "<%=ReportPath %>";
    <% if (ShowClientDebugLog) { %>
    crissCross.showLog = "#clientLog";
    <% } %>
    <% if (RunningReport) { %>
    crissCross.runningMode = true;
    <% } %>
    
    $(function() {
        $("#waitBlock").ajaxStart(function() {
            $(this).show();
        });
        $("#waitBlock").ajaxStop(function() {
            $(this).hide();
        });
        crissCross.initClient();
    });
    
</script>

    
    <script src="Scripts/jquery.multiselect.js" type="text/javascript"></script>
    <script src="Scripts/jquery.multiselect.filter.js" type="text/javascript"></script>
    <script src="Scripts/jquery.selectlist.js" type="text/javascript"></script>
    <script src="Scripts/jquery.tokeninput.js" type="text/javascript"></script>
    <script src="Scripts/jquery.oxbowstilt.js" type="text/javascript"></script>
    
    <link href="Content/selectlist.css" rel="stylesheet" type="text/css" />
    <link href="Content/jquery.multiselect.css" rel="stylesheet" type="text/css" />
    <link href="Content/jquery.multiselect.filter.css" rel="stylesheet" type="text/css" />
    <link href="Content/token-input.css" rel="stylesheet" type="text/css" />
    <link href="Content/token-input-crisscross.css" rel="stylesheet" type="text/css" />
    <link href="Content/jquery.oxbowstilt.css" rel="stylesheet" type="text/css" />

    

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <div class="cornerbackground">
            <div class="cornertop">
                <div class="cornerbottom">
                
                <div class="inner">
                
    <div class="inlineHint" id="welcomeHint" style="display:none;">
        <p>This is the report page, where you enter parameters and run a report.</p>
        <p>Press Take Tour for an explanation of the features.</p>
        <button id="welcomeClose" type="button">OK</button>
        <button id="takeTour" type="button" class="tourButton" data-crcNextTourStop="tba">Take Tour</button>
    </div>
    <asp:Panel ID="uxReportTitlePanel" runat="server" CssClass="reportHeader">
        <div class="reportTitle" id="reportTitle"><asp:Label ID="uxReportName" runat="server" ></asp:Label></div>
        <div class="reportFolder">From folder:<br />
            <asp:HyperLink ID="uxReportFolderLink" runat="server"></asp:HyperLink></div>
        <div class="clear"></div>
    </asp:Panel>
    <asp:Panel ID="uxReportDescriptionPanel" runat="server" CssClass="reportDescription">
        <div id="showReportDesc" class="ui-icon ui-icon-triangle-1-e" style="display:none;"></div>
        <div id="hideReportDesc" class="ui-icon ui-icon-triangle-1-s"></div>
        <div class="reportDescriptionText">
          <asp:Label ID="uxReportHint" runat="server" ></asp:Label>
        </div>
        <div id="repDescEllipsis">...</div>
        <div class="clear"></div>
    
    </asp:Panel>
    <asp:Panel ID="uxParametersPanel" runat="server">
        <div id="clientParamDiv" style="<%=GetParamDivStyle() %>">
        
            <div id="waitBlock" style="display:none;">
                <img src="Content/images/waitanim.gif" />
            </div>
            <div class="spaceBlock">&nbsp;</div>
        </div>
        <asp:Label ID="uxParamDebug" runat="server" visible="false"></asp:Label>
        <asp:Button ID="uxRunReportButton" runat="server" Text="Run Report" 
            CssClass="runButton" OnClientClick="return crissCross.gatherParameters(); " 
            onclick="uxRunReportButton_Click" />
    </asp:Panel>
    <asp:Panel ID="uxParamSummaryPanel" runat="server">
        <div id="paramSummaryWrapper" class="paramSummaryWrapper" style="<%=GetSummaryDivStyle() %>">
        <div class="paramLeftCol">
        Parameters used:<br />
        <button id="changeParams" style="display:none;" type="button" class="changeParams">Change Parameters</button>
        </div>
        <div class="paramRightCol">
        <asp:Label ID="uxParamUserDescription" runat="server" ></asp:Label>
        </div>
        <div class="clear">
        </div>
        
        
        
        </div>
        
    </asp:Panel>
    </div></div></div></div>
    </asp:Content>
    <asp:Content ID="Content3" ContentPlaceHolderID="FullWidthPlaceHolder" runat="server">
    
        <asp:Panel ID="uxResultsPanel" runat="server" CssClass="">
    <div class="viewerWrapper">
    <rsweb:ReportViewer ID="uxReportViewer" runat="server"  
        Width="100%">
    </rsweb:ReportViewer>
    <asp:ScriptManager runat="server"></asp:ScriptManager>
    </div>
    </asp:Panel>
    <div style="height:300px;"></div>
    
    <div class="pagemiddle">
    <asp:TextBox ID="uxHiddenParamString" runat="server" cssclass="hidden hiddenParamString"></asp:TextBox>
    <asp:TextBox ID="uxHiddenViewportWidth" runat="server" cssclass="hidden hiddenViewportWidth"></asp:TextBox>
    <asp:TextBox ID="uxHiddenViewportHeight" runat="server" cssclass="hidden hiddenViewportHeight"></asp:TextBox>

<div id="clientLog">
</div>
</div>
<script type="text/javascript">
    var crcTour = new CrissCrossTour();
    crissCross.crissCrossTourHandler = crcTour;
    crcTour.crissCrossClient = crissCross;
    $(function() {
        if (!crissCross.runningMode)
            crcTour.setupWelcomeHint("showReportHint");
        crcTour.setupReportDescription("showReportDesc");
        
    });
</script> 
<!-- hidden divs for tour content -->   
<div id="tourContent1" class="tourContent" style="display:none;" >
        <p>This is the name of the report thats going to get run</p>
        <button id="tour1Next" type="button" class="tourButton" data-crcThisTourStop="tba" data-crcNextTourStop="tba">Next</button>
</div>
<div id="tourContent2" class="tourContent" style="display:none;" >
        <p>This is the parameters area. Some parameters are displayed by default, but you can add other ones if you need to.</p>
        <button id="tour2Next" type="button" class="tourButton" data-crcThisTourStop="tba" data-crcNextTourStop="tba">Next</button>
</div>
<div id="tourContentParamChooser" class="tourContent" style="display:none;" >
        <p>For reports that have lots of parameters, the 'Choose a Filter' drop-down allows you to choose which ones you want to use.</p>
        <button id="tourParamChooserNext" type="button" class="tourButton" data-crcThisTourStop="tba" data-crcNextTourStop="tba">Next</button>
</div>
<div id="tourContent3" class="tourContent" style="display:none;" >
        <p>This is a Date Parameter - you can type in the box or press the little button to get a clickable calendar. Leave the box blank if you don't want to enter a date.
        When reports have several different date ranges, you usually only need to use one of them.</p>
        <button id="tour3Next" type="button" class="tourButton" data-crcThisTourStop="tba" data-crcNextTourStop="tba">Next</button>
</div>
<div id="tourContent4" class="tourContent" style="display:none;" >
        <p>This is a MultiPick Parameter - click the arrow to drop-down and then choose the options that you want. When there are lots of options, the multi-pick will also let you search within the options.</p>
        <button id="tour4Next" type="button" class="tourButton" data-crcThisTourStop="tba" data-crcNextTourStop="tba">Next</button>
</div>
<div id="tourContent4b" class="tourContent" style="display:none;" >
        <p>This is a MultiPick Parameter - your chosen options are added to the list underneath each time you select one. Click on the round 'minus' sign to remove an option.</p>
        <button id="tour4bNext" type="button" class="tourButton" data-crcThisTourStop="tba" data-crcNextTourStop="tba">Next</button>
</div>
<div id="tourContent5" class="tourContent" style="display:none;" >
        <p>When you've entered all the parameters that you want to enter, push this to run the report.</p>
        <button id="tour5Next" type="button" class="tourButton" data-crcThisTourStop="tba" >OK</button>
</div>
<script type="text/javascript">
    // make report viewer look better in Chrome
    if ($.browser.safari) {
        $(".viewerWrapper table").each(function(i, item) {
            $(item).css('display', 'inline-block');
        });
    }
</script>
<div id="multiPickDialog" style="display:none;"></div>
</asp:Content>
