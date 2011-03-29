using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Text;

namespace Meek.Storage
{

    public class SQLRepository : Repository
    {
        readonly DbProviderFactory _factory;
        readonly string _connectString;

        public SQLRepository(string connectionStringName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connectionString == null)
                throw new ArgumentException("Connection does not exist.");

            _factory = DbProviderFactories.GetFactory(connectionString.ProviderName);
            _connectString = connectionString.ConnectionString;
        }

        public MeekContent Get(string route)
        {
            using (var conn = OpenConnection())
            {
                var command = _factory.CreateCommand();
                command.Connection = conn;
                command.CommandText = "SELECT Route, Title, Partial, Data FROM MeekContent WHERE Route = @Route";
                AddParam(command, SqlDbType.NVarChar, route, "Route");

                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                if (!reader.Read())
                    return null;

                string title = null;
                if (reader[1] != DBNull.Value)
                    title = reader.GetString(1);

                return new MeekContent(title, reader.GetString(3), reader.GetBoolean(2));
            }
        }

        public bool Exists(string route)
        {

            using (var conn = OpenConnection())
            {
                var command = _factory.CreateCommand();
                command.Connection = conn;
                command.CommandText = "SELECT Route FROM MeekContent WHERE Route = @Route";
                AddParam(command, SqlDbType.NVarChar, route, "Route");
                
                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                return reader.Read();
            }

        }

        public IEnumerable<string> AvailableRoutes(ContentTypes? type)
        {
            using (var conn = OpenConnection())
            {
                var command = _factory.CreateCommand();
                command.Connection = conn;
                command.CommandText = "SELECT Route FROM MeekContent";
                if (type.HasValue)
                {
                    command.CommandText += " WHERE Partial = @Partial";
                    AddParam(command, SqlDbType.Bit, type.Value == ContentTypes.Full ? false : true, "Partial");
                }

                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                var results = new List<string>();
                while (reader.Read())
                {
                    results.Add(reader.GetString(0));
                }
                return results;
            }
        }

        public void Save(string route, MeekContent content)
        {

            string commandText;
            if (Exists(route))
                commandText =
                    "UPDATE MeekContent SET Title = @Title, Partial = @Partial, Data = @Data WHERE Route = @Route";
            else
                commandText =
                    "INSERT INTO MeekContent (Route, Title, Partial, Data) Values (@Route, @Title, @Partial, @Data)";

            using (var conn = OpenConnection())
            {
                var command = _factory.CreateCommand();
                command.Connection = conn;
                command.CommandText = commandText;
                AddParam(command, SqlDbType.NVarChar, route, "Route");

                if (string.IsNullOrEmpty(content.Title))
                    AddParam(command, SqlDbType.NVarChar, DBNull.Value, "Title");
                else
                    AddParam(command, SqlDbType.NVarChar, content.Title, "Title");

                AddParam(command, SqlDbType.Bit, content.Partial, "Partial");
                AddParam(command, SqlDbType.NVarChar, new UTF8Encoding().GetString(content.Contents), "Data");

                command.ExecuteNonQuery();
            }

        }

        public void Remove(string route)
        {
            using (var conn = OpenConnection())
            {
                var command = _factory.CreateCommand();
                command.Connection = conn; 
                command.CommandText = "DELETE FROM MeekContent WHERE Route = @Route";
                AddParam(command, SqlDbType.NVarChar, route, "Route");
                command.ExecuteNonQuery();
            }
        }

        public string SaveFile(MeekFile file)
        {

            using (var conn = OpenConnection())
            {
                var fileId = Guid.NewGuid().ToString();
                var command = _factory.CreateCommand();
                command.Connection = conn;
                command.CommandText = "INSERT INTO MeekFile (Id, FileName, ContentType, Data) Values (@Id, @FileName, @ContentType, @Data)"; ;
                AddParam(command, SqlDbType.NVarChar, fileId, "Id");
                AddParam(command, SqlDbType.NVarChar, file.FileName, "FileName");
                AddParam(command, SqlDbType.NVarChar, file.ContentType, "ContentType");
                AddParam(command, SqlDbType.Image, file.Contents, "Data");

                command.ExecuteNonQuery();
                return fileId;
            }

        }

        public MeekFile GetFile(string fileId)
        {
            using (var conn = OpenConnection())
            {
                var command = _factory.CreateCommand();
                command.Connection = conn;
                command.CommandText = "SELECT FileName, ContentType, Data FROM MeekFile WHERE Id = @Id";
                AddParam(command, SqlDbType.NVarChar, fileId, "Id");

                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                if (!reader.Read())
                    return null;

                return new MeekFile(fileId, reader.GetString(0), reader.GetString(1), (byte[])reader.GetValue(2));
               
            }
        }

        public IEnumerable<string> GetFiles()
        {
            using (var conn = OpenConnection())
            {
                var command = _factory.CreateCommand();
                command.Connection = conn;
                command.CommandText = "SELECT Id FROM MeekFile";

                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                while (reader.Read())
                {
                    yield return reader.GetString(0);
                }

            }
        }

        public void RemoveFile(string fileId)
        {
            using (var conn = OpenConnection())
            {
                var command = _factory.CreateCommand();
                command.Connection = conn;
                command.CommandText = "DELETE FROM MeekFile WHERE Id = @Id";
                AddParam(command, SqlDbType.NVarChar, fileId, "Id");
                command.ExecuteNonQuery();
            }
        }

        private DbConnection OpenConnection()
        {
            var conn = _factory.CreateConnection();
            conn.ConnectionString = _connectString;
            conn.Open();
            return conn;
        }

        private void AddParam(DbCommand forCommand, SqlDbType type, object value, string name)
        {
            var param = forCommand.CreateParameter();
            if (param is SqlParameter)
            {
                ((SqlParameter)param).SqlDbType = type;
            }
            else if (param is SqlCeParameter)
            {
                ((SqlCeParameter)param).SqlDbType = type;
            }
            param.Value = value;
            param.ParameterName = "@" + name;
            forCommand.Parameters.Add(param);
        }

    }

}
