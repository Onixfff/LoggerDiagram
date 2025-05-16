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
