using System;

namespace MonoGraph
{
	// Undirected edge class that just holds two vertices in a tuple
	public class UndirectedEdge<TVertex> : IEdge<TVertex> where TVertex : IComparable<TVertex>
	{
		public TVertex Start { get; private set; }
		public TVertex End { get; private set; }

		public UndirectedEdge() { }

		public UndirectedEdge(TVertex first, TVertex second)
		{
			Start = first;
			End = second;
		}

		public IEdge<TVertex> Reversed()
		{
			return new UndirectedEdge<TVertex>(End, Start);
		}

		public override int GetHashCode()
		{
			return Start.GetHashCode() ^ End.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var objCast = obj as UndirectedEdge<TVertex>;
			if (objCast == null)
			{
				return false;
			}

			var same = false;
			same = same || (Start.Equals(objCast.Start)) && (End.Equals(objCast.End));
			same = same || (Start.Equals(objCast.End)) && (End.Equals(objCast.Start));

			return same;
		}

		public bool Equals(IEdge<TVertex> obj)
		{
			if (obj == null)
			{
				return false;
			}

			var same = false;
			same = same || (Start.Equals(obj.Start)) && (End.Equals(obj.End));
			same = same || (Start.Equals(obj.End)) && (End.Equals(obj.Start));

			return same;
		}

		public int CompareTo(IEdge<TVertex> obj)
		{
			// Edges are sorted by comparing the Start properties first, then End

			if (obj == null) return 1;

			int comparison = Start.CompareTo(obj.Start);

			// If Start != obj.Start, use that comparison
			if (comparison != 0)
			{
				return comparison;
			}

			// Else use comparison of End vs End
			return End.CompareTo(obj.End);
		}

		public override string ToString()
		{
			return string.Format("{0}<->{1}", Start, End);
		}
	}
}