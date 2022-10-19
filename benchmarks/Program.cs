using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace benchmarks
{
	class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<MatchCounts>();
        }
    }

	public struct Point 
	{
		public string city { get; init; }
		public double latitude { get; init; }
		public double longitude { get; init; }
		
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

	[MemoryDiagnoser]
	public class MatchCounts
	{
		private Point ourtarget = new Point() 
		{
			latitude = 43.466438,
			longitude = -80.519185
		};

		private VpTree<Point> vpTree = new VpTree<Point>();

		private LinearSearch<Point> linearSearch = new LinearSearch<Point>();	

		public MatchCounts()
		{
			Point[] points = LoadCitiesFromZipFile().ToArray();

			vpTree.Create(points, CalculatePointDistance);

			linearSearch.Create(points, CalculatePointDistance);
		}

		#region One match

		Point[]? resultsLinearOne = null;
		Point[]? resultsVpTreeOne = null;
		
		double[]? distancesLinearOne = null;
		double[]? distancesVpTreeOne = null;

		[Benchmark]
        public void VpTreeOne() => vpTree.Search(ourtarget, 1, out resultsVpTreeOne, out distancesVpTreeOne);

		[Benchmark]
        public void LinearOne() => linearSearch.Search(ourtarget, 1, out resultsLinearOne, out distancesLinearOne);

		#endregion // One match


		#region Ten matches

		Point[]? resultsLinearTen = null;
		Point[]? resultsVpTreeTen = null;
		
		double[]? distancesLinearTen = null;
		double[]? distancesVpTreeTen = null;

		[Benchmark]
        public void VpTreeTen() => vpTree.Search(ourtarget, 10, out resultsVpTreeTen, out distancesVpTreeTen);

		[Benchmark]
        public void LinearTen() => linearSearch.Search(ourtarget, 10, out resultsLinearTen, out distancesLinearTen);

		#endregion // Ten matches


		#region Hundred matches

		Point[]? resultsLinearHundred = null;
		Point[]? resultsVpTreeHundred = null;
		
		double[]? distancesLinearHundred = null;
		double[]? distancesVpTreeHundred = null;

		[Benchmark]
        public void VpTreeHundred() => vpTree.Search(ourtarget, 100, out resultsVpTreeHundred, out distancesVpTreeHundred);

		[Benchmark]
        public void LinearHundred() => linearSearch.Search(ourtarget, 100, out resultsLinearHundred, out distancesLinearHundred);

		#endregion // Hundred matches


		private static List<Point> LoadCitiesFromZipFile()
		{
			List<Point> tempPoints = new List<Point>();

			using (FileStream zipFile = File.OpenRead("cities_small.zip"))
			{
				using (ZipArchive zip = new ZipArchive(zipFile, ZipArchiveMode.Read))
				{
					foreach (ZipArchiveEntry entry in zip.Entries)
					{
						using (StreamReader sr = new StreamReader(entry.Open()))
						{
							string? line = sr.ReadLine();
							line = sr.ReadLine(); // skip first line since it contains just headers and not actual data
							while (line != null)
							{
								string[] splitted = line.Split(',');
								tempPoints.Add(new Point(line, double.Parse(splitted[splitted.Length - 2], CultureInfo.InvariantCulture), double.Parse(splitted[splitted.Length - 1], CultureInfo.InvariantCulture)));
								line = sr.ReadLine();
							}
						}
					}
				}
			}

			return tempPoints;
		}

		private static double CalculatePointDistance(Point p1, Point p2)
		{
			double a = p1.latitude - p2.latitude;
			double b = p1.longitude - p2.longitude;
			return Math.Sqrt(a * a + b * b); // Do NOT remove Math.Sqrt
		}
	}
}