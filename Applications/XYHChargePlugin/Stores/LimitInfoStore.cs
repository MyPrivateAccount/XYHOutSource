using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Models;
using XYHChargePlugin.Models;
using Microsoft.EntityFrameworkCore;

namespace XYHChargePlugin.Stores
{
    public class LimitInfoStore : ILimitInfoStore
    {
        public LimitInfoStore(ChargeDbContext hudb)
        {
            Context = hudb;
        }

        protected ChargeDbContext Context { get; }


        public IQueryable<LimitInfo> SimpleQuery
        {
            get
            {
                var query = from l in Context.LimitInfos.AsNoTracking()
                            join ru1 in Context.HumanInfo.AsNoTracking() on l.UserId equals ru1.ID into ru2
                            from ru in ru2.DefaultIfEmpty()
                            join oe1 in Context.OrganizationExpansions.AsNoTracking() on new { ru.DepartmentId, Type = "Region" } equals new { DepartmentId = oe1.SonId, oe1.Type } into oe2
                            from oe in oe2.DefaultIfEmpty()
                            join o1 in Context.Organizations.AsNoTracking() on ru.DepartmentId equals o1.Id into o2
                            from o in o2.DefaultIfEmpty()
                            join p1 in Context.PositionInfo.AsNoTracking() on ru.Position equals p1.ID into p2
                            from p in p2.DefaultIfEmpty()
                            orderby l.CreateTime descending
                            select new LimitInfo() {
                                 Amount = l.Amount,
                                 CreateTime = l.CreateTime,
                                 CreateUser = l.CreateUser,
                                 UpdateTime = l.UpdateTime,
                                 UpdateUser = l.UpdateUser,
                                 IsDeleted = l.IsDeleted,
                                 DeleteTime = l.DeleteTime,
                                 DeleteUser = l.DeleteUser,
                                 LimitType = l.LimitType,
                                 Memo = l.Memo,
                                 UserId = l.UserId,
                                 UserInfo  = new HumanInfo()
                                 {
                                     ID = l.UserId,
                                     Name = ru.Name,
                                     DepartmentId = ru.DepartmentId,
                                     UserID = ru.UserID,
                                     Position = ru.Position,
                                     PositionInfo = new PositionInfo()
                                     {
                                         ID = ru.Position,
                                         PositionName = p.PositionName,
                                         PositionType = p.PositionType
                                     }
                                 },
                                OrganizationExpansion = new OrganizationExpansion()
                                {
                                    FullName = oe.FullName,
                                    SonId = oe.SonId
                                },
                                Organizations = new Organizations()
                                {
                                    Id = o.Id,
                                    OrganizationName = o.OrganizationName
                                }
                            };


                return query;
            }
        }

        public async Task Delete(SimpleUser user, string userId)
        {
            var old = await Context.LimitInfos.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            if (old != null)
            {
                old.IsDeleted = true;
                old.DeleteTime = DateTime.Now;
                old.DeleteUser = user.Id;
            }
            await Context.SaveChangesAsync();
        }

        public async Task<LimitInfo> Get(SimpleUser user, string userId)
        {
            var query = this.SimpleQuery;
            query = query.Where(x => x.UserId == userId);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<LimitInfo> Save(SimpleUser user, LimitInfo li)
        {
            var old = await Context.LimitInfos.Where(x => x.UserId == li.UserId).FirstOrDefaultAsync();
            if (old != null)
            {
                old.IsDeleted = li.IsDeleted;
                old.Amount = li.Amount;
                old.Memo = li.Memo;
                old.LimitType = li.LimitType;
                old.UpdateTime = DateTime.Now;
                old.UpdateUser = user.Id;
            }
            else
            {
                //新增
                li.CreateTime = DateTime.Now;
                li.CreateUser = user.Id;
                Context.LimitInfos.Add(li);
            }

            await Context.SaveChangesAsync();

            return li;
        }

        public async Task<HumanInfo> GetUserInfo(string userId)
        {
            return await Context.HumanInfo.AsNoTracking().Where(x => x.ID == userId).FirstOrDefaultAsync();
        }

        public async Task<LimitInfo> GetSimple(string userId)
        {
            return await Context.LimitInfos.AsNoTracking().Where(x => x.UserId == userId).FirstOrDefaultAsync();
        }
    }
}
