// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public static class ScaffoldingMetadataExtensions
    {
        public static ScaffoldingModelAnnotations Scaffolding([NotNull] this IModel model)
            => new ScaffoldingModelAnnotations(Check.NotNull(model, nameof(model)));

        public static ScaffoldingPropertyAnnotations Scaffolding([NotNull] this IProperty property)
            => new ScaffoldingPropertyAnnotations(Check.NotNull(property, nameof(property)));

        public static ScaffoldingEntityTypeAnnotations Scaffolding([NotNull] this IEntityType entityType)
            => new ScaffoldingEntityTypeAnnotations(Check.NotNull(entityType, nameof(entityType)));
    }
}
