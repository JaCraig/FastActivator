# FastActivator
It's Activator.CreateInstance... But faster.

So instead of Activator.CreateInstance, you'd call FastActivator.CreateInstance.

The simple case speed differences:

|                         Method |     Mean |    Error |   StdDev | Ratio | RatioSD | Rank |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------- |---------:|---------:|---------:|------:|--------:|-----:|-------:|------:|------:|----------:|
|        ActivatorCreateInstance | 31.38 ns | 0.325 ns | 0.271 ns |  1.00 |    0.00 |    3 | 0.0051 |     - |     - |      32 B |
| ActivatorCreateInstanceGeneric | 30.32 ns | 0.355 ns | 0.315 ns |  0.97 |    0.02 |    2 | 0.0051 |     - |     - |      32 B |
|    FastActivatorCreateInstance | 18.97 ns | 0.315 ns | 0.279 ns |  0.60 |    0.01 |    1 | 0.0051 |     - |     - |      32 B |

And the speed difference where you're sending in parameters:

|                      Method |      Mean |    Error |   StdDev | Ratio | Rank |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|---------------------------- |----------:|---------:|---------:|------:|-----:|-------:|------:|------:|----------:|
|     ActivatorCreateInstance | 509.96 ns | 7.513 ns | 6.660 ns |  1.00 |    2 | 0.0744 |     - |     - |     472 B |
| FastActivatorCreateInstance |  47.37 ns | 0.935 ns | 0.829 ns |  0.09 |    1 | 0.0153 |     - |     - |      96 B |

In most instances it's about 2x to 10x faster.

[![Build status](https://ci.appveyor.com/api/projects/status/d966a7vvo3v2v6yh?svg=true)](https://ci.appveyor.com/project/JaCraig/fastactivator)