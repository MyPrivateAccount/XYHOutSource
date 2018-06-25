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

        IQueryable<CostInfo> CostQuery { get; }

        Task<int> GetChargeNo(string branchId,string prefix, DateTime time, int type);

        Task<int> GetPaymentNo(string branchId, string prefix, DateTime time);

        Task Save(SimpleUser user, ChargeInfo chargeInfo);

        Task<ChargeInfo> GetDetail(SimpleUser user, string id);

        Task<ChargeInfo> Get(string id);

        Task DeleteCharge(SimpleUser user, string id);

        Task UpdateStatus(SimpleUser user, string id, int status, string message, ModifyTypeEnum mtype, string mtypememo);

        Task UpdateBillStatus(SimpleUser user, string id, int status, string message, ModifyTypeEnum mtype, string mtypememo);

        Task<ChargeInfo> BackupBill(SimpleUser user, ChargeInfo chargeInfo);

        Task<PaymentInfo> Payment(SimpleUser user, PaymentInfo payment);

        Task<LimitTipInfo> GetLimitTip(string userId, DateTime startTime, DateTime endTime, string ignoreId);


        Task<List<CostInfo>> GetBillList(string chargeId);

        Task<List<DictionaryDefine>> GetDictionaryDefine(string groupId);
    }
}
