using BenchmarkDotNet.Attributes;
using System;

namespace FastActivator.Benchmarks.Tests
{
    [MemoryDiagnoser, HtmlExporter, RankColumn]
    public class ActivationSimple
    {
        [Benchmark(Baseline = true)]
        public void ActivatorCreateInstance()
        {
            Activator.CreateInstance(typeof(SimpleTestClass));
        }

        [Benchmark]
        public void ActivatorCreateInstanceGeneric()
        {
            Activator.CreateInstance<SimpleTestClass>();
        }

        [Benchmark]
        public void FastActivatorCreateInstance()
        {
            Fast.Activator.FastActivator.CreateInstance(typeof(SimpleTestClass));
        }

        private class SimpleTestClass
        {
            public string A { get; set; }
            public bool B { get; set; }
        }
    }
}