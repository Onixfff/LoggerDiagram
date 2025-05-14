using LoggerDiagram.DTO;
using MySql.Data.MySqlClient;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task SendDataAsync(PlcLogEntityDto plcLogEntryDto,CancellationToken token)
        {
            string sql = @"
            INSERT INTO `diagramrooms`.`datapoints` 
            (`IdGraph`, `BatchNumber`, `NowTime`, `Value`, `Time`) 
            VALUES (@IdGraph, @BatchNumber, @NowTime, @Value, @Time);
        ";
            try
            {
                using(var connection = new MySqlConnection(_connectionString))
                {
                    token.ThrowIfCancellationRequested();

                    await connection.OpenAsync(token);

                    using (var command = new MySqlCommand(sql ,connection))
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
            catch(DbException ex)
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
                            _logger.Warn($"Поле MaxBatchNumber содержит null значение для IdGraph = {idGraph}");
                            result = 0;
                        }

                        if (!(result is int maxBatchNumber))
                        {
                            _logger.Error($"Неверное преобразование MaxBatchNumber из БД. Значение: {result}");
                            throw new InvalidCastException("Неверный тип данных для MaxBatchNumber");
                        }

                        _logger.Info($"Запрос выполнен успешно. MaxBatchNumber = {maxBatchNumber}");
                        return maxBatchNumber;
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
