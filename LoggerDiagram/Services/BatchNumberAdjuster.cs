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

        public int Adjust(int graphId, int lastBatchNumber, PlcLogEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var state = _graphStateManager.GetState(graphId);

            RawByteValueEnum status;

            switch (entity.RawByteValue)
            {
                case 0:
                    status = RawByteValueEnum.ZeroProduct;
                    break;
                case 1:
                    status = RawByteValueEnum.IsHaveProduct;
                    break;
                case 100:
                    throw new InvalidOperationException(
                        $"Ошибка {nameof(entity.RawByteValue)}: {entity.RawByteValue}");
                default:
                    throw new InvalidOperationException(
                        $"Недопустимое значение rawByteValue: {entity.RawByteValue}");
            }

            // Увеличиваем только если значение стало 0 и раньше было не 0
            if (status == RawByteValueEnum.ZeroProduct && state.LastRawByteValue != 0)
            {
                lastBatchNumber++;
            }

            // Обновляем состояние после обработки
            state.LastRawByteValue = entity.RawByteValue;
            _graphStateManager.UpdateState(graphId, state);

            return lastBatchNumber;
        }
    }
}
