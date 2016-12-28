namespace DbExport
{
    /// <summary>
    /// Описание "кусочка" данных для экспорта.
    /// </summary>
    public class ChunkDescription
    {
        /// <summary>
        /// Начиная с этого значения Id мы будем экспортировать строки
        /// </summary>
        public int MinId { get; set; }
        /// <summary>
        /// Верхняя граница Id для экспорта
        /// </summary>
        public int MaxId { get; set; }

    }
}