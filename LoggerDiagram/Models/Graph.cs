namespace LoggerDiagram.Models
{
    internal class Graph
    {
        private readonly int Id;
        private readonly string Comment;

        private Graph(int id, string comment)
        {
            Id = id;
            Comment = comment;
        }

        public static Graph Create(int id, string comment)
        {
            var graph = new Graph(id, comment);
            return graph;
        }
    }
}
