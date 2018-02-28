// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit.Abstractions;

namespace Microsoft.EntityFrameworkCore.Benchmarks
{
    public class NullMetricCollector : MarshalByRefObject, IMetricCollector
    {
        private readonly Scope _scope;

        public NullMetricCollector()
        {
            _scope = new Scope();
        }

        public IDisposable StartCollection() => _scope;

        public void StopCollection()
        {
        }

        public void Reset()
        {
        }

        public void Deserialize(IXunitSerializationInfo info)
        { }

        public void Serialize(IXunitSerializationInfo info)
        { }

        public long TimeElapsed => 0;

        public long MemoryDelta => 0;

        private class Scope : MarshalByRefObject, IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
