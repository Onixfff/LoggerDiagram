using System;

namespace LoggerDiagram.DTO
{
    internal class PlcLogEntryDto
    {
        public readonly int IdGraph;
        public readonly double Value;
        public readonly int Time;

        private PlcLogEntryDto(int idGraph, double value, int time)
        {
            IdGraph = idGraph;
            Value = value;
            Time = time;
        }

        public static PlcLogEntryDto Create(int idGraph, double value, int time)
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

            //Создание

            return new PlcLogEntryDto(idGraph, value, time);
        }
    }
}
