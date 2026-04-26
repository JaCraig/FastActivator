using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;

namespace FastActivator.Benchmarks.Tests
{
    [MemoryDiagnoser, HtmlExporter, RankColumn]
    public class DictionaryKeyComparisons
    {
        private readonly Type[] _Types =
        [
            typeof(string),
            typeof(int),
            typeof(Guid),
            typeof(DateTime),
            typeof(TimeSpan),
            typeof(Uri),
            typeof(List<string>),
            typeof(Dictionary<string, int>)
        ];

        private Dictionary<int, object> HashCodeDictionary;
        private int _Index;
        private Dictionary<Type, object> TypeDictionary;

        [GlobalSetup]
        public void Setup()
        {
            HashCodeDictionary = new Dictionary<int, object>(_Types.Length);
            TypeDictionary = new Dictionary<Type, object>(_Types.Length);
            foreach (var Type in _Types)
            {
                var Value = new object();
                HashCodeDictionary[Type.GetHashCode()] = Value;
                TypeDictionary[Type] = Value;
            }
        }

        [Benchmark(Baseline = true)]
        public object HashCodeLookup()
        {
            var Type = GetNextType();
            return HashCodeDictionary.TryGetValue(Type.GetHashCode(), out var Value) ? Value : null;
        }

        [Benchmark]
        public object TypeLookup()
        {
            var Type = GetNextType();
            return TypeDictionary.TryGetValue(Type, out var Value) ? Value : null;
        }

        private Type GetNextType()
        {
            var Value = _Types[_Index];
            _Index = (_Index + 1) % _Types.Length;
            return Value;
        }
    }
}