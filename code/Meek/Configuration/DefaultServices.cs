using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Meek.Storage;

namespace Meek.Configuration
{
    class DefaultServices : Services
    {
        readonly MeekConfigurationSection _config;

        public DefaultServices(MeekConfigurationSection config)
        {
            _config = config;
        }

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
                    _repository = new SQLRepository(_config.Repository.Source);
                    (_repository as SQLRepository).EnsureSchema();
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
