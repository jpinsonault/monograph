using System;

namespace MonoGraph
{
    // Every edge object should implment these methods
    public interface IEdgeInterface<TVertex, TEdge>
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
        where TVertex : IComparable<TVertex>
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

            return (First.Equals(objCast.First)) && (Second.Equals(objCast.Second));
        }

        public bool Equals(Edge<TVertex> obj)
        {
            if ((object)obj == null)
            {
                return false;
            }

            return (First.Equals(obj.First)) && (Second.Equals(obj.Second));
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