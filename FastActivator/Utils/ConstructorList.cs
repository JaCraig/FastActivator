using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fast.Activator.Utils
{
    /// <summary>
    /// Constructor list
    /// </summary>
    public class ConstructorList
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorList"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="hashCode">The hash code.</param>
        public ConstructorList(Type type, int hashCode)
        {
            var constructors = type?.GetConstructors() ?? Array.Empty<ConstructorInfo>();
            var TempConstructors = new List<Constructor>();
            for (int x = 0; x < constructors.Length; ++x)
            {
                var TempConstructor = constructors[x];
                var Parameters = TempConstructor.GetParameters();
                if (Parameters.Any(y => y.ParameterType.IsPointer))
                    continue;
                TempConstructors.Add(new Constructor(TempConstructor, Parameters));
            }
            if (DefaultValues.Values.TryGetValue(hashCode, out var DefaultValue))
                TempConstructors.Add(new Constructor(_ => DefaultValue));
            Constructors = TempConstructors.OrderBy(x => x.ParameterLength).ToArray();
            if (Constructors.Length > 0 && Constructors[0].ParameterLength == 0)
                DefaultConstructor = Constructors[0].ConstructorDelegate;
        }

        /// <summary>
        /// The default constructor
        /// </summary>
        private ConstructorDelegate DefaultConstructor;

        /// <summary>
        /// Gets the constructors.
        /// </summary>
        /// <value>The constructors.</value>
        private Constructor[] Constructors { get; }

        /// <summary>
        /// Creates an instance.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The instance created.</returns>
        public object CreateInstance(object[] args)
        {
            for (int x = 0; x < Constructors.Length; ++x)
            {
                var Constructor = Constructors[x];
                if (Constructor.IsMatch(args))
                {
                    return Constructor.CreateInstance(args);
                }
            }
            return null;
        }

        /// <summary>
        /// Creates an instance.
        /// </summary>
        /// <returns>The instance created.</returns>
        public object CreateInstance()
        {
            if (DefaultConstructor is null)
                return null;
            return DefaultConstructor(Array.Empty<object>());
        }
    }
}