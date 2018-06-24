using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYHChargePlugin.Models;

namespace XYHChargePlugin.Stores
{
    public interface ILimitInfoStore
    {
        IQueryable<LimitInfo> SimpleQuery { get; }

        Task<LimitInfo> Save(SimpleUser user, LimitInfo li);

        Task<LimitInfo> Get(SimpleUser user, string userId);

        Task<LimitInfo> GetSimple(string userId);

        Task Delete(SimpleUser user, string userId);

        Task<HumanInfo> GetUserInfo(string userId);
    }
}
