using System;
using System.Collections.Generic;
using System.Text;
using XYHHumanPlugin.Stores;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ApplicationCore.Models;
using Dapper;
using XYHChargePlugin.Stores;
using XYHChargePlugin.Models;
using XYHChargePlugin.Dto.Response;

namespace XYHHumanPlugin.Stores
{
    public class ChargeManageStore : IChargeManageStore
    {

        public ChargeManageStore(ChargeDbContext hudb)
        {
            Context = hudb;
        }

        protected ChargeDbContext Context { get; }

        public IEnumerable<T> DapperSelect<T>(string sql)
        {
            return Context.Database.GetDbConnection().Query<T>(sql);
        }

        public async Task<ChargeInfo> CreateChargeAsync(SimpleUser userinfo, ChargeInfo chargeinfo, string modifyid, string checkaction, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (chargeinfo == null)
            {
                throw new ArgumentNullException(nameof(chargeinfo));
            }

            if (string.IsNullOrEmpty(modifyid))
            {
                modifyid = Guid.NewGuid().ToString();
            }

            if (string.IsNullOrEmpty(chargeinfo.ID))
            {
                chargeinfo.ID = Guid.NewGuid().ToString();
            }

            var modify = new ModifyInfo();
            modify.ID = modifyid;
            modify.Type = 1;//创建
            modify.ChargeID = modifyid;
            modify.ModifyPepole = userinfo.Id;
            modify.ModifyStartTime = DateTime.Now;
            modify.ExamineStatus = (int)ExamineStatusEnum.Auditing;
            modify.ExamineTime = modify.ModifyStartTime;
            modify.ModifyCheck = checkaction;

            chargeinfo.CreateUser = userinfo.Id;
            chargeinfo.CreateUserName = userinfo.UserName;
            chargeinfo.Department = userinfo.OrganizationId;
            chargeinfo.PostTime = DateTime.Now;

            Context.Add(modify);
            Context.Add(chargeinfo);
            await Context.SaveChangesAsync(cancellationToken);
            return chargeinfo;
        }

        public Task CreateCostListAsync(List<CostInfo> costinfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (costinfo == null)
            {
                throw new ArgumentNullException(nameof(costinfo));
            }

            foreach (var item in costinfo)
            {
                if (string.IsNullOrEmpty(item.ID))
                {
                    item.ID = Guid.NewGuid().ToString();
                }
            }

            Context.AddRange(costinfo);
            return Context.SaveChangesAsync();
        }

        public Task CreateReceiptListAsync(List<ReceiptInfo> costinfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (costinfo == null)
            {
                throw new ArgumentNullException(nameof(costinfo));
            }

            foreach (var item in costinfo)
            {
                if (string.IsNullOrEmpty(item.ID))
                {
                    item.ID = Guid.NewGuid().ToString();
                }
            }

            Context.AddRange(costinfo);
            return Context.SaveChangesAsync();
        }

        public Task CreateFileListAsync(List<FileInfo> costinfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (costinfo == null)
            {
                throw new ArgumentNullException(nameof(costinfo));
            }
            Context.AddRange(costinfo);
            return Context.SaveChangesAsync(cancellationToken);
        }

        public Task CreateFileScopeAsync(string strid, List<FileScopeInfo> scope, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }
            foreach (var item in scope)
            {
                item.ReceiptID = strid;
            }

            Context.AddRange(scope);
            return Context.SaveChangesAsync(cancellationToken);
        }
        //public Task DeleteAsync(ChargeInfo userinfo, string contractid, CancellationToken cancellationToken = default(CancellationToken))
        //{ }

        public Task<TResult> GetChargeAsync<TResult>(Func<IQueryable<ChargeInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ChargeInfos.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        public Task<List<TResult>> GetRecieptListAsync<TResult>(Func<IQueryable<ReceiptInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ReceiptInfos.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public async Task UpdatePostTime(string chargeid, string department, CancellationToken cancellationToken = default(CancellationToken))
        {
            ChargeInfo info = new ChargeInfo()
            {
                ID = chargeid,
                PostTime = DateTime.Now,
                PostDepartment = department
            };

            Context.Attach(info);
            var entry = Context.Entry(info);
            entry.Property(x => x.PostTime).IsModified = true;
            entry.Property(x => x.PostDepartment).IsModified = true;
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateRecieptList(List<ReceiptInfo> lst, CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var item in lst)
            {
                if (Context.ReceiptInfos.Any(x=>x.ID == item.ID))
                {
                    Context.Attach(item);
                    Context.Update(item);
                }
                else
                {
                    Context.Add(item);
                }
            }

            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task SetLimit(string userid, int limit, CancellationToken cancellationToken = default(CancellationToken))
        {
            LimitInfo var = new LimitInfo();
            var.UserID = userid;
            var.LimitType = 0;
            var.CostLimit = limit;

            if (Context.LimitInfos.Any(x=> x.UserID == userid))
            {
                Context.Attach(var);
                Context.Update(var);
            }
            else
            {
                Context.Add(var);
            }

            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateExamineStatus(string modifyId, ExamineStatusEnum status, int type, CancellationToken cancellationToken = default(CancellationToken))
        {
            ModifyInfo buildings = new ModifyInfo()
            {
                ID = modifyId,
                ExamineTime = DateTime.Now,
                ExamineStatus = (int)status,
                Type = type
            };
            Context.Attach(buildings);
            var entry = Context.Entry(buildings);
            entry.Property(x => x.ExamineStatus).IsModified = true;
            entry.Property(x => x.Type).IsModified = true;
            entry.Property(x => x.ExamineTime).IsModified = true;
            await Context.SaveChangesAsync(cancellationToken);
        }

    }
}
