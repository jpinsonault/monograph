namespace MonoGraph
{
	using NUnit.Framework;
	using System.Collections.Generic;

	// Alias for easy typing
	using E = Edge<string>;

	[TestFixture]
	public class DijkstraShortestPathTest_SimpleDirectedGraph_ComputeAllFromA
	{
		private AdjacencyListGraph<string, Edge<string>> testGraph;

		private List<string> vertices = new List<string> { "a", "b", "c", "d", "e" };

		private readonly Dictionary<E, double> edgesCosts = new Dictionary<E, double> {
			{new E("a", "d"), 3.0},
			{new E("a", "b"), 20.0},
			{new E("a", "e"), 2.0},
			{new E("d", "c"), 2.0},
			{new E("c", "b"), 2.0},
			{new E("e", "d"), 2.0}
		};

		private readonly Dictionary<string, double> expectedSmallestCosts = new Dictionary<string, double> {
			{"a", 0},
			{"b", 7},
			{"c", 5},
			{"d", 3},
			{"e", 2},
		};

		private readonly Dictionary<string, string> expectedPaths = new Dictionary<string, string> {
			{"a", "a"},
			{"b", "c"},
			{"c", "d"},
			{"d", "a"},
			{"e", "a"},
		};

		private Algorithms.DijkstraShortestPath<string, E> shortestPath;

		[SetUp]
		public void Setup()
		{
			testGraph = new AdjacencyListGraph<string, E>();
			foreach (string vertex in vertices)
			{
				testGraph.AddVertex(vertex);
			}

			foreach (E edge in edgesCosts.Keys)
			{
				testGraph.AddDirectedEdge(edge);
			}

			shortestPath = new Algorithms.DijkstraShortestPath<string, E>(testGraph, edgesCosts);

			shortestPath.ComputeAllFromVertex("a");
		}

		[Test]
		public void TestComputedCosts()
		{
			Assert.AreEqual(shortestPath.ComputedCosts.Count, expectedSmallestCosts.Count);

			foreach (var entry in expectedSmallestCosts)
			{
				Assert.AreEqual(shortestPath.ComputedCosts[entry.Key], entry.Value);
			}

			foreach (var entry in shortestPath.ComputedCosts)
			{
				Assert.AreEqual(expectedSmallestCosts[entry.Key], entry.Value);
			}
		}

		[Test]
		public void TestComputedPaths()
		{
			Assert.AreEqual(shortestPath.ComputedPaths.Count, expectedPaths.Count);

			foreach (var entry in expectedPaths)
			{
				Assert.AreEqual(shortestPath.ComputedPaths[entry.Key], entry.Value);
			}

			foreach (var entry in shortestPath.ComputedPaths)
			{
				Assert.AreEqual(expectedPaths[entry.Key], entry.Value);
			}
		}
	}

	[TestFixture]
	public class DijkstraShortestPathTest_SimpleBidirectionalGraph_ComputeAllFromA
	{
		private AdjacencyListGraph<string, Edge<string>> testGraph;

		private List<string> vertices = new List<string> { "a", "b", "c", "d", "e" };

		private readonly Dictionary<E, double> edgesCosts = new Dictionary<E, double> {
			{new E("a", "d"), 3.0},
			{new E("a", "b"), 20.0},
			{new E("a", "e"), 2.0},
			{new E("d", "c"), 2.0},
			{new E("c", "b"), 2.0},
			{new E("e", "d"), 2.0}
		};

		private readonly Dictionary<string, double> expectedSmallestCosts = new Dictionary<string, double> {
			{"a", 0},
			{"b", 7},
			{"c", 5},
			{"d", 3},
			{"e", 2},
		};

		private readonly Dictionary<string, string> expectedPaths = new Dictionary<string, string> {
			{"a", "a"},
			{"b", "c"},
			{"c", "d"},
			{"d", "a"},
			{"e", "a"},
		};

		private Algorithms.DijkstraShortestPath<string, E> shortestPath;

		[SetUp]
		public void Setup()
		{
			var bidirectionalEdgeCosts = new Dictionary<E, double>();

			// Create dict of forward and backward edges
			foreach (var edgePair in edgesCosts)
			{
				var edge = edgePair.Key;
				var reversed = edge.Reversed();
				bidirectionalEdgeCosts.Add(edge, edgePair.Value);
				bidirectionalEdgeCosts.Add(reversed, edgePair.Value);
			}

			testGraph = new AdjacencyListGraph<string, Edge<string>>();
			foreach (string vertex in vertices)
			{
				testGraph.AddVertex(vertex);
			}

			foreach (E edge in edgesCosts.Keys)
			{
				testGraph.AddBidirectionalEdge(edge);
			}

			shortestPath = new Algorithms.DijkstraShortestPath<string, E>(testGraph, bidirectionalEdgeCosts);

			shortestPath.ComputeAllFromVertex("a");
		}

		[Test]
		public void TestComputedCosts()
		{
			Assert.AreEqual(shortestPath.ComputedCosts.Count, expectedSmallestCosts.Count);

			foreach (var entry in expectedSmallestCosts)
			{
				Assert.AreEqual(entry.Value, shortestPath.ComputedCosts[entry.Key]);
			}

			foreach (var entry in shortestPath.ComputedCosts)
			{
				Assert.AreEqual(entry.Value, expectedSmallestCosts[entry.Key]);
			}
		}

		[Test]
		public void TestComputedPaths()
		{
			Assert.AreEqual(shortestPath.ComputedPaths.Count, expectedPaths.Count);

			foreach (var entry in expectedPaths)
			{
				Assert.AreEqual(shortestPath.ComputedPaths[entry.Key], entry.Value);
			}

			foreach (var entry in shortestPath.ComputedPaths)
			{
				Assert.AreEqual(expectedPaths[entry.Key], entry.Value);
			}
		}
	}
}