namespace MonoGraph
{
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;

    // Alias for easy typing
    using E = Edge<string>;

    [TestFixture]
    public class VertexAndEdgeTests
    {
        private DirectedAdjacencyListGraph<string> testGraph;

        [SetUp]
        public void Setup()
        {
            testGraph = new DirectedAdjacencyListGraph<string>();
        }

        [Test]
        public void TestAddingVertex()
        {
            var reason = "graph should contain vertex 'a'";
            testGraph.AddVertex("a");
            Assert.IsTrue(testGraph.ContainsVertex("a"), reason);
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
            testGraph.AddEdge(a_b);

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
            testGraph.AddEdge(a_b);
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
            var collectedEdges = new List<IEdge<string>>(testGraph.AllEdgeIterator());

            Assert.AreEqual(collectedEdges.Count, 2);

            // Make sure both b_a and a_b are in the graph
            Assert.IsTrue(collectedEdges.Contains(a_b));
            Assert.IsTrue(collectedEdges.Contains(b_a));
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
                testGraph.AddEdge(edge);
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
            var allEdges = new List<IEdge<string>>();
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

    [TestFixture]
    class TestRemoveEdgesAndVertices
    {
        private DirectedAdjacencyListGraph<string> testGraph;
        
        private List<string> vertices = new List<string>{"a", "b", "c", "d", "e"};

        private List<E> edges = new List<E> {
            new E("a", "d"),
            new E("a", "b"),
            new E("a", "e"),
            new E("d", "c"),
            new E("c", "d"),
            new E("c", "a"),
            new E("e", "d")
        };

        [SetUp]
        public void Setup()
        {
            testGraph = new DirectedAdjacencyListGraph<string>();
            
            foreach(var vertex in vertices) { testGraph.AddVertex(vertex); }

            foreach(var edge in edges) { testGraph.AddEdge(edge); }
        }

        [Test]
        public void TestRemoveVertexRemovesVertex()
        {
            testGraph.RemoveVertex("a");
            Assert.IsFalse(testGraph.ContainsVertex("a"));
        }

        [Test]
        public void TestRemoveVertexRemovesAssociatedEdges()
        {
            testGraph.RemoveVertex("a");
            Assert.IsFalse(testGraph.ContainsEdge(new E("a", "d")));

            // Make sure edges going to "a" are also gone
            Assert.IsFalse(testGraph.ContainsEdge(new E("c", "a")));
        }

        [Test]
        public void TestRemoveEdgeRemovesEdge()
        {
            testGraph.RemoveEdge(new E("a", "d"));
            Assert.IsFalse(testGraph.ContainsEdge(new E("a", "d")));
        }
        
        [Test]
        public void TestRemoveBidirectionalEdge()
        {
            var c_d = new E("c", "d");
            var d_c = new E("d", "c");
            
            testGraph.RemoveBidirectionalEdge(c_d);

            Assert.IsFalse(testGraph.ContainsEdge(c_d));
            Assert.IsFalse(testGraph.ContainsEdge(d_c));
        }

        [Test]
        [ExpectedException(typeof(VertexNotFoundException))]
        public void TestRemovingEdgeFromNonExistantVertexThrows()
        {
            testGraph.RemoveEdge(new E("not_there", "d"));
        }

        [Test]
        [ExpectedException(typeof(EdgeNotFoundException))]
        public void TestRemovingEdgeFromExistingVertexThrows()
        {
            testGraph.RemoveEdge(new E("a", "not_there"));
        }
    }
}