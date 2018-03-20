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
            ContractFileScopes = Context.ContractFileScopes;
        }
      
        protected ContractDbContext Context { get; }
        public IQueryable<ContractFileScope> ContractFileScopes { get; set; }
       /*
      public async Task<BuildingFileScope> CreateAsync(BuildingFileScope buildingFileScope, CancellationToken cancellationToken = default(CancellationToken))
      {
          if (buildingFileScope == null)
          {
              throw new ArgumentNullException(nameof(buildingFileScope));
          }
          Context.Add(buildingFileScope);
          await Context.SaveChangesAsync(cancellationToken);
          return buildingFileScope;
      }


      public async Task DeleteAsync(BuildingFileScope buildingFileScope, CancellationToken cancellationToken = default(CancellationToken))
      {
          if (buildingFileScope == null)
          {
              throw new ArgumentNullException(nameof(buildingFileScope));
          }
          Context.Remove(buildingFileScope);
          try
          {
              await Context.SaveChangesAsync(cancellationToken);
          }
          catch (DbUpdateConcurrencyException)
          {
              throw;
          }
      }

      public async Task DeleteListAsync(List<BuildingFileScope> buildingFileScopeList, CancellationToken cancellationToken = default(CancellationToken))
      {
          if (buildingFileScopeList == null)
          {
              throw new ArgumentNullException(nameof(buildingFileScopeList));
          }
          Context.RemoveRange(buildingFileScopeList);
          try
          {
              await Context.SaveChangesAsync(cancellationToken);
          }
          catch (DbUpdateConcurrencyException)
          {
              throw;
          }
      }

      public Task<TResult> GetAsync<TResult>(Func<IQueryable<BuildingFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
      {
          if (query == null)
          {
              throw new ArgumentNullException(nameof(query));
          }
          return query.Invoke(Context.BuildingFileScopes).SingleOrDefaultAsync(cancellationToken);
      }



      public async Task UpdateAsync(BuildingFileScope shopsFileScope, CancellationToken cancellationToken = default(CancellationToken))
      {
          if (shopsFileScope == null)
          {
              throw new ArgumentNullException(nameof(shopsFileScope));
          }
          Context.Attach(shopsFileScope);
          Context.Update(shopsFileScope);
          try
          {
              await Context.SaveChangesAsync(cancellationToken);
          }
          catch (DbUpdateConcurrencyException)
          {
              throw;
          }
      }



  */
        public async Task SaveAsync(SimpleUser user, string buildingId, List<ContractFileScope> contractFileScopeList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (contractFileScopeList == null || contractFileScopeList.Count == 0)
                return;

            foreach (ContractFileScope file in contractFileScopeList)
            {
                if (String.IsNullOrEmpty(file.FileGuid))
                {
                    file.FileGuid = Guid.NewGuid().ToString("N").ToLower();
                }
                //查看合同是否存在
                if (!Context.ContractInfos.Any(x => x.ID == file.ContractId))
                {
                    return;
                }
                //附件基本信息
                if (!Context.ContractFileScopes.Any(x => x.FileGuid == file.FileGuid))
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
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<ContractFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ContractFileScopes).ToListAsync(cancellationToken);
        }
        public async Task UpdateListAsync(List<ContractFileScope> contractFileScopeList, CancellationToken cancellationToken = default(CancellationToken))
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
