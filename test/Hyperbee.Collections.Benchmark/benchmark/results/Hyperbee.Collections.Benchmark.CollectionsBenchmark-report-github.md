```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7462/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i9-12900HK 2.50GHz, 1 CPU, 20 logical and 14 physical cores
.NET SDK 10.0.101
  [Host]   : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3 DEBUG
  ShortRun : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                   | List            | Mean     | Error     | StdDev    | Gen0   | Gen1   | Allocated |
|------------------------- |---------------- |---------:|----------:|----------:|-------:|-------:|----------:|
| LinkedDirectorySelectAll | aa2,bb2,cc2,dd2 | 1.781 μs | 0.2390 μs | 0.0131 μs | 0.4444 | 0.0038 |   5.46 KB |
| LinkedDirectorySelectAll | aa1,bb1,cc1,dd1 | 1.880 μs | 0.7184 μs | 0.0394 μs | 0.4444 | 0.0038 |   5.46 KB |
