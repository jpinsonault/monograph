using System.Collections.Generic;
using System.Linq;
using System;

namespace MonoGraph.Algorithms
{
	public class DijkstraShortestPath<TVertex> where TVertex : IComparable<TVertex>
	{
		private readonly IGraph<TVertex> Graph;
		private readonly Dictionary<IEdge<TVertex>, double> EdgeCosts;
		public readonly Dictionary<TVertex, double> ComputedCosts;
		public readonly Dictionary<TVertex, TVertex> ComputedPaths;

		public DijkstraShortestPath(IGraph<TVertex> graph, Dictionary<IEdge<TVertex>, double> edgeCosts)
		{
			Graph = graph;
			EdgeCosts = edgeCosts;

			ComputedCosts = new Dictionary<TVertex, double>();

			// The absence of a route entry in ComputedPaths means it's unreachable
			ComputedPaths = new Dictionary<TVertex, TVertex>();

			// Start every distance to infinity
			foreach (TVertex vertex in Graph.VertexIterator())
			{
				ComputedCosts.Add(vertex, float.PositiveInfinity);
			}
		}

		// Compute distance from startVertex to all other vertices
		public void ComputeAllFromVertex(TVertex startVertex)
		{
			ComputedCosts[startVertex] = 0.0;
			ComputedPaths[startVertex] = startVertex;

			var remainingVertices = new HashSet<TVertex>(Graph.VertexIterator());

			while (remainingVertices.Count > 0)
			{
				var currentVertex = popShortestRemaining(remainingVertices);
				// Cost to get to current vertex
				var cummulativeCost = ComputedCosts[currentVertex];

				foreach (IEdge<TVertex> edgeToNeighbor in Graph.EdgeIterator(currentVertex))
				{
					var neighbor = edgeToNeighbor.End;
					var neighborEdgeCost = EdgeCosts[edgeToNeighbor];
					var neighborComputedCost = ComputedCosts[neighbor];

					var costFromCurrent = cummulativeCost + neighborEdgeCost;

					// Cost to neighbor is either what it already was, or the cost
					// to get here plus the cost of the edge
					if (costFromCurrent < neighborComputedCost)
					{
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

	public static class KruskalMST<TVertex> where TVertex : IComparable<TVertex>
	{
		public static UndirectedAdjacencyListGraph<TVertex> Get(
			UndirectedAdjacencyListGraph<TVertex> graph, Dictionary<IEdge<TVertex>, double> edgeCosts)
		{
			var spanningTree = new UndirectedAdjacencyListGraph<TVertex>();
			var disjointSet = new DisjointSetCollection<TVertex>(graph.VertexIterator());

			var edgeList = new List<IEdge<TVertex>>();

			foreach (var edge in graph.AllEdgeIterator())
			{
				edgeList.Add(edge);
			}

			// Sort by edge weight
			edgeList.Sort((x, y) => edgeCosts[x].CompareTo(edgeCosts[y]));

			// Add the vertices from the original graph to the MST
			foreach (var vertex in graph.VertexIterator())
			{
				spanningTree.AddVertex(vertex);
			}

			foreach (var edge in edgeList)
			{
				if (!disjointSet.SameSet(edge.Start, edge.End))
				{
					spanningTree.AddEdge(edge);
					disjointSet.Union(edge.Start, edge.End);
				}
			}

			return spanningTree;
		}
	}
}