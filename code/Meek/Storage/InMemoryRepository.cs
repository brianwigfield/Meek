using System;
using System.Collections.Generic;
using System.Linq;

namespace Meek.Storage
{
    public class InMemoryRepository : Repository
    {
        public event EventHandler<ResourceChangedArgs> FileChanged;
        public event EventHandler<ResourceChangedArgs> ContentChanged;

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

            if (ContentChanged != null)
                ContentChanged(this, new ResourceChangedArgs { Path = route });
        }

        public void Remove(string route)
        {
            var caseSensitiveKey = Content.Select(x => x.Key).FirstOrDefault(x => x.ToLower() == route.ToLower());
            if (!string.IsNullOrEmpty(caseSensitiveKey))
                Content.Remove(caseSensitiveKey);

            if (ContentChanged != null)
                ContentChanged(this, new ResourceChangedArgs { Path = route });
        }

        public string SaveFile(MeekFile file)
        {
            var fileId = Guid.NewGuid().ToString();
            Files.Add(fileId, file);

            if (FileChanged != null)
                FileChanged(this, new ResourceChangedArgs {Path = fileId});

            return fileId;
        }

        public MeekFile GetFile(string fileId)
        {
            return Files.SingleOrDefault(x => x.Key.ToLower() == fileId.ToLower()).Value;
        }

        public IDictionary<string,string> GetFiles()
        {
            return Files.ToDictionary(_ => _.Key, _ => _.Value.FileName);
        }

        public void RemoveFile(string fileId)
        {
            Files.Remove(fileId);
            
            if (FileChanged != null)
                FileChanged(this, new ResourceChangedArgs { Path = fileId });
        }

        private IDictionary<string, MeekFile> Files { get; set; }
        private IDictionary<string, MeekContent> Content { get; set; }
    }
}
