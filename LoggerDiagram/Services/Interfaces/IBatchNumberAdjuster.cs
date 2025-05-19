using System;
using LoggerDiagram.Models.Plc;

namespace LoggerDiagram.Services.Interfaces
{
    public interface IBatchNumberAdjuster
    {
        /// <summary>
        /// Корректирует номер партии в зависимости от изменения статуса продукта.
        /// Если статус изменился с <see cref="ProductState.IsHaveProduct"/> на <see cref="ProductState.ZeroProduct"/>, 
        /// то номер партии увеличивается на 1.
        /// </summary>
        /// <param name="graphId">Идентификатор графика.</param>
        /// <param name="lastBatchNumber">Последний известный номер партии.</param>
        /// <param name="entity">Объект с данными датчика ПЛК.</param>
        /// <returns>Обновлённый номер партии.</returns>
        /// <exception cref="ArgumentNullException">Если параметр <paramref name="entity"/> равен null.</exception>
        int Adjust(int graphId, int lastBatchNumber, PlcSensorReading entity);
    }
}