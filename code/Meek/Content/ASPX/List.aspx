<%@ Page Title="All content" Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<string>>" %>

<asp:Content ID="MeekAdminContents" ContentPlaceHolderID="{PlaceHolder}" runat="server">

    <h2>All Content</h2>

    <ul>

        <% foreach (var route in Model) { %>
            <li>
                <a href="/Meek/Manage?aspxerrorpath=<%=route%>"><%=route%></a>
            </li>
        <%} %>

    </ul>

</asp:Content>