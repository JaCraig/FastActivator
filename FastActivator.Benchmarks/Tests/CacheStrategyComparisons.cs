using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace FastActivator.Benchmarks.Tests
{
    [MemoryDiagnoser, HtmlExporter, RankColumn]
    public class CacheStrategyComparisons
    {
        private const int OperationsPerThread = 20_000;

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
            typeof(DateOnly),
            typeof(TimeOnly),
            typeof(DateTimeOffset),
            typeof(Dictionary<int, string>),
            typeof(List<int>)
        ];

        private readonly Type _MissType = typeof(Guid);
        private readonly Type[] _Types =
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
        private CopyOnWriteTypeKeyCache _CopyOnWriteCache;
        private LegacyIntKeyCache _LegacyCache;
        private PooledConcurrentTypeKeyCache _PooledConcurrentCache;
        private StripedTypeKeyCache _StripedCache;
        private SnapshotConcurrentTypeKeyCache _SnapshotConcurrentCache;
        private TypeKeyConcurrentCache _TypeCache;

        [Params(1, 4, 8)]
        public int WorkerCount { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            _CopyOnWriteCache = new CopyOnWriteTypeKeyCache();
            _LegacyCache = new LegacyIntKeyCache();
            _PooledConcurrentCache = new PooledConcurrentTypeKeyCache();
            _StripedCache = new StripedTypeKeyCache();
            _SnapshotConcurrentCache = new SnapshotConcurrentTypeKeyCache();
            _TypeCache = new TypeKeyConcurrentCache();

            foreach (var Type in _Types)
            {
                _ = _CopyOnWriteCache.GetOrAdd(Type);
                _ = _LegacyCache.GetOrAdd(Type);
                _ = _PooledConcurrentCache.GetOrAdd(Type);
                _ = _StripedCache.GetOrAdd(Type);
                _ = _SnapshotConcurrentCache.GetOrAdd(Type);
                _ = _TypeCache.GetOrAdd(Type);
            }
        }

        [IterationSetup(Target = nameof(CopyOnWriteTypeKeyGetOrAddMiss))]
        public void SetupCopyOnWriteMiss() => _CopyOnWriteCache.Clear();

        [IterationSetup(Target = nameof(PooledConcurrentTypeKeyGetOrAddMiss))]
        public void SetupPooledConcurrentMiss() => _PooledConcurrentCache.Clear();

        [IterationSetup(Target = nameof(LegacyIntKeyGetOrAddMiss))]
        public void SetupLegacyMiss() => _LegacyCache.Clear();

        [IterationSetup(Target = nameof(TypeKeyGetOrAddMiss))]
        public void SetupTypeMiss() => _TypeCache.Clear();

        [IterationSetup(Target = nameof(StripedTypeKeyGetOrAddMiss))]
        public void SetupStripedMiss() => _StripedCache.Clear();

        [IterationSetup(Target = nameof(SnapshotConcurrentTypeKeyGetOrAddMiss))]
        public void SetupSnapshotConcurrentMiss() => _SnapshotConcurrentCache.Clear();

        [IterationSetup(Target = nameof(LegacyIntKeyContendedHit))]
        public void SetupLegacyContendedHit()
        {
            _LegacyCache.Clear();
            foreach (var Type in _Types)
                _ = _LegacyCache.GetOrAdd(Type);
        }

        [IterationSetup(Target = nameof(TypeKeyContendedHit))]
        public void SetupTypeContendedHit()
        {
            _TypeCache.Clear();
            foreach (var Type in _Types)
                _ = _TypeCache.GetOrAdd(Type);
        }

        [IterationSetup(Target = nameof(CopyOnWriteTypeKeyContendedHit))]
        public void SetupCopyOnWriteContendedHit()
        {
            _CopyOnWriteCache.Clear();
            foreach (var Type in _Types)
                _ = _CopyOnWriteCache.GetOrAdd(Type);
        }

        [IterationSetup(Target = nameof(PooledConcurrentTypeKeyContendedHit))]
        public void SetupPooledConcurrentContendedHit()
        {
            _PooledConcurrentCache.Clear();
            foreach (var Type in _Types)
                _ = _PooledConcurrentCache.GetOrAdd(Type);
        }

        [IterationSetup(Target = nameof(StripedTypeKeyContendedHit))]
        public void SetupStripedContendedHit()
        {
            _StripedCache.Clear();
            foreach (var Type in _Types)
                _ = _StripedCache.GetOrAdd(Type);
        }

        [IterationSetup(Target = nameof(SnapshotConcurrentTypeKeyContendedHit))]
        public void SetupSnapshotConcurrentContendedHit()
        {
            _SnapshotConcurrentCache.Clear();
            foreach (var Type in _Types)
                _ = _SnapshotConcurrentCache.GetOrAdd(Type);
        }

        [IterationSetup(Target = nameof(LegacyIntKeyContendedColdStart))]
        public void SetupLegacyContendedCold() => _LegacyCache.Clear();

        [IterationSetup(Target = nameof(TypeKeyContendedColdStart))]
        public void SetupTypeContendedCold() => _TypeCache.Clear();

        [IterationSetup(Target = nameof(CopyOnWriteTypeKeyContendedColdStart))]
        public void SetupCopyOnWriteContendedCold() => _CopyOnWriteCache.Clear();

        [IterationSetup(Target = nameof(PooledConcurrentTypeKeyContendedColdStart))]
        public void SetupPooledConcurrentContendedCold() => _PooledConcurrentCache.Clear();

        [IterationSetup(Target = nameof(StripedTypeKeyContendedColdStart))]
        public void SetupStripedContendedCold() => _StripedCache.Clear();

        [IterationSetup(Target = nameof(SnapshotConcurrentTypeKeyContendedColdStart))]
        public void SetupSnapshotConcurrentContendedCold() => _SnapshotConcurrentCache.Clear();

        [Benchmark(Baseline = true)]
        public object LegacyIntKeyGetOrAddHit()
        {
            var Type = GetNextType();
            return _LegacyCache.GetOrAdd(Type);
        }

        [Benchmark]
        public object TypeKeyGetOrAddHit()
        {
            var Type = GetNextType();
            return _TypeCache.GetOrAdd(Type);
        }

        [Benchmark]
        public object LegacyIntKeyGetOrAddMiss() => _LegacyCache.GetOrAdd(_MissType);

        [Benchmark]
        public object TypeKeyGetOrAddMiss() => _TypeCache.GetOrAdd(_MissType);

        [Benchmark]
        public object CopyOnWriteTypeKeyGetOrAddHit()
        {
            var Type = GetNextType();
            return _CopyOnWriteCache.GetOrAdd(Type);
        }

        [Benchmark]
        public object CopyOnWriteTypeKeyGetOrAddMiss() => _CopyOnWriteCache.GetOrAdd(_MissType);

        [Benchmark]
        public object PooledConcurrentTypeKeyGetOrAddHit()
        {
            var Type = GetNextType();
            return _PooledConcurrentCache.GetOrAdd(Type);
        }

        [Benchmark]
        public object PooledConcurrentTypeKeyGetOrAddMiss() => _PooledConcurrentCache.GetOrAdd(_MissType);

        [Benchmark]
        public object StripedTypeKeyGetOrAddHit()
        {
            var Type = GetNextType();
            return _StripedCache.GetOrAdd(Type);
        }

        [Benchmark]
        public object StripedTypeKeyGetOrAddMiss() => _StripedCache.GetOrAdd(_MissType);

        [Benchmark]
        public object SnapshotConcurrentTypeKeyGetOrAddHit()
        {
            var Type = GetNextType();
            return _SnapshotConcurrentCache.GetOrAdd(Type);
        }

        [Benchmark]
        public object SnapshotConcurrentTypeKeyGetOrAddMiss() => _SnapshotConcurrentCache.GetOrAdd(_MissType);

        [Benchmark]
        public long LegacyIntKeyContendedHit() => ExecuteContendedHit(_LegacyCache);

        [Benchmark]
        public long TypeKeyContendedHit() => ExecuteContendedHit(_TypeCache);

        [Benchmark]
        public long LegacyIntKeyContendedColdStart() => ExecuteContendedCold(_LegacyCache);

        [Benchmark]
        public long TypeKeyContendedColdStart() => ExecuteContendedCold(_TypeCache);

        [Benchmark]
        public long CopyOnWriteTypeKeyContendedHit() => ExecuteContendedHit(_CopyOnWriteCache);

        [Benchmark]
        public long CopyOnWriteTypeKeyContendedColdStart() => ExecuteContendedCold(_CopyOnWriteCache);

        [Benchmark]
        public long PooledConcurrentTypeKeyContendedHit() => ExecuteContendedHit(_PooledConcurrentCache);

        [Benchmark]
        public long PooledConcurrentTypeKeyContendedColdStart() => ExecuteContendedCold(_PooledConcurrentCache);

        [Benchmark]
        public long StripedTypeKeyContendedHit() => ExecuteContendedHit(_StripedCache);

        [Benchmark]
        public long StripedTypeKeyContendedColdStart() => ExecuteContendedCold(_StripedCache);

        [Benchmark]
        public long SnapshotConcurrentTypeKeyContendedHit() => ExecuteContendedHit(_SnapshotConcurrentCache);

        [Benchmark]
        public long SnapshotConcurrentTypeKeyContendedColdStart() => ExecuteContendedCold(_SnapshotConcurrentCache);

        private Type GetNextType()
        {
            var Value = _Types[_Index];
            _Index = (_Index + 1) % _Types.Length;
            return Value;
        }

        private long ExecuteContendedHit(LegacyIntKeyCache cache)
        {
            long Checksum = 0;
            var Options = new ParallelOptions { MaxDegreeOfParallelism = WorkerCount };

            Parallel.For(0, WorkerCount, Options, WorkerIndex =>
            {
                long Local = 0;
                for (var X = 0; X < OperationsPerThread; ++X)
                {
                    var Type = _Types[(WorkerIndex + X) % _Types.Length];
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
                for (var X = 0; X < OperationsPerThread; ++X)
                {
                    var Type = _Types[(WorkerIndex + X) % _Types.Length];
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
                for (var X = 0; X < OperationsPerThread; ++X)
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
                for (var X = 0; X < OperationsPerThread; ++X)
                {
                    var Type = _ColdTypes[(WorkerIndex + X) % _ColdTypes.Length];
                    Local += cache.GetOrAdd(Type).GetHashCode();
                }

                Interlocked.Add(ref Checksum, Local);
            });

            return Checksum;
        }

        private long ExecuteContendedHit(CopyOnWriteTypeKeyCache cache)
        {
            long Checksum = 0;
            var Options = new ParallelOptions { MaxDegreeOfParallelism = WorkerCount };

            Parallel.For(0, WorkerCount, Options, WorkerIndex =>
            {
                long Local = 0;
                for (var X = 0; X < OperationsPerThread; ++X)
                {
                    var Type = _Types[(WorkerIndex + X) % _Types.Length];
                    Local += cache.GetOrAdd(Type).GetHashCode();
                }

                Interlocked.Add(ref Checksum, Local);
            });

            return Checksum;
        }

        private long ExecuteContendedCold(CopyOnWriteTypeKeyCache cache)
        {
            long Checksum = 0;
            var Options = new ParallelOptions { MaxDegreeOfParallelism = WorkerCount };

            Parallel.For(0, WorkerCount, Options, WorkerIndex =>
            {
                long Local = 0;
                for (var X = 0; X < OperationsPerThread; ++X)
                {
                    var Type = _ColdTypes[(WorkerIndex + X) % _ColdTypes.Length];
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
                for (var X = 0; X < OperationsPerThread; ++X)
                {
                    var Type = _Types[(WorkerIndex + X) % _Types.Length];
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
                for (var X = 0; X < OperationsPerThread; ++X)
                {
                    var Type = _ColdTypes[(WorkerIndex + X) % _ColdTypes.Length];
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
                for (var X = 0; X < OperationsPerThread; ++X)
                {
                    var Type = _Types[(WorkerIndex + X) % _Types.Length];
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
                for (var X = 0; X < OperationsPerThread; ++X)
                {
                    var Type = _ColdTypes[(WorkerIndex + X) % _ColdTypes.Length];
                    Local += cache.GetOrAdd(Type).GetHashCode();
                }

                Interlocked.Add(ref Checksum, Local);
            });

            return Checksum;
        }

        private long ExecuteContendedHit(SnapshotConcurrentTypeKeyCache cache)
        {
            long Checksum = 0;
            var Options = new ParallelOptions { MaxDegreeOfParallelism = WorkerCount };

            Parallel.For(0, WorkerCount, Options, WorkerIndex =>
            {
                long Local = 0;
                for (var X = 0; X < OperationsPerThread; ++X)
                {
                    var Type = _Types[(WorkerIndex + X) % _Types.Length];
                    Local += cache.GetOrAdd(Type).GetHashCode();
                }

                Interlocked.Add(ref Checksum, Local);
            });

            return Checksum;
        }

        private long ExecuteContendedCold(SnapshotConcurrentTypeKeyCache cache)
        {
            long Checksum = 0;
            var Options = new ParallelOptions { MaxDegreeOfParallelism = WorkerCount };

            Parallel.For(0, WorkerCount, Options, WorkerIndex =>
            {
                long Local = 0;
                for (var X = 0; X < OperationsPerThread; ++X)
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
                    if (_cache.TryGetValue(type.GetHashCode(), out var Value))
                        return Value;

                    Value = new object();
                    _cache[type.GetHashCode()] = Value;
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

        private sealed class SnapshotConcurrentTypeKeyCache
        {
            private readonly object _lockObject = new();
            private ConcurrentDictionary<Type, object> _missCache = [];
            private Dictionary<Type, object> _snapshot = [];

            public void Clear()
            {
                lock (_lockObject)
                {
                    Volatile.Write(ref _snapshot, new Dictionary<Type, object>());
                    _missCache = [];
                }
            }

            public object GetOrAdd(Type type)
            {
                var Snapshot = Volatile.Read(ref _snapshot);
                if (Snapshot.TryGetValue(type, out var Value))
                    return Value;

                Value = _missCache.GetOrAdd(type, static _ => new object());

                lock (_lockObject)
                {
                    Snapshot = Volatile.Read(ref _snapshot);
                    if (Snapshot.TryGetValue(type, out var Existing))
                        return Existing;

                    var NewSnapshot = new Dictionary<Type, object>(Snapshot)
                    {
                        [type] = Value
                    };
                    Volatile.Write(ref _snapshot, NewSnapshot);
                    return Value;
                }
            }
        }

        private sealed class CopyOnWriteTypeKeyCache
        {
            private readonly object _lockObject = new();
            private Dictionary<Type, object> _cache = [];

            public void Clear()
            {
                lock (_lockObject)
                {
                    Volatile.Write(ref _cache, new Dictionary<Type, object>());
                }
            }

            public object GetOrAdd(Type type)
            {
                var Cache = Volatile.Read(ref _cache);
                if (Cache.TryGetValue(type, out var Value))
                    return Value;

                lock (_lockObject)
                {
                    Cache = Volatile.Read(ref _cache);
                    if (Cache.TryGetValue(type, out Value))
                        return Value;

                    Value = new object();
                    var NewCache = new Dictionary<Type, object>(Cache)
                    {
                        [type] = Value
                    };
                    Volatile.Write(ref _cache, NewCache);
                    return Value;
                }
            }
        }
    }
}