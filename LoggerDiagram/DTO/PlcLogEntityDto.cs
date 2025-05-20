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

        /// <summary>
        /// Создаёт корректный экземпляр <see cref="PlcLogEntityDto"/> после проверки входных данных.
        /// </summary>
        /// <param name="idGraph">Идентификатор графика</param>
        /// <param name="batchNumber">Номер партии</param>
        /// <param name="status">Состояние продукта</param>
        /// <param name="value">Значение с датчика</param>
        /// <param name="time">Временная метка</param>
        /// <returns>Экземпляр <see cref="PlcLogEntityDto"/></returns>
        /// <exception cref="ArgumentOutOfRangeException">Возникает если:
        /// <list type="bullet">
        /// <item><paramref name="idGraph"/> <= 0</item>
        /// <item><paramref name="batchNumber"/> < 0</item>
        /// <item><paramref name="value"/> < 0</item>
        /// <item><paramref name="time"/> < 0</item>
        /// </list>
        /// </exception>
        public static PlcLogEntityDto Create(int idGraph, int batchNumber, ProductState status, double value, int time)
        {
            //Проверка
            if(idGraph <= 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(idGraph)} вышел за пределы допустимого диапозона");
            }

            if (value < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(value)} вышел за пределы допустимого диапозона");
            }

            if (time < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(time)} вышел за пределы допустимого диапозона");
            }

            if(batchNumber < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(batchNumber)} вышел за пределы допустимого диапозона");
            }

            //Создание

            return new PlcLogEntityDto(idGraph, batchNumber, status, value, time);
        }

        /// <summary>
        /// Проверяет, является ли текущий объект базовым (пустым) значением.
        /// Используется для пропуска некорректных или пустых записей без использования null.
        /// </summary>
        /// <returns>True, если объект представляет собой "базовое" значение.</returns>
        public bool IsEmpty()
        {
            if( IdGraph == 0 &&
                BatchNumber == 0 &&
                Status == ProductState.ZeroProduct &&
                Value == 0 &&
                Time == 0 )
            { 
                return true; 
            }
            else 
            { 
                return false; 
            }
        }
        
        /// <summary>
        /// Создаёт "базовое" значение <see cref="PlcLogEntityDto"/>, 
        /// которое можно использовать как замену null для безопасной обработки.
        /// </summary>
        /// <returns>Базовый экземпляр <see cref="PlcLogEntityDto"/>.</returns>
        public static PlcLogEntityDto CreateBaseValue()
        {
            return new PlcLogEntityDto(0, 0, ProductState.ZeroProduct, 0, 0);
        }
    }
}
