using System.Collections.Generic;
using System;

namespace MonoGraph
{
	using NUnit.Framework;
	using E = UndirectedEdge<int>;
	
	[TestFixture]
	public class KruskalMSTTests
	{
		[Test]
		public void TestMST()
		{
			var graph = new UndirectedAdjacencyListGraph<int>();
			var edgeCosts = new Dictionary<IEdge<int>, double>();
			graph.AddVertex(1);
			graph.AddVertex(2);
			graph.AddVertex(3);
			graph.AddVertex(4);
			graph.AddVertex(5);

			graph.AddEdge(new E(1, 2));
			graph.AddEdge(new E(2, 3));
			graph.AddEdge(new E(3, 4));
			graph.AddEdge(new E(5, 1));
			graph.AddEdge(new E(5, 2));
			graph.AddEdge(new E(5, 3));
			graph.AddEdge(new E(5, 4));

			edgeCosts[new E(1, 2)] = 5;
			edgeCosts[new E(2, 3)] = 5;
			edgeCosts[new E(3, 4)] = 5;
			edgeCosts[new E(5, 1)] = 10;
			edgeCosts[new E(5, 2)] = 1;
			edgeCosts[new E(5, 3)] = 10;
			edgeCosts[new E(5, 4)] = 5;

			var spanningTree = Algorithms.KruskalMST<int>.Get(graph, edgeCosts);

			foreach (var vertex in spanningTree.VertexIterator())
			{
				Console.WriteLine("Vertex {0} goes to:", vertex);

				foreach (var neighbor in spanningTree.NeighborIterator(vertex))
				{
					Console.WriteLine(String.Format("    {0}", neighbor));
				}
			}
		}
	}
}