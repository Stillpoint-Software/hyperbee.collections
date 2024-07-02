```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3593/23H2/2023Update/SunValley3)
12th Gen Intel Core i9-12900HK, 1 CPU, 20 logical and 14 physical cores
.NET SDK 8.0.300
  [Host]   : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                   | List            | Mean     | Error     | StdDev    | Gen0   | Gen1   | Allocated |
|------------------------- |---------------- |---------:|----------:|----------:|-------:|-------:|----------:|
| LinkedDirectorySelectAll | aa2,bb2,cc2,dd2 | 2.113 μs | 0.5533 μs | 0.0303 μs | 0.4463 | 0.0038 |   5.48 KB |
| LinkedDirectorySelectAll | aa1,bb1,cc1,dd1 | 2.151 μs | 0.5910 μs | 0.0324 μs | 0.4463 | 0.0038 |   5.48 KB |
