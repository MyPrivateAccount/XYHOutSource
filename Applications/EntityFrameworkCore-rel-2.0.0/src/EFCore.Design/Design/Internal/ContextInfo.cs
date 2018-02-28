// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Design.Internal
{
    public class ContextInfo
    {
        public virtual string ProviderName { get; [param: NotNull] set; }
        public virtual string DatabaseName { get; [param: NotNull] set; }
        public virtual string DataSource { get; [param: NotNull] set; }
        public virtual string Options { get; [param: NotNull] set; }
    }
}
