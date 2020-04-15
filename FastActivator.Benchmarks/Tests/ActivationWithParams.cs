using BenchmarkDotNet.Attributes;
using System;

namespace FastActivator.Benchmarks.Tests
{
    [MemoryDiagnoser, HtmlExporter, RankColumn]
    public class ActivationWithParams
    {
        [Benchmark(Baseline = true)]
        public void ActivatorCreateInstance()
        {
            Activator.CreateInstance(typeof(ParamsTestClass), "A", true);
        }

        [Benchmark]
        public void FastActivatorCreateInstance()
        {
            Fast.Activator.FastActivator.CreateInstance(typeof(ParamsTestClass), "A", true);
        }

        private class ParamsTestClass
        {
            public ParamsTestClass(string a, bool b)
            {
                A = a;
                B = b;
            }

            public string A { get; set; }
            public bool B { get; set; }
        }
    }
}