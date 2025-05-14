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

        public async Task<PlcLogEntity> GetDataAsync(int byteStart, int doubleStart, int timeStart, CancellationToken token)
        {
            PlcLogEntity plcLogEntry;

            try
            {
                await _plc.OpenAsync(token);

                token.ThrowIfCancellationRequested();

                //Тут считывается Status(0 или 1)
                var resultByte = await _plc.ReadAsync(DataType.DataBlock, 1, byteStart, VarType.Byte, 1, cancellationToken: token);
                
                if (!(resultByte is byte byteValue))
                {
                    _logger.Error($"Неверное преобразование {nameof(byteValue)} из PLC \n" +
                        $"Address: {byteStart}, Expected type: byte, Actual type: {resultByte?.GetType()}");
                    throw new InvalidCastException(nameof(byteValue));
                }
                _logger.Info($"Успешно прочитано значение {byteValue} по адресу {byteStart}.");

                token.ThrowIfCancellationRequested();

                //Тут считывается Value
                var resultFloat = await _plc.ReadAsync(DataType.DataBlock, 1, doubleStart, VarType.Real, 1, cancellationToken: token);

                if (!(resultFloat is float floatValue))
                {
                    _logger.Error($"Неверное преобразование {nameof(floatValue)} из PLC \n" +
                        $"Address: {doubleStart}, Expected type: byte, Actual type: {resultFloat?.GetType()}");
                    throw new InvalidCastException(nameof(floatValue));
                }

                _logger.Info($"Успешно прочитано значение {floatValue} по адресу {doubleStart}.");

                token.ThrowIfCancellationRequested();
                
                //Тут считывается Time
                var resultshort = await _plc.ReadAsync(DataType.DataBlock, 1, timeStart, VarType.Int, 1, cancellationToken: token);

                if (!(resultshort is short intValue))
                {
                    _logger.Error($"Неверное преобразование {nameof(intValue)} PLC \n" +
                        $"Address: {timeStart}, Expected type: byte, Actual type: {resultshort.GetType()}");
                    throw new InvalidCastException(nameof(intValue));
                }

                _logger.Info($"Успешно прочитано значение {intValue} по адресу {timeStart}.");

                plcLogEntry = PlcLogEntity.Create(byteValue, floatValue, intValue);
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
