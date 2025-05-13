using System.Collections.Generic;

namespace LoggerDiagram.Services.Interfaces
{
    public interface IGraphIdSplitter
    {
        (List<int> eventIds, List<int> oddIds) Split(List<int> ids);
    }
}
