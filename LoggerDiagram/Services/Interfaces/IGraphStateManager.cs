using LoggerDiagram.Models.Plc;

namespace LoggerDiagram.Services.Interfaces
{
    /// <summary>
    /// Реализация <see cref="IGraphStateManager"/> для хранения состояния графиков в оперативной памяти.
    /// </summary>
    public interface IGraphStateManager
    {
        /// <summary>
        /// Получает текущее состояние графика по его идентификатору. 
        /// Если состояние отсутствует, создается новое.
        /// </summary>
        /// <param name="graphId">Идентификатор графика.</param>
        /// <returns>Текущее состояние графика.</returns>
        GraphState GetState(int graphId);
        
        /// <summary>
        /// Обновляет состояние указанного графика.
        /// </summary>
        /// <param name="graphId">Идентификатор графика.</param>
        /// <param name="state">Новое состояние графика.</param>
        /// <exception cref="ArgumentNullException"></exception>
        void UpdateState(int graphId, GraphState state);
    }
}
