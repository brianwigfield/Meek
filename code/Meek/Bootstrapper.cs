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
        /// If using this method it is assumed all configurations are in the web.config, but the services may still be overridden with the dependency resolver
        /// </summary>
        public static void Initialize()
        {
            var configValues = new StandardConfigProvider().Section<MeekConfigurationSection>("meek");
            var config = new DefaultConfiguration(configValues);
            Configure(config);
        }


        /// <summary>
        /// If using this method requires all information to be passed in
        /// </summary>
        public static void Initialize(Configuration.Configuration config)
        {
            Configure(config);
        }

        private static void Configure(Configuration.Configuration config)
        {
            if (config == null)
                throw new ArgumentException("Configuration can't be null");

            //prevents double initializations
            if (Configuration != null)
                return;

            Configuration = config;

            //If we map partial routes as well then they will be able to navigated to directly
            foreach (var route in config.GetRepository().AvailableRoutes(ContentTypes.Full))
            {
                RouteTable.Routes.Insert(0, new MeekRoute(route));
            }

            if (!string.IsNullOrEmpty(config.AltManageContentRoute))
                RouteTable.Routes.Insert(0,
                                         new Route(config.AltManageContentRoute,
                                                   new RouteValueDictionary(new { controller = "Meek", action = "Manage" }),
                                                   new MvcRouteHandler()));

            if (config.CkEditorPath.ToLower() == "/meek/ckeditor")
                RouteTable.Routes.Insert(0,
                                         new Route("Meek/ckeditor/{*filename}",
                                                   new RouteValueDictionary(new { controller = "Meek", action = "BogusAction" /* if not here it will mess up the ActionLink HtmlHelper method since its not a named route (MVC bug?) */ }),
                                                   new CkEditorRouteHandler()));

            HostingEnvironment.RegisterVirtualPathProvider(new ContentPathProvider(HostingEnvironment.VirtualPathProvider, Configuration));
        }

        public static Configuration.Configuration Configuration { get; internal set; }

    }
}
