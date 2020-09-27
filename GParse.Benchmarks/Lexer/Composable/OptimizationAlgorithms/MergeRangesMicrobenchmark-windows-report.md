``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.508 (2004/?/20H1)
AMD Ryzen 7 2700, 1 CPU, 16 logical and 8 physical cores
  [Host]        : .NET Framework 4.8 (4.8.4220.0), X64 RyuJIT
  .NET 4.6.1    : .NET Framework 4.8 (4.8.4220.0), X64 RyuJIT
  .NET 4.8      : .NET Framework 4.8 (4.8.4220.0), X64 RyuJIT
  .NET Core 2.1 : .NET Core 2.1.21 (CoreCLR 4.6.29130.01, CoreFX 4.6.29130.02), X64 RyuJIT
  .NET Core 3.1 : .NET Core 3.1.7 (CoreCLR 4.700.20.36602, CoreFX 4.700.20.37001), X64 RyuJIT
  .NET Core 5.0 : .NET Core 5.0.0 (CoreCLR 5.0.20.45114, CoreFX 5.0.20.45114), X64 RyuJIT


```
|   Method |           Job |       Runtime |               Ranges |      Mean |    Error |   StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated | Code Size |
|--------- |-------------- |-------------- |--------------------- |----------:|---------:|---------:|------:|--------:|-------:|------:|------:|----------:|----------:|
|  **InPlace** |    **.NET 4.6.1** |    **.NET 4.6.1** | **a-b|c(...)u|v-w [43]** | **388.08 ns** | **2.168 ns** | **1.811 ns** |  **1.00** |    **0.00** | **0.4182** |     **-** |     **-** |     **329 B** |    **1237 B** |
| OutPlace |    .NET 4.6.1 |    .NET 4.6.1 | a-b|c(...)u|v-w [43] | 258.38 ns | 1.019 ns | 0.903 ns |  0.67 |    0.00 | 0.6118 |     - |     - |     481 B |    1221 B |
|          |               |               |                      |           |          |          |       |         |        |       |       |           |           |
|  InPlace |      .NET 4.8 |      .NET 4.8 | a-b|c(...)u|v-w [43] | 386.20 ns | 1.453 ns | 1.288 ns |  1.00 |    0.00 | 0.4182 |     - |     - |     329 B |    1237 B |
| OutPlace |      .NET 4.8 |      .NET 4.8 | a-b|c(...)u|v-w [43] | 257.39 ns | 2.353 ns | 2.201 ns |  0.67 |    0.01 | 0.6118 |     - |     - |     481 B |    1221 B |
|          |               |               |                      |           |          |          |       |         |        |       |       |           |           |
|  InPlace | .NET Core 2.1 | .NET Core 2.1 | a-b|c(...)u|v-w [43] | 321.86 ns | 1.922 ns | 1.605 ns |  1.00 |    0.00 | 0.2742 |     - |     - |     216 B |    1310 B |
| OutPlace | .NET Core 2.1 | .NET Core 2.1 | a-b|c(...)u|v-w [43] | 196.74 ns | 1.069 ns | 1.000 ns |  0.61 |    0.00 | 0.4678 |     - |     - |     368 B |    1349 B |
|          |               |               |                      |           |          |          |       |         |        |       |       |           |           |
|  InPlace | .NET Core 3.1 | .NET Core 3.1 | a-b|c(...)u|v-w [43] | 296.51 ns | 3.006 ns | 2.811 ns |  1.00 |    0.00 | 0.0515 |     - |     - |     216 B |    1276 B |
| OutPlace | .NET Core 3.1 | .NET Core 3.1 | a-b|c(...)u|v-w [43] | 184.60 ns | 1.640 ns | 1.370 ns |  0.62 |    0.01 | 0.0861 |     - |     - |     360 B |    1306 B |
|          |               |               |                      |           |          |          |       |         |        |       |       |           |           |
|  InPlace | .NET Core 5.0 | .NET Core 5.0 | a-b|c(...)u|v-w [43] | 293.84 ns | 1.573 ns | 1.394 ns |  1.00 |    0.00 | 0.0515 |     - |     - |     216 B |    1276 B |
| OutPlace | .NET Core 5.0 | .NET Core 5.0 | a-b|c(...)u|v-w [43] | 168.14 ns | 1.041 ns | 0.869 ns |  0.57 |    0.00 | 0.0861 |     - |     - |     360 B |    1333 B |
|          |               |               |                      |           |          |          |       |         |        |       |       |           |           |
|  **InPlace** |    **.NET 4.6.1** |    **.NET 4.6.1** |      **a-d|e-h|j-m|n-t** | **141.07 ns** | **1.119 ns** | **1.047 ns** |  **1.00** |    **0.00** | **0.1326** |     **-** |     **-** |     **104 B** |    **1237 B** |
| OutPlace |    .NET 4.6.1 |    .NET 4.6.1 |      a-d|e-h|j-m|n-t | 142.06 ns | 0.369 ns | 0.288 ns |  1.01 |    0.01 | 0.2549 |     - |     - |     201 B |    1221 B |
|          |               |               |                      |           |          |          |       |         |        |       |       |           |           |
|  InPlace |      .NET 4.8 |      .NET 4.8 |      a-d|e-h|j-m|n-t | 140.68 ns | 0.892 ns | 0.835 ns |  1.00 |    0.00 | 0.1326 |     - |     - |     104 B |    1237 B |
| OutPlace |      .NET 4.8 |      .NET 4.8 |      a-d|e-h|j-m|n-t | 141.62 ns | 0.354 ns | 0.314 ns |  1.01 |    0.01 | 0.2549 |     - |     - |     201 B |    1221 B |
|          |               |               |                      |           |          |          |       |         |        |       |       |           |           |
|  InPlace | .NET Core 2.1 | .NET Core 2.1 |      a-d|e-h|j-m|n-t | 107.32 ns | 0.748 ns | 0.584 ns |  1.00 |    0.00 | 0.0609 |     - |     - |      48 B |    1310 B |
| OutPlace | .NET Core 2.1 | .NET Core 2.1 |      a-d|e-h|j-m|n-t | 108.09 ns | 1.178 ns | 1.044 ns |  1.01 |    0.01 | 0.1830 |     - |     - |     144 B |    1349 B |
|          |               |               |                      |           |          |          |       |         |        |       |       |           |           |
|  InPlace | .NET Core 3.1 | .NET Core 3.1 |      a-d|e-h|j-m|n-t |  91.20 ns | 1.147 ns | 0.958 ns |  1.00 |    0.00 | 0.0114 |     - |     - |      48 B |    1276 B |
| OutPlace | .NET Core 3.1 | .NET Core 3.1 |      a-d|e-h|j-m|n-t |  96.22 ns | 1.181 ns | 1.047 ns |  1.05 |    0.01 | 0.0324 |     - |     - |     136 B |    1306 B |
|          |               |               |                      |           |          |          |       |         |        |       |       |           |           |
|  InPlace | .NET Core 5.0 | .NET Core 5.0 |      a-d|e-h|j-m|n-t |  97.72 ns | 0.434 ns | 0.385 ns |  1.00 |    0.00 | 0.0114 |     - |     - |      48 B |    1276 B |
| OutPlace | .NET Core 5.0 | .NET Core 5.0 |      a-d|e-h|j-m|n-t | 101.43 ns | 1.488 ns | 1.391 ns |  1.04 |    0.02 | 0.0324 |     - |     - |     136 B |    1333 B |
|          |               |               |                      |           |          |          |       |         |        |       |       |           |           |
|  **InPlace** |    **.NET 4.6.1** |    **.NET 4.6.1** |              **a-e|g-k** |  **85.41 ns** | **0.664 ns** | **0.588 ns** |  **1.00** |    **0.00** | **0.0509** |     **-** |     **-** |      **40 B** |    **1237 B** |
| OutPlace |    .NET 4.6.1 |    .NET 4.6.1 |              a-e|g-k | 113.29 ns | 0.594 ns | 0.556 ns |  1.33 |    0.01 | 0.1529 |     - |     - |     120 B |    1221 B |
|          |               |               |                      |           |          |          |       |         |        |       |       |           |           |
|  InPlace |      .NET 4.8 |      .NET 4.8 |              a-e|g-k |  86.19 ns | 0.393 ns | 0.349 ns |  1.00 |    0.00 | 0.0509 |     - |     - |      40 B |    1237 B |
| OutPlace |      .NET 4.8 |      .NET 4.8 |              a-e|g-k | 112.93 ns | 0.612 ns | 0.542 ns |  1.31 |    0.01 | 0.1529 |     - |     - |     120 B |    1221 B |
|          |               |               |                      |           |          |          |       |         |        |       |       |           |           |
|  InPlace | .NET Core 2.1 | .NET Core 2.1 |              a-e|g-k |  54.49 ns | 0.840 ns | 0.786 ns |  1.00 |    0.00 |      - |     - |     - |         - |    1310 B |
| OutPlace | .NET Core 2.1 | .NET Core 2.1 |              a-e|g-k |  85.99 ns | 0.342 ns | 0.285 ns |  1.58 |    0.02 | 0.1017 |     - |     - |      80 B |    1349 B |
|          |               |               |                      |           |          |          |       |         |        |       |       |           |           |
|  InPlace | .NET Core 3.1 | .NET Core 3.1 |              a-e|g-k |  48.08 ns | 0.222 ns | 0.207 ns |  1.00 |    0.00 |      - |     - |     - |         - |    1276 B |
| OutPlace | .NET Core 3.1 | .NET Core 3.1 |              a-e|g-k |  72.30 ns | 1.049 ns | 0.930 ns |  1.50 |    0.02 | 0.0172 |     - |     - |      72 B |    1306 B |
|          |               |               |                      |           |          |          |       |         |        |       |       |           |           |
|  InPlace | .NET Core 5.0 | .NET Core 5.0 |              a-e|g-k |  47.78 ns | 0.116 ns | 0.097 ns |  1.00 |    0.00 |      - |     - |     - |         - |    1276 B |
| OutPlace | .NET Core 5.0 | .NET Core 5.0 |              a-e|g-k |  73.65 ns | 0.748 ns | 0.625 ns |  1.54 |    0.01 | 0.0172 |     - |     - |      72 B |    1333 B |
