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
        /// The constructors
        /// </summary>
        private static readonly Dictionary<int, ConstructorList> Constructors = new();

        /// <summary>
        /// The lock object
        /// </summary>
        private static readonly object LockObject = new();

        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        /// <typeparam name="TClass">The type of the class.</typeparam>
        /// <param name="args">The arguments.</param>
        /// <returns>The instance created.</returns>
        public static TClass CreateInstance<TClass>(params object[] args)
        {
            var ReturnValue = CreateInstance(typeof(TClass), args);
            return ReturnValue is null ? default : (TClass)ReturnValue;
        }

        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        /// <typeparam name="TClass">The type of the class.</typeparam>
        /// <returns>The instance created.</returns>
        public static TClass CreateInstance<TClass>()
        {
            var ReturnValue = CreateInstance(typeof(TClass));
            return ReturnValue is null ? default : (TClass)ReturnValue;
        }

        /// <summary>
        /// Creates an instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The object if it can be created, null otherwise.</returns>
        public static object CreateInstance(Type type, params object[] args) => type is null ? throw new ArgumentNullException(nameof(type)) : (GetConstructorList(type)?.CreateInstance(args));

        /// <summary>
        /// Creates an instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The object if it can be created, null otherwise.</returns>
        public static object CreateInstance(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            try
            {
                return System.Activator.CreateInstance(type);
            }
            catch { return null; }
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

        /// <summary>
        /// Gets the constructor list.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The constructor list.</returns>
        /// <exception cref="ArgumentNullException">type</exception>
        private static ConstructorList GetConstructorList(Type type)
        {
            int HashCode;
            try
            {
                HashCode = type.GetHashCode();
            }
            catch { return null; }
            return !Constructors.TryGetValue(HashCode, out ConstructorList TempConstructor) ? CreateConstructors(type, HashCode) : TempConstructor;
        }
    }
}