using System;
using System.Web.Caching;
using Meek.Storage;

namespace Meek
{
    public class RepositoryCacheDependency : CacheDependency
    {
        public RepositoryCacheDependency(Repository repo, string path)
        {
            repo.ContentChanged += (obj, e) =>
                {
                    if (e.Path.ToLower() == path.ToLower())
                        NotifyDependencyChanged(this, EventArgs.Empty);
                };
        }
    }
}