namespace MonoGraph
{
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    // Alias for easy typing
    using E = Edge<string>;

    [TestFixtureAttribute]
    public class VertexAndEdgeTests
    {
        private AdjacencyListGraph<string, Edge<string>> testGraph;

        [SetUpAttribute]
        public void Setup()
        {
            testGraph = new AdjacencyListGraph<string, Edge<string>>();
        }

        [Test]
        public void TestAddingVertex()
        {
            var reason = "graph should contain vertex 'a'";
            testGraph.AddVertex("a");
            Assert.IsTrue(testGraph.ContainsVertex("a"), reason);
        }

        [Test]
        [ExpectedException(typeof(DuplicateVertexException))]
        public void TestDuplicateVertexThrowsException()
        {
            testGraph.AddVertex("a");
            testGraph.AddVertex("a");
        }

        [Test]
        public void TestEdgeComparison()
        {
            var cat_hat = new E("cat", "hat");
            var hat_cat = new E("hat", "cat");

            Assert.IsTrue(cat_hat.CompareTo(cat_hat) == 0, "should be equal");
            Assert.IsTrue(cat_hat.CompareTo(hat_cat) == -1, "should be less than");
            Assert.IsTrue(hat_cat.CompareTo(cat_hat) == 1, "should be greater than");
        }

        [Test]
        public void TestAddingEdge()
        {
            var reason = "graph should contain edge a->b";

            var a_b = new E("a", "b");

            testGraph.AddVertex("a");
            testGraph.AddVertex("b");
            testGraph.AddDirectedEdge(a_b);

            Assert.IsTrue(testGraph.ContainsEdge(a_b), reason);
        }

        [Test]
        [ExpectedException(typeof(VertexNotFoundException))]
        public void TestAddingEdgeWithMissingVertex()
        {
            var a_b = new E("a", "c");

            testGraph.AddVertex("a");
            testGraph.AddVertex("b");
            // Should throw
            testGraph.AddDirectedEdge(a_b);
        }

        [Test]
        [ExpectedException(typeof(DuplicateEdgeException))]
        public void TestDuplicateEdgeThrowsException()
        {
            testGraph.AddVertex("a");
            testGraph.AddVertex("b");
            var a_b = new E("a", "b");

            testGraph.AddDirectedEdge(a_b);
            // Should throw
            testGraph.AddDirectedEdge(a_b);
        }

        [Test]
        public void TestCreatingBidrectionalEdge()
        {
            testGraph.AddVertex("a");
            testGraph.AddVertex("b");
            var a_b = new E("a", "b");
            var b_a = a_b.Reversed();

            testGraph.AddBidirectionalEdge(a_b);

            // Collect all the edges
            var collectedEdges = new List<E>(testGraph.AllEdgeIterator());

            Assert.AreEqual(collectedEdges.Count, 2);

            // Make sure both b_a and a_b are in the graph
            Assert.IsTrue(collectedEdges.Contains(a_b));
            Assert.IsTrue(collectedEdges.Contains(b_a));
        }

        [Test]
        [ExpectedException(typeof(DuplicateEdgeException))]
        public void TestDuplicateBidirectionalEdgeThrowsException()
        {
            testGraph.AddVertex("a");
            testGraph.AddVertex("b");
            var a_b = new E("a", "b");
            var b_a = a_b.Reversed();

            testGraph.AddBidirectionalEdge(a_b);
            // Should throw
            testGraph.AddBidirectionalEdge(b_a);
        }

        [Test]
        public void TestEdgeReversed()
        {
            var edge = new E("a", "b");
            var expectedReverse = new E("b", "a");

            Assert.AreEqual(expectedReverse, edge.Reversed());
        }

        [Test]
        public void TestVertexIteratorReturnsAllVertices()
        {
            var vertices = new List<string> {"a", "b", "c", "d"};
            foreach(string vertex in vertices)
            {
                testGraph.AddVertex(vertex);
            }

            // Consume iterator to get the Count of the list
            var collectedVertices = new List<string>(testGraph.VertexIterator());
            foreach(string vertex in collectedVertices)
            {
                Assert.Contains(vertex, vertices, string.Format("{0} should contain vertex {1}", collectedVertices, vertex));
            }

            Assert.AreEqual(vertices.Count, collectedVertices.Count, "Should have the same number of vertices");
        }

        [Test]
        public void TestEdgeIteratorReturnsAllEdges()
        {
            var vertices = new List<string> {"a", "b", "c", "d"};
            var edges = new List<E> {
                new E("a", "d"),
                new E("b", "c"),
                new E("c", "a"),
                new E("a", "c")
            };

            foreach(string vertex in vertices){
                testGraph.AddVertex(vertex);
            }

            foreach(E edge in edges){
                testGraph.AddDirectedEdge(edge);
            }

            var collectedEdges = new List<E>();

            foreach(string vertex in testGraph.VertexIterator()){
                foreach(E edge in testGraph.EdgeIterator(vertex)){
                    collectedEdges.Add(edge);
                }
            }

            collectedEdges.Sort();
            edges.Sort();
            foreach(int i in Enumerable.Range(0, edges.Count)){
                Assert.AreEqual(edges[i], collectedEdges[i]);
            }

            Assert.AreEqual(collectedEdges.Count, edges.Count, "Should have the same number of edges");
        }

        [Test]
        public void TestBidirectionalGraphHasAllEdges()
        {
            var vertices = new List<string> {"a", "b", "c", "d"};
            var edges = new List<E> {
                new E("a", "d"),
                new E("b", "c"),
                new E("c", "a"),
                new E("d", "c")
            };

            // Create list of edges + all the reversed edges
            var allEdges = new List<E>();
            foreach(E edge in edges){
                allEdges.Add(edge);
                allEdges.Add(edge.Reversed());
            }

            // Add vertices
            foreach(string vertex in vertices){
                testGraph.AddVertex(vertex);
            }

            // Add bidirectional edges
            foreach(E edge in edges){
                testGraph.AddBidirectionalEdge(edge);
            }

            // Collect all the edges from the graph
            var collectedEdges = new List<E>();
            foreach(string vertex in testGraph.VertexIterator()){
                foreach(E edge in testGraph.EdgeIterator(vertex)){
                    collectedEdges.Add(edge);
                }
            }

            // Compare
            collectedEdges.Sort();
            allEdges.Sort();
            foreach(int i in Enumerable.Range(0, allEdges.Count)){
                Assert.AreEqual(allEdges[i], collectedEdges[i]);
            }

            Assert.AreEqual(collectedEdges.Count, allEdges.Count, "Should have the same number of edges");
        }
    }
}