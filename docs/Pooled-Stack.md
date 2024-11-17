---
layout: default
title: Pooled Array 
nav_order: 10
---
# PooledStack
A stack implementation built on `PooledArray`.

## Important
The `PooledStack` class uses `ArrayPool` internally and must be disposed to avoid memory leaks. Always wrap instances in a `using` block or explicitly call `Dispose()`.

## Properties

| Property | Description |
|----------|-------------|
| **Count** | The number of items in the stack. |

## Methods

| Method | Description |
|--------|-------------|
| **Push(T item)** | Pushes an item onto the stack. |
| **Pop()** | Removes and returns the top item. |
| **Peek()** | Returns the top item without removing it. |
| **Clear()** | Removes all items from the stack. |
| **Dispose()** | Disposes the stack and releases the resources. |

## Examples

### Example 1: Basic Usage
```csharp
using (var stack = new PooledStack<int>())
{
    stack.Push(42);
    Console.WriteLine(stack.Pop()); // 42
}
```

### Example 2: Basic Usage with Strings
```csharp
using (var stack = new PooledStack<string>())
{
    stack.Push("Hello");
    stack.Push("World");
    Console.WriteLine(stack.Pop()); // World
    Console.WriteLine(stack.Peek()); // Hello
    stack.Clear();
}
```

### Example 3: Handling Exceptions
```csharp
try
{
    using (var stack = new PooledStack<int>())
    {
        stack.Pop(); // This will throw an InvalidOperationException
    }
}
catch (InvalidOperationException ex)
{
    Console.WriteLine(ex.Message); // Stack is empty.
}
```

### Example 4: Custom Initial Capacity
```csharp
using (var stack = new PooledStack<int>(32))
{
    for (int i = 0; i < 32; i++)
    {
        stack.Push(i);
    }
    Console.WriteLine(stack.Count); // 32
}
```




