using MySql.Data.MySqlClient;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram.DataAccess
{
    internal class DataBaseServices : IDataBaseServices
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;

        public DataBaseServices(string connectionString, ILogger logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task SendDataAsync(int idGraph,CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            int LastBatchNumner = await GetLastBatchNumber(idGraph, token);


        }

        private async Task<int> GetLastBatchNumber(int idGraph, CancellationToken token)
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
                            throw new InvalidOperationException($"Поле MaxBatchNumber содержит null значение для IdGraph = {idGraph}");
                        }

                        if (!(result is int maxBatchNumber))
                        {
                            _logger.Error($"Неверное преобразование MaxBatchNumber из БД. Значение: {result}");
                            throw new InvalidCastException("Неверный тип данных для MaxBatchNumber");

                        }

                        return maxBatchNumber;
                    }
                }
            }
            catch(MySqlException ex)
            {
                _logger.Error(ex, "Ошибка со стороны базы данных");
                throw;
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Непредвиденная ошибка");
                throw;
            }
        }
    }
}
