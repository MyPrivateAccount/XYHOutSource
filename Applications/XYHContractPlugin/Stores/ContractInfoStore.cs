﻿using System;
using System.Collections.Generic;
using System.Text;
using XYHContractPlugin.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Models;

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

        public async Task<ContractInfo> CreateAsync(ContractInfo buildingBaseInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfo == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfo));
            }
            Context.Add(buildingBaseInfo);
            await Context.SaveChangesAsync(cancellationToken);
            return buildingBaseInfo;
        }

        public async Task DeleteAsync(ContractInfo areaDefine, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (areaDefine == null)
            {
                throw new ArgumentNullException(nameof(areaDefine));
            }
            Context.Remove(areaDefine);
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

            //查看楼盘是否存在
            if (!Context.ContractInfos.Any(x => x.ID == contractinfo.ID))
            {
                ContractInfo buildings = new ContractInfo()
                {
                    ID = contractinfo.ID,
                    CREATEUSER = user.Id,
                    CREATETIME = DateTime.Now,
                    ISDELETE = false
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
    }
}