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
    public interface IHumanInfoLeaveStore
    {
        IQueryable<HumanInfoLeave> HumanInfoLeaves { get; set; }

        IQueryable<HumanInfoLeave> SimpleQuery();

        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        Task<HumanInfoLeave> CreateAsync(UserInfo user, HumanInfoLeave humanInfoLeave, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        Task<HumanInfoLeave> UpdateAsync(UserInfo user, HumanInfoLeave humanInfoLeave, CancellationToken cancellationToken = default(CancellationToken));




        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        Task DeleteAsync(UserInfo user, HumanInfoLeave humanInfoLeave, CancellationToken cancellationToken = default(CancellationToken));



        /// <summary>
        /// 查询单个
        /// </summary>
        /// <returns></returns>
        Task<TResult> GetAsync<TResult>(Func<IQueryable<HumanInfoLeave>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<HumanInfoLeave>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));


    }
}
