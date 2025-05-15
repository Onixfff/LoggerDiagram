using LoggerDiagram.Models.Plc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram.Services
{
    public class PlcReaderService : IPlcReaderService
    {

        /// <summary>
        /// Асинхронно считывает данные с датчиков, подключенных к ПЛК, используя указанные смещения для каждого датчика.
        /// </summary>
        /// Для каждого элемента в коллекции <paramref name="ids"/> метод вычисляет 
        /// последовательные смещения и вызывает GetDataWithRetryAsync дл яполучения данных.
        /// Смещения увеличиваются на 8 байт на каждой итерации.
        /// </remarks>
        /// <param name="ids">Список идентификаторов датчиков.</param>
        /// <param name="reader">Объект, реализующий интерфейс чтения данных из ПЛК.</param>
        /// <param name="token">Токен отмены для прерывания операции</param>
        /// <returns>
        /// Возвращает список объектов <see cref="PlcSensorReading"/>, представляющих собой 
        /// показания датчиков. Если данные не были получены, возвращается null.
        /// </returns>
        public async Task<List<PlcSensorReading>> GetPlcSensorReadingsAsync(List<int> ids, IPlcDataReader reader, CancellationToken token)
        {
            var result = new List<PlcSensorReading>();
            int offsetByte = 0;
            int offsetByteNameRoom = 1;
            int offsetDouble = 2;
            int offsetTime = 6;

            foreach (var id in ids)
            {
                var PlcSensorData = await reader.GetDataWithRetryAsync(offsetByte, offsetByteNameRoom, offsetDouble, offsetTime, token);
                
                if(PlcSensorData != null)
                {
                    result.Add(PlcSensorData);
                }

                offsetByte += 8;
                offsetByteNameRoom += 8;
                offsetDouble += 8;
                offsetTime += 8;
            }

            if (result.Count > 0)
                return result;
            else 
                return null;
        }
    }
}
