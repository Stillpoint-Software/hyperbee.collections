```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.2033)
12th Gen Intel Core i9-12900HK, 1 CPU, 20 logical and 14 physical cores
.NET SDK 9.0.100
  [Host]   : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2 DEBUG
  ShortRun : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                   | List            | Mean     | Error     | StdDev    | Gen0   | Gen1   | Allocated |
|------------------------- |---------------- |---------:|----------:|----------:|-------:|-------:|----------:|
| LinkedDirectorySelectAll | aa1,bb1,cc1,dd1 | 1.990 μs | 0.6392 μs | 0.0350 μs | 0.4425 | 0.0038 |   5.45 KB |
| LinkedDirectorySelectAll | aa2,bb2,cc2,dd2 | 2.061 μs | 1.0472 μs | 0.0574 μs | 0.4425 | 0.0038 |   5.45 KB |
