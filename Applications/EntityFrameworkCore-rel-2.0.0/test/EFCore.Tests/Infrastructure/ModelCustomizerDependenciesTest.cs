// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Xunit;

namespace Microsoft.EntityFrameworkCore.Infrastructure
{
    public class ModelCustomizerDependenciesTest
    {
        [Fact]
        public void Can_use_With_methods_to_clone_and_replace_service()
        {
            InMemoryTestHelpers.Instance.TestDependenciesClone<ModelCustomizerDependencies>();
        }
    }
}
