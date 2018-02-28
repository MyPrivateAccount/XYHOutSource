// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class SqliteTypeMapper : RelationalTypeMapper
    {
        private const string _integerTypeName = "INTEGER";
        private const string _realTypeName = "REAL";
        private const string _blobTypeName = "BLOB";
        private const string _textTypeName = "TEXT";

        private static readonly LongTypeMapping _integer = new LongTypeMapping(_integerTypeName);
        private static readonly DoubleTypeMapping _real = new DoubleTypeMapping(_realTypeName);
        private static readonly ByteArrayTypeMapping _blob = new ByteArrayTypeMapping(_blobTypeName);
        private static readonly StringTypeMapping _text = new StringTypeMapping(_textTypeName);

        private readonly Dictionary<string, RelationalTypeMapping> _storeTypeMappings;
        private readonly Dictionary<Type, RelationalTypeMapping> _clrTypeMappings;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public SqliteTypeMapper([NotNull] RelationalTypeMapperDependencies dependencies)
            : base(dependencies)
        {
            _storeTypeMappings
                = new Dictionary<string, RelationalTypeMapping>(StringComparer.OrdinalIgnoreCase);

            _clrTypeMappings
                = new Dictionary<Type, RelationalTypeMapping>
                {
                    { typeof(string), _text },
                    { typeof(byte[]), _blob },
                    { typeof(bool), new BoolTypeMapping(_integerTypeName) },
                    { typeof(byte), new ByteTypeMapping(_integerTypeName) },
                    { typeof(char), new CharTypeMapping(_integerTypeName) },
                    { typeof(int), new IntTypeMapping(_integerTypeName) },
                    { typeof(long), _integer },
                    { typeof(sbyte), new SByteTypeMapping(_integerTypeName) },
                    { typeof(short), new ShortTypeMapping(_integerTypeName) },
                    { typeof(uint), new UIntTypeMapping(_integerTypeName) },
                    { typeof(ulong), new ULongTypeMapping(_integerTypeName) },
                    { typeof(ushort), new UShortTypeMapping(_integerTypeName) },
                    { typeof(DateTime), new SqliteDateTimeTypeMapping(_textTypeName) },
                    { typeof(DateTimeOffset), new SqliteDateTimeOffsetTypeMapping(_textTypeName) },
                    { typeof(TimeSpan), new TimeSpanTypeMapping(_textTypeName) },
                    { typeof(decimal), new DecimalTypeMapping(_textTypeName) },
                    { typeof(double), _real },
                    { typeof(float), new FloatTypeMapping(_realTypeName) },
                    { typeof(Guid), new SqliteGuidTypeMapping(_blobTypeName) }
                };
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override string GetColumnType(IProperty property) => property.Relational().ColumnType;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override RelationalTypeMapping FindMapping(string storeType)
        {
            Check.NotNull(storeType, nameof(storeType));

            if (storeType.Length == 0)
            {
                // This may seem odd, but it's okay because we are matching SQLite's loose typing.
                return _text;
            }

            foreach (var rules in _typeRules)
            {
                var mapping = rules(storeType);
                if (mapping != null)
                {
                    return mapping;
                }
            }

            return _text;
        }

        private readonly Func<string, RelationalTypeMapping>[] _typeRules =
        {
            name => Contains(name, "INT") ? _integer : null,
            name => Contains(name, "CHAR")
                    || Contains(name, "CLOB")
                    || Contains(name, "TEXT") ? _text : null,
            name => Contains(name, "BLOB") ? _blob : null,
            name => Contains(name, "REAL")
                    || Contains(name, "FLOA")
                    || Contains(name, "DOUB") ? _real : null
        };

        private static bool Contains(string haystack, string needle)
            => haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override IReadOnlyDictionary<Type, RelationalTypeMapping> GetClrTypeMappings()
            => _clrTypeMappings;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override IReadOnlyDictionary<string, RelationalTypeMapping> GetStoreTypeMappings()
            => _storeTypeMappings;
    }
}
