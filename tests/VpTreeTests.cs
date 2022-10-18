using NUnit.Framework;
using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;

namespace tests;

public class Tests
{
	private struct Point 
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

	[SetUp]
	public void Setup()
	{

	}

	[Test]
	public void TestWithSmallCitiesExample()
	{
		// Arrange
		VpTree<Point> vpTree = new VpTree<Point>();
		List<Point> points = LoadCitiesFromZipFile();

		Point[]? resultsVpTree = null;
		double[]? distancesVpTree = null;

		Point ourtarget = new Point() 
		{
			latitude = 43.466438,
			longitude = -80.519185
		};

		const int howManyToSeek = 8;

		// Act
		vpTree.Create(points.ToArray(), CalculatePointDistance);
		vpTree.Search(ourtarget, howManyToSeek, out resultsVpTree, out distancesVpTree);

		// Assert
		Assert.AreEqual(howManyToSeek, resultsVpTree.Length);
		Assert.AreEqual(howManyToSeek, distancesVpTree.Length);

		Assert.IsTrue(resultsVpTree[0].city.Contains("Crediton"));
		Assert.IsTrue(resultsVpTree[1].city.Contains("Thornhill"));
		Assert.IsTrue(resultsVpTree[2].city.Contains("Newmarket"));
		Assert.IsTrue(resultsVpTree[3].city.Contains("Port Colborne"));
		Assert.IsTrue(resultsVpTree[4].city.Contains("North East"));
		Assert.IsTrue(resultsVpTree[5].city.Contains("Belleview Heights"));
		Assert.IsTrue(resultsVpTree[6].city.Contains("Girard"));
		Assert.IsTrue(resultsVpTree[7].city.Contains("Stella Niagara"));

		CollectionAssert.AllItemsAreUnique(distancesVpTree);
		for (int i = 0; i < howManyToSeek - 2; i++)
		{
			Assert.Less(distancesVpTree[i], distancesVpTree[i + 1], "Distances are not in sorted order!");
		}
	}

	private static List<Point> LoadCitiesFromZipFile()
	{
		List<Point> tempPoints = new List<Point>();

		using (FileStream zipFile = File.OpenRead(Path.Combine(TestContext.CurrentContext.TestDirectory,"testfiles/cities_small.zip")))
		{
			using (ZipArchive zip = new ZipArchive(zipFile, ZipArchiveMode.Read))
			{
				foreach (ZipArchiveEntry entry in zip.Entries)
				{
					using (StreamReader sr = new StreamReader(entry.Open()))
					{
						string? line = sr.ReadLine();
						line = sr.ReadLine(); // skip first line since it contains just headers and not actual data
						while(line != null)
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