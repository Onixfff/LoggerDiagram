using NLog;
using System;
using System.Text;
using System.Threading;
using LoggerDiagram.DTO;
using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using LoggerDiagram.DataBaseExcepitons;

namespace LoggerDiagram.DataAccess
{
    public class DataBaseRepository : IDataBaseRepository
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;

        public DataBaseRepository(string connectionString, ILogger logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

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
        public async Task BulkInsertWithValuesAsync(IEnumerable<PlcLogEntityDto> dtos, CancellationToken token)
        {
            var sb = new StringBuilder();
            sb.Append(@"INSERT INTO `diagramrooms`.`datapoints`
                        (`IdGraph`, `BatchNumber`, `NowTime`, `Value`, `Time`)
                        VALUES ");

            if (dtos == null) throw new ArgumentNullException(nameof(dtos));

            int count = 0;
            bool first = true;
            foreach (var dto in dtos)
            {
                if (!first) sb.Append(",");

                sb.AppendFormat(
                    "({0}, {1}, {2}, {3}, {4})",
                    dto.IdGraph,
                    dto.BatchNumber,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    dto.Value,
                    dto.Time
                    );

                count++;
                first = false;
            }

            if (first) return; //Нет данных

            string sql = sb.ToString();

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    token.ThrowIfCancellationRequested();

                    await connection.OpenAsync(token).ConfigureAwait(false);
                    
                    using (var command = new MySqlCommand(sql, connection)) 
                    {
                        int rowsAffected = await command.ExecuteNonQueryAsync(token).ConfigureAwait(false);
                        _logger.Info("Данных отравлено {Count}", count);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.Warn("Операция была отменена");
                throw;
            }
            catch (MySqlException ex)
            {
                string errorMessage = "Ошибка со стороны базы данных";
                _logger.Error(ex, errorMessage);
            }
            catch (DbException ex)
            {
                string errorMessage = "Непредвиденная ошибка с базой данных";
                _logger.Error(ex, errorMessage);
                throw new InsertException(errorMessage, ex);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Непредвиденная ошибка");
                throw;
            }
        }

        public async Task SendDataAsync(PlcLogEntityDto plcLogEntryDto, CancellationToken token)
        {
            string sql = @"
            INSERT INTO `diagramrooms`.`datapoints` 
            (`IdGraph`, `BatchNumber`, `NowTime`, `Value`, `Time`) 
            VALUES (@IdGraph, @BatchNumber, @NowTime, @Value, @Time);
        ";
            try
            {
                if (plcLogEntryDto == null) throw new ArgumentNullException(nameof(plcLogEntryDto));

                using (var connection = new MySqlConnection(_connectionString))
                {
                    token.ThrowIfCancellationRequested();

                    await connection.OpenAsync(token);

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        //Добавляем параметры
                        command.Parameters.AddWithValue("@IdGraph", plcLogEntryDto.IdGraph);
                        command.Parameters.AddWithValue("@BatchNumber", plcLogEntryDto.BatchNumber);
                        command.Parameters.AddWithValue("@NowTime", DateTime.Now);
                        command.Parameters.AddWithValue("@Value", plcLogEntryDto.Value);
                        command.Parameters.AddWithValue("@Time", plcLogEntryDto.Time);

                        // Выполняем запрос
                        int rowsAffected = await command.ExecuteNonQueryAsync(token);

                        _logger.Info($"Запись добавлена. Количество затронутых строк: {rowsAffected}");
                    }
                }

            }
            catch (OperationCanceledException)
            {
                _logger.Warn("Операция была отменена");
                throw;
            }
            catch (MySqlException ex)
            {
                _logger.Error(ex, "Ошибка со стороны базы данных");
                throw;
            }
            catch (DbException ex)
            {
                _logger.Error(ex, "Непредвиденная ошибка с базой данных");
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Непредвиденная ошибка");
                throw;
            }
        }

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
        public async Task<int> GetLastBatchNumberByGraphAsync(int idGraph, CancellationToken token)
        {
            string sql = @"
                SELECT MAX(d.BatchNumber) AS MaxBatchNumber
                FROM diagramrooms.datapoints AS d
                INNER JOIN diagramrooms.graph AS g
                    ON d.IdGraph = g.IdGraph
                WHERE g.IdGraph = @IdGraph;
                ";  

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    token.ThrowIfCancellationRequested();

                    await connection.OpenAsync();

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IdGraph", idGraph);
                        
                        token.ThrowIfCancellationRequested();

                        var result = await command.ExecuteScalarAsync(token);

                        if (result == DBNull.Value || result == null)
                        {
                            _logger.Warn($"MaxBatchNumber равен NULL для IdGraph = {idGraph}. Возвращено значение по умолчанию: 0.");
                            return 0;
                        }

                        if (!(result is int maxBatchNumber))
                        {
                            _logger.Error($"Ошибка преобразования значения {nameof(maxBatchNumber)}. Получено: {result.GetType().Name}");
                            throw new InvalidCastException($"Значение {nameof(maxBatchNumber)} из базы данных не является целым числом.");
                        }

                        _logger.Trace($"Успешно получено значение {nameof(maxBatchNumber)} = {maxBatchNumber} для IdGraph = {idGraph}.");
                        return maxBatchNumber;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.Warn("Операция получения BatchNumber была отменена.");
                throw;
            }
            catch (MySqlException ex)   
            {
                _logger.Error(ex, $"Ошибка при выполнении SQL-запроса для IdGraph = {idGraph}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Неизвестная ошибка при получении BatchNumber для IdGraph = {idGraph}");
                throw;
            }
        }

        public async Task<List<int>> GetAllGraphIdsAsync(CancellationToken token)
        {
            string sql = "SELECT IdGraph FROM Graph ORDER BY IdGraph;";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    token.ThrowIfCancellationRequested();

                    await connection.OpenAsync(token);

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        token.ThrowIfCancellationRequested();

                        using (var reader = await command.ExecuteReaderAsync(token))
                        {
                            List<int> graphIds = new List<int>();

                            token.ThrowIfCancellationRequested();

                            while (await reader.ReadAsync(token))
                            {
                                var result = reader["IdGraph"];

                                if (result == DBNull.Value || result == null)
                                {
                                    _logger.Warn($"Поле {nameof(result)} == null");
                                    throw new InvalidOperationException($"Поле {nameof(result)} == null");
                                }

                                if (!(result is int idGraph))
                                {
                                    _logger.Error($"Неверное преобразование {nameof(idGraph)} из БД. Значение: {result}");
                                    throw new InvalidCastException($"Неверный тип данных для {nameof(idGraph)}");
                                }

                                graphIds.Add(idGraph);
                            }

                            if (graphIds == null)
                            {
                                _logger.Error($"Список {nameof(graphIds)} полученный из бд == null");
                                throw new NullReferenceException($"Список {nameof(graphIds)} полученный из бд == null");
                            }

                            _logger.Info("Операция GetAllGraphIdsAsync выполнена");
                            return graphIds;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.Warn("Операция была отменена");
                throw;
            }
            catch (MySqlException ex)
            {
                _logger.Error(ex, "Ошибка со стороны базы данных");
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Непредвиденная ошибка");
                throw;
            }
        }
    }
}
