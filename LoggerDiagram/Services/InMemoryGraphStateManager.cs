using LoggerDiagram.Models.Plc;
using LoggerDiagram.Services.Interfaces;
using System.Collections.Generic;

namespace LoggerDiagram.Services
{
    public class InMemoryGraphStateManager : IGraphStateManager
    {
        private readonly Dictionary<int, GraphState> _state = new Dictionary<int, GraphState>();

        public GraphState GetState(int graphId)
        {
            if (!_state.TryGetValue(graphId, out var state))
            {
                _state[graphId] = new GraphState();
            }
            
            return _state[graphId];
        }

        public void UpdateState(int graphId, GraphState state)
        {
            _state[graphId] = state;
        }

    }
}
