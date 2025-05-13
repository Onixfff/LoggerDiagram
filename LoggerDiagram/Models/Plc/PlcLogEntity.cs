using System;

namespace LoggerDiagram.Models.Plc
{
    public class PlcLogEntity
    {
        public readonly Byte RawByteValue;
        public readonly double Value;
        public readonly int Time;

        private PlcLogEntity(byte rawByteValue, double value, int time)
        {
            RawByteValue = rawByteValue;
            Value = value;
            Time = time;
        }

        public static PlcLogEntity Create(byte rawByteValue, double value, int time)
        {
            //Проверки

            return new PlcLogEntity(rawByteValue, value, time);
        }
    }
}
