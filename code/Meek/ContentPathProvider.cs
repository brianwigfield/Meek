using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Caching;
using System.Web.Hosting;

namespace Meek
{
    public class ContentPathProvider : VirtualPathProvider
    {
        const string VPathPrefix = "/Views/Meek/";
        readonly string _extension;
        readonly Dictionary<string, string> _internalResources;
        readonly VirtualPathProvider _baseProvider;
        readonly Configuration.Configuration _config;

        public ContentPathProvider(VirtualPathProvider baseProvider, Configuration.Configuration services)
        {
            _baseProvider = baseProvider;
            _config = services;
            _internalResources = new Dictionary<string, string>();

            switch (_config.ViewEngineOptions.Type)
            {
                case Configuration.ViewEngineType.Razor:
                    _extension = ".cshtml";
                    break;
                case Configuration.ViewEngineType.ASPX:
                    _extension = ".aspx";
                    break;
                default:
                    throw new ArgumentException("Invalid ViewEngine Specified.");
            }

            Action<string> addResource = (file) =>
                _internalResources.Add(file + _extension, string.Format("Meek.Content.{0}.{1}{2}", _config.ViewEngineOptions.Type.ToString(), file, _extension));

            addResource("Manage");
            addResource("CreatePartial");
            addResource("List");
            addResource("BrowseFiles");
            addResource("UploadFileSuccess");
        }

        public override bool FileExists(string virtualPath)
        {
            var exists = false;
            if (_baseProvider != null)
                exists =_baseProvider.FileExists(virtualPath);

            if (!exists && IsMeekPath(virtualPath))
            {
                if (IsInternalResource(virtualPath))
                    return true;

                var repository = _config.GetRepository();
                exists = repository.Exists(TranslateVirtualPathNoExt(virtualPath));
            }
            return exists;
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            if (_baseProvider != null && _baseProvider.FileExists(virtualPath))
                return _baseProvider.GetFile(virtualPath);

            if (IsMeekPath(virtualPath) && IsInternalResource(virtualPath))
                return GetInternalResource(virtualPath);

            var repository = _config.GetRepository();
            var pathTranslated = TranslateVirtualPathNoExt(virtualPath);
            if (IsMeekPath(virtualPath) && repository.Exists(pathTranslated))
                return new ContentVirtualFile(
                    repository, 
                    virtualPath, 
                    pathTranslated, 
                    _config.ViewEngineOptions);

            return null;
        }

        private static bool IsMeekPath(string virtualPath)
        {
            if (virtualPath.StartsWith("~"))
                virtualPath = virtualPath.Substring(1);

            return virtualPath.StartsWith(VPathPrefix, StringComparison.InvariantCultureIgnoreCase);
        }

        private string TranslateVirtualPathNoExt(string virtualPath)
        {
            return TranslateVirtualPath(virtualPath).Replace(_extension, null);
        }

        private static string TranslateVirtualPath(string virtualPath)
        {
            if (virtualPath.StartsWith("~"))
                virtualPath = virtualPath.Substring(1);

            return Regex.Replace(virtualPath, VPathPrefix, "", RegexOptions.IgnoreCase);
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (_baseProvider != null && _baseProvider.FileExists(virtualPath))
                return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
            
            return new RepositoryCacheDependency(_config.GetRepository(), TranslateVirtualPathNoExt(virtualPath));
        }

        public override string GetFileHash(string virtualPath, IEnumerable virtualPathDependencies)
        {
            if (_baseProvider != null && _baseProvider.FileExists(virtualPath))
                return base.GetFileHash(virtualPath, virtualPathDependencies);

            return Encoding.ASCII.GetString(new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(virtualPath)));
        }

        private bool IsInternalResource(string virtualPath)
        {
            return _internalResources.Any(x => x.Key == TranslateVirtualPath(virtualPath));
        }

        private VirtualFile GetInternalResource(string virtualPath)
        {
            var resource = _internalResources.Single(x => x.Key == TranslateVirtualPath(virtualPath));
            var resourceString =
                Encoding.UTF8.GetString(
                    Assembly.GetExecutingAssembly().GetManifestResourceStream(resource.Value).ReadFully());

            resourceString = resourceString.Replace("{PlaceHolder}", _config.ViewEngineOptions.PlaceHolder);

            return new VirtualFileStream(new MemoryStream(Encoding.UTF8.GetBytes(resourceString)), virtualPath);
        }
    }
}
