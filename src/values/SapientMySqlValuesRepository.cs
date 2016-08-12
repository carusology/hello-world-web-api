using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Configuration;

using Dapper;
using MySql.Data.MySqlClient;

using hwwebapi.Core;

namespace hwwebapi.Values {

    // Using .NET Core 'Sapient' port of MySQL C# Connection & Dapper ODM

    // https://www.nuget.org/packages/SapientGuardian.MySql.Data
    // https://github.com/SapientGuardian/mysql-connector-net-netstandard
    // http://outbreaklabs.com/v2/post/new-release-and-location-for-net-core-port-of-mysqlconnector-net

    public class SapientMySqlValuesRepository : IRepository<int, string> {

        private readonly string connectionString;

        public SapientMySqlValuesRepository(string connectionString) {
            this.connectionString = connectionString;
        }

        public IEnumerable<string> GetAll() {
            using (var connection = this.GetConnection()) {
                const string sql = "select value from testvalues";
                var rows = connection.Query(sql);

                foreach (var row in rows) {
                    yield return (string)row.value;
                }

                connection.Close();
            }
        }

        public bool TryGet(int id, out string value) {
            using (var connection = this.GetConnection()) {
                const string sql = "select value from testvalues where id = @id";
                var row = connection.Query(sql, new { id = id }).SingleOrDefault();

                if (row == null) {
                    value = null;
                } else {
                    value = (string)row.value;
                }

                connection.Close();
            }

            return value != null;
        }

        public int Create(string value) {
            int id;

            using (var connection = this.GetConnection()) {
                const string sql =
                    "insert into testvalues (value) values (@value);" +
                    "select last_insert_id() as id";

                var row = connection.Query(sql, new { value = value }).Single();
                id = (int)row.id;

                connection.Close();
            }

            return id;
        }

        public bool TryUpdate(int id, string value) {
            bool updated;

            using (var connection = this.GetConnection()) {
                const string sql = "update testvalues set value = @value where id = @id";

                updated = connection.Execute(sql, new { id = id, value = value }) > 0;

                connection.Close();
            }

            return updated;
        }

        public bool Delete(int id) {
            bool deleted;

            using (var connection = this.GetConnection()) {
                const string sql = "delete from testvalues where id = @id";

                deleted = connection.Execute(sql, new { id = id }) > 0;

                connection.Close();
            }

            return deleted;
        }

        private IDbConnection GetConnection() {
            var connection = new MySqlConnection();

            connection.ConnectionString = connectionString;
            connection.Open();

            return connection;
        }

    }

}
