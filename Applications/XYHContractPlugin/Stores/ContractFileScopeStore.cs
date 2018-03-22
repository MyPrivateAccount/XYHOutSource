using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHContractPlugin.Models;

namespace XYHContractPlugin.Stores
{
    public class ContractFileScopeStore : IContractFileScopeStore
    {
        
        public ContractFileScopeStore(ContractDbContext contractDbContext)
        {
            Context = contractDbContext;
            ContractAnnexs = Context.AnnexInfos;
        }
      
        protected ContractDbContext Context { get; }
        public IQueryable<AnnexInfo> ContractAnnexs { get; set; }
      
        public async Task SaveAsync(SimpleUser user, string buildingId, List<AnnexInfo> contractFileScopeList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (contractFileScopeList == null || contractFileScopeList.Count == 0)
                return;

            foreach (AnnexInfo file in contractFileScopeList)
            {
                if (String.IsNullOrEmpty(file.FileGuid))
                {
                    file.FileGuid = Guid.NewGuid().ToString("N").ToLower();
                }
                //查看合同是否存在
                if (!Context.ContractInfos.Any(x => x.ID == file.ContractID))
                {
                    return;
                }
                //附件基本信息
                if (!Context.AnnexInfos.Any(x => x.FileGuid == file.FileGuid))
                {
                    file.CreateTime = DateTime.Now;
                    file.CreateUser = user.Id;
                    Context.Add(file);
                }
                else
                {
                    Context.Attach(file);
                    Context.Update(file);
                }
            }
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<AnnexInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.AnnexInfos).ToListAsync(cancellationToken);
        }
        public async Task UpdateListAsync(List<AnnexInfo> contractFileScopeList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (contractFileScopeList == null)
            {
                throw new ArgumentNullException(nameof(contractFileScopeList));
            }
            Context.AttachRange(contractFileScopeList);
            Context.UpdateRange(contractFileScopeList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

    }
}
