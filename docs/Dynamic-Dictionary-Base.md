---
layout: default
title: Dynamic Dictionary Base 
nav_order: 5
---

# Dynamic Dictionary Base

A dynamic dictionary that allows adding and retrieving dynamic properties.

## Properties
- **Source**: The underlying dictionary.

## Methods
- `dynamic this[string name]`: Gets or sets a value dynamically.
- `bool TryGetMember(GetMemberBinder binder, out object result)`: Dynamically retrieves a member value.
- `bool TrySetMember(SetMemberBinder binder, object value)`: Dynamically sets a member value.

## Example
```csharp
dynamic dict = new DynamicDictionary();
dict.Name = "Hyperbee";
Console.WriteLine(dict.Name); // Hyperbee
```
