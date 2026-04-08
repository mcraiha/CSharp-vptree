# CSharp-vptree

Managed .NET (.NET 8 and .NET 10) version of vantage-point tree (VP tree)

## How do I use this?

```csharp
using VpTree;

VpTree<Point> vpTree = VpTree<Point>.Create(points, CalculatePointDistance);
vpTree.Search(targetPoint, 5, out resultsVpTree, out distancesVpTree);
```