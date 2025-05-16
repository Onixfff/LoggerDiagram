using System;
using LoggerDiagram.Models.Plc;
using LoggerDiagram.Services.Interfaces;
using System.Collections.Generic;
using NLog;

namespace LoggerDiagram.Services
{
    /// <summary>
    /// Реализация <see cref="IGraphStateManager"/> для хранения состояния графиков в оперативной памяти.
    /// </summary>
    public class InMemoryGraphStateManager : IGraphStateManager
    {
        private readonly ILogger _logger;
        private readonly Dictionary<int, GraphState> _state = new Dictionary<int, GraphState>();

        public InMemoryGraphStateManager(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Получает текущее состояние графика по его идентификатору. 
        /// Если состояние отсутствует, создается новое.
        /// </summary>
        /// <param name="graphId">Идентификатор графика.</param>
        /// <returns>Текущее состояние графика.</returns>
        public GraphState GetState(int graphId)
        {
            if (_state.TryGetValue(graphId, out var state))
            {
                state = new GraphState();
                _state[graphId] = state;
                _logger.Debug($"Создано новое состояние для графика {graphId}");
            }

            _logger.Trace($"Получено состояние для графика {graphId}: {state}");
            return state;
        }

        /// <summary>
        /// Обновляет состояние указанного графика.
        /// </summary>
        /// <param name="graphId">Идентификатор графика.</param>
        /// <param name="state">Новое состояние графика.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void UpdateState(int graphId, GraphState state)
        {
            if (state == null)
            {
                _logger.Warn("Попытка установить null в состояние графика с Id: {GraphId}", graphId);
                throw new ArgumentNullException(nameof(state), "Состояние графика не может быть null.");
            }

            _state[graphId] = state;
            _logger.Trace("Состояние графика с Id: {GraphId} успешно обновлено.", graphId);
        }

    }
}
