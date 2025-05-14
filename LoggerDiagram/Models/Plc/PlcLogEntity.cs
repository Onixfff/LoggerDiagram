using System;

namespace LoggerDiagram.Models.Plc
{
    public class PlcLogEntity
    {
        public readonly Byte RawByteValue;
        public readonly double Value;
        public readonly short Time;

        private PlcLogEntity(byte rawByteValue, double value, short time)
        {
            RawByteValue = rawByteValue;
            Value = value;
            Time = time;
        }

        public static PlcLogEntity Create(byte rawByteValue, double value, short time)
        {
            //Проверки

            return new PlcLogEntity(rawByteValue, value, time);
        }
    }
}
