using LoggerDiagram.DataAccess;
using LoggerDiagram.DTO;
using LoggerDiagram.Services.Interfaces;
using NLog;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram.Services
{
    internal class PlcDataSender : IPlcDataSender
    {
        private IDataBaseRepository _repository;
        private ILogger _logger;

        public PlcDataSender(IDataBaseRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task SendAsync(IEnumerable<PlcLogEntityDto> dtos, CancellationToken token)
        {
            foreach (var dto in dtos)
            {
                await _repository.SendDataAsync(dto, token);
                _logger.Info("Отправлено {0}", dto.IdGraph);
            }
        }
    }
}
