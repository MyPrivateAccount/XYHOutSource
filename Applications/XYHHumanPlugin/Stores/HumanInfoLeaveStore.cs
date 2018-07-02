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
    public class HumanInfoLeaveStore : IHumanInfoLeaveStore
    {
        protected HumanDbContext Context { get; }

        public IQueryable<HumanInfoLeave> HumanInfoLeaves { get; set; }

        public HumanInfoLeaveStore(HumanDbContext context)
        {
            Context = context;
            HumanInfoLeaves = Context.HumanInfoLeaves;
        }

        public IQueryable<HumanInfoLeave> SimpleQuery()
        {
            var q = from hrl in Context.HumanInfoLeaves.AsNoTracking()
                    join h1 in Context.HumanInfos.AsNoTracking() on hrl.HumanId equals h1.Id into h2
                    from h in h2.DefaultIfEmpty()
                    select new HumanInfoLeave()
                    {
                        Id = hrl.Id,
                        IsProcedure = hrl.IsProcedure,
                        LeaveTime = hrl.LeaveTime,
                        NewHumanId = hrl.NewHumanId,
                        CreateTime = hrl.CreateTime,
                        CreateUser = hrl.CreateUser,
                        DeleteTime = hrl.DeleteTime,
                        DeleteUser = hrl.DeleteUser,
                        HumanId = hrl.HumanId,
                        IsCurrent = hrl.IsCurrent,
                        IsDeleted = hrl.IsDeleted,
                        UpdateTime = hrl.UpdateTime,
                        UpdateUser = hrl.UpdateUser,
                        OrganizationId = h.DepartmentId,
                    };
            return q;
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        public async Task<HumanInfoLeave> CreateAsync(UserInfo user, HumanInfoLeave humanInfoLeave, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (humanInfoLeave == null)
            {
                throw new ArgumentNullException(nameof(humanInfoLeave));
            }
            if (string.IsNullOrEmpty(humanInfoLeave.Id))
            {
                humanInfoLeave.Id = Guid.NewGuid().ToString();
            }
            humanInfoLeave.CreateTime = DateTime.Now;
            humanInfoLeave.CreateUser = user.Id;
            humanInfoLeave.IsDeleted = false;
            Context.Add(humanInfoLeave);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException) { throw; }
            return humanInfoLeave;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        public async Task<HumanInfoLeave> UpdateAsync(UserInfo user, HumanInfoLeave humanInfoLeave, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (humanInfoLeave == null)
            {
                throw new ArgumentNullException(nameof(humanInfoLeave));
            }
            var old = HumanInfoLeaves.Where(a => a.Id == humanInfoLeave.Id).SingleOrDefault();
            if (old == null)
            {
                throw new Exception("更新的对象不存在");
            }
            old.IsProcedure = humanInfoLeave.IsProcedure;
            old.LeaveTime = humanInfoLeave.LeaveTime;
            old.NewHumanId = humanInfoLeave.NewHumanId;
            old.UpdateTime = DateTime.Now;
            old.UpdateUser = user.Id;
            Context.Update(old);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException) { throw; }
            return humanInfoLeave;
        }



        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAsync(UserInfo user, HumanInfoLeave humanInfoLeave, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoLeave == null)
            {
                throw new ArgumentNullException(nameof(humanInfoLeave));
            }
            humanInfoLeave.DeleteTime = DateTime.Now;
            humanInfoLeave.DeleteUser = user.Id;
            humanInfoLeave.IsDeleted = true;
            Context.Attach(humanInfoLeave);
            var entry = Context.Entry(humanInfoLeave);
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
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<HumanInfoLeave>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(HumanInfoLeaves.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<HumanInfoLeave>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(HumanInfoLeaves.AsNoTracking()).ToListAsync(cancellationToken);
        }




    }
}
