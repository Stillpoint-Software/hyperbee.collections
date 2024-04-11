# Hyperbee.Collection

Additional collections types and extensions.

 ## AhoCorasickTrie
 A trie that will find strings or phrases and return values of type `TData` for each string or phrase found.  Type param `TData` is the type of the data returned when a match is found.

 ## ConcurrentSet
 A set based on `System.Collections.Concurrent.ConcurrentDictionary`

 ## Deque
 A generic [double-ended queue](https://en.wikipedia.org/wiki/Double-ended_queue) class based of the work of [Tom Jacques](https://github.com/tejacques/Deque)

## DynamicDictionaryBase
A base class that supports the ability to easily create dynamic dictionaries with specific value types

## LinkedDictionary

A dictionary comprised of a stack of dictionaries this allows pushing and popping scopes

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

## OrderedDictionary
A dictionary that preserves insertion order for enumerations

## OrderedSet
A set using on `System.Collections.ObjectModel.KeyedCollection`


# Status

| Branch     | Action                                                                                                                                                                                                                      |
|------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `develop`  | [![Build status](https://github.com/Stillpoint-Software/hyperbee.collection/actions/workflows/publish.yml/badge.svg?branch=develop)](https://github.com/Stillpoint-Software/hyperbee.collection/actions/workflows/publish.yml)  |
| `main`     | [![Build status](https://github.com/Stillpoint-Software/hyperbee.collection/actions/workflows/publish.yml/badge.svg)](https://github.com/Stillpoint-Software/hyperbee.collection/actions/workflows/publish.yml)                 |


# Help

See [Todo](https://github.com/Stillpoint-Software/hyperbee.collection/blob/main/docs/todo.md)
