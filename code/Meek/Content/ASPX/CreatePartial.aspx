<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Meek.Content.CreatePartial>" %>

<% if (Model.IsContentAdmin) { %>
    <div class="MeekCreateLink">
        <a href="<%=Model.CreateLink%>">Create Missing Content</a>
    </div>
<%} %>