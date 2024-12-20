---
layout: default
title: Pooled Array 
nav_order: 9
---
# PooledArray
A dynamic array implementation that uses `ArrayPool` for efficient memory management.

## Important
The `PooledArray` class uses `ArrayPool` internally and must be disposed to avoid memory leaks. Always wrap instances in a `using` block or explicitly call `Dispose()`.

## Properties

| Property | Description |
|----------|-------------|
| **Count** | The number of items in the array. |

## Methods

| Method | Description |
|--------|-------------|
| **Add(T item)** | Adds an item to the array. |
| **CopyTo(Func<T, bool> predicate)** | Copies items that match the predicate to a new `PooledArray`. |
| **CopyTo(PooledArray<T> destination, int sourceIndex, int destinationIndex, int count)** | Copies a range of elements from the array to another `PooledArray`. |
| **CopyTo(T[] destination, int sourceIndex, int destinationIndex, int count)** | Copies a range of elements from the array to a standard array. |
| **Insert(int index, T item)** | Inserts an item at the specified index. |
| **Remove(int index)** | Removes the item at the specified index. |
| **Remove(Func<T, bool> predicate)** | Removes items that match the predicate. |
| **Remove(Func<T, int, bool> predicate)** | Removes items that match the predicate with index. |
| **Resize(int newSize)** | Resizes the array to the specified size. |
| **Sort(Comparison<T> comparison)** | Sorts the elements in the array using the specified comparison. |
| **Dispose()** | Disposes the array and releases the resources. |

## Examples

### Example 1: Basic Usage
```csharp
using (var array = new PooledArray<int>())
{
    array.Add(42);
    Console.WriteLine(array[0]); // 42
}
```

### Example 2: Copying Items
```csharp
using (var array = new PooledArray<int>())
{
    array.Add(1);
    array.Add(2);
    array.Add(3);

    var newArray = array.CopyTo(item => item > 1);
    foreach (var item in newArray)
    {
        Console.WriteLine(item); // 2, 3
    }
}
```

### Example 3: Inserting and Removing Items
```csharp
using (var array = new PooledArray<int>())
{
    array.Add(1);
    array.Add(3);
    array.Insert(1, 2);
    Console.WriteLine(array[1]); // 2

    array.Remove(1);
    Console.WriteLine(array[1]); // 3
}
```

### Example 4: Sorting Items
```csharp
using (var array = new PooledArray<int>())
{
    array.Add(3);
    array.Add(1);
    array.Add(2);

    array.Sort((x, y) => x.CompareTo(y));
    foreach (var item in array)
    {
        Console.WriteLine(item); // 1, 2, 3
    }
}
```

### Example 5: Handling Exceptions
```csharp
try
{
    using (var array = new PooledArray<int>())
    {
        array.Remove(0); // This will throw an ArgumentOutOfRangeException
    }
}
catch (ArgumentOutOfRangeException ex)
{
    Console.WriteLine(ex.Message); // Index was out of range.
}
```





