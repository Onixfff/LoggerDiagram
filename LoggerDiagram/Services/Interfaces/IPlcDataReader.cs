using LoggerDiagram.Models.Plc;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram.Services
{
    public interface IPlcDataReader
    {
        /// <summary>
        /// Пытается получить данные из PLC с заданным количеством попыток.
        /// </summary>
        /// <param name="byteStart">Начальный адрес байта отвечающий за 1 и 0</param>
        /// <param name="byteStartNameRoom">Начальный адрес байта, содержащий номер помещения</param>
        /// <param name="floatStart">Начальный адрес значения типа float. Отвечает за Value (y) </param>
        /// <param name="shortStart">Начальный адрес значения типа short. Отвечает за Time (x)</param>
        /// <param name="token">Токен отмены для прерывания операции.</param>
        /// <param name="maxRetries">Максимально кол-во попыток</param>
        /// <returns> Объект <see cref="PlcSensorReading"/>, содержащий данные из PLC. Значение может быть null, если все попытки чтения завершились ошибкой.</returns>
        Task<PlcSensorReading> GetDataWithRetryAsync(int byteStart, int byteStartNameRoom, int doubleStart, int timeStart, CancellationToken token, int maxRetries = 3);
    }
}