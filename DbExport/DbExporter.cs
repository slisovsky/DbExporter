using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using NLog.Internal;

namespace DbExport
{
    public class DbExporter
    {
        public DbExporter(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool EnsureTableExists(string name)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = string.Format("select object_id(N'{0}')", name);
                var objectId = connection.ExecuteScalar<int?>(query);
                return objectId.HasValue;
            }
        }

        public int GetRowCount(string tableName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = string.Format("select count(*) from {0}", tableName);
                var count = connection.ExecuteScalar<int>(query);
                return count;
            }
        }

        public void InsertRecord(string tableName, int id, string firstName, string lastName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "merge into dbo.Destination as trg " +
                            "using (select @id as id, @firstName as FirstName, @lastName as LastName) as src on trg.ID = src.ID " +
                            " when not matched then insert values(src.ID, src.FirstName, src.LastName);";
                connection.Execute(query, new { id, firstName, lastName});
            }
        }

        public void Export(List<ChunkDescription> chunks, int threadCount)
        {
            Parallel.ForEach(chunks, new ParallelOptions {MaxDegreeOfParallelism = threadCount}, ExportChunk);
        }

        private void ExportChunk(ChunkDescription chunk)
        {
            var records = new List<Record>();
            var query = "select s.ID, s.FirstName, s.LastName from dbo.Source as s " +
                        "where s.ID between @minId and @maxId";
            using (var connection = new SqlConnection(_connectionString))
            {
                records = connection.Query<Record>(query, new {chunk.MinId, chunk.MaxId}).ToList();
            }
            foreach (var item in records)
            {
                InsertRecord("dbo.Destination", item.Id, item.FirstName, item.LastName);
            }
        }
        private string _connectionString;
    }
}