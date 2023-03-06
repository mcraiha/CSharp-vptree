// C# version of Vantage-point tree aka VP tree
// Original C++ source code is made by 
// Steve Hanov and you can find it from
// http://stevehanov.ca/blog/index.php?id=130
//
// This is free and unencumbered software released into the public domain.

public delegate double CalculateDistance<T>(T item1, T item2);

public sealed class VpTree<T> 
{
	public VpTree()
	{
		this.rand = new Random(); // Used in BuildFromPoints
	}

	public void Create(T[] newItems, CalculateDistance<T> distanceCalculator)
	{
		this.items = newItems;
		this.calculateDistance = distanceCalculator;
		this.root = this.BuildFromPoints(0, newItems.Length);
	}

	public void Search(T target, int numberOfResults, out T[] results, out double[] distances)
	{
		List<HeapItem> closestHits = new List<HeapItem>();

		// Reset tau to longest possible distance
		this.tau = double.MaxValue;

		// Start search
		Search(root, target, numberOfResults, closestHits);

		// Temp arrays for return values
		List<T> returnResults = new List<T>();
		List<double> returnDistance = new List<double>();

		// We have to reverse the order since we want the nearest object to be first in the array
		for (int i = numberOfResults - 1; i > -1; i--)
		{
			returnResults.Add(this.items[closestHits[i].index]);
			returnDistance.Add(closestHits[i].dist);
		}

		results = returnResults.ToArray();
		distances = returnDistance.ToArray();
	}

	private T[] items;
	private double tau;
	private Node root;
	private Random rand; // Used in BuildFromPoints

	private CalculateDistance<T> calculateDistance;

	private sealed class Node // This cannot be struct because Node referring to Node causes error CS0523
	{
		public int index;
		public double threshold;
		public Node? left;
		public Node? right;

		public Node()
		{
			this.index = 0;
			this.threshold = 0.0;
			this.left = null;
			this.right = null;
		}
	}

	private sealed class HeapItem
	{
		public readonly int index;
		public readonly double dist;

		public HeapItem(int index, double dist)
		{
			this.index = index;
			this.dist = dist;
		}

		public static bool operator <(HeapItem h1, HeapItem h2)
		{
			return h1.dist < h2.dist;
		}

		public static bool operator >(HeapItem h1, HeapItem h2)
		{
			return h1.dist > h2.dist;
		}
	}

	private Node? BuildFromPoints(int lowerIndex, int upperIndex)
	{
		if (upperIndex == lowerIndex)
		{
			return null;
		}

		Node node = new Node();
		node.index = lowerIndex;

		if (upperIndex - lowerIndex > 1)
		{
			Swap(items, lowerIndex, this.rand.Next(lowerIndex + 1, upperIndex));

			int medianIndex = ( upperIndex + lowerIndex ) / 2;

			nth_element(items, lowerIndex + 1, medianIndex, upperIndex - 1, 
			            (i1, i2) => System.Collections.Generic.Comparer<double>.Default.Compare(calculateDistance(items[lowerIndex], i1), calculateDistance(items[lowerIndex], i2)));

			node.threshold = this.calculateDistance(this.items[lowerIndex], this.items[medianIndex]);

			node.left = BuildFromPoints(lowerIndex + 1, medianIndex);
			node.right = BuildFromPoints(medianIndex, upperIndex);
		}

		return node;
	}
	
	private void Search(Node? node, T target, int numberOfResults, List<HeapItem> closestHits)
	{
		if (node == null)
		{
			return;
		}

		double dist = this.calculateDistance(this.items[node.index], target);

		/// We found entry with shorter distance
		if (dist < this.tau)
		{
			if (closestHits.Count == numberOfResults)
			{
				// Too many results, remove the first one which has the longest distance
				closestHits.RemoveAt(0);
			}

			// Add new hit
			closestHits.Add(new HeapItem(node.index, dist));

			// Reorder if we have numberOfResults, and set new tau
			if (closestHits.Count == numberOfResults)
			{
				closestHits.Sort((a, b) => System.Collections.Generic.Comparer<double>.Default.Compare(b.dist, a.dist));
				this.tau = closestHits[0].dist;
			}
		}

		if (node.left == null && node.right == null)
		{
			return;
		}

		if (dist < node.threshold)
		{
			if (dist - this.tau <= node.threshold)
			{
				this.Search(node.left, target, numberOfResults, closestHits);
			}

			if (dist + this.tau >= node.threshold)
			{
				this.Search(node.right, target, numberOfResults, closestHits);
			}
		}
		else
		{
			if (dist + this.tau >= node.threshold)
			{
				this.Search(node.right, target, numberOfResults, closestHits);
			}

			if (dist - this.tau <= node.threshold)
			{
				this.Search(node.left, target, numberOfResults, closestHits);
			}
		}
	}

	private static void Swap(T[] arr, int index1, int index2)
	{
		T temp = arr[index1];
		arr[index1] = arr[index2];
		arr [index2] = temp;
	}

	private static void nth_element<T>(T[] array, int startIndex, int nthToSeek, int endIndex, Comparison<T> comparison)
	{
		int from = startIndex; 
		int to = endIndex;
		
		// if from == to we reached the kth element
		while (from < to) 
		{
			int r = from, w = to;
			T mid = array[(r + w) / 2];
			
			// stop if the reader and writer meets
			while (r < w) 
			{
				if (comparison(array[r], mid) > -1) 
				{ // put the large values at the end
					T tmp = array[w];
					array[w] = array[r];
					array[r] = tmp;
					w--;
				} 
				else 
				{ // the value is smaller than the pivot, skip
					r++;
				}
			}
			
			// if we stepped up (r++) we need to step one down
			if (comparison(array[r], mid) > 0)
			{
				r--;
			}
			
			// the r pointer is on the end of the first k elements
			if (nthToSeek <= r) 
			{
				to = r;
			} 
			else 
			{
				from = r + 1;
			}
		}
		
		return;
	}
}
