using LoggerDiagram.DataAccess;
using LoggerDiagram.Services;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram.Application
{
    internal class ApplicationService
    {
        private readonly IPlcDataReader _plcDataReader;
        private readonly IDataBaseRepository _dataBaseRepository;
        private readonly ILogger _logger;

        public ApplicationService(IPlcDataReader plcDataReader, IDataBaseRepository dataBaseRepository, ILogger logger)
        {
            _plcDataReader = plcDataReader;
            _dataBaseRepository = dataBaseRepository;
            _logger = logger;
        }

        public async Task RunAsync(CancellationToken token)
        {
            while (true)
            {
                try
                {
                    _plcDataReader.GetDataAsync();
                }
                catch(Exception ex)
                {
                    _logger.Error(ex, "Поймана не ведомая ошибка");
                }
            }
        }
    }
}
