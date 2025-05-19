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
        /// Асинхронно конвертирует список данных с ПЛК в список DTO.
        /// Если количество входных ID не совпадает с количеством данных — обрабатывается минимальное из них.
        /// </summary>
        /// <param name="ids">Список идентификаторов графиков.</param>
        /// <param name="entity">Список данных с ПЛК.</param>
        /// <param name="token">Токен отмены для прерывания операции.</param>
        /// <returns>Список объектов <see cref="PlcLogEntityDto"/>.</returns>
        /// <exception cref="ArgumentNullException">Выбрасывается, если <paramref name="ids"/> или <paramref name="entity"/> равны null.</exception>
        /// <exception cref="OperationCanceledException">Если операция была отменена.</exception>
        /// <exception cref="MySqlException">Ошибка со стороны базы данных.</exception>
        /// <exception cref="InvalidCastException">Ошибка преобразования данных.</exception>
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

            for (var i = 0; i < count; i++)
            {
                    var dto = await ProcessItemAsync(ids[i], entity[i], token).ConfigureAwait(false);

                    if (dto != null)
                    {
                        dos.Add(dto);
                    }
            }
            return dos;
        }

        private async Task<PlcLogEntityDto> ProcessItemAsync(int id, PlcSensorReading reading, CancellationToken token)
        {
            try
            {
                var batch = await _repository.GetLastBatchNumberByGraphAsync(id, token).ConfigureAwait(false);

                batch = _batchAdjuster.Adjust(id, batch, reading);

                if (reading.Status == ProductState.ZeroProduct)
                {
                    _logger.Warn("Status равен {Status} для IdGraph = {IdGraph}",nameof(ProductState.ZeroProduct), id);
                    return null;
                }

                var uIntPtr = reading.RoomNumber;

                if (uIntPtr == null)
                {
                    _logger.Warn("RoomNumber равно null для IdGraph = {IdGraph}", id);
                    return null;
                }

                var dto = PlcLogEntityDto.Create((int)uIntPtr, batch, reading.Status, reading.Value, reading.Time);
                return dto;

            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (MySqlException)
            {
                throw;
            }
            catch (InvalidCastException)
            {
                throw;
            }
        }
    }
}
