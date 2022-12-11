using BenchmarkDotNet.Attributes;
using System;
using System.Runtime.CompilerServices;

namespace FastActivator.Benchmarks.Tests
{
    [MemoryDiagnoser, HtmlExporter, RankColumn]
    public class ActivationSimple
    {
        [Benchmark]
        public void ActivatorCreateInstance() => Activator.CreateInstance(typeof(SimpleTestClass));

        [Benchmark]
        public void ActivatorCreateInstanceGeneric() => Activator.CreateInstance<SimpleTestClass>();

        [Benchmark]
        public void FastActivatorCreateInstance() => Fast.Activator.FastActivator.CreateInstance(typeof(SimpleTestClass));

        [Benchmark(Baseline = true)]
        public void NewCreateInstance() => Create<SimpleTestClass>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TObject Create<TObject>() where TObject : new() => new();

        private class SimpleTestClass
        {
            public string A { get; set; }
            public bool B { get; set; }
        }
    }
}