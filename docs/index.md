---
layout: default
title: Hyperbee Collections
nav_order: 1
---

# Hyperbee Collections

Additional collections types and extensions.

* **AhoCorasickTrie** - A trie that will find strings or phrases and return values of type `TData` for each string or phrase found.  Type param `TData` is the type of the data returned when a match is found.

* **ConcurrentSet** - A set based on `System.Collections.Concurrent.ConcurrentDictionary`

* **Deque** - A generic [double-ended queue](https://en.wikipedia.org/wiki/Double-ended_queue) class based of the work of [Tom Jacques](https://github.com/tejacques/Deque)

* **DynamicDictionaryBase** - A base class that supports the ability to easily create dynamic dictionaries with specific value types

* **LinkedDictionary** - A dictionary comprised of a stack of dictionaries this allows pushing and popping scopes

* **OrderedDictionary** - A dictionary that preserves insertion order for enumerations

* **OrderedSet** - A set using on `System.Collections.ObjectModel.KeyedCollection`

* **PooledArray** - A generic array that uses the array-pool

* **PooledStack** - A generic stack that uses the array-pool
