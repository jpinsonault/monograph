using System.Collections.Generic;

namespace MonoGraph
{
	public class SetNode<TItem>
	{
		public SetNode<TItem> Parent { get; set; }
		public readonly TItem NodeData;

		public SetNode(TItem nodeData)
		{
			Parent = null;
			NodeData = nodeData;
		}
	}

	public class DisjointSetCollection<TItem>
	{
		private Dictionary<TItem, SetNode<TItem>> ItemToSetMap;
		public int Count { get; private set; }

		public DisjointSetCollection()
		{
			ItemToSetMap = new Dictionary<TItem, SetNode<TItem>>();
			Count = 0;
		}

		public DisjointSetCollection(IEnumerable<TItem> setItems) : base()
		{
			ItemToSetMap = new Dictionary<TItem, SetNode<TItem>>();
			Count = 0;
			
			// Fill with one node trees
			foreach (var item in setItems)
			{
				Add(item);
			}
		}

		// Add new item into its own set
		public void Add(TItem newItem)
		{
			ItemToSetMap.Add(newItem, new SetNode<TItem>(newItem));
			Count++;
		}

		// Add item into an existing set
		public void Add(TItem newItem, TItem representative)
		{
			Add(newItem);
			Union(newItem, representative);
		}

		// Return the TItem representing the set the given item belongs to
		public TItem Find(TItem item)
		{
			return FindNode(item).NodeData;
		}

		// Returns the SetNode that item is in
		private SetNode<TItem> FindNode(TItem item)
		{
			SetNode<TItem> current = ItemToSetMap[item];
			SetNode<TItem> itemParent = current.Parent;
			var visitedNodes = new LinkedList<SetNode<TItem>>();

			while (itemParent != null)
			{
				visitedNodes.AddLast(current);
				current = itemParent;
				itemParent = itemParent.Parent;
			}

			// Set all the nodes parents to the found representative on the set
			// their in, so that the next time the lookup is constant time. 
			// Note that the representative won't be in the list of visited nodes
			foreach (var node in visitedNodes)
			{
				node.Parent = current;
			}

			return current;
		}

		// Merge the two trees. The representative of first will become the 
		// parent of the representative of second
		public void Union(TItem first, TItem second)
		{
			var firstRepresentative = FindNode(first);
			var secondRepresentative = FindNode(second);

			// If they're in the same set, do nothing
			if (!firstRepresentative.NodeData.Equals(secondRepresentative.NodeData))
			{
				secondRepresentative.Parent = firstRepresentative;
				Count--;
			}
		}

		// Return true if the two items are in the same set
		public bool SameSet(TItem first, TItem second)
		{
			return Find(first).Equals(Find(second));
		}
	}
}
