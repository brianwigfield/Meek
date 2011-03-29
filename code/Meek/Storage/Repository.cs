using System.Collections.Generic;
using System.IO;

namespace Meek.Storage
{
    public interface Repository
    {
        MeekContent Get(string route);
        bool Exists(string route);
        IEnumerable<string> AvailableRoutes(ContentTypes? type);
        void Save(string route, MeekContent content);
        void Remove(string route);
        string SaveFile(MeekFile file);
        MeekFile GetFile(string fileID);
        IEnumerable<string> GetFiles();
    }
}
