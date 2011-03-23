using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meek.Storage
{
    public class InMemoryRepository : Repository
    {
        public InMemoryRepository()
        {
            Routes = new Dictionary<string, MeekContent>();
        }

        public bool Exists(string resource)
        {
            return AvailableRoutes(null).Any(x => x.ToLower() == resource.ToLower());
        }

        public MeekContent Get(string resource)
        {
            return Routes.Single(x => x.Key.ToLower() == resource.ToLower()).Value;
        }

        public IEnumerable<string> AvailableRoutes(ContentTypes? type)
        {
            var query = Routes.AsQueryable();

            if (type.HasValue)
            {
                if (type.Value == ContentTypes.Partial)
                    query = query.Where(x => x.Value.Partial);
                else
                    query = query.Where(x => x.Value.Partial == false);
            }

            return query.Select(x => x.Key);

        }

        public void Save(string route, MeekContent content)
        {
            var caseSensitiveKey = Routes.Select(x => x.Key).FirstOrDefault(x => x.ToLower() == route.ToLower());
            if (!string.IsNullOrEmpty(caseSensitiveKey))
                Routes[caseSensitiveKey] = content;
            else
                Routes.Add(route, content);

        }

        public void Remove(string route)
        {
            var caseSensitiveKey = Routes.Select(x => x.Key).FirstOrDefault(x => x.ToLower() == route.ToLower());
            if (!string.IsNullOrEmpty(caseSensitiveKey))
                Routes.Remove(caseSensitiveKey);

        }


        private IDictionary<string, MeekContent> Routes { get; set; }
    }
}
