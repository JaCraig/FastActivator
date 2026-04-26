using Fast.Activator.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Fast.Activator
{
    /// <summary>
    /// A thread-safe, striped cache for storing constructor information organized by type.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This cache uses stripe-based locking to reduce contention in multi-threaded scenarios.
    /// Instead of a single lock protecting the entire cache, the cache is divided into 32 stripes,
    /// each with its own lock. This allows multiple threads to access different stripes concurrently.
    /// </para>
    /// <para>
    /// The stripe is determined by the hash code of the type modulo the stripe count, ensuring
    /// consistent distribution across stripes.
    /// </para>
    /// </remarks>
    internal class ConstructorCache
    {
        /// <summary>
        /// The number of independent cache stripes. Must be a power of two for efficient bitwise modulo.
        /// </summary>
        private const int StripeCount = 32;

        /// <summary>
        /// Array of dictionaries, one per stripe, mapping types to their constructor lists.
        /// </summary>
        private readonly Dictionary<Type, ConstructorList>[] _caches = new Dictionary<Type, ConstructorList>[StripeCount];

        /// <summary>
        /// Array of lock objects, one per stripe, protecting concurrent access to the corresponding cache.
        /// </summary>
        private readonly object[] _locks = new object[StripeCount];

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorCache"/> class.
        /// </summary>
        /// <remarks>
        /// Creates all stripe dictionaries and lock objects during construction.
        /// </remarks>
        public ConstructorCache()
        {
            for (var x = 0; x < StripeCount; ++x)
            {
                _caches[x] = [];
                _locks[x] = new object();
            }
        }

        /// <summary>
        /// Gets a constructor list from the cache for the specified type, or adds it if not present.
        /// </summary>
        /// <param name="type">The type for which to retrieve or create constructor information.</param>
        /// <param name="valueFactory">
        /// A factory function that creates a <see cref="ConstructorList"/> for the type if it is not already cached.
        /// Called only once per type, even in concurrent scenarios.
        /// </param>
        /// <returns>The cached or newly created <see cref="ConstructorList"/> for the specified type.</returns>
        /// <remarks>
        /// <para>
        /// This method is thread-safe and uses stripe-based locking. The type's hash code determines
        /// which stripe (lock) is used, allowing multiple threads to access different types concurrently.
        /// </para>
        /// <para>
        /// If the type is already in the cache, the factory function is not invoked. If not present,
        /// the factory is called exactly once while holding the lock for the corresponding stripe.
        /// </para>
        /// </remarks>
        public ConstructorList GetOrAdd(Type type, Func<Type, ConstructorList> valueFactory)
        {
            // Determine the stripe using the type's hash code and bitwise AND with (StripeCount - 1).
            // This is efficient because StripeCount is a power of two, making the bitwise operation
            // equivalent to modulo without division overhead.
            int stripe = RuntimeHelpers.GetHashCode(type) & (StripeCount - 1);
            var cache = _caches[stripe];

            lock (_locks[stripe])
            {
                // Check if the type is already cached in this stripe.
                if (cache.TryGetValue(type, out var value))
                    return value;

                // Type not found; invoke factory to create the constructor list.
                value = valueFactory(type);
                cache[type] = value;
                return value;
            }
        }
    }
}