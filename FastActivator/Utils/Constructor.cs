using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Fast.Activator.Utils
{
    /// <summary>
    /// Constructor
    /// </summary>
    public class Constructor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Constructor"/> class.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <param name="parameters">The parameters.</param>
        /// <exception cref="ArgumentNullException">constructor</exception>
        public Constructor(ConstructorInfo constructor, ParameterInfo[] parameters)
        {
            if (constructor is null)
                throw new ArgumentNullException(nameof(constructor));

            var TempParameters = parameters ?? Array.Empty<ParameterInfo>();
            Parameters = TempParameters.Select(x => x.ParameterType).ToArray();
            ParameterLength = Parameters.Length;
            ParameterNullable = Parameters.Select(x => !(Nullable.GetUnderlyingType(x) is null)).ToArray();
            ConstructorDelegate = CreateConstructor(constructor, TempParameters);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Constructor"/> class.
        /// </summary>
        /// <param name="delegate">The delegate.</param>
        public Constructor(ConstructorDelegate @delegate)
        {
            ConstructorDelegate = @delegate;
            ParameterLength = 0;
            Parameters = Array.Empty<Type>();
            ParameterNullable = Array.Empty<bool>();
        }

        /// <summary>
        /// Gets the size of the parameter.
        /// </summary>
        /// <value>The size of the parameter.</value>
        public int ParameterLength { get; }

        /// <summary>
        /// Gets the constructor information.
        /// </summary>
        /// <value>The constructor information.</value>
        internal ConstructorDelegate ConstructorDelegate { get; }

        /// <summary>
        /// Gets the parameter nullable.
        /// </summary>
        /// <value>The parameter nullable.</value>
        private bool[] ParameterNullable { get; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        private Type[] Parameters { get; }

        /// <summary>
        /// Create an instance.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The new object</returns>
        public object CreateInstance(object[] args)
        {
            return ConstructorDelegate(args);
        }

        /// <summary>
        /// Determines whether the specified arguments is a match.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if the specified arguments is a match; otherwise, <c>false</c>.</returns>
        public bool IsMatch(object[] args)
        {
            if (args is null || args.Length != ParameterLength)
                return false;
            for (int x = 0; x < ParameterLength; ++x)
            {
                var arg = args[x];
                var Parameter = Parameters[x];
                if (arg is null)
                {
                    if (ParameterNullable[x])
                        continue;
                    return false;
                }
                var ArgType = arg.GetType();
                if (!Parameter.IsAssignableFrom(ArgType))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Creates the argument expression.
        /// </summary>
        /// <param name="parameterExpression">The parameter expression.</param>
        /// <param name="parameterInfo">The parameter information.</param>
        /// <param name="index">The index.</param>
        /// <returns>The argument expression</returns>
        private static Expression CreateArgumentExpression(ParameterExpression parameterExpression, ParameterInfo parameterInfo, int index)
        {
            return Expression.Convert(
                Expression.ArrayIndex(parameterExpression, Expression.Constant(index)),
                parameterInfo.ParameterType);
        }

        /// <summary>
        /// Creates the constructor.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The constructor</returns>
        private static ConstructorDelegate CreateConstructor(ConstructorInfo constructor, ParameterInfo[] parameters)
        {
            var ParameterExpression = Expression.Parameter(typeof(object[]), "args");

            var ArgsExpressions = parameters
                .Select((info, index) => CreateArgumentExpression(ParameterExpression, info, index))
                .ToArray();

            Expression NewExpression = Expression.New(constructor, ArgsExpressions);
            if (constructor.DeclaringType.IsValueType)
                NewExpression = Expression.Convert(NewExpression, typeof(object));

            return Expression.Lambda(
                typeof(ConstructorDelegate),
                NewExpression,
                ParameterExpression).Compile() as ConstructorDelegate;
        }
    }
}