using BenchmarkDotNet.Attributes;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace FastActivator.Benchmarks.Tests
{
    [MemoryDiagnoser, HtmlExporter, RankColumn]
    public class ActivationWithParams
    {
        private Func<string, bool, ParamsTestClass> CreateDelegate;

        [Benchmark]
        public void ActivatorCreateInstance() => Activator.CreateInstance(typeof(ParamsTestClass), "A", true);

        [Benchmark]
        public void DelegateCreateInstance() => CreateDelegate("A", true);

        [Benchmark]
        public void FastActivatorCreateInstance() => Fast.Activator.FastActivator.CreateInstance(typeof(ParamsTestClass), "A", true);

        [Benchmark(Baseline = true)]
        public void NewCreateInstance() => Create("A", true);

        [GlobalSetup]
        public void Setup()
        {
            System.Reflection.ConstructorInfo Constructor = typeof(ParamsTestClass).GetConstructor(new Type[] { typeof(string), typeof(bool) });
            System.Reflection.ParameterInfo[] parameters = Constructor.GetParameters();
            ParameterExpression[] ParameterExpressions = parameters.Select(x => Expression.Parameter(x.ParameterType, x.Name)).ToArray();
            NewExpression NewExpression = Expression.New(Constructor, ParameterExpressions);
            CreateDelegate = Expression.Lambda(
                typeof(Func<string, bool, ParamsTestClass>),
                NewExpression,
                ParameterExpressions).Compile() as Func<string, bool, ParamsTestClass>;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ParamsTestClass Create(string a, bool b) => new(a, b);

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