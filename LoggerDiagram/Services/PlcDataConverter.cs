using LoggerDiagram.DataAccess;
using LoggerDiagram.DTO;
using LoggerDiagram.Models.Plc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram.Services
{
    public class PlcDataConverter : IPlcDataConverter
    {
        private readonly IDataBaseRepository _repository;
        private readonly IBatchNumberAdjuster _batchAdjuster;

        public PlcDataConverter(IDataBaseRepository repository, IBatchNumberAdjuster batchAdjuster)
        {
            _repository = repository;
            _batchAdjuster = batchAdjuster;
        }

        public async Task<List<PlcLogEntityDto>> ConvertAsync(List<int> ids, List<PlcLogEntity> entity, CancellationToken token)
        {
            if (ids.Count != entity.Count)
                throw new ArgumentException("Кол-во id и данных не совпадает");

            var dtos = new List<PlcLogEntityDto>();

            for(int i = 0; i < ids.Count; i++)
            {
                int batch = await _repository.GetLastBatchNumberByGraphAsync(ids[i], token);
                batch = _batchAdjuster.Adjust(ids[i], batch, entity[i]);

                if (entity[i].RawByteValue == 0)
                {
                    continue; // Не отправляем данные
                }

                dtos.Add(PlcLogEntityDto.Create(ids[i], batch, entity[i].RawByteValue, entity[i].Value, entity[i].Time));
            }

            return dtos;
        }
    }
}
