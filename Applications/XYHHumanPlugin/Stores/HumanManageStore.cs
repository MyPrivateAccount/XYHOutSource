using System;
using System.Collections.Generic;
using System.Text;
using XYHHumanPlugin.Stores;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XYHHumanPlugin.Models;
using XYHHumanPlugin.Dto.Response;
using System.Linq;
using ApplicationCore.Models;
using Dapper;

namespace XYHHumanPlugin.Stores
{
    public class HumanManageStore : IHumanManageStore
    {

        public HumanManageStore(HumanDbContext hudb)
        {
            Context = hudb;
        }

        protected HumanDbContext Context { get; }

        public IEnumerable<T> DapperSelect<T>(string sql)
        {
            return Context.Database.GetDbConnection().Query<T>(sql);
        }

        public async Task<HumanInfo> CreateAsync(SimpleUser userinfo, HumanInfo humaninfo, string modifyid, string checkaction, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userinfo == null)
            {
                throw new ArgumentNullException(nameof(userinfo));
            }

            if (string.IsNullOrEmpty(modifyid))
            {
                modifyid = Guid.NewGuid().ToString();
            }

            var modify = new ModifyInfo();
            modify.ID = modifyid;
            modify.Type = 1;//创建
            modify.IDCard = modifyid;
            modify.ModifyPepole = userinfo.Id;
            modify.ModifyStartTime = DateTime.Now;
            modify.ExamineStatus = (int)ExamineStatusEnum.Auditing;
            modify.ExamineTime = modify.ModifyStartTime;
            modify.ModifyCheck = checkaction;

            humaninfo.CreateUser = userinfo.Id;
            humaninfo.Modify = 1;
            humaninfo.RecentModify = modifyid;
            humaninfo.CreateTime = DateTime.Now;
            Context.Add(humaninfo);
            Context.Add(modify);

            await Context.SaveChangesAsync(cancellationToken);
            return humaninfo;
        }

        public Task CreateAsync(AnnexInfo annexinfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (annexinfo == null)
            {
                throw new ArgumentNullException(nameof(annexinfo));
            }

            if (string.IsNullOrEmpty(annexinfo.ID))
            {
                annexinfo.ID = Guid.NewGuid().ToString();
            }

            Context.AnnexInfos.Add(annexinfo);
            return Context.SaveChangesAsync(cancellationToken);
        }

        public Task CreateAsync(FileInfo fileinfo, string tt, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fileinfo == null)
            {
                throw new ArgumentNullException(nameof(fileinfo));
            }


            Context.Add(fileinfo);
            return Context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateListAsync(List<FileInfo> fileInfoList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fileInfoList == null)
            {
                throw new ArgumentNullException(nameof(fileInfoList));
            }
            Context.AddRange(fileInfoList);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(HumanInfo userinfo, string contractid, CancellationToken cancellationToken = default(CancellationToken))
        { }

        public async Task DeleteListAsync(List<HumanInfo> buildingBaseList, CancellationToken cancellationToken = default(CancellationToken))
        { }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<HumanInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.HumanInfos.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<HumanInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.HumanInfos.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public Task<TResult> GetModifyAsync<TResult>(Func<IQueryable<ModifyInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ModifyInfos.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }
        public Task<List<TResult>> ListModifyAsync<TResult>(Func<IQueryable<ModifyInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ModifyInfos.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(HumanInfo buildingBase, CancellationToken cancellationToken = default(CancellationToken))
        {
        }

        public async Task UpdateListAsync(List<HumanInfo> buildingBaseList, CancellationToken cancellationToken = default(CancellationToken))
        { }

        public async Task SaveAsync(HumanInfo huinfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            
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
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { throw; }
        }
    }
}
