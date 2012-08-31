<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="CrissCross.Error" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="cornerbackground">
            <div class="cornertop">
                <div class="cornerbottom">
                <div class="inner">
                <h2>Apologies</h2>
                <p>An unexpected error has occurred.</p>
                <p>The error has been logged - if problems persist, contact the systems administrator.</p>
                <p>Time of error: 
                    <asp:Label ID="uxErrorTime" runat="server" ></asp:Label></p>
                 </div>
        </div></div></div>
</asp:Content>
