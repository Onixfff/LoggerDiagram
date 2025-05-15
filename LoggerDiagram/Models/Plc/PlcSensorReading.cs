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
        //TODO Добавить get set и работы с ними для этих данных
        /// <summary>
        /// Состояние устройства, характеризующее текущий статус (например, наличие продукта, или его отсутствие, ошибку)
        /// </summary>
        public readonly ProductState Status;
        /// <summary>
        /// Номер помещения, от которого получены данные (если доступен).
        /// </summary>
        public readonly Byte? RoomNumber;
        /// <summary>
        /// числовое значение измерения.
        /// </summary>
        public readonly double Value;
        /// <summary>
        /// Временная метка события, связанного с измерением.
        /// </summary>
        public readonly short Time;

        /// <summary>
        /// Представляет 
        /// </summary>
        /// <param name="statusByte">Enum состояния.</param>
        /// <param name="roomNumber">Номер помещения, от которого получено значение.</param>
        /// <param name="value">Числовое значение измерения.</param>
        /// <param name="time">Временная метка события.</param>
        private PlcSensorReading(ProductState statusByte, byte roomNumber, double value, short time)
        {
            Status = statusByte;
            RoomNumber = roomNumber;
            Value = value;
            Time = time;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="PlcSensorReading"/>.
        /// </summary>
        /// <param name="statusByte">Enum состояния.</param>
        /// <param name="rawByteRoom">Номер помещения, от которого получено значение.</param>
        /// <param name="value">Числовое значение измерения.</param>
        /// <param name="time">Временная метка события.</param>
        /// <returns>Обьект <see cref="PlcSensorReading"/></returns>
        public static PlcSensorReading Create(ProductState statusByte, byte rawByteRoom, double value, short time)
        {
            //TODO Проверки данных добавить
            return new PlcSensorReading(statusByte, rawByteRoom, value, time);
        }
    }
}
