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
            ConstructorInfo[] Constructors = type?.GetConstructors() ?? [];
            var TempConstructors = new List<Constructor>(Constructors.Length);
            for (var X = 0; X < Constructors.Length; ++X)
            {
                ConstructorInfo TempConstructor = Constructors[X];
                ParameterInfo[] Parameters = TempConstructor.GetParameters();
                if (Parameters.Any(y => y.ParameterType.IsPointer))
                    continue;
                TempConstructors.Add(new Constructor(TempConstructor, Parameters));
            }
            if (DefaultValues.Values.TryGetValue(hashCode, out var DefaultValue))
                TempConstructors.Add(new Constructor(_ => DefaultValue));
            else if (type.IsEnum && DefaultValues.Values.TryGetValue(type.GetEnumUnderlyingType().GetHashCode(), out DefaultValue))
                TempConstructors.Add(new Constructor(_ => Enum.Parse(type, DefaultValue.ToString(), true)));
            this.Constructors = [.. TempConstructors.OrderBy(x => x.ParameterLength)];
            if (this.Constructors.Length > 0 && this.Constructors[0].ParameterLength == 0)
                _DefaultConstructor = this.Constructors[0].ConstructorDelegate;
        }

        /// <summary>
        /// Gets the constructors.
        /// </summary>
        /// <value>The constructors.</value>
        private Constructor[] Constructors { get; }

        /// <summary>
        /// The default constructor
        /// </summary>
        private readonly ConstructorDelegate _DefaultConstructor;

        /// <summary>
        /// Creates an instance.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The instance created.</returns>
        public object CreateInstance(object[] args)
        {
            for (var X = 0; X < Constructors.Length; ++X)
            {
                Constructor Constructor = Constructors[X];
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
        public object CreateInstance() => _DefaultConstructor is null ? null : _DefaultConstructor([]);
    }
}