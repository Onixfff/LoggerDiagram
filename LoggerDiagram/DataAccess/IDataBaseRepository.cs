using LoggerDiagram.DTO;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram.DataAccess
{
    public interface IDataBaseRepository
    {
        Task SendDataAsync(PlcLogEntryDto plcLogEntryDto, CancellationToken token);

        Task<int> GetLastBatchNumberByGraphAsync(int idGraph, CancellationToken token);
    }
}
