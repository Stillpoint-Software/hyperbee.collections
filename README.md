# Hyperbee.Collection

Additional collections types and extensions.

## Reference Documentation

### AhoCorasickTrie

The `AhoCorasickTrie` class is an implementation of the Aho-Corasick algorithm for pattern matching in text.

#### Properties
- **IgnoreCase**: Determines if the trie ignores case during matching.

#### Methods
- `void Add(string match)`: Adds a search phrase to the trie.
- `IEnumerable<FindResult<TTag>> Find(string text)`: Finds all matches for the given text.

#### Example
```csharp
var trie = new AhoCorasickTrie<string>();
trie.Add("apple", "fruit");
trie.Add("app", "prefix");
var results = trie.Find("apple pie");
foreach (var result in results)
{
    Console.WriteLine($"Match: {result.Tag}, Index: {result.Index}");
}
```


### ConcurrentSet

The `ConcurrentSet<T>` provides a thread-safe set implementation.

#### Properties
- **Count**: The number of items in the set.
- **IsEmpty**: Indicates whether the set is empty.

#### Methods
- `bool TryAdd(T item)`: Attempts to add an item.
- `bool TryRemove(T item)`: Attempts to remove an item.
- `void Clear()`: Removes all items from the set.
- `bool Contains(T item)`: Checks if the set contains an item.

#### Example
```csharp
var set = new ConcurrentSet<int>();
set.TryAdd(1);
Console.WriteLine(set.Contains(1)); // True
set.TryRemove(1);
```


### Deque

A double-ended queue supporting efficient operations at both ends.

#### Properties
- **Count**: The number of items in the deque.
- **IsEmpty**: Indicates whether the deque is empty.

#### Methods
- `void AddFirst(T item)`: Adds an item to the front.
- `void AddLast(T item)`: Adds an item to the end.
- `T RemoveFirst()`: Removes and returns the front item.
- `T RemoveLast()`: Removes and returns the last item.
- `void Insert(int index, T item)`: Inserts an item at a specific index.
- `void RemoveAt(int index)`: Removes an item at a specific index.
- `T this[int index]`: Gets or sets the item at the specified index.

#### Example
```csharp
var deque = new Deque<int>();
deque.AddFirst(1);
deque.AddLast(2);

Console.WriteLine(deque.RemoveFirst()); // 1
```

### DisjointSet

The `DisjointSet` class provides an implementation of the union-find algorithm, which is used to efficiently manage and query disjoint sets.

#### Properties
- **Count**: The number of disjoint sets.
- **Size**: The size of the underlying data structure.

#### Methods
- `void Clear()`: Clears the set.
- `bool TryAdd(T item)`: Creates a new set containing the specified item.
- `bool TryAdd(IEnumerable<T> items)`: Creates a new set containing the specified items.
- `T Find(T item)`: Finds the representative of the set containing the specified item.
- `void Union(T item1, T item2)`: Merges the sets containing the two specified items.
- `bool AreConnected(T item1, T item2)`: Checks if two items belong to the same set.

#### Example
```csharp
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
Console.WriteLine(AreConnected(1, 2)); // Outputs: False
```

### DynamicDictionary

A dynamic dictionary that allows adding and retrieving dynamic properties.

#### Properties
- **Source**: The underlying dictionary.

#### Methods
- `dynamic this[string name]`: Gets or sets a value dynamically.
- `bool TryGetMember(GetMemberBinder binder, out object result)`: Dynamically retrieves a member value.
- `bool TrySetMember(SetMemberBinder binder, object value)`: Dynamically sets a member value.

#### Example
```csharp
dynamic dict = new DynamicDictionary();
dict.Name = "Hyperbee";
Console.WriteLine(dict.Name); // Hyperbee
```


### LinkedDictionary

A stack of dictionaries with layered key-value lookup that supports pushing and popping scopes.

#### Properties
- **Name**: The name of the current dictionary layer.
- **Comparer**: The equality comparer used for keys.

#### Methods
- `void Push(IEnumerable<KeyValuePair<TKey, TValue>> collection)`: Pushes a new dictionary layer.
- `void Push(string name, IEnumerable<KeyValuePair<TKey, TValue>> collection)`: Pushes a named dictionary layer.
- `LinkedDictionaryNode<TKey, TValue> Pop()`: Pops the top dictionary layer.
- `bool TryGetValue(TKey key, out TValue value)`: Attempts to get a value by key.
- `TValue this[TKey key, KeyValueOptions options]`: Layered key-value assignment.

#### Example
```csharp
var linked = new LinkedDictionary<string, string>();

// push an initial set of keys
linked.Push( new Dictionary<string, string>
{
    ["first"] = "default_first",
    ["last"] = "default_last",
    ["suffix"] = "",
} );

linked.Push( new Dictionary<string, string>
{
    ["first"] = "Tim",            // new scope masks original values
    ["last"] = "Jones",           // new scope masks original values
    ["address"] = "123 Main St."  // New key
} );
```


### OrderedSet

A set that maintains the order of insertion.

#### Properties
- **Comparer**: The equality comparer used for items.
- **Count**: The number of items in the set.

#### Methods
- `bool TryGetValue(T key, out T item)`: Gets a value if it exists.
- `void Add(T item)`: Adds an item to the set.
- `bool Remove(T item)`: Removes an item from the set.
- `bool Contains(T item)`: Checks if the set contains an item.
- `T this[T key]`: Accesses the set by key.

#### Example
```csharp
var orderedSet = new OrderedSet<int> { 1, 2, 3 };
Console.WriteLine(orderedSet[1]); // 1
```


### PooledArray

A high-performance array pool for reusable arrays.

### Important
The `PooledArray` class uses `ArrayPool` internally and must be disposed to avoid memory leaks. Always wrap instances in a `using` block or explicitly call `Dispose()`.

#### Properties
- **Count**: The number of items in the array.
- `T this[int index]`: Gets or sets the item at the specified index.

#### Methods
- `void Add(T item)`: Adds an item to the array.
- `void Resize(int newSize)`: Resizes the array.
- `void Remove(int index)`: Removes an item at the specified index.
- `void Insert(int index, T item)`: Inserts an item at a specified index.
- `void CopyTo(T[] destination, int sourceIndex, int destinationIndex, int count)`: Copies items to another array.

#### Example
```csharp
using (var pooledArray = new PooledArray<int>(10))
{
    pooledArray.Add(42);
    Console.WriteLine(pooledArray[0]); // 42
}
```


### PooledStack

A stack implementation built on `PooledArray`.

### Important
The `PooledStack` class uses `ArrayPool` internally and must be disposed to avoid memory leaks. Always wrap instances in a `using` block or explicitly call `Dispose()`.

#### Properties
- **Count**: The number of items in the stack.

#### Methods
- `void Push(T item)`: Pushes an item onto the stack.
- `T Pop()`: Removes and returns the top item.
- `T Peek()`: Returns the top item without removing it.
- `void Clear()`: Removes all items from the stack.

#### Example
```csharp
using (var stack = new PooledStack<int>())
{
    stack.Push(42);
    Console.WriteLine(stack.Pop()); // 42
}
```

### Credits

Special thanks to:

- Tom Jacques - for the original [deque implementation](https://github.com/tejacques/Deque).
- Peteris Nikiforovs - for the original [AhoCorasick implementation](https://github.com/pdonald/aho-corasick).
- [Just The Docs](https://github.com/just-the-docs/just-the-docs) for the documentation theme.

## Contributing

We welcome contributions! Please see our [Contributing Guide](https://github.com/Stillpoint-Software/.github/blob/main/.github/CONTRIBUTING.md) 
for more details.