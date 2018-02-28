using System;
using System.Collections.Generic;

namespace E2E.Sqlite
{
    public partial class SelfRef
    {
        public SelfRef()
        {
            InverseSelfForeignKeyNavigation = new HashSet<SelfRef>();
        }

        public long Id { get; set; }
        public long? SelfForeignKey { get; set; }

        public SelfRef SelfForeignKeyNavigation { get; set; }
        public ICollection<SelfRef> InverseSelfForeignKeyNavigation { get; set; }
    }
}
