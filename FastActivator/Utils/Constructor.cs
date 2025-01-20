using System;
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
            ArgumentNullException.ThrowIfNull(constructor);

            parameters ??= [];

            ParameterLength = parameters.Length;
            Parameters = new Type[ParameterLength];
            ParameterNullable = new bool[ParameterLength];

            for (var X = 0; X < ParameterLength; ++X)
            {
                Parameters[X] = parameters[X].ParameterType;
                ParameterNullable[X] = Nullable.GetUnderlyingType(Parameters[X]) is not null;
            }

            ConstructorDelegate = CreateConstructor(constructor, parameters);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Constructor"/> class.
        /// </summary>
        /// <param name="delegate">The delegate.</param>
        public Constructor(ConstructorDelegate @delegate)
        {
            ConstructorDelegate = @delegate;
        }

        /// <summary>
        /// Gets the size of the parameter.
        /// </summary>
        /// <value>The size of the parameter.</value>
        public int ParameterLength { get; } = 0;

        /// <summary>
        /// Gets the constructor information.
        /// </summary>
        /// <value>The constructor information.</value>
        internal ConstructorDelegate ConstructorDelegate { get; }

        /// <summary>
        /// Gets the parameter nullable.
        /// </summary>
        /// <value>The parameter nullable.</value>
        private bool[] ParameterNullable { get; } = [];

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        private Type[] Parameters { get; } = [];

        /// <summary>
        /// Create an instance.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The new object</returns>
        public object CreateInstance(object[] args) => ConstructorDelegate(args);

        /// <summary>
        /// Determines whether the specified arguments is a match.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if the specified arguments is a match; otherwise, <c>false</c>.</returns>
        public bool IsMatch(object[] args)
        {
            if (args is null || args.Length != ParameterLength)
                return false;
            for (var X = 0; X < ParameterLength; ++X)
            {
                Type ArgType = args[X]?.GetType();
                Type Parameter = Parameters[X];
                if (ArgType is null)
                {
                    if (ParameterNullable[X])
                        continue;
                    return false;
                }
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
        private static UnaryExpression CreateArgumentExpression(ParameterExpression parameterExpression, ParameterInfo parameterInfo, int index)
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
            ParameterExpression ParameterExpression = Expression.Parameter(typeof(object[]), "args");

            var ArgsExpressions = new Expression[parameters.Length];
            for (var X = 0; X < parameters.Length; ++X)
            {
                ArgsExpressions[X] = CreateArgumentExpression(ParameterExpression, parameters[X], X);
            }

            Expression NewExpression = Expression.New(constructor, ArgsExpressions);
            if (constructor.DeclaringType.IsValueType)
                NewExpression = Expression.Convert(NewExpression, typeof(object));

            return Expression.Lambda<ConstructorDelegate>(NewExpression, ParameterExpression).Compile();
        }
    }
}