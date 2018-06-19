using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYHChargePlugin.Models;

namespace XYHChargePlugin.Stores
{
    public interface IChargeInfoStore
    {
        IQueryable<ChargeInfo> SimpleQuery { get; }

        Task<int> GetChargeNo(string branchId, DateTime time, int type);

        Task Save(SimpleUser user, ChargeInfo chargeInfo);
    }
}
