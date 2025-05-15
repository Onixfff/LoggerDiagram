using LoggerDiagram.Enums;
using System;

namespace LoggerDiagram.Models.Plc
{
    /// <summary>
    /// Представляет показания датчика, считанные с PLC.
    /// Содержит значение статуса, номера комнаты, измерения, времени
    /// </summary>
    public class PlcSensorReading
    {
        /// <summary>
        /// Состояние устройства, характеризующее текущий статус (например, наличие продукта, или его отсутствие, ошибку)
        /// </summary>
        public ProductState Status;
        
        public Byte? RoomNumber { get; private set; }
        public double Value;
        public short Time;

        /// <summary>
        /// Представляет 
        /// </summary>
        /// <param name="statusByte"></param>
        /// <param name="roomNumber"></param>
        /// <param name="value"></param>
        /// <param name="time"></param>
        private PlcSensorReading(ProductState statusByte, byte roomNumber, double value, short time)
        {
            Status = statusByte;
            RoomNumber = roomNumber;
            Value = value;
            Time = time;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawByteValue"></param>
        /// <param name="rawByteRoom"></param>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <returns>Обьект <see cref="PlcSensorReading"/></returns>
        public static PlcSensorReading Create(byte rawByteValue, byte rawByteRoom, double value, short time)
        {
            //TODO Проверки данных добавить
            return new PlcSensorReading(rawByteValue, rawByteRoom, value, time);
        }
    }
}
