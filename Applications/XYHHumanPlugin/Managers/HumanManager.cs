using System;
using System.Collections.Generic;
using System.Text;
using XYHHumanPlugin.Stores;

namespace XYHHumanPlugin.Managers
{
    public class HumanManager
    {
        public HumanManager(IHumanManageStore stor)
        {
            _Store = stor;
        }

        protected IHumanManageStore _Store { get; }
    }
}
