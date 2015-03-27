using System.Collections.Generic;

namespace MonoGraph
{
    public class AdjacencyGraph<TVertex, TEdge> where TEdge : IEdgeInterface<TVertex, TEdge>, new()
    {
        private readonly Dictionary<TVertex, List<TEdge>> VertexEdgeDictionary;

        public AdjacencyGraph()
        {
            VertexEdgeDictionary = new Dictionary<TVertex, List<TEdge>>();
        }

        public void AddVertex(TVertex vertex)
        {
            if (ContainsVertex(vertex)) {
                throw new DuplicateVertexException(string.Format("Tried to add vertex '{0}' to the graph twice", vertex));
            }

            VertexEdgeDictionary.Add(vertex, new List<TEdge>());
        }

        public void AddDirectedEdge(TEdge edge)
        {
            if (ContainsEdge(edge)) {
                throw new DuplicateEdgeException(string.Format("Tried to add edge '{0}' to the graph twice", edge));
            }

            VertexEdgeDictionary[edge.First].Add(edge);
        }

        public void AddBidirectionalEdge(TEdge edge)
        {
            var rerverseEdge = edge.Reversed();

            if (ContainsEdge(edge)) {
                throw new DuplicateEdgeException(string.Format("Tried to add edge '{0}' to the graph twice", edge));
            }

            if (ContainsEdge(rerverseEdge)) {
                throw new DuplicateEdgeException(string.Format("Tried to add edge '{0}' to the graph twice", rerverseEdge));
            }

            VertexEdgeDictionary[edge.First].Add(edge);
            VertexEdgeDictionary[edge.Second].Add(rerverseEdge);
        }

        public bool ContainsVertex(TVertex vertex)
        {
            return VertexEdgeDictionary.ContainsKey(vertex);
        }

        public bool ContainsEdge(TEdge edge)
        {
            return VertexEdgeDictionary[edge.First].Contains(edge);
        }

        public IEnumerable<TVertex> VertexIterator()
        {
            foreach(TVertex vertex in VertexEdgeDictionary.Keys)
            {
                yield return vertex;
            }
        }

        public IEnumerable<TEdge> EdgeIterator(TVertex vertex)
        {
            foreach(TEdge edge in VertexEdgeDictionary[vertex])
            {
                yield return edge;
            }
        }
    }
}
