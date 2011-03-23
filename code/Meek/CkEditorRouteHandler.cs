using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Meek
{
    public class CkEditorRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var filename = requestContext.RouteData.Values["filename"] as string;

            if (string.IsNullOrEmpty(filename))
            {
                requestContext.HttpContext.Response.StatusCode = 404;
                requestContext.HttpContext.Response.End();
            }
            else
            {
                requestContext.HttpContext.Response.Clear();
                requestContext.HttpContext.Response.ContentType = GetContentType(Path.GetExtension(filename));

                var file = Assembly.GetExecutingAssembly()
                           .GetManifestResourceStream("Meek.Content.ckeditor." + filename.Replace(@"/", "."))
                           .ReadFully();

                requestContext.HttpContext.Response.BinaryWrite(file);
                requestContext.HttpContext.Response.End();

            }
            return null;
        }

        private static string GetContentType(String extension)
        {
            switch (extension)
            {
                case ".js": return "application/x-javascript";
                case ".jpg":
                case ".jpeg": return "image/jpeg";
                case ".png": return "image/png";
                case ".gif": return "image/gif";
                case ".css": return "text/css";
                default: return "";
            }
        }
    }
}
