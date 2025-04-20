using LoggerDiagram.DataAccess;
using LoggerDiagram.DTO;
using LoggerDiagram.Models.Plc;
using LoggerDiagram.Services;
using NLog;
using System;
using System.Collections.Generic;
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
            IPlcDataReader plcDataReader1 = _plcDataReaderFactory.Create("");
            IPlcDataReader plcDataReader2 = _plcDataReaderFactory.Create("");

            while (token.IsCancellationRequested)
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
                    
                    //TODO Перебор данных до отправки (если 0 или 1 в байтах) нужно добавить логику возможно её стоит добавить в модель при создании чтобы он сам увеличивал значение на 1 в batchNunber

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
                plcLogEntryDtos.Add(ConvertPlcLogEntityInDto(i, plcLogEntitys[i]));

                byteCount += 8;
                doubleCount += 8;
                timeCount += 8;
            }

            return plcLogEntryDtos;
        }

        private PlcLogEntryDto ConvertPlcLogEntityInDto(int id, PlcLogEntry plcLogEntry)
        {
            return PlcLogEntryDto.Create(id, plcLogEntry.Value, plcLogEntry.Time);
        }
    }
}
