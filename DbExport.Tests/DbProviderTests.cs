using System.Data.SqlClient;
using System.Linq;
using Dapper;
using DbExport.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbExport.Tests
{
    [TestClass]
    public class DbProviderTests
    {
        private ISettingsProvider _settings;

        [TestInitialize]
        public void InitTest()
        {
            _settings = new TestSettings();
            using (var connection = new SqlConnection(_settings.ConnectionString))
            {
                connection.Execute(
                    "create table dbo.UnitTest(ID int not null, FirstName varchar(30) null, LastName varchar(30) null)");
            }

        }

        [TestCleanup]
        public void CleanUp()
        {
            using (var connection = new SqlConnection(_settings.ConnectionString))
            {
                connection.Execute("drop table dbo.UnitTest");
            }

        }

        [TestMethod]
        public void EnsureTableExistsOnExistingTable()
        {
            var provider = new DbProvider(_settings);
            var sourceTableExists = provider.EnsureTableExists(_settings.SourceTable);
            Assert.IsTrue(sourceTableExists);
        }

        [TestMethod]
        public void EnsureTableExistsOnNotExistingTable()
        {
            var provider = new DbProvider(_settings);
            var sourceTableExists = provider.EnsureTableExists("SomeNotExistingTable");
            Assert.IsFalse(sourceTableExists);
        }

        [TestMethod]
        public void GetRowCountOnExistingTable()
        {
            var provider = new DbProvider(_settings);
            var rowCount = provider.GetRowCount("dbo.UnitTest");
            Assert.AreEqual(0, rowCount);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void GetRowCountOnNotExistingTable()
        {
            var provider = new DbProvider(_settings);
            var rowCount = provider.GetRowCount("dbo.TableNotExists");
        }

        [TestMethod]
        public void GetRecordListOnExistingTable()
        {
            using (var connection = new SqlConnection(_settings.ConnectionString))
            {

                connection.Execute("insert into dbo.UnitTest(id, firstName, LastName) values(1,'FirstName', 'LastName')");
            }

            var provider = new DbProvider(_settings);
            var list = provider.GetRecords("dbo.UnitTest", 1, 10);
            Assert.AreEqual(1, list.Count);
            var item = list[0];
            Assert.AreEqual(1, item.Id);
            Assert.AreEqual("FirstName", item.FirstName);
            Assert.AreEqual("LastName", item.LastName);

        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void GetRecordListOnNotExistingTable()
        {
            var provider = new DbProvider(_settings);
            var lst = provider.GetRecords("dbo.TableNotExists", 1, 10);
        }

        [TestMethod]
        public void InsertRecordOnExistingTable()
        {
            var provider = new DbProvider(_settings);
            provider.InsertRecord("dbo.UnitTest", 1, "MyName", "MySecondName");
            using (var connection = new SqlConnection(_settings.ConnectionString))
            {
                var record = connection.Query<Record>("select ut.ID, ut.FirstName, ut.LastName from dbo.UnitTest as ut where ID = 1").ToList();
                Assert.AreEqual(1, record.Count);
                Assert.AreEqual(1, record[0].Id);
                Assert.AreEqual("MyName", record[0].FirstName);
                Assert.AreEqual("MySecondName", record[0].LastName);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void InsertRecordOnNotExistingTable()
        {
            var provider = new DbProvider(_settings);
            provider.InsertRecord("dbo.NotExistingRecord", 1, "Name1", "Name2");
        }
        private class TestSettings : ISettingsProvider
        {
            public string ConnectionString {
                get
                {
                    return
                        "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Sample Task;Integrated Security=True;";
                }
            }
            public int MaxNumberOfThreads {
                get { return 10; }
            }
            public string SourceTable {
                get { return "dbo.Source"; }
            }

            public string DestinationTable
            {
                get { return "dbo.Destination"; }
            }
        }
    }
}
