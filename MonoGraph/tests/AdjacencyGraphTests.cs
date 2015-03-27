namespace MonoGraph
{
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using E = Edge<string>;

    [TestFixtureAttribute]
    public class VertexAndEdgeTests
    {
        private AdjacencyGraph<string, Edge<string>> _graph;

        [SetUpAttribute]
        public void Setup()
        {
            _graph = new AdjacencyGraph<string, Edge<string>>();
        }

        [Test]
        public void TestAddingVertex()
        {
            var reason = "graph should contain vertex 'a'";
            _graph.AddVertex("a");
            Assert.IsTrue(_graph.ContainsVertex("a"), reason);
        }

        [Test]
        [ExpectedException(typeof(DuplicateVertexException))]
        public void TestDuplicateVertexThrowsException()
        {
            _graph.AddVertex("a");
            _graph.AddVertex("a");
        }

        [Test]
        public void TestAddingEdge()
        {
            var reason = "graph should contain edge a->b";

            var a_b = new Edge<string>("a", "b");

            _graph.AddVertex("a");
            _graph.AddVertex("b");
            _graph.AddDirectedEdge(a_b);

            Assert.IsTrue(_graph.ContainsEdge(a_b), reason);
        }

        [Test]
        [ExpectedException(typeof(VertexNotFoundException))]
        public void TestAddingEdgeWithMissingVertex()
        {
            var a_b = new Edge<string>("a", "c");

            _graph.AddVertex("a");
            _graph.AddVertex("b");
            // Should throw
            _graph.AddDirectedEdge(a_b);
        }

        [Test]
        [ExpectedException(typeof(DuplicateEdgeException))]
        public void TestDuplicateEdgeThrowsException()
        {
            _graph.AddVertex("a");
            _graph.AddVertex("b");
            var a_b = new Edge<string>("a", "b");

            _graph.AddDirectedEdge(a_b);
            // Should throw
            _graph.AddDirectedEdge(a_b);
        }

        [Test]
        [ExpectedException(typeof(DuplicateEdgeException))]
        public void TestDuplicateBidirectionalEdgeThrowsException()
        {
            _graph.AddVertex("a");
            _graph.AddVertex("b");
            var a_b = new Edge<string>("a", "b");
            var b_a = a_b.Reversed();

            _graph.AddBidirectionalEdge(a_b);
            // Should throw
            _graph.AddBidirectionalEdge(b_a);
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
                _graph.AddVertex(vertex);
            }

            // Consume iterator to get the Count of the list
            var collectedVertices = new List<string>(_graph.VertexIterator());
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
                _graph.AddVertex(vertex);
            }

            foreach(E edge in edges){
                _graph.AddDirectedEdge(edge);
            }

            var collectedEdges = new List<E>();

            foreach(string vertex in _graph.VertexIterator()){
                foreach(E edge in _graph.EdgeIterator(vertex)){
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
                _graph.AddVertex(vertex);
            }

            // Add bidirectional edges
            foreach(E edge in edges){
                _graph.AddBidirectionalEdge(edge);
            }

            // Collect all the edges from the graph
            var collectedEdges = new List<E>();
            foreach(string vertex in _graph.VertexIterator()){
                foreach(E edge in _graph.EdgeIterator(vertex)){
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