using LoggerDiagram.Models.Plc;

namespace LoggerDiagram.Services
{
    public interface IBatchNumberAdjuster
    {
        int Adjust(int lastBatchNumber, PlcLogEntity entity);
    }
}