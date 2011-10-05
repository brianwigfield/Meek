<%@ Page Title="All content" Language="C#" Inherits="System.Web.Mvc.ViewPage<Meek.Content.UploadFileSuccess>" %>

<script type="text/javascript">
    window.parent.CKEDITOR.tools.callFunction('<%=Model.Callback%>', '<%=Model.Url%>', '<%=Model.Message%>');
</script>