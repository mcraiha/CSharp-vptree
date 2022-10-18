// C# version of Linear Search
// Original C++ source code is made by 
// Steve Hanov and you can find it from
// http://stevehanov.ca/blog/cities.cpp
//
// This is free and unencumbered software released into the public domain.

using System.Collections;
using System.Collections.Generic;
using System;

public sealed class LinearSearch<T> 
{
	// Empty constructor
	public LinearSearch()
	{

	}

	// Call Create before you do any Search
	public void Create(T[] newItems, CalculateDistance<T> distanceCalculator)
	{
		this.items = newItems;
		this.calculateDistance = distanceCalculator;
	}

	private sealed class HeapItem
	{
		public int index;
		public double dist;
		
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
	

	public void Search(T target, int numberOfResults, out T[] results, out double[] distances)
	{
		List<HeapItem> closestHits = new List<HeapItem>();
		for (int i = 0; i < items.Length; i++)
		{
			double distance = this.calculateDistance(target, items[i]);
			if (closestHits.Count < numberOfResults || distance < closestHits[0].dist)
			{
				closestHits.Add(new HeapItem(i, distance));
				if (closestHits.Count > numberOfResults)
				{
					closestHits.Sort((a, b) => System.Collections.Generic.Comparer<double>.Default.Compare(b.dist, a.dist));
					closestHits.RemoveAt(0);
				}
			}
		}
		
		List<T> returnResults = new List<T>();
		List<double> returnDistance = new List<double>();
		
		// We have to reverse the order since we want the nearest object to be first in the array
		for (int i = numberOfResults - 1; i > -1; i--)
		{
			returnResults.Add(items[closestHits[i].index]);
			returnDistance.Add(closestHits[i].dist);
		}
		
		results = returnResults.ToArray();
		distances = returnDistance.ToArray();
	}

	private T[] items;
	private CalculateDistance<T> calculateDistance;
}
