using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LoggerDiagram.DTO;
using LoggerDiagram.Models.Plc;
using MySql.Data.MySqlClient;

namespace LoggerDiagram.Services.Interfaces
{
    /// <summary>
    /// Сервис для конвертации данных с ПЛК (<see cref="PlcSensorReading"/>) в DTO (<see cref="PlcLogEntityDto"/>).
    /// </summary>
    public interface IPlcDataConverter
    {
        /// <summary>
        /// Асинхронно конвертирует список данных с ПЛК в список DTO.
        /// Если количество входных ID не совпадает с количеством данных — обрабатывается минимальное из них.
        /// </summary>
        /// <param name="ids">Список идентификаторов графиков.</param>
        /// <param name="entity">Список данных с ПЛК.</param>
        /// <param name="token">Токен отмены для прерывания операции.</param>
        /// <returns>Список объектов <see cref="PlcLogEntityDto"/>.</returns>
        /// <exception cref="ArgumentNullException">Выбрасывается, если <paramref name="ids"/> или <paramref name="entity"/> равны null.</exception>
        /// <exception cref="OperationCanceledException">Если операция была отменена.</exception>
        /// <exception cref="MySqlException">Ошибка со стороны базы данных.</exception>
        /// <exception cref="InvalidCastException">Ошибка преобразования данных.</exception>
        Task<List<PlcLogEntityDto>> ConvertAsync(List<int> ids, List<PlcSensorReading> entity, CancellationToken token);
    }
}
