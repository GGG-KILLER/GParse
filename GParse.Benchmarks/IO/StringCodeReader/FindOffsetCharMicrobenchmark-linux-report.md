``` ini

BenchmarkDotNet=v0.12.1, OS=debian 10
AMD Ryzen 7 2700, 1 CPU, 16 logical and 8 physical cores
.NET Core SDK=5.0.100-rc.1.20452.10
  [Host]        : .NET Core 2.1.22 (CoreCLR 4.6.29220.03, CoreFX 4.6.29220.01), X64 RyuJIT
  .NET Core 2.1 : .NET Core 2.1.22 (CoreCLR 4.6.29220.03, CoreFX 4.6.29220.01), X64 RyuJIT
  .NET Core 3.1 : .NET Core 3.1.8 (CoreCLR 4.700.20.41105, CoreFX 4.700.20.41903), X64 RyuJIT
  .NET Core 5.0 : .NET Core 5.0.0 (CoreCLR 5.0.20.45114, CoreFX 5.0.20.45114), X64 RyuJIT


```
|           Method |           Job |       Runtime | Needle |      Mean |     Error |    StdDev | Ratio |
|----------------- |-------------- |-------------- |------- |----------:|----------:|----------:|------:|
| **FindOffsetNormal** | **.NET Core 2.1** | **.NET Core 2.1** |      **e** | **13.852 ns** | **0.0464 ns** | **0.0412 ns** |  **1.00** |
|   FindOffsetSpan | .NET Core 2.1 | .NET Core 2.1 |      e | 12.980 ns | 0.0664 ns | 0.0589 ns |  0.94 |
|                  |               |               |        |           |           |           |       |
| FindOffsetNormal | .NET Core 3.1 | .NET Core 3.1 |      e | 12.596 ns | 0.1273 ns | 0.1190 ns |  1.00 |
|   FindOffsetSpan | .NET Core 3.1 | .NET Core 3.1 |      e |  9.153 ns | 0.0635 ns | 0.0594 ns |  0.73 |
|                  |               |               |        |           |           |           |       |
| FindOffsetNormal | .NET Core 5.0 | .NET Core 5.0 |      e | 10.036 ns | 0.0654 ns | 0.0546 ns |  1.00 |
|   FindOffsetSpan | .NET Core 5.0 | .NET Core 5.0 |      e |  9.325 ns | 0.1087 ns | 0.1017 ns |  0.93 |
|                  |               |               |        |           |           |           |       |
| **FindOffsetNormal** | **.NET Core 2.1** | **.NET Core 2.1** |      **h** | **14.586 ns** | **0.0407 ns** | **0.0381 ns** |  **1.00** |
|   FindOffsetSpan | .NET Core 2.1 | .NET Core 2.1 |      h | 14.048 ns | 0.0490 ns | 0.0409 ns |  0.96 |
|                  |               |               |        |           |           |           |       |
| FindOffsetNormal | .NET Core 3.1 | .NET Core 3.1 |      h | 12.923 ns | 0.0612 ns | 0.0511 ns |  1.00 |
|   FindOffsetSpan | .NET Core 3.1 | .NET Core 3.1 |      h |  9.727 ns | 0.0554 ns | 0.0463 ns |  0.75 |
|                  |               |               |        |           |           |           |       |
| FindOffsetNormal | .NET Core 5.0 | .NET Core 5.0 |      h | 10.331 ns | 0.0380 ns | 0.0337 ns |  1.00 |
|   FindOffsetSpan | .NET Core 5.0 | .NET Core 5.0 |      h |  9.847 ns | 0.0287 ns | 0.0254 ns |  0.95 |
|                  |               |               |        |           |           |           |       |
| **FindOffsetNormal** | **.NET Core 2.1** | **.NET Core 2.1** |      **m** | **26.328 ns** | **0.1658 ns** | **0.1384 ns** |  **1.00** |
|   FindOffsetSpan | .NET Core 2.1 | .NET Core 2.1 |      m | 26.078 ns | 0.1421 ns | 0.1187 ns |  0.99 |
|                  |               |               |        |           |           |           |       |
| FindOffsetNormal | .NET Core 3.1 | .NET Core 3.1 |      m | 21.972 ns | 0.0555 ns | 0.0492 ns |  1.00 |
|   FindOffsetSpan | .NET Core 3.1 | .NET Core 3.1 |      m | 19.288 ns | 0.0955 ns | 0.0798 ns |  0.88 |
|                  |               |               |        |           |           |           |       |
| FindOffsetNormal | .NET Core 5.0 | .NET Core 5.0 |      m | 18.652 ns | 0.0762 ns | 0.0676 ns |  1.00 |
|   FindOffsetSpan | .NET Core 5.0 | .NET Core 5.0 |      m | 18.482 ns | 0.0665 ns | 0.0622 ns |  0.99 |
