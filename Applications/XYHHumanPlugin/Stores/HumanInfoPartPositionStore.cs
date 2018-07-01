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
    public class HumanInfoPartPositionStore: IHumanInfoPartPositionStore
    {
        protected HumanDbContext Context { get; }

        public IQueryable<HumanInfoPartPosition> HumanInfoPartPostions { get; set; }

        public HumanInfoPartPositionStore(HumanDbContext context)
        {
            Context = context;
            HumanInfoPartPostions = Context.HumanInfoPartPostions;
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        public async Task<HumanInfoPartPosition> CreateAsync(UserInfo user, HumanInfoPartPosition humanInfoPartPostion, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (humanInfoPartPostion == null)
            {
                throw new ArgumentNullException(nameof(humanInfoPartPostion));
            }
            if (string.IsNullOrEmpty(humanInfoPartPostion.Id))
            {
                humanInfoPartPostion.Id = Guid.NewGuid().ToString();
            }
            humanInfoPartPostion.CreateTime = DateTime.Now;
            humanInfoPartPostion.CreateUser = user.Id;
            humanInfoPartPostion.IsDeleted = false;
            Context.Add(humanInfoPartPostion);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException) { throw; }
            return humanInfoPartPostion;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        public async Task<HumanInfoPartPosition> UpdateAsync(UserInfo user, HumanInfoPartPosition humanInfoPartPostion, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (humanInfoPartPostion == null)
            {
                throw new ArgumentNullException(nameof(humanInfoPartPostion));
            }
            var old = HumanInfoPartPostions.Where(a => a.Id == humanInfoPartPostion.Id).SingleOrDefault();
            if (old == null)
            {
                throw new Exception("更新的对象不存在");
            }
            old.DepartmentId = humanInfoPartPostion.DepartmentId;
            old.Desc = humanInfoPartPostion.Desc;
            old.EndTime = humanInfoPartPostion.EndTime;
            old.Position = humanInfoPartPostion.Position;
            old.StartTime = humanInfoPartPostion.StartTime;

            old.UpdateTime = DateTime.Now;
            old.UpdateUser = user.Id;
            Context.Update(old);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException) { throw; }
            return humanInfoPartPostion;
        }



        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAsync(UserInfo user, HumanInfoPartPosition humanInfoPartPostion, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoPartPostion == null)
            {
                throw new ArgumentNullException(nameof(humanInfoPartPostion));
            }
            humanInfoPartPostion.DeleteTime = DateTime.Now;
            humanInfoPartPostion.DeleteUser = user.Id;
            humanInfoPartPostion.IsDeleted = true;
            Context.Attach(humanInfoPartPostion);
            var entry = Context.Entry(humanInfoPartPostion);
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
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<HumanInfoPartPosition>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(HumanInfoPartPostions.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<HumanInfoPartPosition>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(HumanInfoPartPostions.AsNoTracking()).ToListAsync(cancellationToken);
        }

    }
}
