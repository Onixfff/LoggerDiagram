using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LoggerDiagram.DTO;
using LoggerDiagram.Models.Plc;

namespace LoggerDiagram.Services.Interfaces
{
    public interface IPlcDataConverter
    {
        Task<List<PlcLogEntityDto>> ConvertAsync(List<int> ids, List<PlcSensorReading> entities, CancellationToken token);
    }
}
