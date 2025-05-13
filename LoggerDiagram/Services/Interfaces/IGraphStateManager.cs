using LoggerDiagram.Models.Plc;

namespace LoggerDiagram.Services.Interfaces
{
    public interface IGraphStateManager
    {
        GraphState GetState(int graphId);
        void UpdateState(int graphId, GraphState state);
    }
}
