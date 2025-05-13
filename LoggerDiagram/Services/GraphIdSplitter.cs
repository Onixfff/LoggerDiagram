using LoggerDiagram.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace LoggerDiagram.Services
{
    public class GraphIdSplitter : IGraphIdSplitter
    {
        public (List<int> eventIds, List<int> oddIds) Split(List<int> ids)
        {
            if(ids == null || ids.Count == 0)
                throw new ArgumentNullException($"Список {nameof(ids)} пуст или null");

            var even = new List<int>();
            var odd = new List<int>();

            foreach (var id in ids)
            {
                if(id % 2 == 0)
                {
                    even.Add(id);
                }
                else
                {
                    odd.Add(id);
                }
            }

            return (even, odd);
        }
    }
}