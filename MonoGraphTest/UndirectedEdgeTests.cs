using System;
using NUnit.Framework;
using MonoGraph;
namespace MonoGraphTest
{
	[TestFixture]
	public class UndirectedEdgeTests
	{
		[Test]
		public void TestEdgesAreEqual()
		{
			var intEdge1 = new UndirectedEdge<int>(1, 2);
			var intEdge2 = new UndirectedEdge<int>(2, 1);
			Assert.AreEqual(intEdge1, intEdge2);
			Assert.AreEqual(intEdge1, intEdge1.Reversed());
			Assert.AreEqual(intEdge1.GetHashCode(), intEdge2.GetHashCode());

			var stringEdge1 = new UndirectedEdge<string>("bob", "joe");
			var stringEdge2 = new UndirectedEdge<string>("joe", "bob");
			Assert.AreEqual(stringEdge1, stringEdge2);
			Assert.AreEqual(stringEdge1, stringEdge1.Reversed());
			Assert.AreEqual(stringEdge1.GetHashCode(), stringEdge2.GetHashCode());
		}
	}
}
