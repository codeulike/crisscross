<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HelpIntro.ascx.cs" Inherits="CrissCross.Help.HelpIntro" %>
<p><b><asp:Label ID="uxAppTitle" runat="server" Text="CrissCross"></asp:Label></b> is a tool for running reports. It runs exactly the same reports as the ones in SSRS at 
                    <asp:HyperLink ID="uxReportManagerLink" runat="server"></asp:HyperLink>.</p>
<p>All of the same reports are available, looking at the same data with the same parameters and output.</p>
<p>What is different is the way 
    <asp:Label ID="uxAppTitle2" runat="server" Text="CrissCross"></asp:Label> lets you browse the report catalogue, and how it asks you for report parameters. It is designed to be easier to use, with better support
    for reports with lots of parameters, and also better support for multipick parameters.</p>
