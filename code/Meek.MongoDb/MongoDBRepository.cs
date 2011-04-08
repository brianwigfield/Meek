using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Meek.Storage
{
    [SecurityCritical]
    public class MongoDbRepository : Repository
    {

        MongoDatabase _db;

        public MongoDbRepository(string connectionStringName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connectionString == null)
                throw new ArgumentException("Connection does not exist.");

            var server = MongoServer.Create(connectionString.ConnectionString);
            var mongoUri = new Uri(connectionString.ConnectionString);
            _db = server.GetDatabase(mongoUri.AbsolutePath.Substring(1));
        }

        [SecurityCritical]
        public MeekContent Get(string route)
        {
            //To lower to standardize key for uniqueness and ease of querying
            var doc = Content.FindOne(Query.EQ("_id", route.ToLower()));
            if (doc == null)
                return null;

            return new MeekContent(doc["Title"].IsBsonNull ? null : doc["Title"].AsString, doc["Contents"].AsString, doc["Partial"].AsBoolean);
        }

        [SecurityCritical]
        public bool Exists(string route)
        {
            return (Get(route) != null);
        }

        [SecurityCritical]
        public IEnumerable<string> AvailableRoutes(ContentTypes? type)
        {
            IMongoQuery query = Query.Null;

            if (type.HasValue)
            {
                if (type.Value == ContentTypes.Partial)
                    query = Query.EQ("Partial", true);
                else
                    query = Query.EQ("Partial", false);
            }

            return Content.Find(query).Select(x => x["_id"].AsString);
        }

        [SecurityCritical]
        public void Save(string route, MeekContent content)
        {
            //enforce keys to lower for querying
            Content.Save(new { Id = route.ToLower(), content.Title, content.Partial, content.Contents });
        }

        [SecurityCritical]
        public void Remove(string route)
        {
            Content.Remove(Query.EQ("_id", route));
        }

        [SecurityCritical]
        public string SaveFile(MeekFile file)
        {
            var fileId = Guid.NewGuid().ToString();
            //enforce keys to lower for querying
            Files.Save(new { Id = fileId.ToLower(), file.FileName, file.ContentType, file.Contents});
            return fileId;
        }

        [SecurityCritical]
        public MeekFile GetFile(string fileId)
        {
            var doc = Files.FindOne(Query.EQ("_id", fileId.ToLower()));

            return new MeekFile(doc["FileName"].AsString, doc["ContentType"].AsString, doc["Contents"].AsByteArray);
        }

        [SecurityCritical]
        public IEnumerable<string> GetFiles()
        {
            return Files.FindAll().Select(x => x["_id"].AsString);
        }

        [SecurityCritical]
        public void RemoveFile(string fileId)
        {
            Files.Remove(Query.EQ("_id", fileId.ToLower()));
        }

        private MongoCollection<BsonDocument> Files { get { return _db.GetCollection("MeekFile"); } }
        private MongoCollection<BsonDocument> Content { get { return _db.GetCollection("MeekContent"); } }

    }

}
