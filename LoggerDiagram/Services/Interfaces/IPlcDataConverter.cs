using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LoggerDiagram.DTO;
using LoggerDiagram.Models.Plc;

namespace LoggerDiagram.Services.Interfaces
{
    /// <summary>
    /// Сервис для конвертации данных с ПЛК (<see cref="PlcSensorReading"/>) в DTO (<see cref="PlcLogEntityDto"/>).
    /// </summary>
    public interface IPlcDataConverter
    {
        /// <summary>
        /// Асинхронно преобразует список идентификаторов графиков и соответствующие данные с ПЛК в список DTO (<see cref="PlcLogEntityDto"/>).
        /// Пары формируются по индексу: ids[0] + entity[0], ids[1] + entity[1] и т.д.
        /// Если количество элементов в списках не совпадает — будет обработано минимальное количество.
        /// </summary>
        /// <param name="ids">Список идентификаторов графиков</param>
        /// <param name="entity">Список данных с датчиков ПЛК</param>
        /// <param name="token">Токен отмены для прерывания операции</param>
        /// <returns>Список объектов <see cref="PlcLogEntityDto"/>. Возвращается пустой список, если ни одно значение не было обработано.</returns>
        /// <exception cref="ArgumentNullException">Если любой из списков равен null</exception>
        Task<List<PlcLogEntityDto>> ConvertAsync(List<int> ids, List<PlcSensorReading> entity, CancellationToken token);
    }
}
