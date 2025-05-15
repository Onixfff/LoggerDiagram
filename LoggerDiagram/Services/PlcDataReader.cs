using LoggerDiagram.Models.Plc;
using LoggerDiagram.PlcException;
using NLog;
using S7.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram.Services
{
    internal class PlcDataReader : IPlcDataReader
    {
        private Plc _plc;
        private readonly ILogger _logger;
        
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="PlcDataReader"/> с указанием IP-адреса PLC и логгера.
        /// </summary>
        /// <param name="ip">IP-адрес контроллера PLC.</param>
        /// <param name="logger">Логгер для записи диагностических сообщений.</param>
        public PlcDataReader(string ip, ILogger logger)
        {
            _logger = logger;
            _plc = new Plc(CpuType.S71200, ip, 0, 1);
        }

        /// <summary> Выполняет попытку чтения данных из PLC по указанным адресам. </summary>
        /// <param name="byteStart">Начальный адрес байта отвечающий за 1 и 0</param>
        /// <param name="byteStartNameRoom">Начальный адрес байта, содержащий номер помещения</param>
        /// <param name="floatStart">Начальный адрес значения типа float. Отвечает за Value (y) </param>
        /// <param name="shortStart">Начальный адрес значения типа short. Отвечает за Time (x)</param>
        /// <param name="token">Токен отмены для прерывания операции</param>
        /// <returns> Объект <see cref="PlcSensorReading"/> </returns>
        /// <exception cref="PlcDataReadException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        private async Task<PlcSensorReading> GetDataAsync(int byteStart,int byteStartNameRoom, int floatStart, int shortStart, CancellationToken token)
        {
            PlcSensorReading plcLogEntry;

            try
            {
                await _plc.OpenAsync(token);

                var byteValue = await ReadPlcValueAsync<byte>(DataType.DataBlock, 1, byteStart, VarType.Byte, token);
                var byteValueNameRoom = await ReadPlcValueAsync<byte>(DataType.DataBlock, 1, byteStartNameRoom, VarType.Byte, token);
                var doubleValue = await ReadPlcValueAsync<double>(DataType.DataBlock, 1, floatStart, VarType.Real, token);
                var shortValue = await ReadPlcValueAsync<short>(DataType.DataBlock, 1, shortStart, VarType.Int, token);

                plcLogEntry = PlcSensorReading.Create(byteValue, doubleValue, shortValue);
                _logger.Info($"Успешное создание PlcLogEntry");

                return plcLogEntry;
            }
            catch (InvalidCastException ex)
            {
                _logger.Error(ex, $"Ошибка преобразования типов при чтении данных из PLC. byteStart: {byteStart}, doubleStart: {floatStart}, timeStart: {shortStart}");
                throw new PlcDataReadException("Ошибка преобразования типов при чтении данных из PLC", ex);
            }
            catch (OperationCanceledException ex)
            {
                _logger.Warn(ex, $"Операция была отменена. byteStart: {byteStart}, doubleStart: {floatStart}, timeStart: {shortStart}");
                throw;
            }
            catch (PlcDataReadException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Неожиданная ошибка при работе с PLC. byteStart: {byteStart}, doubleStart: {floatStart}, timeStart: {shortStart}");
                throw new PlcDataReadException("Неожиданная ошибка при работе с PLC", ex);
            }
            finally
            {
                try
                {
                    if (_plc.IsConnected)
                    {
                        _plc.Close();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex, "Ошибка при закрытии соединения с PLC.");
                }

            }
        }

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
        public async Task<PlcSensorReading> GetDataWithRetryAsync(int byteStart, int byteStartNameRoom, int floatStart, int shortStart, CancellationToken token, int maxRetries = 3)
        {
            int attempt = 0;
            int delay = 2000;
            int retryDelayInSeconds = delay / 1000;

            while (attempt < maxRetries)
            {
                try
                {
                    return await GetDataAsync(byteStart, byteStartNameRoom, floatStart, shortStart, token);
                }
                catch(PlcDataReadException ex)
                {
                    attempt++;
                    _logger.Warn(ex, $"Попытка {attempt} не удалась. Повтор через {retryDelayInSeconds} сек.");
                    await Task.Delay(delay, token);
                }
            }

            _logger.Error("Все попытки получения данных из PLC завершились неудачей");
            return null;
        }

        /// <summary> Асинхронно читает значение из PLC по указанному адресу и приводит его к типу <typeparamref name="T"/>. </summary>
        /// <typeparam name="T"> Тип значения, в который будет приведён результат чтения. Работает только с этими типами: byte, float, short. </typeparam>
        /// <param name="dataType"> Тип области данных PLC (например, DataBlock). </param>
        /// <param name="dbNumber"> Номер блока данных. </param>
        /// <param name="offset"> Смещение (адрес) внутри блока данных. </param>
        /// <param name="varType"> Тип переменной, определяющий размер значения. Работает только с этими типами: Byte, Real, Int. </param>
        /// <param name="token"> Токен отмены для прерывания операции. </param>
        /// <returns> Прочитанное значение, приведённое к типу <typeparamref name="T"/>. </returns>
        /// <exception cref="PlcDataReadException"> Возникает, если произошла ошибка при чтении значения из PLC. </exception>
        /// <exception cref="InvalidCastException"> Возникает, если произошла ошибка при преобразовании значения из PLC. </exception>
        private async Task<T> ReadPlcValueAsync<T>(DataType dataType, int dbNumber, int offset, VarType varType, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            try
            {
                object result = await _plc.ReadAsync(dataType, dbNumber, offset, varType, 1, cancellationToken: token);

                if (result is T value)
                {
                    _logger.Info($"Успешно прочитано значение {value} по адресу {offset}.");
                    return value;
                }

                _logger.Error($"Неверное преобразование значения из PLC. " +
                      $"Address: {offset}, Expected type: {typeof(T)}, Actual type: {result?.GetType()}");
                throw new InvalidCastException($"Не удалось привести тип к {typeof(T)}");
            }
            catch(Exception ex)
            {
                _logger.Warn(ex, $"Ошибка при чтении значения из PLC по адресу {offset}");
                throw new PlcDataReadException("Ошибка при чтении значения из PLC", ex);
            }
        }
    }
}
