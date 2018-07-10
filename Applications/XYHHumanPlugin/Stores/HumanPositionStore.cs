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
    public class HumanPositionStore : IHumanPositionStore
    {
        protected HumanDbContext Context { get; }

        public IQueryable<HumanPosition> HumanPositions { get; set; }

        public HumanPositionStore(HumanDbContext context)
        {
            Context = context;
            HumanPositions = Context.HumanPositions;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        public async Task<HumanPosition> CreateAsync(UserInfo user, HumanPosition humanPosition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (humanPosition == null)
            {
                throw new ArgumentNullException(nameof(humanPosition));
            }
            if (string.IsNullOrEmpty(humanPosition.Id))
            {
                humanPosition.Id = Guid.NewGuid().ToString();
            }
            humanPosition.CreateTime = DateTime.Now;
            humanPosition.CreateUser = user.Id;
            humanPosition.IsDeleted = false;
            Context.Add(humanPosition);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException) { throw; }
            return humanPosition;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        public async Task<HumanPosition> UpdateAsync(UserInfo user, HumanPosition humanPosition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (humanPosition == null)
            {
                throw new ArgumentNullException(nameof(humanPosition));
            }
            var old = HumanPositions.Where(a => a.Id == humanPosition.Id).SingleOrDefault();
            if (old == null)
            {
                throw new Exception("更新的对象不存在");
            }
            old.DepartmentId = humanPosition.DepartmentId;
            old.Name = humanPosition.Name;
            old.Type = humanPosition.Type;

            old.UpdateTime = DateTime.Now;
            old.UpdateUser = user.Id;
            Context.Update(old);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException) { throw; }
            return humanPosition;
        }

        /// <summary>
        /// 更新职位审核状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateExamineStatus(string id, ExamineStatusEnum status, CancellationToken cancellationToken = default(CancellationToken))
        {
            HumanPosition humanPosition = new HumanPosition()
            {
                Id = id,
                UpdateTime = DateTime.Now,
                ExamineStatus = status
            };
            Context.Attach(humanPosition);
            var entry = Context.Entry(humanPosition);
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
        public async Task DeleteAsync(UserInfo user, HumanPosition humanPosition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanPosition == null)
            {
                throw new ArgumentNullException(nameof(humanPosition));
            }
            humanPosition.DeleteTime = DateTime.Now;
            humanPosition.DeleteUser = user.Id;
            humanPosition.IsDeleted = true;
            Context.Attach(humanPosition);
            var entry = Context.Entry(humanPosition);
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
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<HumanPosition>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(HumanPositions.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<HumanPosition>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(HumanPositions.AsNoTracking()).ToListAsync(cancellationToken);
        }
    }
}
