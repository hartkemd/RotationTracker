using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Databases
{
    public class SqliteDataAccess : ISqliteDataAccess
    {
        private readonly IConfiguration _config;

        public SqliteDataAccess(IConfiguration config)
        {
            _config = config;
        }

        public async Task<List<T>> LoadDataAsync<T, U>(string sqlStatement,
                                      U parameters,
                                      string connectionStringName)
        {
            string connectionString = _config.GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SQLiteConnection(connectionString))
            {
                var results = await connection.QueryAsync<T>(sqlStatement, parameters);
                List<T> rows = results.ToList();
                return rows;
            }
        }

        public async Task SaveDataAsync<T>(string sqlStatement,
                                T parameters,
                                string connectionStringName)
        {
            string connectionString = _config.GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SQLiteConnection(connectionString))
            {
                await connection.ExecuteAsync(sqlStatement, parameters);
            }
        }
    }
}
