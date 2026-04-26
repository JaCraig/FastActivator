using BenchmarkDotNet.Attributes;

namespace FastActivator.Benchmarks.Tests
{
    public interface ITestInterface
    { }

    public class TestClass : ITestInterface
    { }

    public class TypeComparisons
    {
        private readonly TestClass TestObject = new();

        [Benchmark(Baseline = true)]
        public void IsAssignableFrom() => typeof(ITestInterface).IsAssignableFrom(typeof(TestClass));

        [Benchmark]
        public void IsInstanceOfType() => typeof(ITestInterface).IsInstanceOfType(TestObject);
    }
}