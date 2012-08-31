<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="AllCatalog.aspx.cs" Inherits="CrissCross.AllCatalog" %>
<%@ Register src="HierarchicalCatalog.ascx" tagname="HierarchicalCatalog" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div class="cornerbackground">
            <div class="cornertop">
                <div class="cornerbottom">
                <div class="inner">
        
                    <uc1:HierarchicalCatalog ID="uxHierarchicalCatalog" runat="server" />
    
            </div>
            </div></div></div>

    <asp:TextBox ID="uxHiddenFullUsername" runat="server" Visible="false"></asp:TextBox>
    
</asp:Content>
