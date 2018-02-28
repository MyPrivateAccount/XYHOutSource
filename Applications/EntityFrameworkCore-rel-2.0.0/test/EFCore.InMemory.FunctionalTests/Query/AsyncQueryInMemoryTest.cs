// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class AsyncQueryInMemoryTest : AsyncQueryTestBase<NorthwindQueryInMemoryFixture>
    {
        public AsyncQueryInMemoryTest(NorthwindQueryInMemoryFixture fixture)
            : base(fixture)
        {
        }

        public override Task ToList_on_nav_in_projection_is_async()
        {
            // Not valid for in-memory.

            return null;
        }
    }
}
