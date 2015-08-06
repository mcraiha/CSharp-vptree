using System;
using System.Collections.Generic;
using System.Globalization;

namespace Test
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

		static int errorCount = 0;

		public static void Main (string[] args)
		{
			Point[] points = null;

			LoadCities(out points);

			LinearSearch<Point> linearSearch = new LinearSearch<Point>();
			linearSearch.Create(points, CalculatePointDistance);

			VpTree<Point> vpTree = new VpTree<Point>();
			vpTree.Create(points, CalculatePointDistance);

			Point[] resultsLinear = null;
			Point[] resultsVpTree = null;
			
			double[] distancesLinear = null;
			double[] distancesVpTree = null;

			System.Random rand = new System.Random();
			Point ourtarget;
			
			for (int i = 0; i < 100; i++)
			{
				ourtarget = new Point("random", rand.NextDouble() * 180 - 90, rand.NextDouble() * 180 - 90);

				linearSearch.Search(ourtarget, 5+i/10, out resultsLinear, out distancesLinear);

				vpTree.Search(ourtarget, 5+i/10, out resultsVpTree, out distancesVpTree);
				
				CompareResults(resultsLinear, resultsVpTree);
			}

			if (errorCount == 0)
			{
				Console.WriteLine("TEST PASSED!");
			}
			else
			{
				Console.WriteLine("TEST FAILED!");
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

		private static void CompareResults(Point[] linearResults, Point[] vpTreeResults)
		{
			if (linearResults.Length != vpTreeResults.Length)
			{
				Console.WriteLine("Size differs: " + linearResults.Length + " vs. " + vpTreeResults.Length + ", this is FAIL!!!");
				errorCount++;
			}
			
			for (int i = 0; i < linearResults.Length; i++)
			{
				// City name lines are different
				if (linearResults[i].city != vpTreeResults[i].city)
				{
					if (Math.Abs(linearResults[i].latitude - vpTreeResults[i].latitude) < Double.Epsilon && Math.Abs(linearResults[i].longitude - vpTreeResults[i].longitude) < Double.Epsilon)
					{
						Console.WriteLine("Cities have same position but different texts:");
						Console.WriteLine(" " + linearResults[i] + " vs. " + vpTreeResults[i]);
					}
					else
					{
						Console.WriteLine("Entries are completely different, this is FAIL!!!");
						Console.WriteLine(" " + linearResults[i] + " vs. " + vpTreeResults[i]);
						errorCount++;
					}
				}
			}
		}

		private static double CalculatePointDistance(Point p1, Point p2)
		{
			double a = p1.latitude - p2.latitude;
			double b = p1.longitude - p2.longitude;
			return Math.Sqrt(a * a + b * b); // Do NOT remove Math.Sqrt
		}
	}
}
