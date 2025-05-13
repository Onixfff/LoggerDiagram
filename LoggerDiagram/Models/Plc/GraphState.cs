using System;

namespace LoggerDiagram.Models.Plc
{
    public class GraphState
    {
        public int LastBatchNumber { get; set; }
        public byte? LastRawByteValue { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public void MarkAsUpdated()
        {
            LastUpdated = DateTime.UtcNow;
            IsActive = true;
        }
    }
}
