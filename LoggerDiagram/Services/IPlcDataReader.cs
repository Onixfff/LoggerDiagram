using LoggerDiagram.Models.Plc;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram.Services
{
    public interface IPlcDataReader
    {
        Task<PlcLogEntry> GetDataAsync(int byteStart, int doubleStart, int timeStart, CancellationToken token);
    }
}