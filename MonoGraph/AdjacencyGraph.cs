using System.Collections.Generic;
using System.Linq;

namespace MonoGraph
{
    public class AdjacencyListGraph<TVertex, TEdge> where TEdge : IEdgeInterface<TVertex, TEdge>
    {
        private readonly Dictionary<TVertex, List<TEdge>> VertexEdgeDictionary;

        public AdjacencyListGraph()
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

        // Throws an exception if the edge already exists or the vertices don't exist
        private void CheckEdge(TEdge edge)
        {
            if (ContainsEdge(edge)) {
                throw new DuplicateEdgeException(string.Format("Tried to add edge '{0}' to the graph twice", edge));
            }

            if (!VertexEdgeDictionary.ContainsKey(edge.Start)){
                var errorMessage = string.Format("Tried to add edge to non-existant vertex '{0}'", edge.Start);
                throw new VertexNotFoundException(errorMessage);
            }

            if (!VertexEdgeDictionary.ContainsKey(edge.End)){
                var errorMessage = string.Format("Tried to add edge to non-existant vertex '{0}'", edge.End);
                throw new VertexNotFoundException(errorMessage);
            }
        }

        public void AddDirectedEdge(TEdge edge)
        {
            CheckEdge(edge);

            VertexEdgeDictionary[edge.Start].Add(edge);
        }

        public void AddBidirectionalEdge(TEdge edge)
        {
            var rerverseEdge = edge.Reversed();

            CheckEdge(edge);
            CheckEdge(rerverseEdge);

            VertexEdgeDictionary[edge.Start].Add(edge);
            VertexEdgeDictionary[edge.End].Add(rerverseEdge);
        }

        // Removing a vertex implies removing all the edges that go out from it
        // or go to it. This has to search edge lists of all vertices, so is
        // O(n*m) where n is the number of vertices and m is the average length
        // of the edge lists. May have to redesign to make removal less costly
        public void RemoveVertex(TVertex vertexToRemove)
        {
            VertexEdgeDictionary.Remove(vertexToRemove);

            foreach(var vertex in VertexIterator()){
                var edges = VertexEdgeDictionary[vertex];

                var index = FindEdgeContainingVertex(vertexToRemove, edges);
                if (index >= 0){
                    edges.RemoveAt(index);
                }
            }
        }

        // Return index of edge in edges that contains vertex, or -1
        public static int FindEdgeContainingVertex(TVertex vertex, IList<TEdge> edges){
            foreach(var i in Enumerable.Range(0, edges.Count))
            {
                if ((edges[i].Start.Equals(vertex)) || (edges[i].End.Equals(vertex))){
                    return i;
                }
            }

            return -1;
        }

        // Removing an edge is simple, lookup edge.Start and remove edge from its edge list
        public void RemoveEdge(TEdge edge)
        {
            if (VertexEdgeDictionary.ContainsKey(edge.Start))
            {
                if (VertexEdgeDictionary[edge.Start].Contains(edge)){
                    VertexEdgeDictionary[edge.Start].Remove(edge);
                }
                else{
                    var errorTemplate = "Vertex '{0}' exists, but can't find edge '{0}'";
                    throw new EdgeNotFoundException(string.Format(errorTemplate, edge));
                }
            }
            else
            {
                var errorTemplate = "Can't find vertex '{0}' while removing edge '{1}'";
                throw new VertexNotFoundException(string.Format(errorTemplate, edge.Start, edge));
            }
        }

        // Remove edge from edge.Start, remove edge.Reversed() from edge.End
        public void RemoveBidirectionalEdge(TEdge edge)
        {
            var errorTemplate = "Can't find vertex '{0}' while trying to remove bidirectional edge '{1}'";
            if (!ContainsVertex(edge.Start)) {
                throw new VertexNotFoundException(string.Format(errorTemplate, edge.Start, edge));
            }

            if (!ContainsVertex(edge.End)) {
                throw new VertexNotFoundException(string.Format(errorTemplate, edge.End, edge));
            }

            VertexEdgeDictionary[edge.Start].Remove(edge);
            VertexEdgeDictionary[edge.End].Remove(edge.Reversed());
        }

        public bool ContainsVertex(TVertex vertex)
        {
            return VertexEdgeDictionary.ContainsKey(vertex);
        }

        public bool ContainsEdge(TEdge edge)
        {
            return VertexEdgeDictionary.ContainsKey(edge.Start)
                && VertexEdgeDictionary[edge.Start].Contains(edge);
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

        public IEnumerable<TEdge> AllEdgeIterator()
        {
            foreach(TVertex vertex in VertexEdgeDictionary.Keys)
            {
                foreach(TEdge edge in VertexEdgeDictionary[vertex])
                {
                    yield return edge;
                }
            }
        }
    }
}
