---
layout: default
title: Linked Dictionary 
nav_order: 6
---

# Linked Dictionary

The `LinkedDictionary<TKey, TValue>` class is a stack of dictionaries with layered key-value lookup. This allows for the creation of nested scopes where each scope can mask values from previous scopes. This is particularly useful in scenarios where you need to manage multiple layers of configuration or state.

## Properties

| Property | Description |
|----------|-------------|
| **Name** | The name of the current dictionary layer. |
| **Comparer** | The equality comparer used for keys. |

## Methods

| Method | Description |
|--------|-------------|
| **Push(IEnumerable<KeyValuePair<TKey, TValue>> collection)** | Pushes a new dictionary layer. |
| **Push(string name, IEnumerable<KeyValuePair<TKey, TValue>> collection)** | Pushes a named dictionary layer. |
| **LinkedDictionaryNode<TKey, TValue> Pop()** | Pops the top dictionary layer. |
| **bool TryGetValue(TKey key, out TValue value)** | Attempts to get a value by key. |
| **TValue this[TKey key, KeyValueOptions options]** | Layered key-value assignment. |
| **void Clear(KeyValueOptions options)** | Clears the dictionary layers based on the specified options. |
| **bool Remove(TKey key, KeyValueOptions options)** | Removes a key from the dictionary layers based on the specified options. |
| **IEnumerable<LinkedDictionaryNode<TKey, TValue>> Nodes()** | Returns the nodes (layers) of the dictionary. |
| **IEnumerable<KeyValuePair<TKey, TValue>> Items(KeyValueOptions options)** | Returns the items in the dictionary layers based on the specified options. |

## Examples

### Example 1: Basic Usage
```csharp
var linked = new LinkedDictionary<string, string>();

// Push an initial set of keys
linked.Push(new Dictionary<string, string>
{
    ["first"] = "default_first",
    ["last"] = "default_last",
    ["suffix"] = "",
});

// Push a new scope that masks original values
linked.Push(new Dictionary<string, string>
{
    ["first"] = "Tim",            // New scope masks original values
    ["last"] = "Jones",           // New scope masks original values
    ["address"] = "123 Main St."  // New key
});

Console.WriteLine(linked["first"]); // Tim
Console.WriteLine(linked["last"]);  // Jones
Console.WriteLine(linked["suffix"]); // (empty string)
Console.WriteLine(linked["address"]); // 123 Main St.
```

### Example 2: Pushing and Popping Scopes
```csharp
var linked = new LinkedDictionary<string, string>();

// Push an initial set of keys
linked.Push(new Dictionary<string, string>
{
    ["first"] = "default_first",
    ["last"] = "default_last",
    ["suffix"] = "",
});

// Push a new scope that masks original values
linked.Push(new Dictionary<string, string>
{
    ["first"] = "Tim",            // New scope masks original values
    ["last"] = "Jones",           // New scope masks original values
    ["address"] = "123 Main St."  // New key
});

// Pop the top scope
linked.Pop();

Console.WriteLine(linked["first"]); // default_first
Console.WriteLine(linked["last"]);  // default_last
Console.WriteLine(linked["suffix"]); // (empty string)
```

### Example 3: Using KeyValueOptions
```csharp
var linked = new LinkedDictionary<string, string>();

// Push an initial set of keys
linked.Push(new Dictionary<string, string>
{
    ["first"] = "default_first",
    ["last"] = "default_last",
    ["suffix"] = "",
});

// Push a new scope that masks original values
linked.Push(new Dictionary<string, string>
{
    ["first"] = "Tim",            // New scope masks original values
    ["last"] = "Jones",           // New scope masks original values
    ["address"] = "123 Main St."  // New key
});

// Set a value in the current scope
linked["first", KeyValueOptions.Current] = "Tom";

Console.WriteLine(linked["first"]); // Tom
```

### Example 4: Disposable Scopes
```csharp
var linked = new LinkedDictionary<string, string>();

// Push an initial set of keys
linked.Push(new Dictionary<string, string>
{
    ["first"] = "default_first",
    ["last"] = "default_last",
    ["suffix"] = "",
});

// Use the Enter extension method to create a new disposable scope
using linked.Enter(new Dictionary<string, string>
{
    ["first"] = "Tim",            // New scope masks original values
    ["last"] = "Jones",           // New scope masks original values
    ["address"] = "123 Main St."  // New scope has address value
})

{
    Console.WriteLine(linked["first"]); // Tim
    Console.WriteLine(linked["last"]);  // Jones
    Console.WriteLine(linked["address"]); // 123 Main St.
}

// After the using block, the scope is popped
Console.WriteLine(linked["first"]); // default_first
Console.WriteLine(linked["last"]);  // default_last
Console.WriteLine(linked["suffix"]); // (empty string)
```

### Example 5: Iterating Items
```csharp
var linked = new LinkedDictionary<string, string>();

// Push an initial set of keys
linked.Push(new Dictionary<string, string>
{
    ["first"] = "default_first",
    ["last"] = "default_last",
    ["suffix"] = "",
});

// Push a new scope that masks original values
linked.Push(new Dictionary<string, string>
{
    ["first"] = "Tim",            // New scope masks original values
    ["last"] = "Jones",           // New scope masks original values
    ["address"] = "123 Main St."  // New scope has address value
});

// Iterate through all layers
foreach (var pair in linked.Items())
{
    Console.WriteLine($"{pair.Key}: {pair.Value}");
}

// Outputs
//
// first: Tim
// last: Jones
// address: 123 Main St.
// suffix: (empty string)
```


### Example 6: Iterating Each Layer
```csharp
var linked = new LinkedDictionary<string, string>();

// Push an initial set of keys
linked.Push(new Dictionary<string, string>
{
    ["first"] = "default_first",
    ["last"] = "default_last",
    ["suffix"] = "",
});

// Push a new scope that masks original values
linked.Push(new Dictionary<string, string>
{
    ["first"] = "Tim",            // New scope masks original values
    ["last"] = "Jones",           // New scope masks original values
    ["address"] = "123 Main St."  // New scope has address value
});

// Iterate through all layers
foreach (var node in linked.Nodes())
{
    Console.WriteLine($"Layer:");
    foreach (var pair in node.Dictionary)
    {
        Console.WriteLine($" {pair.Key}: {pair.Value}");
    }
}
// Outputs:
// Layer:
//  first: Tim
//  last: Jones
//  address: 123 Main St.
// Layer: 
//  first: default_first
//  last: default_last
//  suffix: (empty string)
```
