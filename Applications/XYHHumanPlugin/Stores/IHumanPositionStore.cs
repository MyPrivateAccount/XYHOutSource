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
    public interface IHumanPositionStore
    {

        IQueryable<HumanPosition> HumanPositions { get; set; }


        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        Task<HumanPosition> CreateAsync(UserInfo user, HumanPosition humanPosition, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        Task<HumanPosition> UpdateAsync(UserInfo user, HumanPosition humanPosition, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateExamineStatus(string id, ExamineStatusEnum status, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        Task DeleteAsync(UserInfo user, HumanPosition humanPosition, CancellationToken cancellationToken = default(CancellationToken));



        /// <summary>
        /// 查询单个
        /// </summary>
        /// <returns></returns>
        Task<TResult> GetAsync<TResult>(Func<IQueryable<HumanPosition>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<HumanPosition>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));


    }
}
