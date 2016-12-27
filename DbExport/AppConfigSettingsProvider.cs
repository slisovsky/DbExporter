using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbExport
{
    public class AppConfigSettingsProvider: ISettingsProvider
    {
        public AppConfigSettingsProvider()
        {
            // тут будем парсить параметры из конфига.
            // если что-то пойдет не так  - бросим исключение
            const int predefinedThreadsCount = 10;
            var errorMsg = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["SourceDBConnection"];
            if (connectionString == null)
            {
                errorMsg = "не найдена строка подключения к БД";
            }
            ConnectionString = connectionString == null ? string.Empty : connectionString.ConnectionString;

            var numberOfThreads = ConfigurationManager.AppSettings["MaxNumberOfThreads"];
            MaxNumberOfThreads = string.IsNullOrEmpty(numberOfThreads) ? predefinedThreadsCount : Convert.ToInt32(numberOfThreads);

            var tableName = ConfigurationManager.AppSettings["SourceTableName"];

            errorMsg += string.IsNullOrEmpty(tableName) ? " не найдено имя таблицы-источника" : string.Empty;
            SourceTable = tableName;

            tableName = ConfigurationManager.AppSettings["DestinationTableName"];
            errorMsg += string.IsNullOrEmpty(tableName) ? " не найдено имя таблицы-назначения" : string.Empty;
            DestinationTable = tableName;

            if (!string.IsNullOrEmpty(errorMsg))
            {
                throw new ArgumentException(string.Format("При чтении параметров из конфига обнаружены ошибки: {0}", errorMsg));
            }
        }
        public string ConnectionString { get; }
        public int MaxNumberOfThreads { get; }
        public string SourceTable { get; }
        public string DestinationTable { get; }
    }
}
