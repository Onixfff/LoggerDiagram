using System;

namespace LoggerDiagram.Models.Plc
{
    public class PlcSensorReading
    {
        public Byte StatusByte;
        public Byte? RoomNumber { get; private set; }
        public double Value;
        public short Time;

        private PlcSensorReading(byte statusByte, byte roomNumber, double value, short time)
        {
            StatusByte = statusByte;
            RoomNumber = roomNumber;
            Value = value;
            Time = time;
        }

        public static PlcSensorReading Create(byte rawByteValue, byte rawByteRoom, double value, short time)
        {
            //TODO Проверки данных добавить
            return new PlcSensorReading(rawByteValue, rawByteRoom, value, time);
        }
    }
}
