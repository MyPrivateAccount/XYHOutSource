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
    public interface IHumanInfoPartPositionStore
    {
        IQueryable<HumanInfoPartPosition> HumanInfoPartPostions { get; set; }


        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        Task<HumanInfoPartPosition> CreateAsync(UserInfo user, HumanInfoPartPosition humanInfoPartPostion, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        Task<HumanInfoPartPosition> UpdateAsync(UserInfo user, HumanInfoPartPosition humanInfoPartPostion, CancellationToken cancellationToken = default(CancellationToken));




        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        Task DeleteAsync(UserInfo user, HumanInfoPartPosition humanInfoPartPostion, CancellationToken cancellationToken = default(CancellationToken));



        /// <summary>
        /// 查询单个
        /// </summary>
        /// <returns></returns>
        Task<TResult> GetAsync<TResult>(Func<IQueryable<HumanInfoPartPosition>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<HumanInfoPartPosition>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

    }
}
