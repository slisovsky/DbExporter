namespace DbExport.Interfaces
{
    /// <summary>
    /// Настройки экспорта
    /// </summary>
    public interface ISettingsProvider
    {
        /// <summary>
        /// Строка подключения к базе данных
        /// </summary>
        string ConnectionString { get; }
        /// <summary>
        /// максимальное количество потоков для экспорта
        /// </summary>
        int MaxNumberOfThreads { get; }
        /// <summary>
        /// Табилца-источник данных
        /// </summary>
        string SourceTable { get; }
        /// <summary>
        /// Таблица-приемник данных
        /// </summary>
        string DestinationTable { get; }

    }
}
