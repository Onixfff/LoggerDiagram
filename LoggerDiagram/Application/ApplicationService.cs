using LoggerDiagram.DataAccess;
using LoggerDiagram.DTO;
using LoggerDiagram.Enums;
using LoggerDiagram.Models.Plc;
using LoggerDiagram.Services;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram.Application
{
    internal class ApplicationService
    {
        private readonly IPlcDataReaderFactory _plcDataReaderFactory;
        private readonly IDataBaseRepository _dataBaseRepository;
        private readonly ILogger _logger;

        private const int StartIndex1 = 1;
        private const int StartIndex2 = 0;
        private const int MaxIndex = 38;

        public ApplicationService(IPlcDataReaderFactory plcDataReaderFactory, IDataBaseRepository dataBaseRepository, ILogger logger)
        {
            _plcDataReaderFactory = plcDataReaderFactory;
            _dataBaseRepository = dataBaseRepository;
            _logger = logger;
        }

        public async Task RunAsync(CancellationToken token)
        {
            string ip1 = ConfigurationManager.AppSettings["PlcEven"];
            string ip2 = ConfigurationManager.AppSettings["PlcOdd"];

            if (string.IsNullOrWhiteSpace(ip1))
            {
                string error = "Ошибка isNull " + nameof(ip1);
                _logger.Error(error);
                throw new ArgumentNullException(nameof(ip1));
            }

            if (string.IsNullOrWhiteSpace(ip2))
            {
                string error = "Ошибка isNull " + nameof(ip2);
                _logger.Error(error);
                throw new ArgumentNullException(nameof(ip2));
            }

            IPlcDataReader plcDataReader1 = _plcDataReaderFactory.Create(ip1);
            IPlcDataReader plcDataReader2 = _plcDataReaderFactory.Create(ip2);

            while (!token.IsCancellationRequested)
            {
                try
                {

                    //Получение данных
                    var tasks = new[]
                    {
                        ProcessReaderAsync(StartIndex1, MaxIndex, plcDataReader1, nameof(plcDataReader1), token),
                        ProcessReaderAsync(StartIndex2, MaxIndex, plcDataReader2, nameof(plcDataReader2), token)
                    };

                    var results = await Task.WhenAll(tasks);
                    List<PlcLogEntryDto> result1 = results[0];
                    List<PlcLogEntryDto> result2 = results[1];
                    
                    //Отправка данных
                    for (int i = 0; i < MaxIndex/2; i++)
                    {
                        await ProcessSendDataAsync(result1[i], token);
                        await ProcessSendDataAsync(result2[i], token);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Поймана не ведомая ошибка");
                }
            }
        }

        private async Task ProcessSendDataAsync(PlcLogEntryDto plcLogEntryDto ,CancellationToken token)
        {
            await _dataBaseRepository.SendDataAsync(plcLogEntryDto, token);
            _logger.Info("Отправил данные");
        }

        private async Task<List<PlcLogEntryDto>> ProcessReaderAsync(int startI, int maxI, IPlcDataReader plcDataReader, string readerName, CancellationToken token)
        {
            var result = await Get(startI, maxI, plcDataReader, token);
            _logger.Info($"Получил данные для {readerName}");
            return result;
        }

        private async Task<List<PlcLogEntryDto>> Get(int startI, int maxI, IPlcDataReader plcDataReader, CancellationToken token)
        {
            List<PlcLogEntry> plcLogEntitys = new List<PlcLogEntry>();
            List<PlcLogEntryDto> plcLogEntryDtos = new List<PlcLogEntryDto>();

            int byteCount = 0;
            int doubleCount = 2;
            int timeCount = 6;

            for(int i = startI; i < maxI/2; i+=2)
            {
                plcLogEntitys.Add(await plcDataReader.GetDataAsync(byteCount, doubleCount, timeCount, token));
                plcLogEntryDtos.Add(await ConvertPlcLogEntityInDtoAsync(i, plcLogEntitys[i], token));

                byteCount += 8;
                doubleCount += 8;
                timeCount += 8;
            }

            return plcLogEntryDtos;
        }

        private async Task<PlcLogEntryDto> ConvertPlcLogEntityInDtoAsync(int id, PlcLogEntry plcLogEntry, CancellationToken token)
        {
            int lastBatchNumber = await _dataBaseRepository.GetLastBatchNumberByGraphAsync(id, token);

            lastBatchNumber = ChangeBatchNumber(lastBatchNumber, plcLogEntry);

            return PlcLogEntryDto.Create(id, lastBatchNumber, plcLogEntry.RawByteValue, plcLogEntry.Value, plcLogEntry.Time);
        }

        private int ChangeBatchNumber(int lastBatchNumber, PlcLogEntry plcLogEntryDtos)
        {
            RawByteValueEnum status;

            switch (plcLogEntryDtos.RawByteValue)
            {
                case 0:
                    status = RawByteValueEnum.SameProduct;
                    break;
                case 1:
                    status = RawByteValueEnum.NewProduct;
                    break;
                case 100:
                    status = RawByteValueEnum.Error;
                    throw new InvalidOperationException($"Ошибка {nameof(plcLogEntryDtos.RawByteValue)}: {plcLogEntryDtos.RawByteValue} получил значение ошибки");
                default:
                    throw new InvalidOperationException($"Недопустимое значение rawByteValue: {plcLogEntryDtos.RawByteValue}");
            }

            if(status == RawByteValueEnum.SameProduct)
            {
                lastBatchNumber++;
            }

            return lastBatchNumber;
        }
    }
}
