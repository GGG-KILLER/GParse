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
|   Method |           Job |       Runtime |           Characters |       Mean |    Error |   StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated | Code Size |
|--------- |-------------- |-------------- |--------------------- |-----------:|---------:|---------:|------:|--------:|-------:|------:|------:|----------:|----------:|
|  **InPlace** |    **.NET 4.6.1** |    **.NET 4.6.1** | **abcde(...)56789 [36]** | **1,178.5 ns** | **11.00 ns** |  **9.75 ns** |  **1.00** |    **0.00** | **0.1831** |     **-** |     **-** |     **144 B** |    **1148 B** |
| OutPlace |    .NET 4.6.1 |    .NET 4.6.1 | abcde(...)56789 [36] |   665.3 ns |  3.99 ns |  3.54 ns |  0.56 |    0.00 | 0.4787 |     - |     - |     377 B |    1290 B |
|          |               |               |                      |            |          |          |       |         |        |       |       |           |           |
|  InPlace |      .NET 4.8 |      .NET 4.8 | abcde(...)56789 [36] | 1,168.3 ns |  7.36 ns |  6.52 ns |  1.00 |    0.00 | 0.1831 |     - |     - |     144 B |    1148 B |
| OutPlace |      .NET 4.8 |      .NET 4.8 | abcde(...)56789 [36] |   656.5 ns |  6.34 ns |  5.62 ns |  0.56 |    0.00 | 0.4787 |     - |     - |     377 B |    1290 B |
|          |               |               |                      |            |          |          |       |         |        |       |       |           |           |
|  InPlace | .NET Core 2.1 | .NET Core 2.1 | abcde(...)56789 [36] | 1,247.5 ns |  3.94 ns |  3.49 ns |  1.00 |    0.00 | 0.0591 |     - |     - |      48 B |    1223 B |
| OutPlace | .NET Core 2.1 | .NET Core 2.1 | abcde(...)56789 [36] |   572.9 ns |  2.30 ns |  2.04 ns |  0.46 |    0.00 | 0.3557 |     - |     - |     280 B |    1454 B |
|          |               |               |                      |            |          |          |       |         |        |       |       |           |           |
|  InPlace | .NET Core 3.1 | .NET Core 3.1 | abcde(...)56789 [36] | 1,070.4 ns |  4.72 ns |  4.41 ns |  1.00 |    0.00 | 0.0114 |     - |     - |      48 B |    1188 B |
| OutPlace | .NET Core 3.1 | .NET Core 3.1 | abcde(...)56789 [36] |   554.4 ns |  3.03 ns |  2.69 ns |  0.52 |    0.00 | 0.0629 |     - |     - |     264 B |    1273 B |
|          |               |               |                      |            |          |          |       |         |        |       |       |           |           |
|  InPlace | .NET Core 5.0 | .NET Core 5.0 | abcde(...)56789 [36] | 1,085.7 ns |  9.36 ns |  8.29 ns |  1.00 |    0.00 | 0.0114 |     - |     - |      48 B |    1188 B |
| OutPlace | .NET Core 5.0 | .NET Core 5.0 | abcde(...)56789 [36] |   457.3 ns |  1.78 ns |  1.67 ns |  0.42 |    0.00 | 0.0629 |     - |     - |     264 B |    1205 B |
|          |               |               |                      |            |          |          |       |         |        |       |       |           |           |
|  **InPlace** |    **.NET 4.6.1** |    **.NET 4.6.1** | **abcde(...)56789 [32]** | **1,036.1 ns** |  **4.21 ns** |  **3.29 ns** |  **1.00** |    **0.00** | **0.2346** |     **-** |     **-** |     **185 B** |    **1148 B** |
| OutPlace |    .NET 4.6.1 |    .NET 4.6.1 | abcde(...)56789 [32] |   609.5 ns |  2.30 ns |  1.92 ns |  0.59 |    0.00 | 0.5198 |     - |     - |     409 B |    1290 B |
|          |               |               |                      |            |          |          |       |         |        |       |       |           |           |
|  InPlace |      .NET 4.8 |      .NET 4.8 | abcde(...)56789 [32] | 1,043.4 ns | 11.01 ns | 10.30 ns |  1.00 |    0.00 | 0.2346 |     - |     - |     185 B |    1148 B |
| OutPlace |      .NET 4.8 |      .NET 4.8 | abcde(...)56789 [32] |   634.0 ns | 11.08 ns | 14.01 ns |  0.61 |    0.01 | 0.5198 |     - |     - |     409 B |    1290 B |
|          |               |               |                      |            |          |          |       |         |        |       |       |           |           |
|  InPlace | .NET Core 2.1 | .NET Core 2.1 | abcde(...)56789 [32] | 1,124.1 ns |  7.74 ns |  7.24 ns |  1.00 |    0.00 | 0.1202 |     - |     - |      96 B |    1223 B |
| OutPlace | .NET Core 2.1 | .NET Core 2.1 | abcde(...)56789 [32] |   561.3 ns |  8.13 ns |  7.61 ns |  0.50 |    0.01 | 0.4063 |     - |     - |     320 B |    1454 B |
|          |               |               |                      |            |          |          |       |         |        |       |       |           |           |
|  InPlace | .NET Core 3.1 | .NET Core 3.1 | abcde(...)56789 [32] |   932.3 ns |  7.86 ns |  7.36 ns |  1.00 |    0.00 | 0.0229 |     - |     - |      96 B |    1188 B |
| OutPlace | .NET Core 3.1 | .NET Core 3.1 | abcde(...)56789 [32] |   549.2 ns | 11.00 ns | 13.91 ns |  0.59 |    0.02 | 0.0725 |     - |     - |     304 B |    1273 B |
|          |               |               |                      |            |          |          |       |         |        |       |       |           |           |
|  InPlace | .NET Core 5.0 | .NET Core 5.0 | abcde(...)56789 [32] |   905.1 ns |  5.19 ns |  4.60 ns |  1.00 |    0.00 | 0.0229 |     - |     - |      96 B |    1188 B |
| OutPlace | .NET Core 5.0 | .NET Core 5.0 | abcde(...)56789 [32] |   430.1 ns |  1.72 ns |  1.61 ns |  0.48 |    0.00 | 0.0725 |     - |     - |     304 B |    1205 B |
|          |               |               |                      |            |          |          |       |         |        |       |       |           |           |
|  **InPlace** |    **.NET 4.6.1** |    **.NET 4.6.1** |   **acegikmoqsuwy02468** |   **287.3 ns** |  **1.40 ns** |  **1.31 ns** |  **1.00** |    **0.00** | **0.0815** |     **-** |     **-** |      **64 B** |    **1148 B** |
| OutPlace |    .NET 4.6.1 |    .NET 4.6.1 |   acegikmoqsuwy02468 |   413.5 ns |  2.63 ns |  2.33 ns |  1.44 |    0.01 | 0.2651 |     - |     - |     209 B |    1290 B |
|          |               |               |                      |            |          |          |       |         |        |       |       |           |           |
|  InPlace |      .NET 4.8 |      .NET 4.8 |   acegikmoqsuwy02468 |   287.6 ns |  1.84 ns |  1.63 ns |  1.00 |    0.00 | 0.0815 |     - |     - |      64 B |    1148 B |
| OutPlace |      .NET 4.8 |      .NET 4.8 |   acegikmoqsuwy02468 |   418.2 ns |  5.00 ns |  4.44 ns |  1.45 |    0.02 | 0.2651 |     - |     - |     209 B |    1290 B |
|          |               |               |                      |            |          |          |       |         |        |       |       |           |           |
|  InPlace | .NET Core 2.1 | .NET Core 2.1 |   acegikmoqsuwy02468 |   239.1 ns |  1.16 ns |  0.97 ns |  1.00 |    0.00 |      - |     - |     - |         - |    1223 B |
| OutPlace | .NET Core 2.1 | .NET Core 2.1 |   acegikmoqsuwy02468 |   335.1 ns |  2.71 ns |  2.40 ns |  1.40 |    0.01 | 0.1826 |     - |     - |     144 B |    1454 B |
|          |               |               |                      |            |          |          |       |         |        |       |       |           |           |
|  InPlace | .NET Core 3.1 | .NET Core 3.1 |   acegikmoqsuwy02468 |   223.7 ns |  1.44 ns |  1.20 ns |  1.00 |    0.00 |      - |     - |     - |         - |    1040 B |
| OutPlace | .NET Core 3.1 | .NET Core 3.1 |   acegikmoqsuwy02468 |   317.8 ns |  6.31 ns |  6.47 ns |  1.42 |    0.03 | 0.0305 |     - |     - |     128 B |    1273 B |
|          |               |               |                      |            |          |          |       |         |        |       |       |           |           |
|  InPlace | .NET Core 5.0 | .NET Core 5.0 |   acegikmoqsuwy02468 |   179.4 ns |  1.99 ns |  1.86 ns |  1.00 |    0.00 |      - |     - |     - |         - |     974 B |
| OutPlace | .NET Core 5.0 | .NET Core 5.0 |   acegikmoqsuwy02468 |   279.4 ns |  3.46 ns |  3.24 ns |  1.56 |    0.02 | 0.0305 |     - |     - |     128 B |    1205 B |
