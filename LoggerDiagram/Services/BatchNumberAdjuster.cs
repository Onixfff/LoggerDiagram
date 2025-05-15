using LoggerDiagram.Enums;
using LoggerDiagram.Models;
using LoggerDiagram.Models.Plc;
using LoggerDiagram.Services.Interfaces;
using System;

namespace LoggerDiagram.Services
{
    public class BatchNumberAdjuster : IBatchNumberAdjuster
    {
        private readonly IGraphStateManager _graphStateManager;

        public BatchNumberAdjuster(IGraphStateManager graphStateManager)
        {
            _graphStateManager = graphStateManager ?? throw new ArgumentNullException(nameof(graphStateManager));
        }

        public int Adjust(int graphId, int lastBatchNumber, PlcSensorReading entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

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
