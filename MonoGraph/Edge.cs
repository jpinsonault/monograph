using System;

namespace MonoGraph
{
    // Every edge object should implment these methods
    public interface IEdgeInterface<TVertex, TEdge>
    {
        TVertex Start { get; }
        TVertex End { get; }

        TEdge Reversed();
    }

    // Basic edge class that just holds two vertices in a tuple
    public class Edge<TVertex> :
        IEdgeInterface<TVertex, Edge<TVertex>>,
        IEquatable<Edge<TVertex>>,
        IComparable<Edge<TVertex>>
        where TVertex : IComparable<TVertex>
    {
        public TVertex Start { get; private set; }
        public TVertex End { get; private set; }

        public Edge() {}

        internal Edge(TVertex first, TVertex second)
        {
            Start = first;
            End = second;
        }

        public Edge<TVertex> Reversed()
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

        public bool Equals(Edge<TVertex> obj)
        {
            if ((object)obj == null)
            {
                return false;
            }

            return (Start.Equals(obj.Start)) && (End.Equals(obj.End));
        }

        public int CompareTo(Edge<TVertex> obj){
            // Edges are sorted by comparing the Start properties first, then End

            if (obj == null) return 1;

            int comparison = Start.CompareTo(obj.Start);

            // If Start != obj.Start, use that comparison
            if (comparison != 0){
                return comparison;
            }

            // Else use comparison of End vs End
            return End.CompareTo(obj.End);
        }

        public override string ToString()
        {
            return string.Format("{0}->{1}", Start, End);
        }
    }
}