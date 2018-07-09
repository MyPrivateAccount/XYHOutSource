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
    public class HumanInfoBlackStore : IHumanInfoBlackStore
    {
        protected HumanDbContext Context { get; }

        public IQueryable<HumanInfoBlack> HumanInfoBlacks { get; set; }

        public HumanInfoBlackStore(HumanDbContext context)
        {
            Context = context;
            HumanInfoBlacks = Context.HumanInfoBlacks;
        }

        public IQueryable<HumanInfoBlack> GetQuery()
        {
            var q = from hb in Context.HumanInfoBlacks.AsNoTracking()
                    join cu1 in Context.Users.AsNoTracking() on hb.CreateUser equals cu1.Id into cu2
                    from cu in cu2.DefaultIfEmpty()
                    select new HumanInfoBlack()
                    {
                        CreateTime = hb.CreateTime,
                        CreateUser = hb.CreateUser,
                        Email = hb.Email,
                        Id = hb.Id,
                        IDCard = hb.IDCard,
                        IsDeleted = hb.IsDeleted,
                        Name = hb.Name,
                        Phone = hb.Phone,
                        Reason = hb.Reason,
                        Sex = hb.Sex,
                        UserId = hb.UserId
                    };
            return q;
        }


        /// <summary>
        /// 新增,存在则更新
        /// </summary>
        /// <returns></returns>
        public async Task<HumanInfoBlack> SaveAsync(UserInfo user, HumanInfoBlack humanInfoBlack, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (humanInfoBlack == null)
            {
                throw new ArgumentNullException(nameof(humanInfoBlack));
            }
            if (string.IsNullOrEmpty(humanInfoBlack.Id))
            {
                humanInfoBlack.Id = Guid.NewGuid().ToString();
                humanInfoBlack.CreateTime = DateTime.Now;
                humanInfoBlack.CreateUser = user.Id;
                humanInfoBlack.IsDeleted = false;
                Context.Add(humanInfoBlack);
            }
            else if (!HumanInfoBlacks.Any(a => a.Id == humanInfoBlack.Id))
            {
                humanInfoBlack.CreateTime = DateTime.Now;
                humanInfoBlack.CreateUser = user.Id;
                humanInfoBlack.IsDeleted = false;
                Context.Add(humanInfoBlack);
            }
            else
            {
                var old = HumanInfoBlacks.FirstOrDefault(a => a.Id == humanInfoBlack.Id);
                old.IDCard = humanInfoBlack.IDCard;
                old.Name = humanInfoBlack.Name;
                old.Phone = humanInfoBlack.Phone;
                old.Reason = humanInfoBlack.Reason;
                old.Sex = humanInfoBlack.Sex;
                old.UserId = humanInfoBlack.UserId;
                old.Email = humanInfoBlack.Email;
                Context.Update(old);
            }
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException) { throw; }
            return humanInfoBlack;
        }

        /// <summary>
        /// 更新人事黑名单审核状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateExamineStatus(string id, ExamineStatusEnum status, CancellationToken cancellationToken = default(CancellationToken))
        {
            HumanInfoBlack humanInfoBlack = new HumanInfoBlack()
            {
                Id = id,
                UpdateTime = DateTime.Now,
                ExamineStatus = status
            };
            Context.Attach(humanInfoBlack);
            var entry = Context.Entry(humanInfoBlack);
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
        public async Task DeleteListAsync(UserInfo user, List<HumanInfoBlack> list, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }
            foreach (var item in list)
            {
                item.DeleteTime = DateTime.Now;
                item.DeleteUser = user.Id;
                item.IsDeleted = true;
                Context.Attach(item);
                var entry = Context.Entry(item);
                entry.Property(x => x.IsDeleted).IsModified = true;
                entry.Property(x => x.DeleteUser).IsModified = true;
                entry.Property(x => x.DeleteTime).IsModified = true;
            }
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
        /// 删除
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAsync(UserInfo user, HumanInfoBlack humanInfoBlack, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoBlack == null)
            {
                throw new ArgumentNullException(nameof(humanInfoBlack));
            }
            humanInfoBlack.DeleteTime = DateTime.Now;
            humanInfoBlack.DeleteUser = user.Id;
            humanInfoBlack.IsDeleted = true;
            Context.Attach(humanInfoBlack);
            var entry = Context.Entry(humanInfoBlack);
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
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<HumanInfoBlack>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(HumanInfoBlacks.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<HumanInfoBlack>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(HumanInfoBlacks.AsNoTracking()).ToListAsync(cancellationToken);
        }









    }
}
