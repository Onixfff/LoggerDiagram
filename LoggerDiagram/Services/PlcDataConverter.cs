using LoggerDiagram.DataAccess;
using LoggerDiagram.DTO;
using LoggerDiagram.Models.Plc;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram.Services
{
    public class PlcDataConverter : IPlcDataConverter
    {
        private readonly ILogger _logger;
        private readonly IDataBaseRepository _repository;
        private readonly IBatchNumberAdjuster _batchAdjuster;

        public PlcDataConverter(IDataBaseRepository repository, IBatchNumberAdjuster batchAdjuster, ILogger logger)
        {
            _repository = repository;
            _batchAdjuster = batchAdjuster;
            _logger = logger;
        }

        public async Task<List<PlcLogEntityDto>> ConvertAsync(List<int> ids, List<PlcSensorReading> entity, CancellationToken token)
        {
            if (ids == null || entity == null)
                throw new ArgumentNullException("Параметры ids или entity не могут быть null");

            var dtos = new List<PlcLogEntityDto>();

            int count = Math.Min(ids.Count, entity.Count);

            if (ids.Count != entity.Count)
            {
                _logger.Warn($"Кол-во данных {nameof(ids)} - {ids.Count} != {nameof(entity)} - {entity.Count}. Обработано: {count}");
            }

            for(int i = 0; i < count; i++)
            {
                try
                {
                    int batch = await _repository.GetLastBatchNumberByGraphAsync(ids[i], token);
                    batch = _batchAdjuster.Adjust(ids[i], batch, entity[i]);

                    if (entity[i].Status == 0)
                    {
                        continue;
                    }

                    dtos.Add(PlcLogEntityDto.Create((int)entity[i].RoomNumber, batch, entity[i].Status, entity[i].Value, entity[i].Time));

                }
                catch
                {

                }

            }

            return dtos;
        }
    }
}
