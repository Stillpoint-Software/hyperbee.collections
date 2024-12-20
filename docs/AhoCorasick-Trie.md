---
layout: default
title: AhoCorasick Trie 
nav_order: 2
---

# AhoCorasick Trie 

The `AhoCorasickTrie` class is an implementation of the Aho-Corasick algorithm for pattern matching in text.

## Properties

| Property | Description |
|----------|-------------|
| **IgnoreCase** | Determines if the trie ignores case during matching. |

## Methods

| Method | Description |
|--------|-------------|
| **Add(string match)** | Adds a search phrase to the trie. |
| **IEnumerable<FindResult<TTag>> Find(string text)** | Finds all matches for the given text. |

## Examples

### Example 1: Basic Usage
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

### Example 2: Case Insensitive Matching
```csharp
var trie = new AhoCorasickTrie<string>(ignoreCase: true);
trie.Add("apple", "fruit");
trie.Add("app", "prefix");
var results = trie.Find("Apple pie");
foreach (var result in results)
{
    Console.WriteLine($"Match: {result.Tag}, Index: {result.Index}");
}
```

### Example 3: Multiple Matches
```csharp
var trie = new AhoCorasickTrie<string>();
trie.Add("he", "pronoun");
trie.Add("she", "pronoun");
trie.Add("his", "pronoun");
trie.Add("hers", "pronoun");
var results = trie.Find("ushers");
foreach (var result in results)
{
    Console.WriteLine($"Match: {result.Tag}, Index: {result.Index}");
}
```

### Example 4: Handling Overlaps
```csharp
var trie = new AhoCorasickTrie<string>();
trie.Add("ab", "prefix");
trie.Add("bc", "suffix");
var results = trie.Find("abc");
foreach (var result in results)
{
    Console.WriteLine($"Match: {result.Tag}, Index: {result.Index}, Overlap: {result.Overlap}");
}
```

### Example 5: Empty Trie
```csharp
var trie = new AhoCorasickTrie<string>();
var results = trie.Find("text");
Console.WriteLine($"Number of matches: {results.Count()}"); // 0
```





