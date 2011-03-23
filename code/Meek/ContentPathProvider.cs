using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Caching;
using System.Web.Hosting;
using Meek.Configuration;

namespace Meek
{
    public class ContentPathProvider : VirtualPathProvider
    {
        const string VPathPrefix = "/Views/Meek/";
        readonly Dictionary<string, string> _internalResources;
        readonly VirtualPathProvider _baseProvider;
        readonly Services _services;

        public ContentPathProvider(VirtualPathProvider baseProvider, Services services)
        {
            _baseProvider = baseProvider;
            _services = services;

            _internalResources = new Dictionary<string, string>()
                                    {
                                        {"Manage.cshtml",  "Meek.Content.Manage.cshtml"},
                                        {"CreatePartial.cshtml", "Meek.Content.CreatePartial.cshtml"}
                                    };

        }

        public override bool FileExists(string virtualPath)
        {
            bool exists = false;
            if (_baseProvider != null)
                exists =_baseProvider.FileExists(virtualPath);

            if (!exists && IsMeekPath(virtualPath))
            {
                if (IsInternalResource(virtualPath))
                    return true;

                var repository = _services.GetRepository();
                exists = repository.Exists(TranslateVirtualPath(virtualPath).Replace(".cshtml", string.Empty).Replace(".vbhtml", string.Empty));
            }
            return exists;
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            if (_baseProvider != null && _baseProvider.FileExists(virtualPath))
                return _baseProvider.GetFile(virtualPath);

            if (IsMeekPath(virtualPath) && IsInternalResource(virtualPath))
                return GetInternalResource(virtualPath);

            var repository = _services.GetRepository();
            if (IsMeekPath(virtualPath) && repository.Exists(TranslateVirtualPath(virtualPath).Replace(".cshtml", string.Empty).Replace(".vbhtml", string.Empty)))
                return new ContentVirtualFile(repository, virtualPath, TranslateVirtualPath(virtualPath), _services.GetAuthorization());

            return null;
        }

        private static bool IsMeekPath(string virtualPath)
        {
            if (virtualPath.StartsWith("~"))
                virtualPath = virtualPath.Substring(1);

            return virtualPath.StartsWith(VPathPrefix);
        }

        private static string TranslateVirtualPath(string virtualPath)
        {
            if (virtualPath.StartsWith("~"))
                virtualPath = virtualPath.Substring(1);

            return virtualPath.Replace(VPathPrefix, null);
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (_baseProvider != null && _baseProvider.FileExists(virtualPath))
                return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);

            return null;
        }

        public override string GetFileHash(string virtualPath, IEnumerable virtualPathDependencies)
        {
            if (_baseProvider != null && _baseProvider.FileExists(virtualPath))
                return base.GetFileHash(virtualPath, virtualPathDependencies);
            
            return null;
        }

        private bool IsInternalResource(string virtualPath)
        {
            return _internalResources.Any(x => x.Key == TranslateVirtualPath(virtualPath));
        }

        private VirtualFile GetInternalResource(string virtualPath)
        {
            var resource = _internalResources.Single(x => x.Key == TranslateVirtualPath(virtualPath));

            return new VirtualFileStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(resource.Value)
                                                                   , virtualPath);
        }

    }
}
