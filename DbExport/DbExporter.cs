using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DbExport.Interfaces;
using NLog;

namespace DbExport
{
    public class DbExporter
    {
        public DbExporter(IDbProvider dbProvider, ISettingsProvider settings, ILogger logger)
        {
            if (dbProvider == null)
            {
                throw new ArgumentNullException(@"Не указан провайдер для работы с БД.");
            }

            if(settings == null)
            {
                throw new ArgumentNullException(@"Не указаны настройки экспорта.");
            }

            if (logger == null)
            {
                throw new ArgumentNullException(@"Не указан логгер.");
            }
            _provider = dbProvider;
            _settings = settings;
            _logger = logger;
        }

        /// <summary>
        /// Экспортирует строки из одной таблицы в другую по "кусочкам"
        /// проверяет существование таблиц (источника и применика). Если какая-либо из них не существует - выдает исключение
        /// </summary>
        public void Export()
        {
            if (!_provider.EnsureTableExists(_settings.SourceTable))
            {
                throw new Exception(string.Format("Таблица {0} не найдена", _settings.SourceTable));
            }
            if (!_provider.EnsureTableExists(_settings.DestinationTable))
            {
                throw new Exception(string.Format("Таблица {0} не найдена", _settings.DestinationTable));
            }
            int threadCount = _settings.MaxNumberOfThreads;
            var count = _provider.GetRowCount(_settings.SourceTable);
            var rowsPerThread = (int)Math.Ceiling((double)count / threadCount);
            var chunks = new List<ChunkDescription>();
            for (var i = 1; i <= 10; i++)
            {
                chunks.Add(new ChunkDescription { MinId = (i - 1) * rowsPerThread + 1, MaxId = i * rowsPerThread });
            }

            _logger.Trace("Начинаем экспорт. Всего строк: {0}, всего потоков: {1}", count, threadCount);
            _logger.Trace("Таблица-источник: {0}, таблица-приемник: {1}", _settings.SourceTable, _settings.DestinationTable);
            Parallel.ForEach(chunks, new ParallelOptions {MaxDegreeOfParallelism = threadCount}, ExportChunk);
            count = _provider.GetRowCount(_settings.DestinationTable);
            _logger.Trace("Закончили экспорт. Всего строк скопировано: {0}", count);
        }

        private void ExportChunk(ChunkDescription chunk)
        {
            _logger.Trace("Начинаем экспорт записей {0} - {1}", chunk.MinId, chunk.MaxId);
            var records = _provider.GetRecords(_settings.SourceTable, chunk.MinId, chunk.MaxId);
            try
            {

                foreach (var item in records)
                {
                    _provider.InsertRecord(_settings.DestinationTable, item.Id, item.FirstName, item.LastName);
                }
            }
            catch (SqlException ex)
            {
                // В данной сиутации ошибка про PK constraint нас не пугает.
                // Это означает, что кто-то другой уже вставил строку,
                // пока merge пытался ее прочитать. а так как по умолчанию уровень изолированности read commited
                // мы вставленную строку увидим "в процессе" мержа.
                // такого можно избежать, наложив holdlock в запросе.
                if (ex.Number != 2627)
                {
                    throw;
                }
            }
            _logger.Trace("Закончили экспорт записей {0} - {1}", chunk.MinId, chunk.MaxId);
        }
        private readonly IDbProvider _provider;
        private readonly ISettingsProvider _settings;
        private readonly ILogger _logger;
    }
}