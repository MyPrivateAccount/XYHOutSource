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

        Task<int> GetChargeNo(string branchId,string prefix, DateTime time, int type);

        Task Save(SimpleUser user, ChargeInfo chargeInfo);

        Task<ChargeInfo> GetDetail(SimpleUser user, string id);
    }
}
