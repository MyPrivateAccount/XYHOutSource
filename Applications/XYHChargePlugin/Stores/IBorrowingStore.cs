using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYHChargePlugin.Models;

namespace XYHChargePlugin.Stores
{
    public interface IBorrowingStore
    {
        IQueryable<ChargeInfo> SimpleQuery { get; }


        Task<int> GetChargeNo(string branchId, string prefix, DateTime time, int type);

        Task Save(SimpleUser user, ChargeInfo chargeInfo);

        Task<ChargeInfo> GetDetail(SimpleUser user, string id);

        Task<ChargeInfo> Get(string id);

        Task DeleteBorrowing(SimpleUser user, string id);

        Task UpdateStatus(SimpleUser user, string id, int status, string message, ModifyTypeEnum mtype, string mtypememo);

        Task<LimitTipInfo> UpdateReimbursedAmount(SimpleUser user, ChargeInfo bill);
    }
}
