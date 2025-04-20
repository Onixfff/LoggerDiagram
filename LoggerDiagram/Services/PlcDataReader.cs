using LoggerDiagram.Models.Plc;
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

        public PlcDataReader(string ip, ILogger logger)
        {
            _logger = logger;
            _plc = new Plc(CpuType.S71200, ip, 0, 1);
        }

        public async Task<PlcLogEntry> GetDataAsync(int byteStart, int doubleStart, int timeStart, CancellationToken token)
        {
            PlcLogEntry plcLogEntry;

            try
            {
                await _plc.OpenAsync(token);

                token.ThrowIfCancellationRequested();

                var resultByte = await _plc.ReadAsync(DataType.DataBlock, 1, byteStart, VarType.Byte, 1, cancellationToken: token);

                if (!(resultByte is byte byteValue))
                {
                    _logger.Error($"Неверное преобразование {nameof(byteValue)} из PLC \n" +
                        $"Address: {byteStart}, Expected type: byte, Actual type: {resultByte?.GetType()}");
                    throw new InvalidCastException(nameof(byteValue));
                }
                _logger.Info($"Успешно прочитано значение {byteValue} по адресу {byteStart}.");

                token.ThrowIfCancellationRequested();

                var resultDouble = await _plc.ReadAsync(DataType.DataBlock, 1, doubleStart, VarType.Real, 1, cancellationToken: token);

                if (!(resultDouble is double doubleValue))
                {
                    _logger.Error($"Неверное преобразование {nameof(doubleValue)} из PLC \n" +
                        $"Address: {doubleStart}, Expected type: byte, Actual type: {resultDouble?.GetType()}");
                    throw new InvalidCastException(nameof(doubleValue));
                }

                _logger.Info($"Успешно прочитано значение {doubleValue} по адресу {doubleStart}.");

                token.ThrowIfCancellationRequested();

                var resultInt = await _plc.ReadAsync(DataType.DataBlock, 1, timeStart, VarType.Real, 1, cancellationToken: token);

                if (!(resultInt is int intValue))
                {
                    _logger.Error($"Неверное преобразование {nameof(intValue)} PLC \n" +
                        $"Address: {timeStart}, Expected type: byte, Actual type: {resultInt?.GetType()}");
                    throw new InvalidCastException(nameof(intValue));
                }

                _logger.Info($"Успешно прочитано значение {intValue} по адресу {timeStart}.");

                plcLogEntry = PlcLogEntry.Create(byteValue, doubleValue, intValue);
                _logger.Info($"Успешное создание PlcLogEntry");

                return plcLogEntry;
            }
            catch (InvalidCastException ex)
            {
                _logger.Error(ex, $"Ошибка преобразования типов при чтении данных из PLC. byteStart: {byteStart}, doubleStart: {doubleStart}, timeStart: {timeStart}");
                throw;
            }
            catch (OperationCanceledException ex)
            {
                _logger.Warn(ex, $"Операция была отменена. byteStart: {byteStart}, doubleStart: {doubleStart}, timeStart: {timeStart}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Неожиданная ошибка при работе с PLC. byteStart: {byteStart}, doubleStart: {doubleStart}, timeStart: {timeStart}");
                throw;
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
    }
}
