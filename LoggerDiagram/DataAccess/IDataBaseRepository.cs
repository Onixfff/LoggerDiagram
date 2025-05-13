using LoggerDiagram.DTO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram.DataAccess
{
    public interface IDataBaseRepository
    {
        Task SendDataAsync(PlcLogEntityDto plcLogEntryDto, CancellationToken token);

        Task<int> GetLastBatchNumberByGraphAsync(int idGraph, CancellationToken token);

        Task<List<int>> GetAllGraphIdsAsync(CancellationToken token);
    }
}
