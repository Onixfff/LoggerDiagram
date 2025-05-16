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
            if (entity == null) { throw new ArgumentNullException(nameof(entity), "Равен null"); }

            if (ids == null) { throw new ArgumentNullException(nameof(ids), "Равен null"); }

            var dos = new List<PlcLogEntityDto>();

            var count = Math.Min(ids.Count, entity.Count);

            if (ids.Count != entity.Count)
            {
                _logger.Warn($"Кол-во данных {nameof(ids)} - {ids.Count} != {nameof(entity)} - {entity.Count}. Обработано: {count}");
            }

            for(var i = 0; i < count; i++)
            {
                try
                {
                    var batch = await _repository.GetLastBatchNumberByGraphAsync(ids[i], token).ConfigureAwait(false);
                    batch = _batchAdjuster.Adjust(ids[i], batch, entity[i]);

                    if (entity[i].Status == 0)
                    {
                        continue;
                    }

                    var uIntPtr = entity[i].RoomNumber;

                    if (uIntPtr == null)
                    {
                        continue;
                    }

                    _logger.Warn(new ArgumentNullException(nameof(uIntPtr)));
                    dos.Add(PlcLogEntityDto.Create((int)uIntPtr, batch, entity[i].Status, entity[i].Value, entity[i].Time));
                }
                catch
                {
                }
            }
            return dos;
        }
    }
}
