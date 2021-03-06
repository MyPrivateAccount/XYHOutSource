﻿using ApplicationCore.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHHumanPlugin.Models;

namespace XYHHumanPlugin.Stores
{
    public interface IPositionSalaryStore
    {

        IQueryable<PositionSalary> PositionSalaries { get; set; }


        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        Task<PositionSalary> CreateAsync(UserInfo user, PositionSalary positionSalary, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        Task<PositionSalary> UpdateAsync(UserInfo user, PositionSalary positionSalary, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateExamineStatus(string id, ExamineStatusEnum status, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        Task DeleteAsync(UserInfo user, PositionSalary positionSalary, CancellationToken cancellationToken = default(CancellationToken));



        /// <summary>
        /// 查询单个
        /// </summary>
        /// <returns></returns>
        Task<TResult> GetAsync<TResult>(Func<IQueryable<PositionSalary>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<PositionSalary>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));




    }
}
