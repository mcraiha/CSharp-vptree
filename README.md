# CSharp-vptree
CSharp (C#) version of vantage-point tree (VP tree)

## Introduction to this project
This is one implementation of vantage-point tree (VP tree) for C#. As usual most of the error handling code has been stripped away, so YMMV

## Introduction to vantage-point tree (VP tree)
Vantage-point tree as Wiki says: "VP tree is a BSP tree that segregates data in a metric space by choosing a position in the space (the "vantage point") and dividing the data points into two partitions: those that are nearer to the vantage point than a threshold, and those that are not."

So VP tree (once created) can be used to find data that is distance-wise near certain point. e.g. this project provides implementation that allows one to find X nearest cities of given latitude and longitude point.

## Implementation
This project is a C# port of C++ code that was given to the world by **Steve Hanov** in his [blog](http://stevehanov.ca/blog/index.php?id=130). Biggest change as usual is that this C# version uses zero based indexing and not iterators (like the C++ version does).
