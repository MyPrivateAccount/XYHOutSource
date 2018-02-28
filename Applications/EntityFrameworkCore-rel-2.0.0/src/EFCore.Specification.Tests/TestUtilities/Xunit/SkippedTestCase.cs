﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Microsoft.EntityFrameworkCore.TestUtilities.Xunit
{
    internal class SkippedTestCase : XunitTestCase
    {
        private string _skipReason;

        public SkippedTestCase(string skipReason, IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay, ITestMethod testMethod, object[] testMethodArguments = null)
            : base(diagnosticMessageSink, defaultMethodDisplay, testMethod, testMethodArguments)
        {
            _skipReason = skipReason;
        }

        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public SkippedTestCase() : base()
        {
        }

        protected override string GetSkipReason(IAttributeInfo factAttribute)
            => _skipReason ?? base.GetSkipReason(factAttribute);

        public override void Deserialize(IXunitSerializationInfo data)
        {
            base.Deserialize(data);
            _skipReason = data.GetValue<string>(nameof(_skipReason));
        }

        public override void Serialize(IXunitSerializationInfo data)
        {
            base.Serialize(data);
            data.AddValue(nameof(_skipReason), _skipReason);
        }
    }
}
