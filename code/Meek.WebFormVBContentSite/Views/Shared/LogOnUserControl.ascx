<%@ Control Language="VB" Inherits="System.Web.Mvc.ViewUserControl" %>
<%-- The following line works around an ASP.NET compiler warning --%>
<%: ""%>
<%
    If Request.IsAuthenticated Then
    %>
        Welcome <strong><%: Page.User.Identity.Name %></strong>!
        [ <%: Html.ActionLink("Log Off", "LogOff", "Account")%> ]
    <%
    Else
    %>
        [ <%: Html.ActionLink("Log On", "LogOn", "Account")%> ]
    <%        
    End If
%>