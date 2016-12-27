using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fclp;

namespace DbExport
{
    public class CommandLineSettingsProvider: ISettingsProvider
    {
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

        public string ConnectionString
        {
            get
            {
                return _settings.ConnectionString;
            }
        }

        public int MaxNumberOfThreads
        {
            get
            {
                return _settings.NumberOfThreads;
            }
        }

        public string SourceTable
        {
            get
            {
                return _settings.SourceTable;
            }
        }

        public string DestinationTable
        {
            get
            {
                return _settings.DestinationTable;
            }
        }

        private readonly Settings _settings;
        private class Settings
        {
            public string ConnectionString { get; set; }
            public int NumberOfThreads { get; set; }
            public string SourceTable { get; set; }
            public string DestinationTable { get; set; }

        }
    }
}
