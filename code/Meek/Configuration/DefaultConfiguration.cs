using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Meek.Storage;

namespace Meek.Configuration
{
    public class DefaultConfiguration : Configuration
    {

        readonly MeekConfigurationSection _config;

        public DefaultConfiguration(MeekConfigurationSection config)
        {
            _config = config;

            AltManageContentRoute = string.IsNullOrWhiteSpace(_config.AltManageContentRoute) ? "Missing" : _config.AltManageContentRoute;
            CkEditorPath = string.IsNullOrWhiteSpace(_config.CkEditorPath) ? "/Meek/ckeditor" : _config.CkEditorPath;
            NotFoundView = config.NotFoundView;
            ViewEngineOptions = new ViewEngineOptions
                                    {
                                        Type = string.IsNullOrWhiteSpace(_config.ViewEngine) ? ViewEngineType.Razor : (ViewEngineType)Enum.Parse(typeof(ViewEngineType), _config.ViewEngine, true),
                                        IncludeFormTag = true
                                    };
            if (ViewEngineOptions.Type == ViewEngineType.ASPX)
            {
                ViewEngineOptions.Layout = _config.AspxConfig.MasterPage;
                ViewEngineOptions.PlaceHolder = _config.AspxConfig.ContentPlaceHolderId;
                ViewEngineOptions.IncludeFormTag = _config.AspxConfig.IncludeFormTag;
            }
        }

        public string CkEditorPath { get; set; }
        public string AltManageContentRoute { get; set; }
        public string NotFoundView { get; set; }
        public ViewEngineOptions ViewEngineOptions { get; set; }

        public Repository GetRepository()
        {
            return DependencyResolver.Current.GetService<Repository>() ?? RepositoryFactory();
        }

        public Authorization GetAuthorization()
        {
            var auth = DependencyResolver.Current.GetService<Authorization>();
            if (auth != null)
                return auth;

            Func<HttpContextBase, bool> isContentAdmin = x => true;
            if (_config.ContentAdmin != null && _config.ContentAdmin.Count > 0)
                isContentAdmin = x => _config.ContentAdmin.OfType<MeekConfigurationSection.RoleElement>().Any(s => x.User.IsInRole(s.Role));

            return new BasicAuthorization(isContentAdmin);
        }

        public ImageResizer GetImageResizer()
        {
            return DependencyResolver.Current.GetService<ImageResizer>() ?? new DefaultImageResizer();
        }

        private Repository RepositoryFactory()
        {
            if (_repository == null)
            {
                if (_config.Repository.Type == "Sql")
                {
                    _repository = new SqlRepository(_config.Repository.Source);
                    (_repository as SqlRepository).EnsureSchema();
                }

                if (_config.Repository.Type == "InMemory")
                    _repository = new InMemoryRepository();

                if (_config.Repository.Type == "FileSystem")
                    _repository = new FileSystemRepository(Path.Combine(HttpRuntime.AppDomainAppPath, _config.Repository.Source));

            }

            return _repository;
        }

        Repository _repository;
    }
}