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
    public class HumanInfoPartPositionStore : IHumanInfoPartPositionStore
    {
        protected HumanDbContext Context { get; }

        public IQueryable<HumanInfoPartPosition> HumanInfoPartPostions { get; set; }

        public HumanInfoPartPositionStore(HumanDbContext context)
        {
            Context = context;
            HumanInfoPartPostions = Context.HumanInfoPartPositions;
        }

        public IQueryable<HumanInfoPartPosition> SimpleQuery()
        {
            var q = from hp in Context.HumanInfoPartPositions.AsNoTracking()
                    join h1 in Context.HumanInfos.AsNoTracking() on hp.HumanId equals h1.Id into h2
                    from h in h2.DefaultIfEmpty()
                    select new HumanInfoPartPosition()
                    {
                        Id = hp.Id,
                        Desc = hp.Desc,
                        EndTime = hp.EndTime,
                        StartTime = hp.StartTime,
                        CreateTime = hp.CreateTime,
                        CreateUser = hp.CreateUser,
                        DeleteTime = hp.DeleteTime,
                        DeleteUser = hp.DeleteUser,
                        HumanId = hp.HumanId,
                        IsCurrent = hp.IsCurrent,
                        IsDeleted = hp.IsDeleted,
                        UpdateTime = hp.UpdateTime,
                        UpdateUser = hp.UpdateUser,
                        Position = hp.Position,
                        DepartmentId = hp.DepartmentId,
                        OrganizationId = h.DepartmentId,
                    };
            return q;
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
        /// 更新人事兼职审核状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateExamineStatus(string id, ExamineStatusEnum status, CancellationToken cancellationToken = default(CancellationToken))
        {
            HumanInfoPartPosition humanInfoPartPosition = new HumanInfoPartPosition()
            {
                Id = id,
                UpdateTime = DateTime.Now,
                ExamineStatus = status
            };
            Context.Attach(humanInfoPartPosition);
            var entry = Context.Entry(humanInfoPartPosition);
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
