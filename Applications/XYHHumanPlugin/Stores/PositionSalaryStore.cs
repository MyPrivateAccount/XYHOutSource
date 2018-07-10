using ApplicationCore.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHHumanPlugin.Models;

namespace XYHHumanPlugin.Stores
{
    public class PositionSalaryStore : IPositionSalaryStore
    {
        protected HumanDbContext Context { get; }

        public IQueryable<PositionSalary> PositionSalaries { get; set; }

        public PositionSalaryStore(HumanDbContext context)
        {
            Context = context;
            PositionSalaries = Context.PositionSalaries;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        public async Task<PositionSalary> CreateAsync(UserInfo user, PositionSalary positionSalary, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (positionSalary == null)
            {
                throw new ArgumentNullException(nameof(positionSalary));
            }
            if (string.IsNullOrEmpty(positionSalary.Id))
            {
                positionSalary.Id = Guid.NewGuid().ToString();
            }
            positionSalary.CreateTime = DateTime.Now;
            positionSalary.CreateUser = user.Id;
            positionSalary.IsDeleted = false;
            Context.Add(positionSalary);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException) { throw; }
            return positionSalary;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        public async Task<PositionSalary> UpdateAsync(UserInfo user, PositionSalary positionSalary, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (positionSalary == null)
            {
                throw new ArgumentNullException(nameof(positionSalary));
            }
            var old = PositionSalaries.Where(a => a.Id == positionSalary.Id).SingleOrDefault();
            if (old == null)
            {
                throw new Exception("更新的对象不存在");
            }
            old.BaseWages = positionSalary.BaseWages;
            old.CommunicationAllowance = positionSalary.CommunicationAllowance;
            old.OtherAllowance = positionSalary.OtherAllowance;
            old.PostWages = positionSalary.PostWages;
            old.TrafficAllowance = positionSalary.TrafficAllowance;

            old.UpdateTime = DateTime.Now;
            old.UpdateUser = user.Id;
            Context.Update(old);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException) { throw; }
            return positionSalary;
        }

        /// <summary>
        /// 更新职位薪酬审核状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateExamineStatus(string id, ExamineStatusEnum status, CancellationToken cancellationToken = default(CancellationToken))
        {
            PositionSalary positionSalary = new PositionSalary()
            {
                Id = id,
                UpdateTime = DateTime.Now,
                ExamineStatus = status
            };
            Context.Attach(positionSalary);
            var entry = Context.Entry(positionSalary);
            entry.Property(x => x.ExamineStatus).IsModified = true;
            entry.Property(x => x.UpdateTime).IsModified = true;
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException) { throw; }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAsync(UserInfo user, PositionSalary positionSalary, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (positionSalary == null)
            {
                throw new ArgumentNullException(nameof(positionSalary));
            }
            positionSalary.DeleteTime = DateTime.Now;
            positionSalary.DeleteUser = user.Id;
            positionSalary.IsDeleted = true;
            Context.Attach(positionSalary);
            var entry = Context.Entry(positionSalary);
            entry.Property(x => x.IsDeleted).IsModified = true;
            entry.Property(x => x.DeleteUser).IsModified = true;
            entry.Property(x => x.DeleteTime).IsModified = true;
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                throw;
            }
        }


        /// <summary>
        /// 查询单个
        /// </summary>
        /// <returns></returns>
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<PositionSalary>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(PositionSalaries.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<PositionSalary>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(PositionSalaries.AsNoTracking()).ToListAsync(cancellationToken);
        }


    }
}
