using LoggerDiagram.Models.Plc;

namespace LoggerDiagram.Services.Interfaces
{
    public interface IBatchNumberAdjuster
    {
        int Adjust(int graphId, int lastBatchNumber, PlcSensorReading entity);
    }
}