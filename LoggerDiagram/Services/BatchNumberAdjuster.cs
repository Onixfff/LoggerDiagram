using LoggerDiagram.Enums;
using LoggerDiagram.Models.Plc;
using LoggerDiagram.Services.Interfaces;
using System;
using NLog;

namespace LoggerDiagram.Services
{
    /// <summary>
    /// Сервис для корректировки номера партии (BatchNumber) на основе изменений статуса продукта.
    /// </summary>
    public class BatchNumberAdjuster : IBatchNumberAdjuster
    {
        private readonly ILogger _logger;
        private readonly IGraphStateManager _graphStateManager;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="BatchNumberAdjuster"/>.
        /// </summary>
        /// <param name="graphStateManager">Сервис для хранения текущего состояния графиков.</param>
        /// <param name="logger">Логгер для записи диагностических сообщений.</param>
        public BatchNumberAdjuster(IGraphStateManager graphStateManager, ILogger logger)
        {
            _graphStateManager = graphStateManager ?? throw new ArgumentNullException(nameof(graphStateManager));
            _logger = logger;
        }

        /// <summary>
        /// Корректирует номер партии в зависимости от изменения статуса продукта.
        /// Если статус изменился с любого другого на ProductState.ZeroProduct, то номер партии увеличивается на 1.
        /// </summary>
        /// <param name="graphId">Идентификатор графика.</param>
        /// <param name="lastBatchNumber">Последний известный номер партии.</param>
        /// <param name="entity">Объект с данными датчика ПЛК.</param>
        /// <returns>Обновлённый номер партии.</returns>
        /// <exception cref="ArgumentNullException">Если параметр <paramref name="entity"/> равен null.</exception>
        public int Adjust(int graphId, int lastBatchNumber, PlcSensorReading entity)
        {
            if (entity == null)
            {
                _logger.Warn("");
                throw new ArgumentNullException(nameof(entity));
            }

            var state = _graphStateManager.GetState(graphId);

            var status = entity.Status;

            // Увеличиваем только если значение стало 0 и раньше было не 0
            if (status == ProductState.ZeroProduct && state.LastStatus != ProductState.ZeroProduct)
            {
                lastBatchNumber++;
            }

            // Обновляем состояние после обработки
            state.LastStatus = entity.Status;
            _graphStateManager.UpdateState(graphId, state);

            return lastBatchNumber;
        }
    }
}
