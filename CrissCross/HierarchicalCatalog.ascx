<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HierarchicalCatalog.ascx.cs" Inherits="CrissCross.HierarchicalCatalog" %>
<asp:Label ID="uxAllCatalogHierarchical" runat="server" ></asp:Label>
<asp:TextBox ID="uxHiddenFullUsername" runat="server" Visible="false"></asp:TextBox>
<asp:TextBox ID="uxHiddenInitialFolder" runat="server" Visible="false"></asp:TextBox>
<script type="text/javascript">
        $(function() {
            $(".folderName").click(function() {
                $(this).next(".folderChildren").toggle("fast");
            });

            $(".vanillaHover").mouseover(function() {
                $(this).addClass("ui-state-hover");
            });

            $(".vanillaHover").mouseout(function() {
                $(this).removeClass("ui-state-hover");
            });

            if ($(".scrollToFolder").length > 0) {
                $(window).scrollTop($(".scrollToFolder").offset().top);
            }
        });
    
    </script>