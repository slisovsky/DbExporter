using System;
using System.Configuration;
using DbExport.Interfaces;

namespace DbExport.Settings
{
    /// <summary>
    /// Реализация настроек экспорта. Читает настройки из конфига приложения
    /// </summary>
    public class AppConfigSettingsProvider: ISettingsProvider
    {
        /// <summary>
        /// Конструктор. Собирает все настройки из конфига.
        /// Если обязательные настройки не указаны(строка подключения к БД,
        /// табица-источник, таблица-приемник) - выбросит исключение.
        /// Кол-во потоков является необязательным параметром, значение по умолчанию - 10
        /// </summary>
        public AppConfigSettingsProvider()
        {
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
        
        /// <inheritdoc />
        public string ConnectionString { get; }

        /// <inheritdoc />
        public int MaxNumberOfThreads { get; }

        /// <inheritdoc />
        public string SourceTable { get; }

        /// <inheritdoc />
        public string DestinationTable { get; }
    }
}
