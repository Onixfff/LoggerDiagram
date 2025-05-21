using NLog;
using System;
using System.Threading;
using System.Configuration;
using LoggerDiagram.Services;
using System.Threading.Tasks;
using LoggerDiagram.DataAccess;
using System.Collections.Generic;
using LoggerDiagram.Services.Interfaces;

namespace LoggerDiagram.Application
{
    /// <summary>
    /// Предоставляет функционал для обработки данных с ПЛК: чтение, преобразование и сохранение в БД.
    /// </summary>
    public class PlcDataProcessor
    {
        private readonly IDataBaseRepository _repository;
        private readonly IPlcReaderService _readerService;
        private readonly IPlcDataConverter _converter;
        private readonly IPlcDataSender _sender;
        private readonly IPlcDataReaderFactory _readerFactory;
        private readonly ILogger _logger;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="PlcDataProcessor"/>.
        /// </summary>
        /// <param name="repository">Репозиторий для работы с БД</param>
        /// <param name="readerService">Сервис для чтения данных с ПЛК</param>
        /// <param name="converter">Сервис для преобразования данных в DTO</param>
        /// <param name="sender">Сервис для отправки данных в БД</param>
        /// <param name="readerFactory">Фабрика для создания читателя ПЛК</param>
        /// <param name="logger">Инструмент для логирования</param>
        public PlcDataProcessor(IDataBaseRepository repository,IPlcReaderService readerService, IPlcDataConverter converter, IPlcDataSender sender, IPlcDataReaderFactory readerFactory, ILogger logger)
        {
            _repository = repository;
            _readerService = readerService;
            _converter = converter;
            _sender = sender;
            _readerFactory = readerFactory;
            _logger = logger;
        }

        /// <summary>
        /// Асинхронно запускает процесс обработки данных с ПЛК.
        /// Получает список ID графиков, считывает данные, преобразует их и отправляет в БД.
        /// </summary>
        /// <param name="token">Токен отмены для прерывания операции.</param>
        public async Task ProcessAsync(CancellationToken token)
        {
            string ipEven = ConfigurationManager.AppSettings["PlcEven"];

            try
            {
                ValidateIps(ipEven);
                
                var allIds = await GetAllGraphIdsAsync(token);
                
                await ProcessGroupAsync(allIds, ipEven, token);
                
                _logger.Info("Обработка завершена для IP: {Ip}", ipEven);

            }
            catch (ArgumentNullException ex)
            {
                _logger.Error(ex, "Не переданы параметры: {Message}", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Критическая ошибка");
            }
        }

        /// <summary>
        /// Проверяет, что строка IP-адреса корректна.
        /// </summary>
        /// <param name="ip">IP-адрес ПЛК</param>
        /// <exception cref="ArgumentNullException">Если IP равен null или пустой строки</exception>
        private void ValidateIps(string ipEven)
        {
            if (string.IsNullOrWhiteSpace(ipEven))
                throw new ArgumentNullException(nameof(ipEven), "IP для PLC не задан");
        }

        /// <summary>
        /// Получает список идентификаторов графиков из базы данных.
        /// </summary>
        /// <param name="token">Токен отмены для прерывания операции.</param>
        /// <returns>Список идентификаторов графиков</returns>
        /// <exception cref="InvalidOperationException">Если получить список не удалось</exception>
        private async Task<List<int>> GetAllGraphIdsAsync(CancellationToken token)
        {
            var ids = await _repository.GetAllGraphIdsAsync(token);

            if (ids == null)
                throw new InvalidOperationException("Не удалось получить список id графиков");

            return ids;
        }

        /// <summary>
        /// Обрабатывает группу данных с ПЛК: считывает, конвертирует, отправляет в БД.
        /// </summary>
        /// <param name="ids">Список идентификаторов графиков</param>
        /// <param name="ip">IP-адрес ПЛК</param>
        /// <param name="token">Токен отмены для прерывания операции</param>
        private async Task ProcessGroupAsync(List<int> ids, string ip,  CancellationToken token)
        {
            try
            {
                IPlcDataReader reader = _readerFactory.Create(ip);
                var data = await _readerService.GetPlcSensorReadingsAsync(ids, reader, token);

                if (data == null)
                {
                    _logger.Warn("Данные с PLC равны null для IP: {Ip}", ip);
                    return;
                }

                var dtos = await _converter.ConvertAsync(ids, data, token);

                if (dtos == null || dtos.Count <= 0)
                {
                    _logger.Warn("Не найдено данных для отправки для IP: {Ip}", ip);
                    return;
                }

                await _sender.SendAsync(dtos, token);
            }
            catch (ArgumentNullException ex)
            {
                _logger.Error(ex, "Ошибка: Неверные аргументы при обработке группы для IP: {Ip}", ip);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Неизвестная ошибка при обработке группы для IP: {Ip}", ip);
            }
        }
    }
}
