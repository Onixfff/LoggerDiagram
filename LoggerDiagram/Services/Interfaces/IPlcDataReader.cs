using LoggerDiagram.Models.Plc;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerDiagram.Services
{
    public interface IPlcDataReader
    {
        Task<PlcSensorReading> GetDataWithRetryAsync(int byteStart, int byteStartNameRoom, int doubleStart, int timeStart, CancellationToken token, int maxRetries = 3);
    }
}