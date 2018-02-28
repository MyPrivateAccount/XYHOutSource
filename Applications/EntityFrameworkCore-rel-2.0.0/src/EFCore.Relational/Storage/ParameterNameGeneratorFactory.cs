// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Storage
{
    /// <summary>
    ///     <para>
    ///         Creates instances of the <see cref="ParameterNameGenerator" /> type.
    ///     </para>
    ///     <para>
    ///         This type is typically used by database providers (and other extensions). It is generally
    ///         not used in application code.
    ///     </para>
    /// </summary>
    public class ParameterNameGeneratorFactory : IParameterNameGeneratorFactory
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ParameterNameGeneratorFactory" /> class.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this service. </param>
        public ParameterNameGeneratorFactory([NotNull] ParameterNameGeneratorDependencies dependencies)
        {
            Check.NotNull(dependencies, nameof(dependencies));
        }

        /// <summary>
        ///     Creates a new <see cref="ParameterNameGenerator" />.
        /// </summary>
        /// <returns> The newly created generator. </returns>
        public virtual ParameterNameGenerator Create() => new ParameterNameGenerator();
    }
}
