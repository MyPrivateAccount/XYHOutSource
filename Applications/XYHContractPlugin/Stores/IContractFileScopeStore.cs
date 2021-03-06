﻿using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHContractPlugin.Models;

namespace XYHContractPlugin.Stores
{
    public interface IContractFileScopeStore
    {
        IQueryable<AnnexInfo> ContractAnnexs { get; set; }

        //Task<ContractFileScope> CreateAsync(ContractFileScope buildingFileScope, CancellationToken cancellationToken = default(CancellationToken));

        //Task DeleteAsync(ContractFileScope buildingFileScope, CancellationToken cancellationToken = default(CancellationToken));

        //Task DeleteListAsync(List<ContractFileScope> buildingFileScopeList, CancellationToken cancellationToken = default(CancellationToken));

        //Task<TResult> GetAsync<TResult>(Func<IQueryable<ContractFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<AnnexInfo> , IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        //Task UpdateAsync(ContractFileScope buildingFileScope, CancellationToken cancellationToken = default(CancellationToken));
        Task<TResult> GetAsync<TResult>(Func<IQueryable<AnnexInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
        Task UpdateListAsync(List<AnnexInfo> contractFileScopeList, CancellationToken cancellationToken = default(CancellationToken));

        Task SaveAsync(SimpleUser user, string buildingId, List<AnnexInfo> shopsFileScopeList, CancellationToken cancellationToken = default(CancellationToken));
    }
}
