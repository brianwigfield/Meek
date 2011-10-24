<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Meek.Content.BrowseFiles>" %>

<asp:Content ID="MeekAdminContents" ContentPlaceHolderID="{PlaceHolder}" runat="server">

    <script type='text/javascript'>
      function callBack(fileId)
      {
      window.opener.CKEDITOR.tools.callFunction('<%=Model.Callback%>', '/Meek/GetFile/' + fileId, '<%=Model.Message%>');
      window.close();
      }
    </script>

    <% foreach (var fileId in Model.Files) { %>

    <div style="float:left; width:150px;">
      <img src="/Meek/GetFileThumbnail/<%=fileId%>" onclick="callBack('<%=fileId%>');" />
    </div>

    <%}%>

    <div style="clear:both;"></div>

</asp:Content>