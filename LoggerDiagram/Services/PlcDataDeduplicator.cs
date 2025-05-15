using LoggerDiagram.DTO;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerDiagram.Services
{
    /// <summary>
    /// Сервис для удаления дубликатов данных из ПЛК на основе времени и IdGraph.
    /// </summary>
    public class PlcDataDeduplicator
    {
        private readonly ILogger _logger;

        public PlcDataDeduplicator(ILogger logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// Проверяет, является ли новая запись дубликатом существующей по IdGraph, Time и Value.
        /// </summary>
        /// <param name="newData">Новая запись.</param>
        /// <param name="oldData">Существующая запись.</param>
        /// <returns>Возвращает null, если это дубликат. В противном случае возвращает <see cref="PlcLogEntityDto"/> newData.</returns>
        private PlcLogEntityDto TryGetNonDuplicateAsync(PlcLogEntityDto newData, PlcLogEntityDto oldData)
        {
            if (newData == null)
            {
                _logger.Error($"{nameof(newData)} = null");
                throw new ArgumentNullException(nameof(newData));
            }

            if (oldData == null)
            {
                _logger.Warn($"{nameof(oldData)} = null");
                return newData;
            }

            // Проверяем, совпадает ли IdGraph и Time, и оба имеют статус IsHaveProduct
            if (newData.IdGraph == oldData.IdGraph &&
                newData.Status == Enums.ProductState.IsHaveProduct &&
                oldData.Status == Enums.ProductState.IsHaveProduct &&
                newData.Time == oldData.Time &&
                newData.Value == oldData.Value)
            {
                return null; // Это дубликат
            }

            return newData;
        }

        /// <summary>
        /// Фильтрует список новых данных, исключая те, которые являются дубликатами старых.
        /// </summary>
        /// <param name="newDataList">Список новых данных.</param>
        /// <param name="oldDataList">Список существующих данных.</param>
        /// <returns>Список уникальных записей.</returns>
        public List<PlcLogEntityDto> Deduplicate(List<PlcLogEntityDto> newDataList, List<PlcLogEntityDto> oldDataList)
        {
            if (newDataList == null)
            {
                _logger.Error(new ArgumentException(), "");
                throw new ArgumentNullException(nameof(newDataList));
            }
            if (oldDataList == null)
            {

                throw new ArgumentNullException(nameof(oldDataList));
            }

            var result = new List<PlcLogEntityDto>();
            
            foreach (var newData in newDataList)
            {
                // Находим все записи с тем же IdGraph среди старых
                var sameGraphOldData = oldDataList.Where(x => x.IdGraph == newData.IdGraph).ToList();

                // Проверяем на дубликаты
                bool isDuplicate = false;

                foreach (var oldData in sameGraphOldData)
                {
                    var checkResult = TryGetNonDuplicateAsync(newData, oldData);

                    if (checkResult == null)
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (!isDuplicate)
                {
                    result.Add(newData);
                }
            }

            return result;
        }
    }
}
