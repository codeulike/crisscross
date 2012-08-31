<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="HomeHelp.aspx.cs" Inherits="CrissCross.Help.HomeHelp" %>
<%@ MasterType  virtualPath="~/Main.Master"%>
<%@ Register src="HelpIntro.ascx" tagname="HelpIntro" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="cornerbackground">
            <div class="cornertop">
                <div class="cornerbottom">
                <div class="inner">
<h2>Help</h2>

<uc1:HelpIntro ID="HelpIntro1" runat="server" />

 <h2>Home page help</h2>
 <p>The Home page shows a series of sections with links to run reports:</p>
 <ul>
 <li><b>
     <asp:Label ID="uxFeaturedReportsTitle" runat="server" Text="Featured Reports"></asp:Label></b> - this links to the key featured reports.</li>
     <li><b>Your Most Used Reports</b> - this shows the 5 reports that you have run the most over the last few months</li>
     <li><b>Your Report History</b> - this shows your 5 most recent report runs. Note that the time the report was run and the number of rows returned is shown. Clicking on one of them will load the report with
     the same parameter choices that you used. Pressing the (more...) link will allow you to look through all your previous report runs.</li>
     <li><b>All Reports</b> - this provides a link to let you browse the whole report catalogue.</li>
 </ul>
 <h2>Report page help</h2>
 <p>The report page lets you actually run a report.</p> 
 <h3>Choosing parameters</h3>
 <p>Usually, a few parameters will be shown at the top of the page. However, when reports have lots of parameters, they are not all displayed initially. You can
 choose to add new parameters to the page by choosing them from the drop-down that says '&lt;Choose a filter&gt;'.</p>
 <p>Date Parameters allow you to enter a date directly or choose it from a calendar. To leave the date blank, simply leave the date box blank.</p>
 <p>Single-pick parameters work in the usual way - with a drop down. If there are many options, a small 'search' box will appear in the drop down to let you filter the items.</p>
 <p>Multi-pick parameters show as a drop-down with checkboxes. If there are many options, a small 'search' box will appear in the drop down to let you filter the items.</p>
 <h3>Running the report</h3>
 <p>When you have chosen all the parameers you want, press the Run Report button.</p>
 <p>When the report is running, your parameter choices are summarised as text (to save screen space). When the report is run, if you want
 to adjust your parameters and run again, press the 'Change Parameters' button.</p>
 <p>Report results will look the same as usual, with the same Export options (to Excel, and so on) working in the same way.</p>
                    

</div></div></div></div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FullWidthPlaceHolder" runat="server">
</asp:Content>
