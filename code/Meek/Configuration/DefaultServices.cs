using System;
using System.Collections.Generic;
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
            if (_config.ContentAdmin != null)
                isContentAdmin = x => _config.ContentAdmin.OfType<MeekConfigurationSection.RoleElement>().Any(s => x.User.IsInRole(s.Role));

            return new BasicAuthorization(isContentAdmin);
        }

        private Repository RepositoryFactory()
        {
            if (_config.Repository.Type == "Sql")
                return new SQLRepository(_config.Repository.Source);

            if (_config.Repository.Type == "InMemory")
                return new InMemoryRepository();

            return null;
        }

    }
}
