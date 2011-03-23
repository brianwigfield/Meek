using System;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Meek.Storage;
using System.Linq;

namespace Meek.Configuration
{
    public static class Configuration
    {

        /// <summary>
        /// If using this method it is assumed all configurations are in the web.config, but the services may still be overridden
        /// </summary>
        public static void Initialize()
        {
            Config = new StandardConfigProvider().Section<MeekConfigurationSection>("meek");
            Configure(Config.AltManageContentRoute, Config.CkEditorPath, Config.NotFoundView, new DefaultServices(Config));
        }

        /// <summary>
        /// If using this method it is assumed that you will provide resolution for ContentRepository & Authorization through the DependencyResolver
        /// </summary>
        public static void Initialize(string altManageContentRoute, string ckEditorPath, string notFoundView)
        {
            Configure(altManageContentRoute, ckEditorPath, notFoundView, new DefaultServices(null));
        }

        /// <summary>
        /// If using this method requires all information to be passed in
        /// </summary>
        public static void Initialize(string altManageContentRoute, string ckEditorPath, string notFoundView, Services services)
        {
            Configure(altManageContentRoute, ckEditorPath, notFoundView, services);
        }

        private static void Configure(string altManageContentRoute, string ckEditorPath, string notFoundView, Services services)
        {
            Services = services;
            //If we map partial routes as well then they will be able to navigated to directly
            foreach (var route in Services.GetRepository().AvailableRoutes(ContentTypes.Full))
            {
                RouteTable.Routes.Insert(0, new MeekRoute(route));
            }

            Config.AltManageContentRoute = string.IsNullOrEmpty(altManageContentRoute) ? "Missing" : altManageContentRoute;
            RouteTable.Routes.Insert(0,
                                     new Route(altManageContentRoute,
                                               new RouteValueDictionary(new { controller = "Meek", action = "Manage" }),
                                               new MvcRouteHandler()));


            Config.CkEditorPath = string.IsNullOrEmpty(ckEditorPath) ? "/Meek/ckeditor" : ckEditorPath;
            if (Config.CkEditorPath == "/Meek/ckeditor")
                RouteTable.Routes.Insert(0,
                                         new Route("Meek/ckeditor/{*filename}",
                                                   new RouteValueDictionary(new { controller = "Meek", action = "BogusAction" /* if not here it will mess up the ActionLink HtmlHelper method since its not a named route (MVC bug?) */ }),
                                                   new CkEditorRouteHandler()));

            Config.NotFoundView = string.IsNullOrEmpty(notFoundView) ? "NotFound" : notFoundView;


            HostingEnvironment.RegisterVirtualPathProvider(new ContentPathProvider(HostingEnvironment.VirtualPathProvider, Services));
        }

        internal static MeekConfigurationSection Config { get; set; }
        internal static Services Services { get; set; }

    }
}
