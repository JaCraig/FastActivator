using System;
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
        /// <param name="constructors">The constructors.</param>
        public ConstructorList(ConstructorInfo[] constructors)
        {
            Constructors = constructors.Select(x => new Constructor(x)).OrderBy(x => x.ParameterLength).ToArray();
            DefaultConstructor = Constructors[0].ConstructorDelegate;
        }

        /// <summary>
        /// Gets the constructors.
        /// </summary>
        /// <value>The constructors.</value>
        private Constructor[] Constructors { get; }

        /// <summary>
        /// The default constructor
        /// </summary>
        private ConstructorDelegate DefaultConstructor;

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
            return DefaultConstructor(Array.Empty<object>());
        }
    }
}