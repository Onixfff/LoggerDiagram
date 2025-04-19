using System;

namespace LoggerDiagram.Models
{
    internal class DataPoints
    {
        public readonly int BatchNumber;
        public readonly DateTime NowTime;
        public readonly float Value;
        public readonly int Time;

        private DataPoints(int batchNumber, DateTime nowTime, float value, int time)
        {
            BatchNumber = batchNumber;
            NowTime = nowTime;
            Value = value;
            Time = time;
        }

        public static DataPoints Create(int batchNumber, DateTime nowTime, float value, int time)
        {
            int maxGraphId = 38;

            //Проверка данных
            if(batchNumber < 0 && batchNumber > maxGraphId)
            {
                throw new ArgumentOutOfRangeException("Номер партии вышел за допустимый диапозон");
            }

            return new DataPoints(batchNumber, nowTime, value, time);
        }
    }
}
