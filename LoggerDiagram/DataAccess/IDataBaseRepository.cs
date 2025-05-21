using LoggerDiagram.DTO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram.DataAccess
{
    public interface IDataBaseRepository
    {
        Task SendDataAsync(PlcLogEntityDto plcLogEntryDto, CancellationToken token);

        /// <summary>
        /// Асинхронно выполняет массовую вставку данных из коллекции <see cref="PlcLogEntityDto"/> в таблицу MySQL.
        /// Вставка выполняется с помощью одного SQL-запроса типа INSERT INTO ... VALUES (...), (...).
        /// </summary>
        /// <param name="dtos">Коллекция объектов <see cref="PlcLogEntityDto"/>, которые будут сохранены в базе данных.</param>
        /// <param name="token">Токен отмены, позволяющий прервать операцию.</param>
        /// <exception cref="ArgumentNullException">Выбрасывается, если <paramref name="dtos"/> равен null.</exception>
        /// <exception cref="OperationCanceledException">Выбрасывается, если операция была отменена через <paramref name="token"/>.</exception>
        /// <exception cref="InsertException">Выбрасывается при ошибках взаимодействия с базой данных MySQL. 
        /// И при других ошибках работы с базой данных.</exception>
        /// <exception cref="Exception">Выбрасывается при прочих непредвиденных ошибках.</exception>
        /// <remarks>
        /// Метод формирует SQL-запрос путём объединения всех записей в одну строку и отправляет их одним вызовом.
        /// Для повышения производительности рекомендуется использовать не более 500–1000 записей за один вызов,
        /// чтобы избежать переполнения строки или ограничений сервера.
        /// </remarks>
        Task BulkInsertWithValuesAsync(IEnumerable<PlcLogEntityDto> dtos, CancellationToken token);

        /// <summary>
        /// Асинхронно получает последнее значение номера партии (BatchNumber) для указанного графика из базы данных.
        /// Если данные отсутствуют, возвращается 0.
        /// </summary>
        /// <param name="idGraph">Идентификатор графика, для которого запрашивается последний BatchNumber.</param>
        /// <param name="token">Токен отмены для прерывания операции.</param>
        /// <returns>Последнее значение BatchNumber или 0, если записей нет.</returns>
        /// <exception cref="OperationCanceledException">Возникает, если операция была отменена через CancellationToken.</exception>
        /// <exception cref="MySqlException">Возникает при ошибках взаимодействия с базой данных.</exception>
        /// <exception cref="InvalidCastException">Возникает, если значение MaxBatchNumber из БД не может быть преобразовано в int.</exception>
        /// <exception cref="Exception">Непредвиденные ошибки во время выполнения запроса.</exception>
        Task<int> GetLastBatchNumberByGraphAsync(int idGraph, CancellationToken token);

        Task<List<int>> GetAllGraphIdsAsync(CancellationToken token);
    }
}
