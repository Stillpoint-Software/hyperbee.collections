---
layout: default
title: Linked Dictionary 
nav_order: 6
---

# Linked Dictionary

A dictionary comprised of a stack of dictionaries this allows pushing and popping scopes

## Example
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