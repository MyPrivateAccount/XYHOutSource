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
    public interface IHumanInfoBlackStore
    {
        IQueryable<HumanInfoBlack> HumanInfoBlacks { get; set; }
        IQueryable<HumanInfoBlack> GetQuery();
        /// <summary>
        /// 新增,存在则更新
        /// </summary>
        /// <returns></returns>
        Task<HumanInfoBlack> SaveAsync(UserInfo user, HumanInfoBlack humanInfoBlack, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        Task DeleteListAsync(UserInfo user, List<HumanInfoBlack> list, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(UserInfo user, HumanInfoBlack humanInfoBlack, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 查询单个
        /// </summary>
        /// <returns></returns>
        Task<TResult> GetAsync<TResult>(Func<IQueryable<HumanInfoBlack>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<HumanInfoBlack>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

    }
}
