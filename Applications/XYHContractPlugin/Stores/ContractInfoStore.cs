using System;
using System.Collections.Generic;
using System.Text;
using XYHContractPlugin.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Models;
using XYHContractPlugin.Dto.Response;

namespace XYHContractPlugin.Stores
{
    public class ContractInfoStore : IContractInfoStore
    {
        public ContractInfoStore(ContractDbContext contractdbcont)
        {
            Context = contractdbcont;
            ContractInfos = Context.ContractInfos;
        }

        protected ContractDbContext Context { get; }
        public IQueryable<ContractInfo> ContractInfos { get; set; }

        public IEnumerable<T> DapperSelect<T>(string sql)
        {
            return Context.Database.GetDbConnection().Query<T>(sql);
        }

        public async Task<ContractInfo> CreateAsync(SimpleUser userinfo, ContractInfo buildingBaseInfo, string modifyid, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfo == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfo));
            }
            var modify = new ModifyInfo();
            modify.ID = modifyid;
            modify.Type = 1;//创建
            modify.ContractID = buildingBaseInfo.ID;
            modify.ModifyPepole = userinfo.Id;
            modify.ModifyStartTime = DateTime.Now;
            modify.ExamineStatus = (int)ExamineStatusEnum.UnSubmit;
            modify.ExamineTime = modify.ModifyStartTime;
            modify.ModifyCheck = "0";

            buildingBaseInfo.IsDelete = false;
            buildingBaseInfo.CreateUser = userinfo.Id;
            buildingBaseInfo.CreateDepartment = userinfo.OrganizationName;
            buildingBaseInfo.Modify = 1;
            buildingBaseInfo.CurrentModify = modifyid;
            buildingBaseInfo.CreateTime = DateTime.Now;
            Context.Add(buildingBaseInfo);
            Context.Add(modify);
            await Context.SaveChangesAsync(cancellationToken);
            return buildingBaseInfo;
        }

        public async Task<bool> CreateAsync(SimpleUser userinfo, List<AnnexInfo> annexinfo, CancellationToken cancle = default(CancellationToken))
        {
            if (annexinfo == null)
            {
                throw new ArgumentNullException(nameof(annexinfo));
            }

            if (annexinfo.Count > 0)
            {
                Context.AddRange(annexinfo);
                await Context.SaveChangesAsync(cancle);
            }
            
            return true;
        }

        public async Task<bool> CreateAsync(SimpleUser userinfo, List<ComplementInfo> compleinfo, CancellationToken cancle = default(CancellationToken))
        {
            if (compleinfo == null)
            {
                throw new ArgumentNullException(nameof(compleinfo));
            }

            if (compleinfo.Count > 0)
            {
                Context.AddRange(compleinfo);
                await Context.SaveChangesAsync(cancle);
            }
            
            return true;
        }

        public async Task DeleteAsync(SimpleUser userinfo, string contractid, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (contractid == null)
            {
                throw new ArgumentNullException(nameof(contractid));
            }
            //Context.Remove(areaDefine);
            ContractInfo buildings = new ContractInfo()
            {
                ID = contractid,
                DeleteUser = userinfo.Id,
                DeleteTime = DateTime.Now,
                IsDelete = true
            };

            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<ContractInfo> areaDefineList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (areaDefineList == null)
            {
                throw new ArgumentNullException(nameof(areaDefineList));
            }
            Context.RemoveRange(areaDefineList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<ContractInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ContractInfos.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<ContractInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ContractInfos.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public Task<TResult> GetModifyAsync<TResult>(Func<IQueryable<ModifyInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ModifyInfos.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        public Task<List<TResult>> GetListModifyAsync<TResult>(Func<IQueryable<ModifyInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ModifyInfos.AsNoTracking()).ToListAsync(cancellationToken);
        }

       public Task<List<TResult>> GetListAnnexAsync<TResult>(Func<IQueryable<AnnexInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.AnnexInfos.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public Task<List<TResult>> GetListComplementAsync<TResult>(Func<IQueryable<ComplementInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ComplementInfos.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(ContractInfo areaDefine, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (areaDefine == null)
            {
                throw new ArgumentNullException(nameof(areaDefine));
            }
            Context.ContractInfos.Attach(areaDefine);
            Context.ContractInfos.Update(areaDefine);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { throw; }
        }

        public async Task UpdateListAsync(List<ContractInfo> areaDefineList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (areaDefineList == null)
            {
                throw new ArgumentNullException(nameof(areaDefineList));
            }
            Context.AttachRange(areaDefineList);
            Context.UpdateRange(areaDefineList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        public async Task SaveAsync(SimpleUser user, ContractInfo contractinfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (contractinfo == null)
            {
                throw new ArgumentNullException(nameof(contractinfo));
            }

            //查看合同是否存在
            if (!Context.ContractInfos.Any(x => x.ID == contractinfo.ID))
            {
                ContractInfo buildings = new ContractInfo()
                {
                    ID = contractinfo.ID,
                    CreateUser = user.Id,
                    CreateTime = DateTime.Now,
                    IsDelete = false
                };

                Context.Add(buildings);
            }
            else
                Context.Update(contractinfo);

            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        public async Task UpdateExamineStatus(string modifyId, ExamineStatusEnum status, CancellationToken cancellationToken = default(CancellationToken))
        {
            ModifyInfo buildings = new ModifyInfo()
            {
                ID = modifyId,
                ExamineTime = DateTime.Now,
                ExamineStatus = (int)status
            };
            var entry = Context.Attach(buildings);
            //var entry = Context.Entry(buildings);
            entry.Property(x => x.ExamineStatus).IsModified = true;
            entry.Property(x => x.ExamineTime).IsModified = true;
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { throw; }
        }
    }
}
