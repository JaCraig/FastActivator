using System;
using System.Collections.Generic;

namespace Fast.Activator.Utils
{
    /// <summary>
    /// Default values
    /// </summary>
    internal static class DefaultValues
    {
        /// <summary>
        /// The values
        /// </summary>
        internal static readonly IReadOnlyDictionary<Type, object> Values = new Dictionary<Type, object>
        {
            [typeof(byte)] = default(byte),
            [typeof(sbyte)] = default(sbyte),
            [typeof(short)] = default(short),
            [typeof(int)] = default(int),
            [typeof(long)] = default(long),
            [typeof(ushort)] = default(ushort),
            [typeof(uint)] = default(uint),
            [typeof(ulong)] = default(ulong),
            [typeof(double)] = default(double),
            [typeof(float)] = default(float),
            [typeof(decimal)] = default(decimal),
            [typeof(bool)] = default(bool),
            [typeof(char)] = default(char),

            [typeof(byte?)] = default(byte?),
            [typeof(sbyte?)] = default(sbyte?),
            [typeof(short?)] = default(short?),
            [typeof(int?)] = default(int?),
            [typeof(long?)] = default(long?),
            [typeof(ushort?)] = default(ushort?),
            [typeof(uint?)] = default(uint?),
            [typeof(ulong?)] = default(ulong?),
            [typeof(double?)] = default(double?),
            [typeof(float?)] = default(float?),
            [typeof(decimal?)] = default(decimal?),
            [typeof(bool?)] = default(bool?),
            [typeof(char?)] = default(char?),

            [typeof(string)] = default(string),
            [typeof(Guid)] = default(Guid),
            [typeof(DateTime)] = default(DateTime),
            [typeof(DateOnly)] = default(DateOnly),
            [typeof(TimeOnly)] = default(TimeOnly),
            [typeof(DateTimeOffset)] = default(DateTimeOffset),
            [typeof(TimeSpan)] = default(TimeSpan),
            [typeof(Guid?)] = default(Guid?),
            [typeof(DateTime?)] = default(DateTime?),
            [typeof(DateTimeOffset?)] = default(DateTimeOffset?),
            [typeof(TimeSpan?)] = default(TimeSpan?),
            [typeof(byte[])] = default(byte[]),
        };
    }
}