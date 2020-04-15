using Fast.Activator.Utils;
using System;
using System.Collections.Generic;

namespace Fast.Activator
{
    /// <summary>
    /// Fast activator static class
    /// </summary>
    public static class FastActivator
    {
        /// <summary>
        /// The lock object
        /// </summary>
        private static readonly object LockObject = new object();

        /// <summary>
        /// The constructors
        /// </summary>
        private static Dictionary<int, ConstructorList> Constructors = new Dictionary<int, ConstructorList>();

        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        /// <typeparam name="TClass">The type of the class.</typeparam>
        /// <param name="args">The arguments.</param>
        /// <returns>The instance created.</returns>
        public static TClass CreateInstance<TClass>(params object[] args)
        {
            var ReturnValue = CreateInstance(typeof(TClass), args);
            if (ReturnValue is null)
                return default;
            return (TClass)ReturnValue;
        }

        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        /// <typeparam name="TClass">The type of the class.</typeparam>
        /// <returns>The instance created.</returns>
        public static TClass CreateInstance<TClass>()
        {
            var ReturnValue = CreateInstance(typeof(TClass));
            if (ReturnValue is null)
                return default;
            return (TClass)ReturnValue;
        }

        /// <summary>
        /// Creates an instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The object if it can be created, null otherwise.</returns>
        public static object CreateInstance(Type type, params object[] args)
        {
            int HashCode = type.GetHashCode();
            if (!Constructors.TryGetValue(HashCode, out var TempConstructor))
            {
                TempConstructor = CreateConstructors(type, HashCode);
            }
            return TempConstructor.CreateInstance(args);
        }

        /// <summary>
        /// Creates an instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The object if it can be created, null otherwise.</returns>
        public static object CreateInstance(Type type)
        {
            int HashCode = type.GetHashCode();
            if (!Constructors.TryGetValue(HashCode, out var TempConstructor))
            {
                TempConstructor = CreateConstructors(type, HashCode);
            }
            return TempConstructor.CreateInstance();
        }

        /// <summary>
        /// Creates the constructors.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="HashCode">The hash code.</param>
        /// <returns>The constructors</returns>
        private static ConstructorList CreateConstructors(Type type, int HashCode)
        {
            ConstructorList TempConstructor = null;
            lock (LockObject)
            {
                if (Constructors.TryGetValue(HashCode, out TempConstructor))
                    return TempConstructor;
                TempConstructor = new ConstructorList(type, HashCode);
                Constructors.Add(HashCode, TempConstructor);
            }
            return TempConstructor;
        }
    }
}