using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fast.Activator.Tests
{
    public class BasicTests
    {
        [Fact]
        public void Enums()
        {
            Assert.Equal(Status.Start, FastActivator.CreateInstance<Status>());
            Assert.Equal(Status.Start, FastActivator.CreateInstance(typeof(Status)));
        }

        [Fact]
        public void ConcurrentCachePopulation()
        {
            var Exceptions = new ConcurrentQueue<string>();

            Parallel.For(0, 500, Index =>
            {
                try
                {
                    if (Index % 2 == 0)
                    {
                        var Result = FastActivator.CreateInstance<ConcurrentStringClass>(Index.ToString());
                        if (Result is null || Result.Value != Index.ToString())
                            Exceptions.Enqueue($"ConcurrentStringClass failed at {Index}.");
                    }
                    else
                    {
                        var Result = FastActivator.CreateInstance<ConcurrentIntClass>(Index);
                        if (Result is null || Result.Value != Index)
                            Exceptions.Enqueue($"ConcurrentIntClass failed at {Index}.");
                    }
                }
                catch (System.Exception Ex)
                {
                    Exceptions.Enqueue(Ex.ToString());
                }
            });

            Assert.Empty(Exceptions);
        }

        [Fact]
        public async Task ConcurrentColdStartAcrossManyTypes()
        {
            var Types = BuildTaggedTypes(ConcurrentTagTypes.Length);
            var Exceptions = new ConcurrentQueue<string>();
            var StartGate = new ManualResetEventSlim(false);
            var Tasks = new Task[Types.Length];

            for (var X = 0; X < Types.Length; ++X)
            {
                var Index = X;
                Tasks[Index] = Task.Run(() =>
                {
                    StartGate.Wait();

                    var Expected = Index;
                    var Result = FastActivator.CreateInstance(Types[Index], Expected);
                    if (Result is not IConcurrentValue ValueHolder || ValueHolder.Value != Expected || Result.GetType() != Types[Index])
                    {
                        Exceptions.Enqueue($"Cold start failed for {Types[Index].Name} at {Index}.");
                    }
                });
            }

            StartGate.Set();
            await Task.WhenAll(Tasks);

            Assert.Empty(Exceptions);
        }

        [Fact]
        public async Task ConcurrentHighContentionOnCachedTypesRemainsCorrect()
        {
            var Types = BuildTaggedTypes(ConcurrentTagTypes.Length);

            // Prime all entries first to stress hit paths under contention.
            for (var X = 0; X < Types.Length; ++X)
                _ = FastActivator.CreateInstance(Types[X], X);

            var Exceptions = new ConcurrentQueue<string>();
            var StartGate = new ManualResetEventSlim(false);
            var WorkerCount = Math.Max(Environment.ProcessorCount * 2, 8);
            const int IterationsPerWorker = 2000;
            var Tasks = new Task[WorkerCount];

            for (var Worker = 0; Worker < WorkerCount; ++Worker)
            {
                var WorkerIndex = Worker;
                Tasks[Worker] = Task.Run(() =>
                {
                    StartGate.Wait();

                    for (var Iteration = 0; Iteration < IterationsPerWorker; ++Iteration)
                    {
                        var TypeIndex = (WorkerIndex + Iteration) % Types.Length;
                        var Expected = WorkerIndex * IterationsPerWorker + Iteration;
                        var Result = FastActivator.CreateInstance(Types[TypeIndex], Expected);

                        if (Result is not IConcurrentValue ValueHolder || ValueHolder.Value != Expected || Result.GetType() != Types[TypeIndex])
                        {
                            Exceptions.Enqueue($"Contention path failed for {Types[TypeIndex].Name} at worker {WorkerIndex}, iteration {Iteration}.");
                            return;
                        }
                    }
                });
            }

            StartGate.Set();
            await Task.WhenAll(Tasks);

            Assert.Empty(Exceptions);
        }

        [Fact]
        public void CreateInstanceTypeOverloadsThrowOnNullType()
        {
            Assert.Throws<ArgumentNullException>(() => FastActivator.CreateInstance((Type)null));
            Assert.Throws<ArgumentNullException>(() => FastActivator.CreateInstance(null, Array.Empty<object>()));
        }

        [Fact]
        public void TypeWithoutPublicParameterlessConstructorReturnsNull()
            => Assert.Null(FastActivator.CreateInstance(typeof(NoDefaultConstructorClass)));

        [Fact]
        public void Nullable()
        {
            Assert.Null(FastActivator.CreateInstance<int?>());
            Assert.Null(FastActivator.CreateInstance(typeof(int?)));
            Assert.Null(FastActivator.CreateInstance<double?>());
            Assert.Null(FastActivator.CreateInstance(typeof(double?)));
            Assert.Null(FastActivator.CreateInstance<float?>());
            Assert.Null(FastActivator.CreateInstance(typeof(float?)));
        }

        [Fact]
        public void NullReferenceTypeParameter()
        {
            var Result = FastActivator.CreateInstance<ParamsTestClass>(null, true);
            Assert.NotNull(Result);
            Assert.Null(Result.A);
            Assert.True(Result.B);
        }

        [Fact]
        public void Simple()
        {
            var Result = FastActivator.CreateInstance<SimpleTestClass>();
            Assert.Null(Result.A);
            Assert.False(Result.B);
        }

        [Fact]
        public void SimpleValue()
        {
            Assert.Equal(0, FastActivator.CreateInstance<int>());
            Assert.Equal(0, FastActivator.CreateInstance(typeof(int)));
            Assert.Null(FastActivator.CreateInstance<string>());
            Assert.Null(FastActivator.CreateInstance(typeof(string)));
        }

        [Fact]
        public void StructCreation()
        {
            var Result = FastActivator.CreateInstance<StructTest>(1, 2);
            Assert.Equal(1, Result.A);
            Assert.Equal(2, Result.B);
        }

        [Fact]
        public void WithParams()
        {
            var Result = FastActivator.CreateInstance<ParamsTestClass>("A", true);
            Assert.Equal("A", Result.A);
            Assert.True(Result.B);
        }

        [Fact]
        public void WithParamsMultipleConstructors()
        {
            var Result = FastActivator.CreateInstance<ParamsMultipleTestClass>("A", true);
            Assert.Equal("A", Result.A);
            Assert.True(Result.B);
        }

        private struct StructTest
        {
            public StructTest(int a, int b)
            {
                A = a;
                B = b;
            }

            public int A;
            public int B;
        }

        private class ParamsMultipleTestClass
        {
            public ParamsMultipleTestClass()
            {
                A = "C";
                B = false;
            }

            public ParamsMultipleTestClass(string a, bool b)
            {
                A = a;
                B = b;
            }

            public ParamsMultipleTestClass(string a, int b)
            {
                A = a;
                B = false;
            }

            public string A { get; set; }
            public bool B { get; set; }
        }

        private class ConcurrentIntClass
        {
            public ConcurrentIntClass(int value)
            {
                Value = value;
            }

            public int Value { get; }
        }

        private class ConcurrentStringClass
        {
            public ConcurrentStringClass(string value)
            {
                Value = value;
            }

            public string Value { get; }
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

        private class SimpleTestClass
        {
            public string A { get; set; }
            public bool B { get; set; }
        }

        private class NoDefaultConstructorClass
        {
            public NoDefaultConstructorClass(int value)
            {
                Value = value;
            }

            public int Value { get; }
        }

        private interface IConcurrentValue
        {
            int Value { get; }
        }

        private class ConcurrentTaggedClass<TTag> : IConcurrentValue
        {
            public ConcurrentTaggedClass(int value)
            {
                Value = value;
            }

            public int Value { get; }
        }

        private static Type[] BuildTaggedTypes(int count)
        {
            var Result = new Type[count];
            for (var X = 0; X < count; ++X)
                Result[X] = typeof(ConcurrentTaggedClass<>).MakeGenericType(ConcurrentTagTypes[X]);

            return Result;
        }

        private static readonly Type[] ConcurrentTagTypes =
        [
            typeof(int),
            typeof(string),
            typeof(double),
            typeof(float),
            typeof(long),
            typeof(short),
            typeof(byte),
            typeof(char),
            typeof(bool),
            typeof(decimal),
            typeof(DateTime),
            typeof(Guid),
            typeof(object),
            typeof(TimeSpan),
            typeof(Uri),
            typeof(Version),
            typeof(Tuple<int>),
            typeof(Tuple<string>),
            typeof(List<int>),
            typeof(Dictionary<string, int>),
            typeof(ConcurrentQueue<int>),
            typeof(Task),
            typeof(Task<int>),
            typeof(ArgumentException),
            typeof(InvalidOperationException),
            typeof(IComparable),
            typeof(IDisposable),
            typeof(Action),
            typeof(Func<int>),
            typeof(Random),
            typeof(Thread),
            typeof(ManualResetEventSlim)
        ];

        private enum Status
        {
            Start = 0,
            Stop
        }
    }
}