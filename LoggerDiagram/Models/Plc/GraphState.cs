using LoggerDiagram.Enums;
using System;

namespace LoggerDiagram.Models.Plc
{
    public class GraphState
    {
        public ProductState status { get; set; }
        public ProductState LastStatus { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public void MarkAsUpdated()
        {
            LastUpdated = DateTime.UtcNow;
            IsActive = true;
        }
    }
}
