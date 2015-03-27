using System;

namespace MonoGraph
{
    // Every edge object should implment these methods
    public interface IEdgeInterface<TVertex, TEdge> where TEdge : new()
    {
        TVertex First { get; }
        TVertex Second { get; }

        TEdge Reversed();
    }

    // Basic edge class that just holds two vertices in a tuple
    public class Edge<TVertex> :
        IEdgeInterface<TVertex, Edge<TVertex>>,
        IEquatable<Edge<TVertex>>,
        IComparable<Edge<TVertex>>
        where TVertex : class, IComparable<TVertex>
    {
        public TVertex First { get; private set; }
        public TVertex Second { get; private set; }

        public Edge() {}

        internal Edge(TVertex first, TVertex second)
        {
            First = first;
            Second = second;
        }

        public Edge<TVertex> Reversed()
        {
            return new Edge<TVertex>(Second, First);
        }

        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode();
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

            return (First == objCast.First) && (Second == objCast.Second);
        }

        public bool Equals(Edge<TVertex> other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return First == other.First && Second == other.Second;
        }

        public int CompareTo(Edge<TVertex> obj){
            // Edges are sorted by comparing the First properties first, then Second

            if (obj == null) return 1;

            int comparison = First.CompareTo(obj.First);

            // If First != obj.First, use that comparison
            if (comparison != 0){
                return comparison;
            }

            // Else use comparison of Second vs Second
            return Second.CompareTo(obj.Second);
        }

        public override string ToString()
        {
            return string.Format("{0}->{1}", First, Second);
        }
    }
}