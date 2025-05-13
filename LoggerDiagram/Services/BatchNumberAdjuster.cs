using LoggerDiagram.Enums;
using LoggerDiagram.Models.Plc;
using System;

namespace LoggerDiagram.Services
{
    public class BatchNumberAdjuster : IBatchNumberAdjuster
    {
        public int Adjust(int lastBatchNumber, PlcLogEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            RawByteValueEnum status;

            switch (entity.RawByteValue)
            {
                case 0:
                    status = RawByteValueEnum.SameProduct;
                    break;
                case 1:
                    status = RawByteValueEnum.NewProduct;
                    break;
                case 100:
                    throw new InvalidOperationException(
                        $"Ошибка {nameof(entity.RawByteValue)}: {entity.RawByteValue}");
                default:
                    throw new InvalidOperationException(
                        $"Недопустимое значение rawByteValue: {entity.RawByteValue}");
            }

            if (status == RawByteValueEnum.SameProduct)
                lastBatchNumber++;

            return lastBatchNumber;
        }
    }
}
