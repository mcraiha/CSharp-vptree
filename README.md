# CSharp-vptree
CSharp (C#) version of vantage-point tree (VP tree)

## Build status
[![.NET](https://github.com/mcraiha/CSharp-vptree/actions/workflows/dotnet.yml/badge.svg)](https://github.com/mcraiha/CSharp-vptree/actions/workflows/dotnet.yml)

## Introduction to this project
This is one implementation of vantage-point tree (VP tree) for C#. As usual most of the error handling code has been stripped away, so YMMV.

## Introduction to vantage-point tree (VP tree)
Vantage-point tree as [Wiki says](https://en.wikipedia.org/wiki/Vantage-point_tree): *"A vantage-point tree (or VP tree) is a metric tree that segregates data in a metric space by choosing a position in the space (the "vantage point") and partitioning the data points into two parts: those points that are nearer to the vantage point than a threshold, and those points that are not. By recursively applying this procedure to partition the data into smaller and smaller sets, a tree data structure is created where neighbors in the tree are likely to be neighbors in the space."*

So VP tree (once created) can be used to find data that is distance-wise near certain point. e.g. this project provides implementation (in [Example.cs](https://github.com/mcraiha/CSharp-vptree/blob/master/example/Example.cs)) that allows one to find X nearest cities of given latitude and longitude point.

## Implementation
This project is a C# port of C++ code that was given to the world by **Steve Hanov** in his [blog](http://stevehanov.ca/blog/index.php?id=130). 

[VpTree.cs](https://github.com/mcraiha/CSharp-vptree/blob/master/src/VpTree.cs) has the VP tree related code.  
[LinearSearch.cs](https://github.com/mcraiha/CSharp-vptree/blob/master/example/LinearSearch.cs) provides a basic linear search that you can use for performance comparision.  
[Example.cs](https://github.com/mcraiha/CSharp-vptree/blob/master/exapmle/Example.cs) gives sample code and some performance measurements.  
[tests](https://github.com/mcraiha/CSharp-vptree/blob/master/tests) folder contains test cases.  

You can get the full cities.txt file used in [Example.cs](https://github.com/mcraiha/CSharp-vptree/blob/master/Example.cs) and [Test.cs](https://github.com/mcraiha/CSharp-vptree/blob/master/Test.cs) from [here](http://stevehanov.ca/blog/cities.txt.gz). When extracted the size of the file is 125 806 517 bytes (~119MB). The cities.txt contains duplicate latitude and longitude entries for certain places, so order of results might vary on different runs.

You can find smaller (and zipped) cities example file from [tests/testfiles](tests/testfiles) folder

## Differences between C++ and C# version
Biggest change is that this C# version uses zero based indexing and not iterators (like the C++ version does).

## Don't optimize Math.Sqrt away!
As Steve Hanov said in his blog: *"It is worth repeating that you must use a distance metric that satisfies the triangle inequality. I spent a lot of time wondering why my VP tree was not working. It turns out that I had not bothered to find the square root in the distance calculation. This step is important to satisfy the requirements of a metric space, because if the straight line distance to a <= b+c, it does not necessarily follow that a^2 <= b^2 + c^2."*

Which means that square root calculations for distance metrics are MANDATORY!

## Examples
```
VpTree<Point> vpTree = new VpTree<Point>();
vpTree.Create(points, CalculatePointDistance);
vpTree.Search(targetPoint, 5, out resultsVpTree, out distancesVpTree);
```

## License
This document and source code files are released into the public domain. See [LICENSE](LICENSE) file

