using LoggerDiagram.DataAccess;
using LoggerDiagram.Services;
using LoggerDiagram.Services.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram.Application
{
    public class PlcDataProcessor
    {
        private readonly IDataBaseRepository _repository;
        private readonly IPlcReaderService _readerService;
        private readonly IPlcDataConverter _converter;
        private readonly IPlcDataSender _sender;
        private readonly IPlcDataReaderFactory _readerFactory;
        private readonly ILogger _logger;

        public PlcDataProcessor(IDataBaseRepository repository,IPlcReaderService readerService, IPlcDataConverter converter, IPlcDataSender sender, IPlcDataReaderFactory readerFactory, ILogger logger)
        {
            _repository = repository;
            _readerService = readerService;
            _converter = converter;
            _sender = sender;
            _readerFactory = readerFactory;
            _logger = logger;
        }   

        public async Task ProcessAsync(CancellationToken token)
        {
            string ipEven = ConfigurationManager.AppSettings["PlcEven"];

            ValidateIps(ipEven);

            var allIds = await GetAllGraphIdsAsync(token);

            await ProcessGroupAsync(allIds, ipEven, token);
        }

        private void ValidateIps(string ipEven)
        {
            if (string.IsNullOrWhiteSpace(ipEven))
                throw new ArgumentNullException(nameof(ipEven), "IP для PLC не задан");
        }

        private async Task<List<int>> GetAllGraphIdsAsync(CancellationToken token)
        {
            var ids = await _repository.GetAllGraphIdsAsync(token);

            if (ids == null)
                throw new InvalidOperationException("Не удалось получить список id графиков");

            return ids;
        }

        private async Task ProcessGroupAsync(List<int> ids, string ip,  CancellationToken token)
        {
            IPlcDataReader reader = _readerFactory.Create(ip);
            var data = await _readerService.GetPlcSensorReadingsAsync(ids, reader, token);
            var dtos = await _converter.ConvertAsync(ids, data, token);
            
            if (dtos == null || dtos.Count == 0)
                return;

            await _sender.SendAsync(dtos, token);
        }

    }
}
