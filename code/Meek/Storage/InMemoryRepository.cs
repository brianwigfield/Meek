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
            Content = new Dictionary<string, MeekContent>();
            Files = new Dictionary<string, MeekFile>();
        }

        public bool Exists(string resource)
        {
            return AvailableRoutes(null).Any(x => x.ToLower() == resource.ToLower());
        }

        public MeekContent Get(string resource)
        {
            return Content.Single(x => x.Key.ToLower() == resource.ToLower()).Value;
        }

        public IEnumerable<string> AvailableRoutes(ContentTypes? type)
        {
            var query = Content.AsQueryable();

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
            var caseSensitiveKey = Content.Select(x => x.Key).FirstOrDefault(x => x.ToLower() == route.ToLower());
            if (!string.IsNullOrEmpty(caseSensitiveKey))
                Content[caseSensitiveKey] = content;
            else
                Content.Add(route, content);

        }

        public void Remove(string route)
        {
            var caseSensitiveKey = Content.Select(x => x.Key).FirstOrDefault(x => x.ToLower() == route.ToLower());
            if (!string.IsNullOrEmpty(caseSensitiveKey))
                Content.Remove(caseSensitiveKey);
        }

        public string SaveFile(MeekFile file)
        {
            var fileId = Guid.NewGuid().ToString();
            Files.Add(fileId, file);
            return fileId;
        }

        public MeekFile GetFile(string fileId)
        {
            return Files.SingleOrDefault(x => x.Key.ToLower() == fileId.ToLower()).Value;
        }

        public IEnumerable<string> GetFiles()
        {
            return Files.Select(x => x.Key);
        }

        private IDictionary<string, MeekFile> Files { get; set; }
        private IDictionary<string, MeekContent> Content { get; set; }
    }
}
