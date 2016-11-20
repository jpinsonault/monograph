namespace MonoGraph
{
	using NUnit.Framework;
	using System.Collections.Generic;

	// Alias for easy typing
	using E = Edge<string>;

	[TestFixture]
	public class UndirectedGraphTests
	{
		UndirectedAdjacencyListGraph<string> testGraph;

		[SetUp]
		public void Setup()
		{
			testGraph = new UndirectedAdjacencyListGraph<string>();
		}

		[Test]
		public void TestAddingVertex()
		{
			testGraph.AddVertex("a");
			testGraph.AddVertex("b");
			Assert.IsTrue(testGraph.ContainsVertex("a"));
			Assert.IsTrue(testGraph.ContainsVertex("b"));
		}

		[Test]
		public void TestAddingEdges()
		{
			testGraph.AddVertex("a");
			testGraph.AddVertex("b");
			testGraph.AddVertex("c");
			var a_b = new E("a", "b");
			testGraph.AddEdge(a_b);

			// Should recognize both directions
			Assert.IsTrue(testGraph.ContainsEdge(a_b));
			Assert.IsTrue(testGraph.ContainsEdge(a_b.Reversed()));

			Assert.IsTrue(testGraph.VertexHasNeighbor("a", "b"));
			Assert.IsTrue(testGraph.VertexHasNeighbor("b", "a"));
			Assert.IsFalse(testGraph.VertexHasNeighbor("b", "c"));

			// Shouldn't distinguish between a->b and b->a
			Assert.IsTrue(testGraph.VertexHasEdge("a", a_b));
			Assert.IsTrue(testGraph.VertexHasEdge("a", a_b.Reversed()));
		}
	}
}