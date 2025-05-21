using LoggerDiagram.DTO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram.Services.Interfaces
{
    public interface IPlcDataSender
    {
        Task SendAsync(IEnumerable<PlcLogEntityDto> dtos, CancellationToken token, int maxRetries = 3);
    }
}
