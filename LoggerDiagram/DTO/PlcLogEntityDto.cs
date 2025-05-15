using LoggerDiagram.Enums;
using System;

namespace LoggerDiagram.DTO
{
    public class PlcLogEntityDto
    {
        public readonly int IdGraph;
        public readonly int BatchNumber;
        public readonly ProductState Status;
        public readonly double Value;
        public readonly int Time;

        private PlcLogEntityDto(int idGraph, int batchNumber, ProductState status, double value, int time)
        {
            IdGraph = idGraph;
            BatchNumber = batchNumber;
            Status = status;
            Value = value;
            Time = time;
        }

        public static PlcLogEntityDto Create(int idGraph, int batchNumber, ProductState status, double value, int time)
        {
            //Проверка
            if(idGraph <= 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(idGraph)} вышел за пределы допустимого диапозона");
            }

            if (value <= -1)
            {
                throw new ArgumentOutOfRangeException($"{nameof(value)} вышел за пределы допустимого диапозона");
            }

            if (time <= -1)
            {
                throw new ArgumentOutOfRangeException($"{nameof(time)} вышел за пределы допустимого диапозона");
            }

            if(batchNumber <= -1)
            {
                throw new ArgumentOutOfRangeException($"{nameof(batchNumber)} вышел за пределы допустимого диапозона");
            }

            //Создание

            return new PlcLogEntityDto(idGraph, batchNumber, status, value, time);
        }
    }
}
