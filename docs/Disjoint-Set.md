---
layout: default
title: Disjoint Set
nav_order: 10
---

# Disjoint Set

The `DisjointSet<T>` class is a data structure that tracks a partition of a set into disjoint (non-overlapping) subsets. It supports efficient union and find operations, making it useful for algorithms like Kruskal's Minimum Spanning Tree and connected components in graphs.

## Important
The `DisjointSet<T>` class is not thread-safe. If used in a multithreaded environment, external synchronization is required.

## Properties

| Property | Description |
|----------|-------------|
| **Count** | The number of elements in the disjoint set. |
| **Keys** | An enumerable collection of all elements in the disjoint set. |

## Methods

| Method | Description |
|--------|-------------|
| **AreConnected(T item1, T item2)** | Returns `ture` if two items share the same representative, or `false` if the items are in different groups. |
| **TryAdd(T item)** | Adds a new element to the disjoint set as its own group. Returns `true` if the element was added, or `false` if it already exists. |
| **TryAdd(IEnumerable<T> items)** | Adds multiple elements to the disjoint set, grouping them together. Returns `true` if at least one element was added, or `false` if all elements already exist. |
| **T Find(T item)** | Finds the representative (root) of the group containing the specified element. Throws an exception if the element does not exist. |
| **IEnumerable<T> FindGroup(T item)** | Returns all elements in the group containing the specified element. |
| **bool Union(T item1, T item2)** | Merges the groups containing the two specified elements. Returns `true` if the groups were merged, or `false` if they were already in the same group. |

## Examples

### Example 1: Basic Usage
```
var set = new DisjointSet<int>();

// Add elements to the set
set.TryAdd(1);
set.TryAdd(2);
set.TryAdd(3);

// Union two elements
set.Union(1, 2);

// Find the representative of a group
Console.WriteLine(set.Find(1)); // Outputs: 1
Console.WriteLine(set.Find(2)); // Outputs: 1 (same group as 1)

// Check if two elements are in the same group
Console.WriteLine(set.Find(1) == set.Find(3)); // Outputs: False
```

### Example 2: Adding Multiple Elements
```
var set = new DisjointSet<string>();

// Add multiple elements and group them together
set.TryAdd(new[] { "A", "B", "C" });

// Find the representative of the group
Console.WriteLine(set.Find("A")); // Outputs: A
Console.WriteLine(set.Find("B")); // Outputs: A (same group as A)
Console.WriteLine(set.Find("C")); // Outputs: A (same group as A)
```

### Example 3: Finding a Group
```
var set = new DisjointSet<int>();

// Add elements and union them
set.TryAdd(1);
set.TryAdd(2);
set.TryAdd(3);
set.Union(1, 2);

// Find all elements in the group containing 1
var group = set.FindGroup(1);
Console.WriteLine(string.Join(", ", group)); // Outputs: 1, 2
```

### Example 4: Using a Custom Equality Comparer
```
var set = new DisjointSet<string>(StringComparer.OrdinalIgnoreCase);

// Add elements with case-insensitive comparison
set.TryAdd("A");
set.TryAdd("a"); // Will not be added because "A" already exists

Console.WriteLine(set.Count); // Outputs: 1
```

### Example 5: Handling Union Events
```
void OnUnion(string representative, string retired)
{
    Console.WriteLine($"Union: {retired} merged into {representative}");
}

var set = new DisjointSet<string>(OnUnion);

// Add elements and union them
set.TryAdd("X");
set.TryAdd("Y");
set.Union("X", "Y"); // Outputs: Union: Y merged into X
```

## Notes
- The `DisjointSet<T>` class uses path compression in the `Find` method to optimize subsequent operations.
- The `FindGroup` method is computationally expensive as it iterates through all elements to find those in the same group.
