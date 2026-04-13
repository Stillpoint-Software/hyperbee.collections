---
layout: default
title: Concurrent Set 
nav_order: 3
---

# Concurrent Set  

The `ConcurrentSet<T>` provides a thread-safe set implementation.

## Properties

| Property | Description |
|----------|-------------|
| **Count** | The number of items in the set. |
| **IsEmpty** | Indicates whether the set is empty. |

## Methods

| Method | Description |
|--------|-------------|
| **TryAdd(T item)** | Attempts to add an item. |
| **TryRemove(T item)** | Attempts to remove an item. |
| **Clear()** | Removes all items from the set. |
| **Contains(T item)** | Checks if the set contains an item. |

## Examples

### Example 1: Basic Usage
```csharp
var set = new ConcurrentSet<int>();
set.TryAdd(1);
Console.WriteLine(set.Contains(1)); // True
set.TryRemove(1);
```

### Example 2: Adding and Removing Items
```csharp
var set = new ConcurrentSet<string>();
set.TryAdd("Hello");
set.TryAdd("World");
Console.WriteLine(set.Contains("Hello")); // True
set.TryRemove("Hello");
Console.WriteLine(set.Contains("Hello")); // False
```

### Example 3: Iterating Through the Set
```csharp
var set = new ConcurrentSet<int>();
set.TryAdd(1);
set.TryAdd(2);
set.TryAdd(3);
foreach (var item in set)
{
    Console.WriteLine(item); // 1, 2, 3
}
```

### Example 4: Clearing the Set
```csharp
var set = new ConcurrentSet<int>();
set.TryAdd(1);
set.TryAdd(2);
set.Clear();
Console.WriteLine(set.Count); // 0
```

### Example 5: Handling Exceptions
```csharp
try
{
    var set = new ConcurrentSet<int>();
    set.TryRemove(0); // This will throw an InvalidOperationException
}
catch (InvalidOperationException ex)
{
    Console.WriteLine(ex.Message); // Item not found in the set.
}
```


