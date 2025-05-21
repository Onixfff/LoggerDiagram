using NLog;
using System;
using System.Threading;
using LoggerDiagram.DTO;
using System.Threading.Tasks;
using LoggerDiagram.DataAccess;
using System.Collections.Generic;
using LoggerDiagram.Services.Interfaces;
using LoggerDiagram.DataBaseExcepitons;

namespace LoggerDiagram.Services
{
    internal class PlcDataSender : IPlcDataSender
    {
        private IDataBaseRepository _repository;
        private ILogger _logger;

        public PlcDataSender(IDataBaseRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task SendAsync(IEnumerable<PlcLogEntityDto> dtos, CancellationToken token, int maxRetries = 3)
        {
            int attempt = 0;
            int delay = 2000;
            int retryDelayInSeconds = delay / 1000;

            while (attempt < maxRetries)
            {
                try
                {
                    await _repository.BulkInsertWithValuesAsync(dtos, token).ConfigureAwait(false);
                    return;
                }
                catch (InsertException ex)
                {
                    attempt++;
                    _logger.Warn(ex, $"Попытка {attempt} не удалась. Повтор через {retryDelayInSeconds} сек.");
                    await Task.Delay(delay, token);
                }
                catch (OperationCanceledException ex)
                {
                    _logger.Info(ex, "Данные не добавлены в бд");
                    return;
                }
                catch (ArgumentNullException ex)
                {
                    _logger.Info(ex, "Не переданы данные для отправки в БД");
                    return;
                }
                catch(Exception ex)
                {
                    _logger.Info(ex, "Данные не добавлены в бд");
                    return;
                }
            }
        }
    }
}
