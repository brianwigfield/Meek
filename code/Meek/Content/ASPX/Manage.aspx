<%@ Page Title="Manage content" Language="C#" Inherits="System.Web.Mvc.ViewPage<Meek.Content.Manage>" %>

<asp:Content ID="MeekAdminContents" ContentPlaceHolderID="{PlaceHolder}" runat="server">

    <script type="text/javascript" src="<%=ViewBag.CkEditorPath%>"></script>

    <form method="post">
        <%= Html.ValidationSummary(true) %>
        <div style="padding:5px;">
            <label style="width:125px; float:left;">Content URL:</label> <%=Html.TextBoxFor(m => m.ManageUrl, new { style = "width:300px;" })%>
            <%=Html.ValidationMessageFor(m => m.ManageUrl)%>
        </div>
        <div style="padding:5px;">
            <label style="width:125px; float:left;">Title:</label> <%=Html.TextBoxFor(m => m.ContentTitle, new {style = "width:300px;"})%>
            <%=Html.ValidationMessageFor(m => m.ContentTitle)%>
        </div>
        <div style="padding:5px;">
            <label style="width:125px; float:left;">Is Partial Content?:</label> <%=Html.CheckBoxFor(m => m.Partial)%>
            <%=Html.ValidationMessageFor(m => m.Partial)%>
        </div>
        <div style="padding:5px;">
            <%=Html.ValidationMessageFor(m => m.EditorContents)%>
            <textarea class="ckeditor" cols="80" id="EditorContents" name="EditorContents" rows="10">
                <%=Model.EditorContents%>
            </textarea>
        </div>
        <button name="submit" value="save" id="SaveContent">Save Content</button>
        <button name="submit" value="delete" id="DeleteContent">Delete Content</button>
    </form>

</asp:Content>