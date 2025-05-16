using LoggerDiagram.Models.Plc;

namespace LoggerDiagram.Services.Interfaces
{
    /// <summary>
    /// Реализация <see cref="IGraphStateManager"/> для хранения состояния графиков в оперативной памяти.
    /// </summary>
    public interface IGraphStateManager
    {
        GraphState GetState(int graphId);
        void UpdateState(int graphId, GraphState state);
    }
}
