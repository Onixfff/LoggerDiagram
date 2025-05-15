using System;

namespace LoggerDiagram.Models.Plc
{
    public class PlcLogEntity
    {
        public Byte RawByteValue;
        public Byte? RawByteRoom { get; private set; }
        public double Value;
        public int Time;

        private PlcLogEntity(byte rawByteValue, double value, int time)
        {
            RawByteValue = rawByteValue;
            Value = value;
            Time = time;
        }

        private PlcLogEntity(byte rawByteValue, byte rawByteValueRoom, double value, int time)
        {
            RawByteValue = rawByteValue;
            RawByteRoom = rawByteValueRoom;
            Value = value;
            Time = time;
        }

        public static PlcLogEntity Create(byte rawByteValue, double value, int time)
        {
            //TODO Проверки данных добавить
            return new PlcLogEntity(rawByteValue, value, time);
        }

        public static PlcLogEntity Create(byte rawByteValue, byte rawByteRoom, double value, int time)
        {
            //TODO Проверки данных добавить
            return new PlcLogEntity(rawByteValue, rawByteRoom, value, time);
        }
    }
}
