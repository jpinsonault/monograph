namespace MonoGraph
{
	using NUnit.Framework;

	[TestFixture]
	public class DisjointSetCollectionTests
	{
		private DisjointSetCollection<int> disjointSet;

		[SetUp]
		public void Setup()
		{
			disjointSet = new DisjointSetCollection<int>();
		}

		[Test]
		public void TestAddingSets()
		{
			disjointSet.Add(1);
			disjointSet.Add(2);
			disjointSet.Add(3);

			// Make sure they're their own representatives
			Assert.AreEqual(1, disjointSet.Find(1));
			Assert.AreEqual(2, disjointSet.Find(2));
			Assert.AreEqual(3, disjointSet.Find(3));

			Assert.AreEqual(3, disjointSet.Count);
		}

		[Test]
		public void TestSetUnion()
		{
			disjointSet.Add(1);
			disjointSet.Add(2);
			disjointSet.Add(3);

			Assert.AreEqual(3, disjointSet.Count);

			disjointSet.Union(1, 3);
			Assert.AreEqual(2, disjointSet.Count);

			// 3 should be in 1's set now
			Assert.IsTrue(disjointSet.SameSet(1, 3));
			Assert.IsFalse(disjointSet.SameSet(1, 2));

			disjointSet.Union(2, 1);
			Assert.AreEqual(1, disjointSet.Count);

			// All should be in 2's set now
			Assert.IsTrue(disjointSet.SameSet(2, 1));
			Assert.IsTrue(disjointSet.SameSet(2, 2));
			Assert.IsTrue(disjointSet.SameSet(2, 3));
			
			// Should do nothing since they're in the same set already
			disjointSet.Union(1, 2);
			Assert.AreEqual(1, disjointSet.Count);
		}
	}
}