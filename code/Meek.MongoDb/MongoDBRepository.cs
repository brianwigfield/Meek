using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Meek.Storage
{
    public class MongoDbRepository : Repository
    {
        MongoDatabase _db;
        public event EventHandler<ResourceChangedArgs> FileChanged;
        public event EventHandler<ResourceChangedArgs> ContentChanged;

        public MongoDbRepository(string connectionStringName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connectionString == null)
                throw new ArgumentException("Connection does not exist.");

            var server = MongoServer.Create(connectionString.ConnectionString);
            var mongoUri = new Uri(connectionString.ConnectionString);
            _db = server.GetDatabase(mongoUri.AbsolutePath.Substring(1));
        }

        public MeekContent Get(string route)
        {
            //To lower to standardize key for uniqueness and ease of querying
            var doc = Content.FindOne(Query.EQ("_id", route.ToLower()));
            if (doc == null)
                return null;

            return new MeekContent(doc["Title"].IsBsonNull ? null : doc["Title"].AsString, doc["Contents"].AsString, doc["Partial"].AsBoolean);
        }

        public bool Exists(string route)
        {
            return (Get(route) != null);
        }

        //[SecurityCritical]
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

        public void Save(string route, MeekContent content)
        {
            //enforce keys to lower for querying
            Content.Save(new { Id = route.ToLower(), content.Title, content.Partial, content.Contents });

            if (ContentChanged != null)
                ContentChanged(this, new ResourceChangedArgs { Path = route });
        }

        public void Remove(string route)
        {
            Content.Remove(Query.EQ("_id", route));

            if (ContentChanged != null)
                ContentChanged(this, new ResourceChangedArgs { Path = route });
        }

        public string SaveFile(MeekFile file)
        {
            var fileId = Guid.NewGuid().ToString();
            //enforce keys to lower for querying
            Files.Save(new { Id = fileId.ToLower(), file.FileName, file.ContentType, file.Contents});

            if (FileChanged != null)
                FileChanged(this, new ResourceChangedArgs { Path = fileId });

            return fileId;
        }

        public MeekFile GetFile(string fileId)
        {
            var doc = Files.FindOne(Query.EQ("_id", fileId.ToLower()));

            return new MeekFile(doc["FileName"].AsString, doc["ContentType"].AsString, doc["Contents"].AsByteArray);
        }

        public IDictionary<string, string> GetFiles()
        {
            return Files.FindAll().ToDictionary(_ => _["_id"].AsString, _ => _["FileName"].AsString);
        }

        public void RemoveFile(string fileId)
        {
            Files.Remove(Query.EQ("_id", fileId.ToLower()));
            
            if (FileChanged != null)
                FileChanged(this, new ResourceChangedArgs { Path = fileId });
        }

        private MongoCollection<BsonDocument> Files { get { return _db.GetCollection("MeekFile"); } }
        private MongoCollection<BsonDocument> Content { get { return _db.GetCollection("MeekContent"); } }

    }

}
