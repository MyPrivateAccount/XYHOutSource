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
    public class HumanInfoChangeStore : IHumanInfoChangeStore
    {
        protected HumanDbContext Context { get; }

        public IQueryable<HumanInfoChange> HumanInfoChanges { get; set; }

        public HumanInfoChangeStore(HumanDbContext context)
        {
            Context = context;
            HumanInfoChanges = Context.HumanInfoChanges;
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        public async Task<HumanInfoChange> CreateAsync(UserInfo user, HumanInfoChange humanInfoChange, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (humanInfoChange == null)
            {
                throw new ArgumentNullException(nameof(humanInfoChange));
            }
            if (string.IsNullOrEmpty(humanInfoChange.Id))
            {
                humanInfoChange.Id = Guid.NewGuid().ToString();
            }
            humanInfoChange.CreateTime = DateTime.Now;
            humanInfoChange.CreateUser = user.Id;
            humanInfoChange.IsDeleted = false;
            Context.Add(humanInfoChange);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException) { throw; }
            return humanInfoChange;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        public async Task<HumanInfoChange> UpdateAsync(UserInfo user, HumanInfoChange humanInfoChange, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (humanInfoChange == null)
            {
                throw new ArgumentNullException(nameof(humanInfoChange));
            }
            var old = HumanInfoChanges.Where(a => a.Id == humanInfoChange.Id).SingleOrDefault();
            if (old == null)
            {
                throw new Exception("更新的对象不存在");
            }
            old.ChangeContent = humanInfoChange.ChangeContent;
            old.ChangeId = humanInfoChange.ChangeId;
            old.ChangeReason = humanInfoChange.ChangeReason;
            old.ChangeTime = humanInfoChange.ChangeTime;
            old.ChangeType = humanInfoChange.ChangeType;
            old.UserId = humanInfoChange.UserId;

            old.UpdateTime = DateTime.Now;
            old.UpdateUser = user.Id;
            Context.Update(old);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException) { throw; }
            return humanInfoChange;
        }



        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAsync(UserInfo user, HumanInfoChange humanInfoChange, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoChange == null)
            {
                throw new ArgumentNullException(nameof(humanInfoChange));
            }
            humanInfoChange.DeleteTime = DateTime.Now;
            humanInfoChange.DeleteUser = user.Id;
            humanInfoChange.IsDeleted = true;
            Context.Attach(humanInfoChange);
            var entry = Context.Entry(humanInfoChange);
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
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<HumanInfoChange>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(HumanInfoChanges.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<HumanInfoChange>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(HumanInfoChanges.AsNoTracking()).ToListAsync(cancellationToken);
        }

    }
}
