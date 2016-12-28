using System.Collections.Generic;

namespace DbExport.Interfaces
{
    /// <summary>
    /// Интерфейс доступа к БД
    /// </summary>
    public interface IDbProvider
    {
        /// <summary>
        /// Убедится в существовании таблицы
        /// </summary>
        /// <param name="name">Наименование таблицы, существование которой проверяется</param>
        /// <returns>Если таблицы существует - true, если нет(либо к ней отсутствует доступ) - false</returns>
        bool EnsureTableExists(string name);

        /// <summary>
        /// Получить кол-во строк в таблице по имени
        /// </summary>
        /// <param name="tableName">Наименование таблицы</param>
        /// <returns>Кол-во записей в таблице</returns>
        int GetRowCount(string tableName);

        /// <summary>
        /// Добавить строку в таблицу
        /// </summary>
        /// <param name="tableName">Наименование таблицы</param>
        /// <param name="id">идентификатор записи</param>
        /// <param name="firstName">Имя</param>
        /// <param name="lastName">Фамилия</param>
        void InsertRecord(string tableName, int id, string firstName, string lastName);

        /// <summary>
        /// Получить список записей из таблицы по имени.
        /// использует выражение BETWEEN, чтобы выбрать нужные записи
        /// </summary>
        /// <param name="tableName">Наименование таблицы</param>
        /// <param name="minId">минимальный Id, с которого начинаются интересующие нас записи</param>
        /// <param name="maxId">максимальный Id, которым заканчиваются интересующие нас записи</param>
        /// <returns></returns>
        List<Record> GetRecords(string tableName, int minId, int maxId);

    }
}
