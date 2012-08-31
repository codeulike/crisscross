<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="CrissCross.Home" %>
<%@ Register src="Help/HelpIntro.ascx" tagname="HelpIntro" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="cornerbackground">
            <div class="cornertop">
                <div class="cornerbottom">
                <div class="inner">
    <div class="inlineHint" id="welcomeHint" style="display:none;">
    <p>Welcome!</p>
    
        <uc1:HelpIntro ID="HelpIntro1" runat="server" />
        <button id="welcomeClose" type="button">OK</button>
        <button id="takeTour" type="button" class="tourButton" data-crcNextTourStop="tourStop1">Take Tour</button>
    </div>
    <h2 >
        <asp:Label ID="uxFeaturedReportsTitle" runat="server" ></asp:Label><span id="tourStop1" class="tourStop" data-crcTourContent="tourContent1">&nbsp;</span>
        <span id="tourStop5" class="tourStop" data-crcTourContent="tourContent5">&nbsp;</span>
    </h2>
    <div id="tourContent1" class="tourContent" style="display:none;" >
        <p>This section links to the featured reports that you might find useful</p>
        <button id="tour1Next" type="button" class="tourButton" data-crcThisTourStop="tourStop1" data-crcNextTourStop="tourStop2">Next</button>
    </div>
    <div id="tourContent5" class="tourContent" style="display:none;" >
        <p>Now click on one of the featured reports to continue the tour ...</p>
        <button id="tour5Next" type="button" class="tourButton" data-crcThisTourStop="tourStop5" >OK</button>
    </div>
    <asp:ListView ID="uxFeaturedListView" runat="server">
        <LayoutTemplate>
        <div class="reportList">
           <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
        </div>
       </LayoutTemplate>
       <ItemTemplate>
       <div class="reportRow">
            <a class="reportLink vanillaHover" href="Report.aspx?path=<%# Server.UrlEncode(Eval("Path").ToString()) %>" style="width:300px;"><%# Eval("Name") %></a>
            <div class="reportPath"><%# Eval("Path") %></div>
            <div class="clear"></div>
       </div>
       </ItemTemplate>
    </asp:ListView>
    <asp:Panel ID="uxUsersMostUsedPanel" runat="server">
    
    <h2 >Your Most Used Reports<span id="tourStop2" class="tourStop" data-crcTourContent="tourContent2">&nbsp;</span></h2>
    <div id="tourContent2" class="tourContent" style="display:none;" >
        <p>This section links to the reports that you personally run most often</p>
        <button id="tour2Next" type="button" class="tourButton" data-crcThisTourStop="tourStop2" data-crcNextTourStop="tourStop3">Next</button>
    </div>
    <asp:ListView ID="uxPopularListView" runat="server">
        <LayoutTemplate>
        <div class="reportList">
           <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
        </div>
       </LayoutTemplate>
       <ItemTemplate>
       <div class="reportRow">
            <a class="reportLink vanillaHover" href="Report.aspx?path=<%# Server.UrlEncode(Eval("Path").ToString()) %>" style="width:300px;"><%# Eval("Name") %></a>
            <div class="reportPath"><%# Eval("Path") %></div>
            <div class="clear"></div>
       </div>
       </ItemTemplate>
    </asp:ListView>
    </asp:Panel>
    <asp:Panel ID="uxEveryonesMostUsedPanel" runat="server">
    <h2>Everyones Most Used Reports</h2>
    <asp:ListView ID="uxGlobalPopularListView" runat="server">
        <LayoutTemplate>
        <div class="reportList">
           <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
        </div>
       </LayoutTemplate>
       <ItemTemplate>
       <div class="reportRow">
            <a class="reportLink vanillaHover" href="Report.aspx?path=<%# Server.UrlEncode(Eval("Path").ToString()) %>" style="width:300px;"><%# Eval("Name") %></a>
            <div class="reportPath"><%# Eval("Path") %></div>
            <div class="clear"></div>
       </div>
       </ItemTemplate>
    </asp:ListView>
    </asp:Panel>
    <asp:Panel ID="uxUserReportHistoryPanel" runat="server">
    <h2 >Your Report History<span id="tourStop3" class="tourStop" data-crcTourContent="tourContent3">&nbsp;</span></h2>
    <div id="tourContent3" class="tourContent" style="display:none;" >
        <p>This section shows your recent report runs, including the time you ran the report and the number of rows returned. Clicking the report links
        will load the report with the exact parameters that you previously used.</p>
        <button id="tour3Next" type="button" class="tourButton" data-crcThisTourStop="tourStop3" data-crcNextTourStop="tourStop4">Next</button>
    </div>
    <asp:ListView ID="uxRecentListView" runat="server">
        <LayoutTemplate>
        <div class="reportList">
           <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
        </div>
       </LayoutTemplate>
       <ItemTemplate>
       <div class="reportRow">
            <a class="reportLink vanillaHover" href="Report.aspx?path=<%# Server.UrlEncode(Eval("ReportPath").ToString()) %>&parameters=<%# Server.UrlEncode(Eval("Parameters").ToString()) %>" style="width:300px;" title="<%# Server.HtmlEncode(Eval("Parameters").ToString()) %>"><%# Eval("ReportName") %></a>
            <div class="reportPath" style="width:250px;"><%# Eval("ReportPath") %></div>
            <div class="reportInfo" style="width:120px;">Run at: <%# ((DateTime)Eval("TimeStart")).ToString("dd/MMM/yyyy HH:mm") %></div>
            <div class="reportInfo">Rows returned: <%# Eval("RowCount") %></div>
            <div class="clear"></div>
            
       </div>
       </ItemTemplate>
    </asp:ListView>
    <p><a href="AllHistory.aspx">(More ...)</a></p>
    </asp:Panel>
    <h2>All Reports<span id="tourStop4" class="tourStop" data-crcTourContent="tourContent4">&nbsp;</span></h2>
    <div id="tourContent4" class="tourContent" style="display:none;" >
        <p>This section links you through to the full list of all reports. You can also get to that by clicking 'Report List' at the top of the page.</p>
        <button id="tour4Next" type="button" class="tourButton" data-crcThisTourStop="tourStop4" data-crcNextTourStop="tourStop5">Next</button>
    </div>
    <p><a href="AllCatalog.aspx">Click here for full report list</a> or choose one of these folders:</p>
    <asp:ListView ID="uxCatalogFoldersListView" runat="server">
        <LayoutTemplate>
        <div class="smallFolderList">
           <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
           <div class="smallFolderRow"><a href="AllCatalog.aspx">(More ...)</a></div>
        </div>
       </LayoutTemplate>
       <ItemTemplate>
       <div class="smallFolderRow">
            <a class="smallFolderName" href="AllCatalog.aspx?Folder=<%# Server.UrlEncode(Container.DataItem.ToString()) %>" ><%# Container.DataItem.ToString() %></a>
            <div class="clear"></div>
       </div>
       </ItemTemplate>
    </asp:ListView>
    </div>
</div></div></div>    
    <asp:TextBox ID="uxHiddenFullUsername" runat="server" Visible="false"></asp:TextBox>
<script type="text/javascript">
    var crcTour = new CrissCrossTour();
    $(function() {
        $(".vanillaHover").mouseover(function() {
            $(this).addClass("ui-state-hover");
        });

        $(".vanillaHover").mouseout(function() {
            $(this).removeClass("ui-state-hover");
        });

        crcTour.setupWelcomeHint("showHomeHint");
        crcTour.setupTour();
        
    });
    
</script>    
</asp:Content>
