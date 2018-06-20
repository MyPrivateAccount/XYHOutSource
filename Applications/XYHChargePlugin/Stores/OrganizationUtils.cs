using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using XYHChargePlugin.Models;
using System.Linq;

namespace XYHChargePlugin.Stores
{
    public class OrganizationUtils : IOrganizationUtils
    {
        class OrgPar
        {
            public string OrganizationId { get; set; }

            public string ParentId { get; set; }

            public string ParValue { get; set; }

            public OrgPar Parent { get; set; }
        }

        public OrganizationUtils(ChargeDbContext hudb)
        {
            Context = hudb;
        }

        protected ChargeDbContext Context { get; }

        public async Task<string> GetBranchPrefix(string branchId, string defaultPrefix)
        {
            string prefix = defaultPrefix;

            var bq = from oe in Context.OrganizationExpansions.AsNoTracking()
                     join o in Context.Organizations.AsNoTracking() on oe.OrganizationId equals o.Id
                     join op1 in Context.OrganizationPars.AsNoTracking() on new { o.Id, ParGroup = "ORG_PREFIX_CODE" } equals new { Id = op1.OrganizationId, op1.ParGroup } into op2
                     from op in op2.DefaultIfEmpty()
                     where oe.SonId == branchId
                     select new OrgPar
                     {
                         OrganizationId = oe.OrganizationId,
                         ParentId = o.ParentId,
                         ParValue = op.ParValue1
                     };
            var blist = await bq.ToListAsync();
            if (blist.Count > 0)
            {
                blist.ForEach(x =>
                {
                    x.Parent = blist.FirstOrDefault(y => y.OrganizationId == x.ParentId);
                });
                List<OrgPar> levelList = new List<OrgPar>();
                List<OrgPar> parents = blist.Where(x => x.Parent == null).ToList();
                levelList.AddRange(parents);
                while (parents.Count > 0)
                {
                    parents = blist.Where(x => parents.Contains(x.Parent)).ToList();
                    levelList.AddRange(parents);
                }

                for (int i = levelList.Count - 1; i >= 0; i--)
                {
                    var item = levelList[i];
                    if (!String.IsNullOrEmpty(item.ParValue))
                    {
                        prefix = item.ParValue;
                        break;
                    }
                }
            }

            return prefix;
        }

        public async Task<Organizations> GetNearParent(string branchId, List<string> orders)
        {
            var q = from oe in Context.OrganizationExpansions.AsNoTracking()
                    join o in Context.Organizations.AsNoTracking() on oe.OrganizationId equals o.Id
                    where oe.SonId == branchId && orders.Contains(oe.Type)
                    select new Organizations()
                    {
                        Id = o.Id,
                        ParentId = o.ParentId,
                        OrganizationName = o.OrganizationName,
                        Type = o.Type
                    };
            var list = await q.ToListAsync();
            var orgItem = await Context.Organizations.AsNoTracking().Where(x => x.Id == branchId).FirstOrDefaultAsync();
            if (orgItem != null)
            {
                list.Add(orgItem);
            }

            Organizations item = null;
            foreach(string t in orders)
            {
                item = list.FirstOrDefault(x => x.Type == t);
                if (item != null)
                {
                    break;
                }
            }

            return item;

        }
    }
}
