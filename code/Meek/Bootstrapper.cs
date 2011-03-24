using System;
using System.Collections.Generic;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Meek.Configuration;
using Meek.Storage;

namespace Meek
{
    public static class BootStrapper
    {

        /// <summary>
        /// If using this method it is assumed all configurations are in the web.config, but the services may still be overridden
        /// </summary>
        public static void Initialize()
        {
            var config = new StandardConfigProvider().Section<MeekConfigurationSection>("meek");
            var settings = new StandardSettings()
                               {
                                   AltManageContentRoute = config.AltManageContentRoute,
                                   CkEditorPath = config.CkEditorPath,
                                   NotFoundView = config.NotFoundView
                               };
            Configure(settings, new DefaultServices(config));
        }

        /// <summary>
        /// If using this method it is assumed that you will provide resolution for ContentRepository & Authorization through the DependencyResolver
        /// </summary>
        public static void Initialize(string altManageContentRoute, string ckEditorPath, string notFoundView)
        {
            var settings = new StandardSettings()
                               {
                                   AltManageContentRoute = altManageContentRoute,
                                   CkEditorPath = ckEditorPath,
                                   NotFoundView = notFoundView
                               };
            Configure(settings, new DefaultServices(null));
        }

        /// <summary>
        /// If using this method requires all information to be passed in
        /// </summary>
        public static void Initialize(string altManageContentRoute, string ckEditorPath, string notFoundView, Services services)
        {
            var settings = new StandardSettings()
                               {
                                   AltManageContentRoute = altManageContentRoute,
                                   CkEditorPath = ckEditorPath,
                                   NotFoundView = notFoundView
                               };
            Configure(settings, services);
        }

        /// <summary>
        /// If using this method requires all information to be passed in
        /// </summary>
        public static void Initialize(Settings settings, Services services)
        {
            Configure(settings, services);
        }

        private static void Configure(Settings settings, Services services)
        {
            if (settings == null)
                throw new ArgumentException("Settings can not be null");

            if (services == null)
                throw new ArgumentException("Services can not be null");

            Services = services;
            Settings = settings;

            //If we map partial routes as well then they will be able to navigated to directly
            foreach (var route in Services.GetRepository().AvailableRoutes(ContentTypes.Full))
            {
                RouteTable.Routes.Insert(0, new MeekRoute(route));
            }

            Settings.AltManageContentRoute = string.IsNullOrEmpty(settings.AltManageContentRoute) ? "Missing" : settings.AltManageContentRoute;
            RouteTable.Routes.Insert(0,
                                     new Route(settings.AltManageContentRoute,
                                               new RouteValueDictionary(new { controller = "Meek", action = "Manage" }),
                                               new MvcRouteHandler()));


            Settings.CkEditorPath = string.IsNullOrEmpty(settings.CkEditorPath) ? "/Meek/ckeditor" : settings.CkEditorPath;
            if (Settings.CkEditorPath == "/Meek/ckeditor")
                RouteTable.Routes.Insert(0,
                                         new Route("Meek/ckeditor/{*filename}",
                                                   new RouteValueDictionary(new { controller = "Meek", action = "BogusAction" /* if not here it will mess up the ActionLink HtmlHelper method since its not a named route (MVC bug?) */ }),
                                                   new CkEditorRouteHandler()));

            Settings.NotFoundView = string.IsNullOrEmpty(settings.NotFoundView) ? "NotFound" : settings.NotFoundView;


            HostingEnvironment.RegisterVirtualPathProvider(new ContentPathProvider(HostingEnvironment.VirtualPathProvider, Services));
        }

        internal static Settings Settings { get; set; }
        internal static Services Services { get; set; }

    }
}
