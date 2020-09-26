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
|           Method |           Job |       Runtime | Needle |       Mean |     Error |    StdDev | Ratio |
|----------------- |-------------- |-------------- |------- |-----------:|----------:|----------:|------:|
|   **FindOffsetSpan** |    **.NET 4.6.1** |    **.NET 4.6.1** |      **e** |  **23.213 ns** | **0.0492 ns** | **0.0461 ns** |  **1.68** |
| FindOffsetNormal |    .NET 4.6.1 |    .NET 4.6.1 |      e |  13.805 ns | 0.0655 ns | 0.0580 ns |  1.00 |
|                  |               |               |        |            |           |           |       |
|   FindOffsetSpan |      .NET 4.8 |      .NET 4.8 |      e |  23.125 ns | 0.0515 ns | 0.0456 ns |  1.66 |
| FindOffsetNormal |      .NET 4.8 |      .NET 4.8 |      e |  13.944 ns | 0.1018 ns | 0.0952 ns |  1.00 |
|                  |               |               |        |            |           |           |       |
|   FindOffsetSpan | .NET Core 2.1 | .NET Core 2.1 |      e |  12.274 ns | 0.0485 ns | 0.0430 ns |  0.78 |
| FindOffsetNormal | .NET Core 2.1 | .NET Core 2.1 |      e |  15.648 ns | 0.0342 ns | 0.0303 ns |  1.00 |
|                  |               |               |        |            |           |           |       |
|   FindOffsetSpan | .NET Core 3.1 | .NET Core 3.1 |      e |  10.603 ns | 0.0492 ns | 0.0460 ns |  0.89 |
| FindOffsetNormal | .NET Core 3.1 | .NET Core 3.1 |      e |  11.854 ns | 0.0800 ns | 0.0749 ns |  1.00 |
|                  |               |               |        |            |           |           |       |
|   FindOffsetSpan | .NET Core 5.0 | .NET Core 5.0 |      e |  10.528 ns | 0.0335 ns | 0.0280 ns |  1.10 |
| FindOffsetNormal | .NET Core 5.0 | .NET Core 5.0 |      e |   9.579 ns | 0.0413 ns | 0.0386 ns |  1.00 |
|                  |               |               |        |            |           |           |       |
|   **FindOffsetSpan** |    **.NET 4.6.1** |    **.NET 4.6.1** |      **h** |  **24.110 ns** | **0.0434 ns** | **0.0384 ns** |  **1.16** |
| FindOffsetNormal |    .NET 4.6.1 |    .NET 4.6.1 |      h |  20.701 ns | 0.1119 ns | 0.0992 ns |  1.00 |
|                  |               |               |        |            |           |           |       |
|   FindOffsetSpan |      .NET 4.8 |      .NET 4.8 |      h |  23.948 ns | 0.0272 ns | 0.0254 ns |  1.16 |
| FindOffsetNormal |      .NET 4.8 |      .NET 4.8 |      h |  20.706 ns | 0.0275 ns | 0.0244 ns |  1.00 |
|                  |               |               |        |            |           |           |       |
|   FindOffsetSpan | .NET Core 2.1 | .NET Core 2.1 |      h |  13.370 ns | 0.0714 ns | 0.0668 ns |  0.78 |
| FindOffsetNormal | .NET Core 2.1 | .NET Core 2.1 |      h |  17.103 ns | 0.1827 ns | 0.1620 ns |  1.00 |
|                  |               |               |        |            |           |           |       |
|   FindOffsetSpan | .NET Core 3.1 | .NET Core 3.1 |      h |  11.633 ns | 0.1699 ns | 0.1506 ns |  0.93 |
| FindOffsetNormal | .NET Core 3.1 | .NET Core 3.1 |      h |  12.574 ns | 0.1914 ns | 0.1696 ns |  1.00 |
|                  |               |               |        |            |           |           |       |
|   FindOffsetSpan | .NET Core 5.0 | .NET Core 5.0 |      h |  12.209 ns | 0.0833 ns | 0.0779 ns |  1.02 |
| FindOffsetNormal | .NET Core 5.0 | .NET Core 5.0 |      h |  11.985 ns | 0.1586 ns | 0.1484 ns |  1.00 |
|                  |               |               |        |            |           |           |       |
|   **FindOffsetSpan** |    **.NET 4.6.1** |    **.NET 4.6.1** |      **m** |  **36.586 ns** | **0.3055 ns** | **0.2858 ns** |  **0.30** |
| FindOffsetNormal |    .NET 4.6.1 |    .NET 4.6.1 |      m | 123.316 ns | 0.4520 ns | 0.3774 ns |  1.00 |
|                  |               |               |        |            |           |           |       |
|   FindOffsetSpan |      .NET 4.8 |      .NET 4.8 |      m |  36.231 ns | 0.4328 ns | 0.3379 ns |  0.29 |
| FindOffsetNormal |      .NET 4.8 |      .NET 4.8 |      m | 125.297 ns | 2.3135 ns | 2.1640 ns |  1.00 |
|                  |               |               |        |            |           |           |       |
|   FindOffsetSpan | .NET Core 2.1 | .NET Core 2.1 |      m |  27.029 ns | 0.2398 ns | 0.2002 ns |  0.97 |
| FindOffsetNormal | .NET Core 2.1 | .NET Core 2.1 |      m |  27.889 ns | 0.3110 ns | 0.2909 ns |  1.00 |
|                  |               |               |        |            |           |           |       |
|   FindOffsetSpan | .NET Core 3.1 | .NET Core 3.1 |      m |  20.166 ns | 0.1605 ns | 0.1502 ns |  1.04 |
| FindOffsetNormal | .NET Core 3.1 | .NET Core 3.1 |      m |  19.367 ns | 0.0660 ns | 0.0551 ns |  1.00 |
|                  |               |               |        |            |           |           |       |
|   FindOffsetSpan | .NET Core 5.0 | .NET Core 5.0 |      m |  20.782 ns | 0.2012 ns | 0.1882 ns |  0.96 |
| FindOffsetNormal | .NET Core 5.0 | .NET Core 5.0 |      m |  21.602 ns | 0.1078 ns | 0.0900 ns |  1.00 |
