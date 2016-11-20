using System;

namespace MonoGraph
{
    // Every edge object should implment these methods
	public interface IEdge<TVertex> :
	IComparable<IEdge<TVertex>>, IEquatable<IEdge<TVertex>> where TVertex : IComparable<TVertex>
	{
        TVertex Start { get; }
        TVertex End { get; }

        IEdge<TVertex> Reversed();
    }

    // Basic edge class that just holds two vertices in a tuple
	public class Edge<TVertex> : IEdge<TVertex> where TVertex : IComparable<TVertex>
    {
        public TVertex Start { get; private set; }
        public TVertex End { get; private set; }

        public Edge() {}

        public Edge(TVertex first, TVertex second)
        {
            Start = first;
            End = second;
        }

        public IEdge<TVertex> Reversed()
        {
            return new Edge<TVertex>(End, Start);
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode() ^ End.GetHashCode();
        }

        public override bool Equals (Object obj)
        {
            if (obj == null)
                return false;

            var objCast = obj as Edge<TVertex>;
            if ((Object)objCast == null)
            {
                return false;
            }

            return (Start.Equals(objCast.Start)) && (End.Equals(objCast.End));
        }

        public bool Equals(IEdge<TVertex> obj)
        {
            if ((object)obj == null)
            {
                return false;
            }

            return (Start.Equals(obj.Start)) && (End.Equals(obj.End));
        }

        public int CompareTo(object obj){
            // Edges are sorted by comparing the Start properties first, then End

            if ((Edge<TVertex>)obj == null) return 1;

			var edgeObject = (Edge<TVertex>)obj;

			int comparison = Start.CompareTo(edgeObject.Start);

            // If Start != obj.Start, use that comparison
            if (comparison != 0){
                return comparison;
            }

            // Else use comparison of End vs End
            return End.CompareTo(edgeObject.End);
        }

		public int CompareTo(IEdge<TVertex> other)
		{
			int comparison = Start.CompareTo(other.Start);

			// If Start != obj.Start, use that comparison
			if (comparison != 0)
			{
				return comparison;
			}

			// Else use comparison of End vs End
			return End.CompareTo(other.End);
		}

        public override string ToString()
        {
            return string.Format("{0}->{1}", Start, End);
        }
	}
}