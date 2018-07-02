using System;
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
    public interface IHumanInfoRegularStore
    {
        IQueryable<HumanInfoRegular> HumanInfoRegulars { get; set; }

        IQueryable<HumanInfoRegular> SimpleQuery();

        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        Task<HumanInfoRegular> CreateAsync(UserInfo user, HumanInfoRegular humanInfoRegular, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        Task<HumanInfoRegular> UpdateAsync(UserInfo user, HumanInfoRegular humanInfoRegular, CancellationToken cancellationToken = default(CancellationToken));




        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        Task DeleteAsync(UserInfo user, HumanInfoRegular humanInfoRegular, CancellationToken cancellationToken = default(CancellationToken));



        /// <summary>
        /// 查询单个
        /// </summary>
        /// <returns></returns>
        Task<TResult> GetAsync<TResult>(Func<IQueryable<HumanInfoRegular>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<HumanInfoRegular>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));


    }
}
