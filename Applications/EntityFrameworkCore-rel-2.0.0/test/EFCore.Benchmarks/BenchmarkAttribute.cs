// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Xunit;
using Xunit.Sdk;

namespace Microsoft.EntityFrameworkCore.Benchmarks
{
    [XunitTestCaseDiscoverer("Microsoft.EntityFrameworkCore.Benchmarks.BenchmarkTestCaseDiscoverer", "Microsoft.EntityFrameworkCore.Benchmarks")]
    public class BenchmarkAttribute : FactAttribute
    {
        public int Iterations { get; set; } = 100;
        public int WarmupIterations { get; set; } = 1;
    }
}
