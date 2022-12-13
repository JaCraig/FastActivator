using BenchmarkDotNet.Attributes;

namespace FastActivator.Benchmarks.Tests
{
    public class IsVsReferenceEquals
    {
        private readonly TestClass TestObject = new();

        [Benchmark(Baseline = true)]
        public void Is() => _ = TestObject is null;

        [Benchmark]
        public void ReferenceEquals() => _ = TestObject is null;

        private class TestClass
        { }
    }
}