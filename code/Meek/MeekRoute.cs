using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Meek
{
    public class MeekRoute : Route
    {
        public MeekRoute(string url)
            : base(url, new MvcRouteHandler())
        {
            Defaults = new RouteValueDictionary(new { controller = "Meek", action = "GetContent" });
        }
    }
}
