// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;

namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public static class DbParameterCollectionExtensions
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static string FormatParameters(
            [NotNull] this DbParameterCollection parameters,
            bool logParameterValues)
            => parameters
                .Cast<DbParameter>()
                .Select(
                    p => FormatParameter(
                        p.ParameterName,
                        logParameterValues ? p.Value : "?",
                        logParameterValues,
                        p.Direction,
                        p.DbType,
                        p.IsNullable,
                        p.Size,
                        p.Precision,
                        p.Scale))
                .Join();

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static string FormatParameter(
            [NotNull] string name,
            [CanBeNull] object value,
            bool hasValue,
            ParameterDirection direction,
            DbType dbType,
            bool nullable,
            int size,
            byte precision,
            byte scale)
        {
            var builder = new StringBuilder();

            var clrType = value?.GetType();

            builder
                .Append(name)
                .Append("=");

            FormatParameterValue(builder, value);

            if (nullable
                && value != null
                && !clrType.IsNullableType())
            {
                builder.Append(" (Nullable = true)");
            }
            else
            {
                if (!nullable
                    && hasValue
                    && (value == null
                        || clrType.IsNullableType()))
                {
                    builder.Append(" (Nullable = false)");
                }
            }

            if (size != 0)
            {
                builder
                    .Append(" (Size = ")
                    .Append(size.ToString(CultureInfo.InvariantCulture))
                    .Append(')');
            }

            if (precision != 0)
            {
                builder
                    .Append(" (Precision = ")
                    .Append(precision.ToString(CultureInfo.InvariantCulture))
                    .Append(')');
            }

            if (scale != 0)
            {
                builder
                    .Append(" (Scale = ")
                    .Append(scale.ToString(CultureInfo.InvariantCulture))
                    .Append(')');
            }

            if (direction != ParameterDirection.Input)
            {
                builder
                    .Append(" (Direction = ")
                    .Append(direction)
                    .Append(')');
            }

            if (hasValue
                && !IsNormalDbType(dbType, clrType))
            {
                builder
                    .Append(" (DbType = ")
                    .Append(dbType)
                    .Append(')');
            }

            return builder.ToString();
        }

        private static void FormatParameterValue(StringBuilder builder, object parameterValue)
        {
            builder.Append('\'');

            if (parameterValue?.GetType() != typeof(byte[]))
            {
                builder.Append(Convert.ToString(parameterValue, CultureInfo.InvariantCulture));
            }
            else
            {
                var buffer = (byte[])parameterValue;
                builder.Append("0x");

                for (var i = 0; i < buffer.Length; i++)
                {
                    if (i > 31)
                    {
                        builder.Append("...");
                        break;
                    }
                    builder.Append(buffer[i].ToString("X2", CultureInfo.InvariantCulture));
                }
            }

            builder.Append('\'');
        }

        private static bool IsNormalDbType(DbType dbType, Type clrType)
        {
            if (clrType == null)
            {
                return false;
            }

            clrType = clrType.UnwrapNullableType().UnwrapEnumType();

            switch (dbType)
            {
                case DbType.AnsiString: // Zero
                    return clrType != typeof(string);
                case DbType.Binary:
                    return clrType == typeof(byte[]);
                case DbType.Byte:
                    return clrType == typeof(byte);
                case DbType.Boolean:
                    return clrType == typeof(bool);
                case DbType.Decimal:
                    return clrType == typeof(decimal);
                case DbType.Double:
                    return clrType == typeof(double);
                case DbType.Guid:
                    return clrType == typeof(Guid);
                case DbType.Int16:
                    return clrType == typeof(short);
                case DbType.Int32:
                    return clrType == typeof(int);
                case DbType.Int64:
                    return clrType == typeof(long);
                case DbType.Object:
                    return clrType == typeof(object);
                case DbType.SByte:
                    return clrType == typeof(sbyte);
                case DbType.Single:
                    return clrType == typeof(float);
                case DbType.String:
                    return clrType == typeof(string);
                case DbType.Time:
                    return clrType == typeof(TimeSpan);
                case DbType.UInt16:
                    return clrType == typeof(ushort);
                case DbType.UInt32:
                    return clrType == typeof(uint);
                case DbType.UInt64:
                    return clrType == typeof(ulong);
                case DbType.DateTime2:
                    return clrType == typeof(DateTime);
                case DbType.DateTimeOffset:
                    return clrType == typeof(DateTimeOffset);
                //case DbType.VarNumeric:
                //case DbType.AnsiStringFixedLength:
                //case DbType.StringFixedLength:
                //case DbType.Xml:
                //case DbType.Currency:
                //case DbType.Date:
                //case DbType.DateTime:
                default:
                    return false;
            }
        }
    }
}
