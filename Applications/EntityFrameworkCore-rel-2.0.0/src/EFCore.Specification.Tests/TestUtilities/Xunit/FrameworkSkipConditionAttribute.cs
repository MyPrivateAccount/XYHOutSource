// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.EntityFrameworkCore.TestUtilities.Xunit
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly)]
    public class FrameworkSkipConditionAttribute : Attribute, ITestCondition
    {
        private readonly RuntimeFrameworks _excludedFrameworks;

        public FrameworkSkipConditionAttribute(RuntimeFrameworks excludedFrameworks)
        {
            _excludedFrameworks = excludedFrameworks;
        }

        public bool IsMet => CanRunOnThisFramework(_excludedFrameworks);

        public string SkipReason { get; set; } = "Test cannot run on this runtime framework.";

        private static bool CanRunOnThisFramework(RuntimeFrameworks excludedFrameworks)
        {
            if (excludedFrameworks == RuntimeFrameworks.None)
            {
                return true;
            }

#if NET461
            if (excludedFrameworks.HasFlag(RuntimeFrameworks.CLR))
            {
                return false;
            }
#elif NETCOREAPP2_0
            if (excludedFrameworks.HasFlag(RuntimeFrameworks.CoreCLR))
            {
                return false;
            }
#else
#error target frameworks need to be updated.
#endif
            return true;
        }
    }
}
