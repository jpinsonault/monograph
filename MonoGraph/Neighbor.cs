using System;
namespace MonoGraph
{
	public class Neighbor<TVertex, TEdge>
	{
		public readonly TVertex Destination;
		public readonly TEdge Edge;
		public Neighbor(TVertex neighbor, TEdge edge)
		{
			Destination = neighbor;
			Edge = edge;
		}
	}
}
