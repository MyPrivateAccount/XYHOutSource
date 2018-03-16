using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    /// <summary>
    /// 接口
    /// </summary>
    public interface ICustomerTransactionsFollowUpStore
    {
        /// <summary>
        /// 查询成交信息详情
        /// </summary>
        /// <returns></returns>
        IQueryable<CustomerTransactionsFollowUp> CustomerTransactionsFollowUpAll();

        /// <summary>
        /// 根据某一字段获取一条成交信息
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerTransactionsFollowUp>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 根据条件获取集合
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerTransactionsFollowUp>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 创建成交信息
        /// </summary>
        /// <param name="customertransactionstollowup"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<CustomerTransactionsFollowUp> CreateAsync(CustomerTransactionsFollowUp customertransactionstollowup, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 批量创建成交跟进信息
        /// </summary>
        /// <param name="customertransactionstollowups"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<CustomerTransactionsFollowUp>> CreateListAsync(List<CustomerTransactionsFollowUp> customertransactionstollowups, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 删除客户成交
        /// </summary>
        /// <param name="user">登陆用户基本信息</param>
        /// <param name="customertransactionstollowup">客户成交实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        Task DeleteAsync(SimpleUser user, CustomerTransactionsFollowUp customertransactionstollowup, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// 从数据库中删除List
        /// </summary>
        /// <param name="customertransactionstollowupList">集合</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        Task DeleteListAsync(List<CustomerTransactionsFollowUp> customertransactionstollowupList, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 修改客户成交信息
        /// </summary>
        /// <param name="customertransactionstollowup"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateAsync(CustomerTransactionsFollowUp customertransactionstollowup, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 批量修改客户成交信息(一般修改删除状态)
        /// </summary>
        /// <param name="customertransactionstollowups"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateListAsync(List<CustomerTransactionsFollowUp> customertransactionstollowups, CancellationToken cancellationToken = default(CancellationToken));
    }
}
