using LoggerDiagram.DTO;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram.DataAccess
{
    internal interface IDataBaseRepository
    {
        Task SendDataAsync(PlcLogEntryDto plcLogEntryDto, CancellationToken token);
    }
}
