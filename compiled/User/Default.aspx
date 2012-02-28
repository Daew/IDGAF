<%@ Page Title="" Language="C#" MasterPageFile="~/Edu.Master" AutoEventWireup="true" Inherits="EduWeb.User.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="script" runat="server">
<script type="text/javascript">
    function Confirm() {
        if ((document.getElementById("mail").value == document.getElementById("mailc").value) &&
            (document.getElementById("pwd").value == document.getElementById("pwdc").value))
        { return true; }
        else
        { return false; }
    }
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="content" runat="server">
<% Write(); %>
</asp:Content>
