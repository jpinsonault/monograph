using System.Collections.Generic;
using System.Linq;
using System;


namespace MonoGraph.Algorithms
{
	class DijkstraShortestPath<TVertex, TEdge> 
		where TEdge : IEdgeInterface<TVertex, TEdge>
		where TVertex: struct
	{
		private readonly AdjacencyGraph<TVertex, TEdge> Graph;
		private readonly Dictionary<TEdge, double> EdgeCosts;
		public readonly Dictionary<TVertex, double> ComputedCosts;
		public readonly Dictionary<TVertex, TVertex?> Routes;

		public DijkstraShortestPath(AdjacencyGraph<TVertex, TEdge> graph, Dictionary<TEdge, double> edgeCosts)
		{
			this.Graph = graph;
			this.EdgeCosts = edgeCosts;

			ComputedCosts = new Dictionary<TVertex, double>();
			Routes = new Dictionary<TVertex, TVertex?>();

			// Start every route as null
			foreach(TVertex vertex in Graph.VertexIterator()){
				Routes.Add(vertex, null);
			}

			// Start every distance to infinity
			foreach(TVertex vertex in Graph.VertexIterator()){
				ComputedCosts.Add(vertex, float.PositiveInfinity);
			}
		}

		// Compute distance from startVertex to all other vertices
		public void Compute(TVertex startVertex)
		{
			ComputedCosts[startVertex] = 0.0;

			var remainingVertices = new HashSet<TVertex>(Graph.VertexIterator());

			while (remainingVertices.Count > 0) {
				var currentVertex = popShortestRemaining(remainingVertices);
				// Cost to get to current vertex
				var cummulativeCost = ComputedCosts[currentVertex];

				foreach(TEdge edgeToNeighbor in Graph.EdgeIterator(currentVertex)){
					var neighbor = edgeToNeighbor.Second;
					var neighborEdgeCost = EdgeCosts[edgeToNeighbor];
					var neighborComputedCost = ComputedCosts[neighbor];

					// Cost to neighbor is either what it already was, or the cost
					// to get here plus the cost of the edge
					ComputedCosts[neighbor] = Math.Min(neighborComputedCost, cummulativeCost + neighborEdgeCost);
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