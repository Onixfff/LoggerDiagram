using System;

namespace LoggerDiagram.Models.Plc
{
    public class PlcLogEntry
    {
        public readonly Byte RawByteValue;
        public readonly double Value;
        public readonly int Time;

        private PlcLogEntry(byte rawByteValue, double value, int time)
        {
            RawByteValue = rawByteValue;
            Value = value;
            Time = time;
        }

        public static PlcLogEntry Create(byte rawByteValue, double value, int time)
        {
            //Проверки

            return new PlcLogEntry(rawByteValue, value, time);
        }
    }
}
