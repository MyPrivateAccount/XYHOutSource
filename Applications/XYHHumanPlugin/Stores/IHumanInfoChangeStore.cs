using ApplicationCore.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHHumanPlugin.Models;

namespace XYHHumanPlugin.Stores
{
    public interface IHumanInfoChangeStore
    {
        IQueryable<HumanInfoChange> HumanInfoChanges { get; set; }


        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        Task<HumanInfoChange> CreateAsync(UserInfo user, HumanInfoChange humanInfoChange, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        Task<HumanInfoChange> UpdateAsync(UserInfo user, HumanInfoChange humanInfoChange, CancellationToken cancellationToken = default(CancellationToken));




        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        Task DeleteAsync(UserInfo user, HumanInfoChange humanInfoChange, CancellationToken cancellationToken = default(CancellationToken));



        /// <summary>
        /// 查询单个
        /// </summary>
        /// <returns></returns>
        Task<TResult> GetAsync<TResult>(Func<IQueryable<HumanInfoChange>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<HumanInfoChange>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

    }
}
