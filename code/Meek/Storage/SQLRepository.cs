using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
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
                AddParam(command, DbType.String, route, "Route");

                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                if (!reader.Read())
                    return null;

                return new MeekContent(reader.GetString(1), reader.GetString(3), reader.GetBoolean(2));
            }
        }

        public bool Exists(string route)
        {

            using (var conn = OpenConnection())
            {
                var command = _factory.CreateCommand();
                command.Connection = conn;
                command.CommandText = "SELECT Route FROM MeekContent WHERE Route = @Route";
                AddParam(command, DbType.String, route, "Route");
                
                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                return reader.Read();
            }

        }

        private DbConnection OpenConnection()
        {
            var conn = _factory.CreateConnection();
            conn.ConnectionString = _connectString;
            conn.Open();
            return conn;
        }

        private void AddParam(DbCommand forCommand, DbType type, object value, string name)
        {
            var param = forCommand.CreateParameter();
            param.DbType = DbType.String;
            param.Value = value;
            param.ParameterName = "@" + name;
            forCommand.Parameters.Add(param);
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
                    AddParam(command, DbType.Boolean, type.Value == ContentTypes.Full ? false : true, "Partial");
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
                AddParam(command, DbType.String, route, "Route");
                AddParam(command, DbType.String, content.Title, "Title");
                AddParam(command, DbType.Boolean, content.Partial, "Partial");
                AddParam(command, DbType.String, new UTF8Encoding().GetString(content.Contents), "Data");

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
                AddParam(command, DbType.String, route, "Route");
                command.ExecuteNonQuery();
            }
        }
    }

}
