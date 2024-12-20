---
layout: default
title: Deque 
nav_order: 4
---
# Deque
A double-ended queue implementation that allows adding and removing elements from both ends.

## Important
The `Deque` class provides efficient operations for adding and removing elements from both the front and back of the queue.

## Properties

| Property | Description |
|----------|-------------|
| **Count** | The number of items in the deque. |
| **IsReadOnly** | Indicates whether the deque is read-only. |
| **SyncRoot** | An object that can be used to synchronize access to the deque. |
| **IsSynchronized** | Indicates whether access to the deque is synchronized (thread-safe). |

## Methods

| Method | Description |
|--------|-------------|
| **AddToFront(T item)** | Adds an item to the front of the deque. |
| **AddToBack(T item)** | Adds an item to the back of the deque. |
| **RemoveFromFront()** | Removes and returns the item from the front of the deque. |
| **RemoveFromBack()** | Removes and returns the item from the back of the deque. |
| **PeekFront()** | Returns the item at the front of the deque without removing it. |
| **PeekBack()** | Returns the item at the back of the deque without removing it. |
| **Clear()** | Removes all items from the deque. |
| **GetEnumerator()** | Returns an enumerator that iterates through the deque. |
| **CopyTo(Array array, int index)** | Copies the elements of the deque to an array, starting at a particular array index. |

## Examples

### Example 1: Basic Usage
```csharp
var deque = new Deque<int>();
deque.AddToFront(1);
deque.AddToBack(2);
Console.WriteLine(deque.RemoveFromFront()); // 1
Console.WriteLine(deque.RemoveFromBack()); // 2
```

### Example 2: Adding and Removing Items
```csharp
var deque = new Deque<string>();
deque.AddToFront("Hello");
deque.AddToBack("World");
Console.WriteLine(deque.PeekFront()); // Hello
Console.WriteLine(deque.PeekBack()); // World
deque.RemoveFromFront();
Console.WriteLine(deque.PeekFront()); // World
```

### Example 3: Iterating Through the Deque
```csharp
var deque = new Deque<int>();
deque.AddToFront(1);
deque.AddToBack(2);
deque.AddToFront(0);
foreach (var item in deque)
{
    Console.WriteLine(item); // 0, 1, 2
}
```

### Example 4: Clearing the Deque
```csharp
var deque = new Deque<int>();
deque.AddToFront(1);
deque.AddToBack(2);
deque.Clear();
Console.WriteLine(deque.Count); // 0
```

### Example 5: Handling Exceptions
```csharp
try
{
    var deque = new Deque<int>();
    deque.RemoveFromFront(); // This will throw an InvalidOperationException
}
catch (InvalidOperationException ex)
{
    Console.WriteLine(ex.Message); // Deque is empty.
}
```




