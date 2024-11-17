---
layout: default
title: Ordered Set
nav_order: 8
---
# OrderedSet
A set implementation that maintains the order of elements.

## Important
The `OrderedSet` class ensures that elements are unique and maintains the order in which they were added.

## Properties

| Property | Description |
|----------|-------------|
| **Count** | The number of items in the set. |

## Methods

| Method | Description |
|--------|-------------|
| **Add(T item)** | Adds an item to the set. |
| **Remove(T item)** | Removes the specified item from the set. |
| **Contains(T item)** | Determines whether the set contains a specific item. |
| **Clear()** | Removes all items from the set. |
| **GetEnumerator()** | Returns an enumerator that iterates through the set. |

## Examples

### Example 1: Basic Usage
```csharp
var set = new OrderedSet<int>();
set.Add(1);
set.Add(2);
set.Add(3);
Console.WriteLine(set.Count); // 3
```

### Example 2: Adding and Removing Items
```csharp
var set = new OrderedSet<string>();
set.Add("Hello");
set.Add("World");
Console.WriteLine(set.Contains("Hello")); // True
set.Remove("Hello");
Console.WriteLine(set.Contains("Hello")); // False
```

### Example 3: Iterating Through the Set
```csharp
var set = new OrderedSet<int>();
set.Add(1);
set.Add(2);
set.Add(3);
foreach (var item in set)
{
    Console.WriteLine(item); // 1, 2, 3
}
```

### Example 4: Clearing the Set
```csharp
var set = new OrderedSet<int>();
set.Add(1);
set.Add(2);
set.Clear();
Console.WriteLine(set.Count); // 0
```





