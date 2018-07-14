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
    public interface IHumanInfoStore
    {
        IQueryable<HumanInfo> HumanInfos { get; set; }


        IQueryable<HumanInfo> SimpleQuery { get; }



        IQueryable<HumanInfo> GetQuery();



        IQueryable<HumanInfo> GetDetailQuery();



        Task<HumanInfo> SaveAsync(UserInfo user, HumanInfo humanInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateExamineStatus(string humanId, ExamineStatusEnum status, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        Task<HumanInfo> CreateAsync(HumanInfo humanInfo, CancellationToken cancellationToken = default(CancellationToken));
        Task<HumanInfo> UpdateAsync(UserInfo user, HumanInfo humanInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task<HumanSocialSecurity> UpdateHumanSocialSecurityAsync(UserInfo user, HumanSocialSecurity humanSocialSecurity, CancellationToken cancellationToken = default(CancellationToken));

        Task<HumanSalaryStructure> UpdateHumanSalaryStructureAsync(UserInfo user, HumanSalaryStructure humanSalaryStructure, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        Task DeleteListAsync(UserInfo user, List<HumanInfo> list, CancellationToken cancellationToken = default(CancellationToken));



        /// <summary>
        /// 查询单个
        /// </summary>
        /// <returns></returns>
        Task<TResult> GetAsync<TResult>(Func<IQueryable<HumanInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<HumanInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));


    }
}
