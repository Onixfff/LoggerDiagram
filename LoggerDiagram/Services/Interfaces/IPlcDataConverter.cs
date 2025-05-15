using LoggerDiagram.DTO;
using LoggerDiagram.Models.Plc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace LoggerDiagram.Services
{
    public interface IPlcDataConverter
    {
        Task<List<PlcLogEntityDto>> ConvertAsync(List<int> ids, List<PlcSensorReading> entities, CancellationToken token);
    }
}
