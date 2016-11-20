using System.Collections.Generic;
using System.Linq;
using System;

namespace MonoGraph
{
	public interface IGraph<TVertex> where TVertex : IComparable<TVertex>
	{
		void AddVertex(TVertex vertex);
		void AddEdge(IEdge<TVertex> edge);
		void RemoveVertex(TVertex vertex);
		void RemoveEdge(IEdge<TVertex> edge);
		bool ContainsVertex(TVertex vertex);
		bool ContainsEdge(IEdge<TVertex> edge);
		IEnumerable<TVertex> VertexIterator();
		IEnumerable<IEdge<TVertex>> EdgeIterator(TVertex vertex);
		IEnumerable<IEdge<TVertex>> AllEdgeIterator();
	}

	public class DirectedAdjacencyListGraph<TVertex> : IGraph<TVertex> where TVertex : IComparable<TVertex>
	{
		private readonly Dictionary<TVertex, List<IEdge<TVertex>>> VertexEdgeDictionary;
		public int VertexCount { get { return VertexEdgeDictionary.Count; } }

		public DirectedAdjacencyListGraph()
		{
			VertexEdgeDictionary = new Dictionary<TVertex, List<IEdge<TVertex>>>();
		}

		public void AddVertex(TVertex vertex)
		{
			VertexEdgeDictionary.Add(vertex, new List<IEdge<TVertex>>());
		}

		// Throws an exception if the edge already exists or the vertices don't exist
		private void CheckEdge(IEdge<TVertex> edge)
		{
			if (!VertexEdgeDictionary.ContainsKey(edge.Start))
			{
				var errorMessage = string.Format("Tried to add edge to non-existant vertex '{0}'", edge.Start);
				throw new VertexNotFoundException(errorMessage);
			}

			if (!VertexEdgeDictionary.ContainsKey(edge.End))
			{
				var errorMessage = string.Format("Tried to add edge to non-existant vertex '{0}'", edge.End);
				throw new VertexNotFoundException(errorMessage);
			}
		}

		public void AddEdge(IEdge<TVertex> edge)
		{
			CheckEdge(edge);

			VertexEdgeDictionary[edge.Start].Add(edge);
		}

		public void AddBidirectionalEdge(IEdge<TVertex> edge)
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

			foreach (var vertex in VertexIterator())
			{
				var edges = VertexEdgeDictionary[vertex];

				var index = FindEdgeContainingVertex(vertexToRemove, edges);
				if (index >= 0)
				{
					edges.RemoveAt(index);
				}
			}
		}

		// Return index of edge in edges that contains vertex, or -1
		static int FindEdgeContainingVertex(TVertex vertex, IList<IEdge<TVertex>> edges)
		{
			foreach (var i in Enumerable.Range(0, edges.Count))
			{
				if ((edges[i].Start.Equals(vertex)) || (edges[i].End.Equals(vertex)))
				{
					return i;
				}
			}

			return -1;
		}

		// Removing an edge is simple, lookup edge.Start and remove edge from its edge list
		public void RemoveEdge(IEdge<TVertex> edge)
		{
			if (VertexEdgeDictionary.ContainsKey(edge.Start))
			{
				if (VertexEdgeDictionary[edge.Start].Contains(edge))
				{
					VertexEdgeDictionary[edge.Start].Remove(edge);
				}
				else {
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
		public void RemoveBidirectionalEdge(IEdge<TVertex> edge)
		{
			var errorTemplate = "Can't find vertex '{0}' while trying to remove bidirectional edge '{1}'";
			if (!ContainsVertex(edge.Start))
			{
				throw new VertexNotFoundException(string.Format(errorTemplate, edge.Start, edge));
			}

			if (!ContainsVertex(edge.End))
			{
				throw new VertexNotFoundException(string.Format(errorTemplate, edge.End, edge));
			}

			VertexEdgeDictionary[edge.Start].Remove(edge);
			VertexEdgeDictionary[edge.End].Remove(edge.Reversed());
		}

		public bool ContainsVertex(TVertex vertex)
		{
			return VertexEdgeDictionary.ContainsKey(vertex);
		}

		public bool ContainsEdge(IEdge<TVertex> edge)
		{
			return VertexEdgeDictionary.ContainsKey(edge.Start)
				&& VertexEdgeDictionary[edge.Start].Contains(edge);
		}

		public IEnumerable<TVertex> VertexIterator()
		{
			foreach (TVertex vertex in VertexEdgeDictionary.Keys)
			{
				yield return vertex;
			}
		}

		public IEnumerable<IEdge<TVertex>> EdgeIterator(TVertex vertex)
		{
			foreach (IEdge<TVertex> edge in VertexEdgeDictionary[vertex])
			{
				yield return edge;
			}
		}

		public IEnumerable<IEdge<TVertex>> AllEdgeIterator()
		{
			foreach (TVertex vertex in VertexEdgeDictionary.Keys)
			{
				foreach (IEdge<TVertex> edge in VertexEdgeDictionary[vertex])
				{
					yield return edge;
				}
			}
		}
	}

	public class UndirectedAdjacencyListGraph<TVertex> : IGraph<TVertex>  where TVertex : IComparable<TVertex>
	{
		private Dictionary<TVertex, List<Neighbor<TVertex, IEdge<TVertex>>>> VertexEdgeDictionary;
		public HashSet<IEdge<TVertex>> EdgeSet { get; }
		public int VertexCount { get { return VertexEdgeDictionary.Count; } }

		public UndirectedAdjacencyListGraph()
		{
			VertexEdgeDictionary = new Dictionary<TVertex, List<Neighbor<TVertex, IEdge<TVertex>>>>();
			EdgeSet = new HashSet<IEdge<TVertex>>();
		}

		public void AddVertex(TVertex vertex)
		{
			VertexEdgeDictionary.Add(vertex, new List<Neighbor<TVertex, IEdge<TVertex>>>());
		}

		// Throws an exception if the edge already exists or the vertices don't exist
		private void CheckEdge(IEdge<TVertex> edge)
		{
			if (!VertexEdgeDictionary.ContainsKey(edge.Start))
			{
				var errorMessage = string.Format("Tried to add edge to non-existant vertex '{0}'", edge.Start);
				throw new VertexNotFoundException(errorMessage);
			}

			if (!VertexEdgeDictionary.ContainsKey(edge.End))
			{
				var errorMessage = string.Format("Tried to add edge to non-existant vertex '{0}'", edge.End);
				throw new VertexNotFoundException(errorMessage);
			}
		}

		public bool VertexHasEdge(TVertex vertex, IEdge<TVertex> edge)
		{
			return Utils.ContainsByKey(
				VertexEdgeDictionary[vertex],
				(neighbor) =>
					neighbor.Edge.Equals(edge) ||
				    neighbor.Edge.Equals(edge.Reversed())
			);
		}

		public bool VertexHasNeighbor(TVertex vertex, TVertex neighborVertex)
		{
			return Utils.ContainsByKey(
				VertexEdgeDictionary[vertex],
				(neighbor) => neighbor.Destination.Equals(neighborVertex)
			);
		}

		public void AddEdge(IEdge<TVertex> edge)
		{
			CheckEdge(edge);

			if (!ContainsEdge(edge))
			{
				VertexEdgeDictionary[edge.Start].Add(new Neighbor<TVertex, IEdge<TVertex>>(edge.End, edge));
				VertexEdgeDictionary[edge.End].Add(new Neighbor<TVertex, IEdge<TVertex>>(edge.Start, edge));
				EdgeSet.Add(edge);
			}
		}

		public void RemoveVertex(TVertex vertex)
		{
			var neighbors = VertexEdgeDictionary[vertex];
			VertexEdgeDictionary.Remove(vertex);

			foreach (var neighbor in neighbors)
			{
				var neighborsNeighbors = VertexEdgeDictionary[neighbor.Destination];
				int index = Utils.FindByKey(neighborsNeighbors, (n) => n.Destination.Equals(vertex));

				VertexEdgeDictionary[neighbor.Destination].RemoveAt(index);
			}
		}

		// Removing an edge is simple, lookup edge.Start and remove edge from its edge list
		public void RemoveEdge(IEdge<TVertex> edge)
		{
			if (ContainsEdge(edge))
			{
				RemoveNeighbor(edge.Start, edge);
				RemoveNeighbor(edge.End, edge);
			}
			else
			{
				var errorTemplate = "Can't find vertex '{0}' while removing edge '{1}'";
				throw new VertexNotFoundException(string.Format(errorTemplate, edge.Start, edge));
			}
		}

		private void RemoveNeighbor(TVertex vertex, IEdge<TVertex> edge)
		{
			var neighborList = VertexEdgeDictionary[vertex];
			foreach (var index in Enumerable.Range(0, neighborList.Count))
			{
				if (neighborList[index].Edge.Equals(edge))
				{
					neighborList.RemoveAt(index);
					break;
				}
			}
		}

		public bool ContainsVertex(TVertex vertex)
		{
			return VertexEdgeDictionary.ContainsKey(vertex);
		}

		public bool ContainsEdge(IEdge<TVertex> edge)
		{
			return EdgeSet.Contains(edge) || EdgeSet.Contains(edge.Reversed());
		}

		public IEnumerable<TVertex> VertexIterator()
		{
			foreach (TVertex vertex in VertexEdgeDictionary.Keys)
			{
				yield return vertex;
			}
		}

		// Yields IEdgeInterface<TVertex> objects connected to the given vertex
		public IEnumerable<IEdge<TVertex>> EdgeIterator(TVertex vertex)
		{
			foreach (var neighbor in VertexEdgeDictionary[vertex])
			{
				yield return neighbor.Edge;
			}
		}

		// Yields TVertex objects connected to the given vertex
		public IEnumerable<TVertex> NeighborIterator(TVertex vertex)
		{
			foreach (var neighbor in VertexEdgeDictionary[vertex])
			{
				yield return neighbor.Destination;
			}
		}

		public IEnumerable<IEdge<TVertex>> AllEdgeIterator()
		{
			foreach (var edge in EdgeSet)
			{
				yield return edge;
			}
		}
	}
}
