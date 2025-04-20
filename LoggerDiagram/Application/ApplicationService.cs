using LoggerDiagram.DataAccess;
using LoggerDiagram.Models.Plc;
using LoggerDiagram.Services;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram.Application
{
    internal class ApplicationService
    {
        private readonly IPlcDataReaderFactory _plcDataReaderFactory;
        private readonly IDataBaseRepository _dataBaseRepository;
        private readonly ILogger _logger;

        public ApplicationService(IPlcDataReaderFactory plcDataReaderFactory, IDataBaseRepository dataBaseRepository, ILogger logger)
        {
            _plcDataReaderFactory = plcDataReaderFactory;
            _dataBaseRepository = dataBaseRepository;
            _logger = logger;
        }

        public async Task RunAsync(CancellationToken token)
        {

            IPlcDataReader plcDataReader1 = _plcDataReaderFactory.Create("");
            IPlcDataReader plcDataReader2 = _plcDataReaderFactory.Create("");

            while (true)
            {
                try
                {
                    int byteCount = 0;
                    int doubleCount = 2;
                    int timeCount = 6;

                    for (int i = 0; i < 18; i++)
                    {
                        PlcLogEntry plcLogEntity = await _plcDataReader.GetDataAsync(byteCount, doubleCount, timeCount, token);

                        byteCount += 8;
                        doubleCount += 8;
                        timeCount += 8;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Поймана не ведомая ошибка");
                }
            }
        }
    }
}
