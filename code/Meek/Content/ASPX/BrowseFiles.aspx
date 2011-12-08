<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Meek.Content.BrowseFiles>" %>

<asp:Content ID="MeekAdminContents" ContentPlaceHolderID="{PlaceHolder}" runat="server">

    <script type='text/javascript'>
      function callBack(fileId)
      {
      window.opener.CKEDITOR.tools.callFunction('<%=Model.Callback%>', '/Meek/GetFile/' + fileId, '<%=Model.Message%>');
      window.close();
      }
    </script>

    <% foreach (var file in Model.Files) { %>

    <div class="meekBrowsingFile"  style="float:left; width:150px;">
        <img src="/Meek/GetFileThumbnail/<%=file.Key%>" onclick="callBack('<%=file.Key%>');" />
        <label><%=file.Value %></label>
        <a href="/Meek/RemoveFile/<%=file.Key%>">Delete</a>
    </div>

    <%}%>

    <div style="clear:both;"></div>

</asp:Content>