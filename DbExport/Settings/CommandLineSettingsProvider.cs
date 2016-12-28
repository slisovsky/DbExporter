using System;
using System.Collections.Generic;
using System.Linq;
using DbExport.Interfaces;
using Fclp;

namespace DbExport.Settings
{
    /// <summary>
    /// Реализация настроек для чтения их из параметров командной строки приложения
    /// </summary>
    public class CommandLineSettingsProvider: ISettingsProvider
    {
        /// <summary>
        /// Конструктор. Получает список параметров командной строки и инициализирует соответсвующие поля.
        /// </summary>
        /// <param name="args">Список параметров командной строки со значениями в формате /paramName paramValue</param>
        public CommandLineSettingsProvider(List<string> args)
        {
            var parser = new FluentCommandLineParser<Settings>();
            parser.Setup(argument => argument.ConnectionString)
                .As("ConnectionString")
                .WithDescription("Строка подключения к БД")
                .Required();

            parser.Setup(argument => argument.NumberOfThreads)
                .As("NumberOfThreads")
                .WithDescription("Максимальное кол-во потоков для работы")
                .SetDefault(10);

            parser.Setup(argument => argument.SourceTable)
                .As("SourceTableName")
                .WithDescription("Наименование таблицы-источника")
                .Required();

            parser.Setup(argument => argument.SourceTable)
                .As("DestinationTableName")
                .WithDescription("Наименование таблицы-назначения")
                .Required();

            var result = parser.Parse(args.ToArray());
            if (result.HasErrors)
            {
                var errorText = "Ошибка обработки аргументов командной строки: " + string.Join(", ",
                    result.Errors.Select(error => error.Option.LongName + "(" + error.Option.Description + ")")
                    );
                throw new ArgumentException(errorText);
            }
            _settings = parser.Object;
        }

        /// <inheritdoc />
        public string ConnectionString
        {
            get
            {
                return _settings.ConnectionString;
            }
        }

        /// <inheritdoc />
        public int MaxNumberOfThreads
        {
            get
            {
                return _settings.NumberOfThreads;
            }
        }

        /// <inheritdoc />
        public string SourceTable
        {
            get
            {
                return _settings.SourceTable;
            }
        }

        /// <inheritdoc />
        public string DestinationTable
        {
            get
            {
                return _settings.DestinationTable;
            }
        }

        private readonly Settings _settings;
        /// <summary>
        /// Сюда FluentCommandLineParser будет писать значения параметров.
        /// </summary>
        private class Settings
        {
            public string ConnectionString { get; set; }
            public int NumberOfThreads { get; set; }
            public string SourceTable { get; set; }
            public string DestinationTable { get; set; }

        }
    }
}
