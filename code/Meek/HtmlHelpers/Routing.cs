using System.Web.Mvc;

namespace Meek.HtmlHelpers
{
    public static class Routing
    {

        public static void RenderRoute(this HtmlHelper htmlHelper, string routeUrl)
        {
            System.Web.Mvc.Html.ChildActionExtensions.RenderAction(htmlHelper, "GetPartialResource", "Meek", new {content = routeUrl});
        }

    }
}
