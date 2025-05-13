using LoggerDiagram.Models.Plc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram.Services
{
    public interface IPlcReaderService
    {
        Task<List<PlcLogEntity>> ReadPlcLogsAsync(List<int> ids, IPlcDataReader reader, CancellationToken token);
    }
}
