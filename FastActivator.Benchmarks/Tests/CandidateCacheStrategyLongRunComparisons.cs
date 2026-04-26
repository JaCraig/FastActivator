using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FastActivator.Benchmarks.Tests
{
    [MemoryDiagnoser, HtmlExporter, RankColumn]
    [SimpleJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 8, iterationCount: 15)]
    public class CandidateCacheStrategyLongRunComparisons
    {
        private const int ContendedOperationsPerThread = 200_000;
        private const int HotPathOperations = 256;
        private const int MissPathOperations = 32;

        private readonly Type[] _ColdTypes =
        [
            typeof(decimal),
            typeof(double),
            typeof(float),
            typeof(bool),
            typeof(char),
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(uint),
            typeof(ulong),
            typeof(IntPtr),
            typeof(UIntPtr),
            typeof(DateOnly),
            typeof(TimeOnly),
            typeof(DateTimeOffset),
            typeof(Version),
            typeof(StringComparer),
            typeof(StringBuilder),
            typeof(UriBuilder),
            typeof(MemoryStream),
            typeof(FileInfo),
            typeof(DirectoryInfo),
            typeof(CultureInfo),
            typeof(Tuple<int>),
            typeof(Tuple<string, int>),
            typeof(List<int>),
            typeof(HashSet<int>),
            typeof(Queue<int>),
            typeof(Stack<int>),
            typeof(LinkedList<int>),
            typeof(ConcurrentQueue<int>)
        ];

        private readonly Type[] _HotTypes =
        [
            typeof(string),
            typeof(int),
            typeof(DateTime),
            typeof(TimeSpan),
            typeof(Uri),
            typeof(List<string>),
            typeof(Dictionary<string, int>),
            typeof(ConcurrentDictionary<string, int>)
        ];

        private int _Index;
        private ConcurrentIntKeyCollisionSafeCache _ConcurrentIntKeyCache;
        private LegacyIntKeyCache _LegacyCache;
        private PooledConcurrentTypeKeyCache _PooledConcurrentCache;
        private StripedTypeKeyCache _StripedCache;
        private StripedIntKeyCache _StripedIntKeyCache;
        private TypeKeyConcurrentCache _TypeCache;

        [Params(1, 4, 8)]
        public int WorkerCount { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            _ConcurrentIntKeyCache = new ConcurrentIntKeyCollisionSafeCache();
            _LegacyCache = new LegacyIntKeyCache();
            _PooledConcurrentCache = new PooledConcurrentTypeKeyCache();
            _StripedCache = new StripedTypeKeyCache();
            _StripedIntKeyCache = new StripedIntKeyCache();
            _TypeCache = new TypeKeyConcurrentCache();

            foreach (var Type in _HotTypes)
            {
                _ = _ConcurrentIntKeyCache.GetOrAdd(Type);
                _ = _LegacyCache.GetOrAdd(Type);
                _ = _PooledConcurrentCache.GetOrAdd(Type);
                _ = _StripedCache.GetOrAdd(Type);
                _ = _StripedIntKeyCache.GetOrAdd(Type);
                _ = _TypeCache.GetOrAdd(Type);
            }
        }

        [Benchmark(Baseline = true, OperationsPerInvoke = HotPathOperations)]
        public object LegacyIntKeyGetOrAddHit()
        {
            object Value = null;
            for (var X = 0; X < HotPathOperations; ++X)
                Value = _LegacyCache.GetOrAdd(GetNextHotType());
            return Value;
        }

        [Benchmark(OperationsPerInvoke = HotPathOperations)]
        public object TypeKeyGetOrAddHit()
        {
            object Value = null;
            for (var X = 0; X < HotPathOperations; ++X)
                Value = _TypeCache.GetOrAdd(GetNextHotType());
            return Value;
        }

        [Benchmark(OperationsPerInvoke = HotPathOperations)]
        public object ConcurrentIntKeyCollisionSafeGetOrAddHit()
        {
            object Value = null;
            for (var X = 0; X < HotPathOperations; ++X)
                Value = _ConcurrentIntKeyCache.GetOrAdd(GetNextHotType());
            return Value;
        }

        [Benchmark(OperationsPerInvoke = HotPathOperations)]
        public object PooledConcurrentTypeKeyGetOrAddHit()
        {
            object Value = null;
            for (var X = 0; X < HotPathOperations; ++X)
                Value = _PooledConcurrentCache.GetOrAdd(GetNextHotType());
            return Value;
        }

        [Benchmark(OperationsPerInvoke = HotPathOperations)]
        public object StripedTypeKeyGetOrAddHit()
        {
            object Value = null;
            for (var X = 0; X < HotPathOperations; ++X)
                Value = _StripedCache.GetOrAdd(GetNextHotType());
            return Value;
        }

        [Benchmark(OperationsPerInvoke = HotPathOperations)]
        public object StripedIntKeyGetOrAddHit()
        {
            object Value = null;
            for (var X = 0; X < HotPathOperations; ++X)
                Value = _StripedIntKeyCache.GetOrAdd(GetNextHotType());
            return Value;
        }

        [Benchmark(OperationsPerInvoke = MissPathOperations)]
        public object LegacyIntKeyGetOrAddMiss()
        {
            _LegacyCache.Clear();
            object Value = null;
            for (var X = 0; X < MissPathOperations; ++X)
                Value = _LegacyCache.GetOrAdd(_ColdTypes[X]);
            return Value;
        }

        [Benchmark(OperationsPerInvoke = MissPathOperations)]
        public object TypeKeyGetOrAddMiss()
        {
            _TypeCache.Clear();
            object Value = null;
            for (var X = 0; X < MissPathOperations; ++X)
                Value = _TypeCache.GetOrAdd(_ColdTypes[X]);
            return Value;
        }

        [Benchmark(OperationsPerInvoke = MissPathOperations)]
        public object ConcurrentIntKeyCollisionSafeGetOrAddMiss()
        {
            _ConcurrentIntKeyCache.Clear();
            object Value = null;
            for (var X = 0; X < MissPathOperations; ++X)
                Value = _ConcurrentIntKeyCache.GetOrAdd(_ColdTypes[X]);
            return Value;
        }

        [Benchmark(OperationsPerInvoke = MissPathOperations)]
        public object PooledConcurrentTypeKeyGetOrAddMiss()
        {
            _PooledConcurrentCache.Clear();
            object Value = null;
            for (var X = 0; X < MissPathOperations; ++X)
                Value = _PooledConcurrentCache.GetOrAdd(_ColdTypes[X]);
            return Value;
        }

        [Benchmark(OperationsPerInvoke = MissPathOperations)]
        public object StripedTypeKeyGetOrAddMiss()
        {
            _StripedCache.Clear();
            object Value = null;
            for (var X = 0; X < MissPathOperations; ++X)
                Value = _StripedCache.GetOrAdd(_ColdTypes[X]);
            return Value;
        }

        [Benchmark(OperationsPerInvoke = MissPathOperations)]
        public object StripedIntKeyGetOrAddMiss()
        {
            _StripedIntKeyCache.Clear();
            object Value = null;
            for (var X = 0; X < MissPathOperations; ++X)
                Value = _StripedIntKeyCache.GetOrAdd(_ColdTypes[X]);
            return Value;
        }

        [Benchmark]
        public long LegacyIntKeyContendedHit() => ExecuteContendedHit(_LegacyCache);

        [Benchmark]
        public long TypeKeyContendedHit() => ExecuteContendedHit(_TypeCache);

        [Benchmark]
        public long ConcurrentIntKeyCollisionSafeContendedHit() => ExecuteContendedHit(_ConcurrentIntKeyCache);

        [Benchmark]
        public long PooledConcurrentTypeKeyContendedHit() => ExecuteContendedHit(_PooledConcurrentCache);

        [Benchmark]
        public long StripedTypeKeyContendedHit() => ExecuteContendedHit(_StripedCache);

        [Benchmark]
        public long StripedIntKeyContendedHit() => ExecuteContendedHit(_StripedIntKeyCache);

        [Benchmark]
        public long LegacyIntKeyContendedColdStart()
        {
            _LegacyCache.Clear();
            return ExecuteContendedCold(_LegacyCache);
        }

        [Benchmark]
        public long TypeKeyContendedColdStart()
        {
            _TypeCache.Clear();
            return ExecuteContendedCold(_TypeCache);
        }

        [Benchmark]
        public long ConcurrentIntKeyCollisionSafeContendedColdStart()
        {
            _ConcurrentIntKeyCache.Clear();
            return ExecuteContendedCold(_ConcurrentIntKeyCache);
        }

        [Benchmark]
        public long PooledConcurrentTypeKeyContendedColdStart()
        {
            _PooledConcurrentCache.Clear();
            return ExecuteContendedCold(_PooledConcurrentCache);
        }

        [Benchmark]
        public long StripedTypeKeyContendedColdStart()
        {
            _StripedCache.Clear();
            return ExecuteContendedCold(_StripedCache);
        }

        [Benchmark]
        public long StripedIntKeyContendedColdStart()
        {
            _StripedIntKeyCache.Clear();
            return ExecuteContendedCold(_StripedIntKeyCache);
        }

        private Type GetNextHotType()
        {
            var Value = _HotTypes[_Index];
            _Index = (_Index + 1) % _HotTypes.Length;
            return Value;
        }

        private long ExecuteContendedHit(LegacyIntKeyCache cache)
        {
            long Checksum = 0;
            var Options = new ParallelOptions { MaxDegreeOfParallelism = WorkerCount };

            Parallel.For(0, WorkerCount, Options, WorkerIndex =>
            {
                long Local = 0;
                for (var X = 0; X < ContendedOperationsPerThread; ++X)
                {
                    var Type = _HotTypes[(WorkerIndex + X) % _HotTypes.Length];
                    Local += cache.GetOrAdd(Type).GetHashCode();
                }

                Interlocked.Add(ref Checksum, Local);
            });

            return Checksum;
        }

        private long ExecuteContendedHit(TypeKeyConcurrentCache cache)
        {
            long Checksum = 0;
            var Options = new ParallelOptions { MaxDegreeOfParallelism = WorkerCount };

            Parallel.For(0, WorkerCount, Options, WorkerIndex =>
            {
                long Local = 0;
                for (var X = 0; X < ContendedOperationsPerThread; ++X)
                {
                    var Type = _HotTypes[(WorkerIndex + X) % _HotTypes.Length];
                    Local += cache.GetOrAdd(Type).GetHashCode();
                }

                Interlocked.Add(ref Checksum, Local);
            });

            return Checksum;
        }

        private long ExecuteContendedHit(PooledConcurrentTypeKeyCache cache)
        {
            long Checksum = 0;
            var Options = new ParallelOptions { MaxDegreeOfParallelism = WorkerCount };

            Parallel.For(0, WorkerCount, Options, WorkerIndex =>
            {
                long Local = 0;
                for (var X = 0; X < ContendedOperationsPerThread; ++X)
                {
                    var Type = _HotTypes[(WorkerIndex + X) % _HotTypes.Length];
                    Local += cache.GetOrAdd(Type).GetHashCode();
                }

                Interlocked.Add(ref Checksum, Local);
            });

            return Checksum;
        }

        private long ExecuteContendedHit(ConcurrentIntKeyCollisionSafeCache cache)
        {
            long Checksum = 0;
            var Options = new ParallelOptions { MaxDegreeOfParallelism = WorkerCount };

            Parallel.For(0, WorkerCount, Options, WorkerIndex =>
            {
                long Local = 0;
                for (var X = 0; X < ContendedOperationsPerThread; ++X)
                {
                    var Type = _HotTypes[(WorkerIndex + X) % _HotTypes.Length];
                    Local += cache.GetOrAdd(Type).GetHashCode();
                }

                Interlocked.Add(ref Checksum, Local);
            });

            return Checksum;
        }

        private long ExecuteContendedHit(StripedTypeKeyCache cache)
        {
            long Checksum = 0;
            var Options = new ParallelOptions { MaxDegreeOfParallelism = WorkerCount };

            Parallel.For(0, WorkerCount, Options, WorkerIndex =>
            {
                long Local = 0;
                for (var X = 0; X < ContendedOperationsPerThread; ++X)
                {
                    var Type = _HotTypes[(WorkerIndex + X) % _HotTypes.Length];
                    Local += cache.GetOrAdd(Type).GetHashCode();
                }

                Interlocked.Add(ref Checksum, Local);
            });

            return Checksum;
        }

        private long ExecuteContendedHit(StripedIntKeyCache cache)
        {
            long Checksum = 0;
            var Options = new ParallelOptions { MaxDegreeOfParallelism = WorkerCount };

            Parallel.For(0, WorkerCount, Options, WorkerIndex =>
            {
                long Local = 0;
                for (var X = 0; X < ContendedOperationsPerThread; ++X)
                {
                    var Type = _HotTypes[(WorkerIndex + X) % _HotTypes.Length];
                    Local += cache.GetOrAdd(Type).GetHashCode();
                }

                Interlocked.Add(ref Checksum, Local);
            });

            return Checksum;
        }

        private long ExecuteContendedCold(LegacyIntKeyCache cache)
        {
            long Checksum = 0;
            var Options = new ParallelOptions { MaxDegreeOfParallelism = WorkerCount };

            Parallel.For(0, WorkerCount, Options, WorkerIndex =>
            {
                long Local = 0;
                for (var X = 0; X < ContendedOperationsPerThread; ++X)
                {
                    var Type = _ColdTypes[(WorkerIndex + X) % _ColdTypes.Length];
                    Local += cache.GetOrAdd(Type).GetHashCode();
                }

                Interlocked.Add(ref Checksum, Local);
            });

            return Checksum;
        }

        private long ExecuteContendedCold(TypeKeyConcurrentCache cache)
        {
            long Checksum = 0;
            var Options = new ParallelOptions { MaxDegreeOfParallelism = WorkerCount };

            Parallel.For(0, WorkerCount, Options, WorkerIndex =>
            {
                long Local = 0;
                for (var X = 0; X < ContendedOperationsPerThread; ++X)
                {
                    var Type = _ColdTypes[(WorkerIndex + X) % _ColdTypes.Length];
                    Local += cache.GetOrAdd(Type).GetHashCode();
                }

                Interlocked.Add(ref Checksum, Local);
            });

            return Checksum;
        }

        private long ExecuteContendedCold(PooledConcurrentTypeKeyCache cache)
        {
            long Checksum = 0;
            var Options = new ParallelOptions { MaxDegreeOfParallelism = WorkerCount };

            Parallel.For(0, WorkerCount, Options, WorkerIndex =>
            {
                long Local = 0;
                for (var X = 0; X < ContendedOperationsPerThread; ++X)
                {
                    var Type = _ColdTypes[(WorkerIndex + X) % _ColdTypes.Length];
                    Local += cache.GetOrAdd(Type).GetHashCode();
                }

                Interlocked.Add(ref Checksum, Local);
            });

            return Checksum;
        }

        private long ExecuteContendedCold(ConcurrentIntKeyCollisionSafeCache cache)
        {
            long Checksum = 0;
            var Options = new ParallelOptions { MaxDegreeOfParallelism = WorkerCount };

            Parallel.For(0, WorkerCount, Options, WorkerIndex =>
            {
                long Local = 0;
                for (var X = 0; X < ContendedOperationsPerThread; ++X)
                {
                    var Type = _ColdTypes[(WorkerIndex + X) % _ColdTypes.Length];
                    Local += cache.GetOrAdd(Type).GetHashCode();
                }

                Interlocked.Add(ref Checksum, Local);
            });

            return Checksum;
        }

        private long ExecuteContendedCold(StripedTypeKeyCache cache)
        {
            long Checksum = 0;
            var Options = new ParallelOptions { MaxDegreeOfParallelism = WorkerCount };

            Parallel.For(0, WorkerCount, Options, WorkerIndex =>
            {
                long Local = 0;
                for (var X = 0; X < ContendedOperationsPerThread; ++X)
                {
                    var Type = _ColdTypes[(WorkerIndex + X) % _ColdTypes.Length];
                    Local += cache.GetOrAdd(Type).GetHashCode();
                }

                Interlocked.Add(ref Checksum, Local);
            });

            return Checksum;
        }

        private long ExecuteContendedCold(StripedIntKeyCache cache)
        {
            long Checksum = 0;
            var Options = new ParallelOptions { MaxDegreeOfParallelism = WorkerCount };

            Parallel.For(0, WorkerCount, Options, WorkerIndex =>
            {
                long Local = 0;
                for (var X = 0; X < ContendedOperationsPerThread; ++X)
                {
                    var Type = _ColdTypes[(WorkerIndex + X) % _ColdTypes.Length];
                    Local += cache.GetOrAdd(Type).GetHashCode();
                }

                Interlocked.Add(ref Checksum, Local);
            });

            return Checksum;
        }

        private sealed class LegacyIntKeyCache
        {
            private readonly Dictionary<int, object> _cache = [];
            private readonly object _lockObject = new();

            public void Clear()
            {
                lock (_lockObject)
                {
                    _cache.Clear();
                }
            }

            public object GetOrAdd(Type type)
            {
                lock (_lockObject)
                {
                    int HashCode = type.GetHashCode();
                    if (_cache.TryGetValue(HashCode, out var Value))
                        return Value;

                    Value = new object();
                    _cache[HashCode] = Value;
                    return Value;
                }
            }
        }

        private sealed class TypeKeyConcurrentCache
        {
            private ConcurrentDictionary<Type, object> _cache = [];

            public void Clear() => _cache = [];

            public object GetOrAdd(Type type) => _cache.GetOrAdd(type, static _ => new object());
        }

        private sealed class ConcurrentIntKeyCollisionSafeCache
        {
            private ConcurrentDictionary<int, CollisionNode> _cache = [];

            public void Clear() => _cache = [];

            public object GetOrAdd(Type type)
            {
                int HashCode = type.GetHashCode();

                while (true)
                {
                    if (_cache.TryGetValue(HashCode, out var Head))
                    {
                        for (var Current = Head; Current != null; Current = Current.Next)
                        {
                            if (ReferenceEquals(Current.Type, type))
                                return Current.Value;
                        }

                        var NewNode = new CollisionNode(type, new object(), Head);
                        if (_cache.TryUpdate(HashCode, NewNode, Head))
                            return NewNode.Value;

                        continue;
                    }

                    var Value = new object();
                    if (_cache.TryAdd(HashCode, new CollisionNode(type, Value, null)))
                        return Value;
                }
            }
        }

        private sealed class PooledConcurrentTypeKeyCache
        {
            private ConcurrentDictionary<Type, object> _cache = [];
            private readonly ConcurrentBag<object> _pool = [];

            public void Clear()
            {
                foreach (var Item in _cache.Values)
                    _pool.Add(Item);

                _cache = [];
            }

            public object GetOrAdd(Type type) => _cache.GetOrAdd(type, static (_, state) => state.Rent(), this);

            private object Rent() => _pool.TryTake(out var Value) ? Value : new object();
        }

        private sealed class StripedTypeKeyCache
        {
            private const int StripeCount = 32;
            private readonly Dictionary<Type, object>[] _caches = new Dictionary<Type, object>[StripeCount];
            private readonly object[] _locks = new object[StripeCount];

            public StripedTypeKeyCache()
            {
                for (var X = 0; X < StripeCount; ++X)
                {
                    _caches[X] = [];
                    _locks[X] = new object();
                }
            }

            public void Clear()
            {
                for (var X = 0; X < StripeCount; ++X)
                {
                    lock (_locks[X])
                    {
                        _caches[X].Clear();
                    }
                }
            }

            public object GetOrAdd(Type type)
            {
                int Stripe = RuntimeHelpers.GetHashCode(type) & (StripeCount - 1);
                var Cache = _caches[Stripe];

                lock (_locks[Stripe])
                {
                    if (Cache.TryGetValue(type, out var Value))
                        return Value;

                    Value = new object();
                    Cache[type] = Value;
                    return Value;
                }
            }
        }

        private sealed class StripedIntKeyCache
        {
            private const int StripeCount = 32;
            private readonly Dictionary<int, CollisionNode>[] _caches = new Dictionary<int, CollisionNode>[StripeCount];
            private readonly object[] _locks = new object[StripeCount];

            public StripedIntKeyCache()
            {
                for (var X = 0; X < StripeCount; ++X)
                {
                    _caches[X] = [];
                    _locks[X] = new object();
                }
            }

            public void Clear()
            {
                for (var X = 0; X < StripeCount; ++X)
                {
                    lock (_locks[X])
                    {
                        _caches[X].Clear();
                    }
                }
            }

            public object GetOrAdd(Type type)
            {
                int HashCode = type.GetHashCode();
                int Stripe = HashCode & (StripeCount - 1);
                var Cache = _caches[Stripe];

                lock (_locks[Stripe])
                {
                    if (Cache.TryGetValue(HashCode, out var Head))
                    {
                        for (var Current = Head; Current != null; Current = Current.Next)
                        {
                            if (ReferenceEquals(Current.Type, type))
                                return Current.Value;
                        }

                        var NewNode = new CollisionNode(type, new object(), Head);
                        Cache[HashCode] = NewNode;
                        return NewNode.Value;
                    }

                    var Value = new object();
                    Cache[HashCode] = new CollisionNode(type, Value, null);
                    return Value;
                }
            }
        }

        private sealed class CollisionNode(Type type, object value, CollisionNode next)
        {
            public CollisionNode Next { get; } = next;
            public Type Type { get; } = type;
            public object Value { get; } = value;
        }
    }
}