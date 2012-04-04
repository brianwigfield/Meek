using System;
using System.IO;
using System.Text;
using System.Web.Hosting;
using Meek.Configuration;
using Meek.Storage;

namespace Meek
{
    public class ContentVirtualFile : VirtualFile
    {
        readonly string _pathKey;
        readonly Repository _repository;
        readonly ViewEngineOptions _viewEngine;
        const string EditLink = "<div class=\"MeekEditLink\"><a href=\"/Meek/Manage?aspxerrorpath={0}\">Edit Content</a></div>";

        public ContentVirtualFile(Repository repository, string requestedPath, string pathKey, ViewEngineOptions viewEngine)
            : base(requestedPath)
        {
            _pathKey = pathKey;
            _repository = repository;
            _viewEngine = viewEngine;
        }

        public override Stream Open()
        {
            var content = _repository.Get(_pathKey);
            var contentMarkup = content.Contents;
            var constructedContent = string.Empty;

            switch (_viewEngine.Type)
            {
                case ViewEngineType.Razor:
                    contentMarkup = contentMarkup.Replace("@", "@@");
                    contentMarkup = AddRazorEditLinkMarkup(contentMarkup, content.Partial);
                    constructedContent = string.Format(
                        "@{{ {0} ViewBag.Title = \"{1}\";}} {2}",
                        content.Partial ? " Layout = null;" : null,
                        content.Title,
                        contentMarkup);
                    break;
                case ViewEngineType.ASPX:
                    contentMarkup = AddWebformsEditLinkMarkup(contentMarkup, content.Partial);
                    if (!content.Partial)
                        contentMarkup = string.Format(
                            @"<asp:Content ID=""MeekContents"" ContentPlaceHolderID=""{0}"" runat=""server"">{1}</asp:Content>",
                            _viewEngine.PlaceHolder, contentMarkup);
                    constructedContent = string.Format(
                        @"<%@ Page Title=""{0}"" Language=""C#"" {1} Inherits=""System.Web.Mvc.ViewPage"" %>{2}",
                        content.Title,
                        content.Partial ? null : "MasterPageFile=\"" + _viewEngine.Layout + "\"",
                        contentMarkup);
                    break;
            }
            
            return new MemoryStream(Encoding.UTF8.GetBytes(constructedContent));
        }

        private string AddRazorEditLinkMarkup(string content, bool isPartial)
        {
            var editLink = string.Format(Environment.NewLine + "@if(ViewBag.IsContentAdmin){{" + EditLink + "}}", _pathKey);

            if (isPartial || content.IndexOf(@"</html>") == -1)
                return content + editLink;
            else
                return content.Insert(content.IndexOf(@"</html>"), editLink);
        }

        private string AddWebformsEditLinkMarkup(string content, bool isPartial)
        {
            var editLink = string.Format(Environment.NewLine + "<% if(ViewBag.IsContentAdmin) {{ %>" + EditLink + "<% }} %>", _pathKey);

            if (isPartial || content.IndexOf(@"</html>") == -1)
                return content + editLink;
            else
                return content.Insert(content.IndexOf(@"</html>"), editLink);
        }

    }
}
