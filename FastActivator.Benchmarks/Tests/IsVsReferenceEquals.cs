using BenchmarkDotNet.Attributes;
using System;

namespace FastActivator.Benchmarks.Tests
{
    public class IsVsReferenceEquals
    {
        private readonly TestClass TestObject = new();

        [Benchmark(Baseline = true)]
        public void Is() => _ = TestObject is null;

        [Benchmark]
        public void ReferenceEquals() => _ = ReferenceEquals(TestObject, null);

        private class TestClass
        { }
    }
}