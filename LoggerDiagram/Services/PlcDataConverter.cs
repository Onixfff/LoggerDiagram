using NLog;
using System;
using System.Threading;
using LoggerDiagram.DTO;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using LoggerDiagram.DataAccess;
using LoggerDiagram.Models.Plc;
using System.Collections.Generic;
using LoggerDiagram.Enums;
using LoggerDiagram.Services.Interfaces;

namespace LoggerDiagram.Services
{
    /// <summary>
    /// Сервис для конвертации данных с ПЛК (<see cref="PlcSensorReading"/>) в DTO (<see cref="PlcLogEntityDto"/>).
    /// </summary>
    public class PlcDataConverter : IPlcDataConverter
    {
        private readonly ILogger _logger;

        private readonly IDataBaseRepository _repository;
        private readonly IBatchNumberAdjuster _batchAdjuster;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="PlcDataConverter"/>.
        /// </summary>
        /// <param name="repository">Репозиторий для взаимодействия с базой данных.</param>
        /// <param name="batchAdjuster">Сервис корректировки номера партии.</param>
        /// <param name="logger">Логгер для записи диагностических сообщений.</param>
        public PlcDataConverter(IDataBaseRepository repository, IBatchNumberAdjuster batchAdjuster, ILogger logger)
        {
            _repository = repository;
            _batchAdjuster = batchAdjuster;
            _logger = logger;
        }

        /// <summary>
        /// Асинхронно преобразует список идентификаторов графиков и соответствующие данные с ПЛК в список DTO (<see cref="PlcLogEntityDto"/>).
        /// Пары формируются по индексу: ids[0] + entity[0], ids[1] + entity[1] и т.д.
        /// Если количество элементов в списках не совпадает — будет обработано минимальное количество.
        /// </summary>
        /// <param name="ids">Список идентификаторов графиков</param>
        /// <param name="entity">Список данных с датчиков ПЛК</param>
        /// <param name="token">Токен отмены для прерывания операции</param>
        /// <returns>Список объектов <see cref="PlcLogEntityDto"/>. Возвращается пустой список, если ни одно значение не было обработано.</returns>
        /// <exception cref="ArgumentNullException">Если любой из списков равен null</exception>
        public async Task<List<PlcLogEntityDto>> ConvertAsync(List<int> ids, List<PlcSensorReading> entity, CancellationToken token)
        {
            if (entity == null) { throw new ArgumentNullException(nameof(entity), $"Список {nameof(entity)} null"); }

            if (ids == null) { throw new ArgumentNullException(nameof(ids), $"Список {nameof(ids)} null"); }

            var dos = new List<PlcLogEntityDto>();

            var count = Math.Min(ids.Count, entity.Count);

            if (ids.Count != entity.Count)
            {
                _logger.Warn($"Кол-во данных {nameof(ids)} - {ids.Count} != {nameof(entity)} - {entity.Count}. Обработано: {count}");
            }

            for (var i = 0; i < count; i++)
            {
                var dto = await ProcessItemAsync(ids[i], entity[i], token).ConfigureAwait(false);

                if (!dto.IsEmpty())
                {
                    dos.Add(dto);
                }
            }
            return dos;
        }

        /// <summary>
        /// Асинхронно конвертирует данные с ПЛК в объект <see cref="PlcLogEntityDto"/> для указанного идентификатора графика.
        /// При возникновении ошибки возвращается базовое значение, чтобы избежать NullReferenceException и обеспечить непрерывную обработку.
        /// </summary>
        /// <param name="id">Идентификаторор графика.</param>
        /// <param name="entity">Данных с ПЛК.</param>
        /// <param name="token">Токен отмены для прерывания операции.</param>
        /// <returns>Объект <see cref="PlcLogEntityDto"/>. Может быть базовым значением</returns>
        /// <remarks>
        /// Следующие типы ошибок обрабатываются внутри метода:
        /// - ArgumentNullException: если данные с датчика отсутствуют
        /// - OperationCanceledException: если операция была отменена
        /// - MySqlException: при ошибках подключения к БД
        /// - InvalidCastException: при ошибках преобразования данных
        /// Все ошибки логируются и не приводят к завершению процесса.
        /// </remarks>
        private async Task<PlcLogEntityDto> ProcessItemAsync(int id, PlcSensorReading reading, CancellationToken token)
        {
            try
            {
                var batch = await _repository.GetLastBatchNumberByGraphAsync(id, token).ConfigureAwait(false);

                batch = _batchAdjuster.Adjust(id, batch, reading);

                if (reading.Status == ProductState.ZeroProduct)
                {
                    _logger.Warn("Status равен {Status} для IdGraph = {IdGraph}",nameof(ProductState.ZeroProduct), id);
                    return PlcLogEntityDto.CreateBaseValue();
                }

                var uIntPtr = reading.RoomNumber;

                if (uIntPtr == null)
                {
                    _logger.Warn("RoomNumber равно null для IdGraph = {IdGraph}", id);
                    return PlcLogEntityDto.CreateBaseValue();
                }

                var dto = PlcLogEntityDto.Create((int)uIntPtr, batch, reading.Status, reading.Value, reading.Time);
                return dto;

            }
            catch (ArgumentNullException ex)
            {
                _logger.Error(ex, "Ошибка: Не переданы данные с датчика для IdGraph = {IdGraph}", id);
                return PlcLogEntityDto.CreateBaseValue();
            }
            catch (OperationCanceledException ex)
            {
                _logger.Warn(ex, "Операция отменена для IdGraph = {IdGraph}", id);
                return PlcLogEntityDto.CreateBaseValue();
            }
            catch (MySqlException ex)
            {
                _logger.Error(ex, "Ошибка базы данных при обработке IdGraph = {IdGraph}", id);
                return PlcLogEntityDto.CreateBaseValue();
            }
            catch (InvalidCastException ex)
            {
                _logger.Error(ex, "Ошибка преобразования RoomNumber для IdGraph = {IdGraph}", id);
                return PlcLogEntityDto.CreateBaseValue();
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Неизвестная ошибка при обработке IdGraph = {IdGraph}", id);
                return PlcLogEntityDto.CreateBaseValue();
            }
        }
    }
}
