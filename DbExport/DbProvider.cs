using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using DbExport.Interfaces;

namespace DbExport
{
    public class DbProvider : IDbProvider
    {
        /// <summary>
        /// Конструктор, бросает исключение, если settings == null
        /// </summary>
        /// <param name="settings">Настройки экспорта</param>
        public DbProvider(ISettingsProvider settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("Не указан провайдер настроек экспорта.");
            }
            _connectionString = settings.ConnectionString;
        }

        /// <inheritdoc />
        public bool EnsureTableExists(string name)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = string.Format("select object_id(N'{0}')", name);
                var objectId = connection.ExecuteScalar<int?>(query);
                return objectId.HasValue;
            }
        }

        /// <inheritdoc />
        public int GetRowCount(string tableName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = string.Format("select count(*) from {0}", tableName);
                var count = connection.ExecuteScalar<int>(query);
                return count;
            }
        }

        /// <inheritdoc />
        public void InsertRecord(string tableName, int id, string firstName, string lastName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = string.Format("merge into {0} as trg  " +
                                            "using (select @id as id, @firstName as FirstName, @lastName as LastName) as src on trg.ID = src.ID " +
                                            " when not matched then insert values(src.ID, src.FirstName, src.LastName);", tableName);
                connection.Execute(query, new { id, firstName, lastName });
            }
        }

        /// <inheritdoc />
        public List<Record> GetRecords(string tableName, int minId, int maxId)
        {
            var records = new List<Record>();
            var query = string.Format("select s.ID, s.FirstName, s.LastName from {0} as s " +
                                      "where s.ID between @minId and @maxId", tableName);
            using (var connection = new SqlConnection(_connectionString))
            {
                records = connection.Query<Record>(query, new { minId, maxId }).ToList();
            }
            return records;
        }

        private readonly string _connectionString;
    }
}