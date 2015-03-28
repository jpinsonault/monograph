using System.Collections.Generic;
using System.Linq;
using System;


namespace MonoGraph.Algorithms
{
	class DijkstraShortestPath<TVertex, TEdge> 
		where TEdge : IEdgeInterface<TVertex, TEdge>
	{
		private readonly AdjacencyListGraph<TVertex, TEdge> Graph;
		private readonly Dictionary<TEdge, double> EdgeCosts;
		public readonly Dictionary<TVertex, double> ComputedCosts;
		public readonly Dictionary<TVertex, TVertex> ComputedPaths;

		public DijkstraShortestPath(AdjacencyListGraph<TVertex, TEdge> graph, Dictionary<TEdge, double> edgeCosts)
		{
			this.Graph = graph;
			this.EdgeCosts = edgeCosts;

			ComputedCosts = new Dictionary<TVertex, double>();

			// The absence of a route entry in ComputedPaths means it's unreachable
			ComputedPaths = new Dictionary<TVertex, TVertex>();

			// Start every distance to infinity
			foreach(TVertex vertex in Graph.VertexIterator()){
				ComputedCosts.Add(vertex, float.PositiveInfinity);
			}
		}

		// Compute distance from startVertex to all other vertices
		public void ComputeAllFromVertex(TVertex startVertex)
		{
			ComputedCosts[startVertex] = 0.0;
			ComputedPaths[startVertex] = startVertex;

			var remainingVertices = new HashSet<TVertex>(Graph.VertexIterator());

			while (remainingVertices.Count > 0) {
				var currentVertex = popShortestRemaining(remainingVertices);
				// Cost to get to current vertex
				var cummulativeCost = ComputedCosts[currentVertex];

				foreach(TEdge edgeToNeighbor in Graph.EdgeIterator(currentVertex)){
					var neighbor = edgeToNeighbor.End;
					var neighborEdgeCost = EdgeCosts[edgeToNeighbor];
					var neighborComputedCost = ComputedCosts[neighbor];

					var costFromCurrent = cummulativeCost + neighborEdgeCost;

					// Cost to neighbor is either what it already was, or the cost
					// to get here plus the cost of the edge
					if (costFromCurrent < neighborComputedCost){
						ComputedCosts[neighbor] = costFromCurrent;
						ComputedPaths[neighbor] = currentVertex;
					}
				}
			}
		}

		// Returns and removes the vertex with the shortest distance from vertices
		private TVertex popShortestRemaining(ICollection<TVertex> vertices)
		{
			var shortest = vertices.OrderBy(v => ComputedCosts[v]).First();
			vertices.Remove(shortest);

			return shortest;
		}
	}
}