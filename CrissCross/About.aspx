<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="CrissCross.About" %>
<%@ MasterType  virtualPath="~/Main.Master"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="cornerbackground">
            <div class="cornertop">
                <div class="cornerbottom">
                <div class="inner">
<h2>About 
    <asp:Label ID="uxAppTitle" runat="server" Text="CrissCross"></asp:Label></h2>
<asp:Label ID="uxNameExplanation" runat="server" Text=""></asp:Label>
<p>CrissCross is an Open-Source custom front-end for running SSRS Reports.</p>
<h2>Configuration</h2>
<p>Version: 
    <asp:Label ID="uxVersionNo" runat="server" ></asp:Label></p>
<p>Linked to SSRS server: 
    <asp:Label ID="uxReportViewerUrl" runat="server" ></asp:Label></p>
<p>Also via web service at: 
    <asp:Label ID="uxReportWebServiceUrl" runat="server" ></asp:Label></p>
<p>Standard Report Manager is at: 
    <asp:HyperLink ID="uxReportManagerUrl" runat="server"></asp:HyperLink></p>
<p>CrissCross Impersonation: 
    <asp:Label ID="uxCrissCrossImpersonationMode" runat="server" ></asp:Label></p>
    <asp:Panel ID="uxCrissCrossFixedUserPanel" runat="server">
        <p>CrissCross Fixed User: 
            <asp:Label ID="uxCrissCrossFixedUser" runat="server" ></asp:Label></p>
    </asp:Panel>
<p>Asp.Net Impersonation: 
    <asp:Label ID="uxAspNetImpersonationMode" runat="server" ></asp:Label></p>
    <asp:Panel ID="uxAspNetFixedUserPanel" runat="server">
        <p>ASP.Net Fixed User: 
            <asp:Label ID="uxAspNetFixedUser" runat="server" ></asp:Label></p>
    </asp:Panel>
<asp:Panel ID="uxWarningPanel" runat="server" Visible="false">
    <asp:Label ID="uxWarningMessage" runat="server" CssClass="warningtext"></asp:Label>
</asp:Panel>
<p>You are logged in as: 
    <asp:Label ID="uxCurrentUser" runat="server" ></asp:Label></p>
<asp:Panel ID="uxLocalOnlyPanel" runat="server" Visible="false">
<p>Elmah Error log: <a href="Elmah.axd">Elmah.axd</a></p>
</asp:Panel>
<h2>Credits</h2>
<p>CrissCross is an Open-Source app. Documentation, downloads and source code are available from the project homepage on GitHub:</p>
<p><a href="https://github.com/codeulike/crisscross">https://github.com/codeulike/crisscross</a></p>
<p>CrissCross is licensed under the <a href="http://www.gnu.org/licenses/gpl-2.0.html">GNU General Public License version 2</a>.</p>
<p>CrissCross is built with the following Open-Source components:</p>
<ul>
<li><a href="http://jquery.com/">jQuery</a> (Open-Source MIT license)</li>
<li><a href="http://jqueryui.com/">jQuery UI</a> (Open-Source MIT license)</li>
<li><a href="https://github.com/ehynds/jquery-ui-multiselect-widget">jQuery MultiSelect UI Widget</a> by <a href="http://www.erichynds.com/jquery/jquery-ui-multiselect-widget/">Eric Hynds</a> (Open-Source MIT license)</li>
<li><a href="https://github.com/douglascrockford/JSON-js">json2.js</a> (Public Domain)</li>
<li><a href="http://hibernatingrhinos.com/open-source/rhino-mocks">Rhino Mocks</a> (Open Source BSD License)</li>
<li><a href="http://logging.apache.org/log4net/">log4net</a> (Open Source Apache License)</li>
<li><a href="http://code.google.com/p/elmah/">Elmah</a> (Open Source Apache License)</li>
<li><a href="http://craigsworks.com/projects/qtip2/">Q-Tip2</a> (Open-Source MIT license)</li>
<li><a href="http://code.google.com/p/cookies/">jQuery Cookies</a> (Open-Source MIT license)</li>
<li><a href="http://loopj.com/jquery-tokeninput/">jQuery Tokeninput</a> (Open-Source MIT license)</li>
</ul>
</div></div></div></div>
</asp:Content>
