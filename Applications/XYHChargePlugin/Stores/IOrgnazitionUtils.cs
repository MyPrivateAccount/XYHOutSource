using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XYHChargePlugin.Stores
{
    public interface IOrganizationUtils
    {
        Task<string> GetBranchPrefix(string branchId, string defaultPrefix);

        Task<Organizations> GetNearParent(string branchId, List<string> orders);
    }
}
