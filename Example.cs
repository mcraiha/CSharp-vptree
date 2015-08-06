using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;

namespace Example
{
	class MainClass
	{
		public struct Point {
			public string city;
			public double latitude;
			public double longitude;
			
			public Point(string cityName, double newLatitude, double newLongitude)
			{
				city = cityName;
				latitude = newLatitude;
				longitude = newLongitude;
			}
			
			public override string ToString()
			{
				return city;
			}
		};

		public static void Main (string[] args)
		{
			Point[] points = null;
			Stopwatch stopwatch = new Stopwatch();

			stopwatch.Start();
			LoadCities(out points);
			stopwatch.Stop();
			Console.WriteLine("Loading of cities.txt took: " + stopwatch.Elapsed);

			stopwatch.Reset();
			stopwatch.Start();
			LinearSearch<Point> linearSearch = new LinearSearch<Point>();
			linearSearch.Create(points, CalculatePointDistance);
			stopwatch.Stop();
			Console.WriteLine("Creation of linear search took: " + stopwatch.Elapsed);

			stopwatch.Reset();
			stopwatch.Start();
			VpTree<Point> vpTree = new VpTree<Point>();
			vpTree.Create(points, CalculatePointDistance);
			stopwatch.Stop();
			Console.WriteLine("Creation of VP tree search took: " + stopwatch.Elapsed);

			Point[] resultsLinear = null;
			Point[] resultsVpTree = null;
			
			double[] distancesLinear = null;
			double[] distancesVpTree = null;

			Point ourtarget = new Point();
			ourtarget.latitude = 43.466438; // Use same target as Steve Hanov did
			ourtarget.longitude = -80.519185;

			stopwatch.Reset();
			stopwatch.Start();
			linearSearch.Search(ourtarget, 8, out resultsLinear, out distancesLinear);
			stopwatch.Stop();
			Console.WriteLine("Linear search took: " + stopwatch.Elapsed);

			stopwatch.Reset();
			stopwatch.Start();
			vpTree.Search(ourtarget, 8, out resultsVpTree, out distancesVpTree);
			stopwatch.Stop();
			Console.WriteLine("VP tree search took: " + stopwatch.Elapsed);

			Console.WriteLine("RESULTS:");
			for (int i = 0; i < resultsVpTree.Length; i++)
			{
				Console.WriteLine(resultsVpTree[i].city);
				Console.WriteLine(" " + distancesVpTree[i]);
			}
		}

		private static void LoadCities(out Point[] points)
		{
			List<Point> tempPoints = new List<Point> ();
			System.IO.StreamReader tr = new System.IO.StreamReader("cities.txt"); // Adjust the path if needed
			string line = tr.ReadLine();
			line = tr.ReadLine(); // skip first line since it contains just headers and not actual data
			while(line != null)
			{
				string[] splitted = line.Split(',');
				tempPoints.Add(new Point(line, double.Parse(splitted[splitted.Length - 2], CultureInfo.InvariantCulture), double.Parse(splitted[splitted.Length - 1], CultureInfo.InvariantCulture)));
				line = tr.ReadLine();
			}
			
			points = tempPoints.ToArray();
		}

		private static double CalculatePointDistance(Point p1, Point p2)
		{
			double a = p1.latitude - p2.latitude;
			double b = p1.longitude - p2.longitude;
			return Math.Sqrt(a * a + b * b); // Do NOT remove Math.Sqrt
		}
	}
}
