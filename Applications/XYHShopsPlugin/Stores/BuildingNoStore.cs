using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Models;

namespace XYHShopsPlugin.Stores
{
    public class BuildingNoStore : IBuildingNoStore
    {
        //Db
        protected ShopsDbContext Context { get; }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="customerDbContext">Context</param>
        public BuildingNoStore(ShopsDbContext shopsDbContext)
        {
            Context = shopsDbContext;
        }

        //获取所有楼栋批次信息
        public IQueryable<BuildingNo> BuildingNoAll()
        {
            return Context.BuildingNos;
        }

        /// <summary>
        /// 根据某一成员获取一条楼栋批次信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<BuildingNo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.BuildingNos.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 根据某一成员获取列表楼栋批次信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<BuildingNo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.BuildingNos.AsNoTracking()).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 新增楼栋批次信息
        /// </summary>
        /// <param name="buildingNo">实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task<BuildingNo> CreateAsync(BuildingNo buildingNo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingNo == null)
            {
                throw new ArgumentNullException(nameof(buildingNo));
            }
            Context.Add(buildingNo);
            await Context.SaveChangesAsync(cancellationToken);
            return buildingNo;
        }

        /// <summary>
        /// 新增楼栋批次信息
        /// </summary>
        /// <param name="buildingNo">实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task<List<BuildingNo>> CreateListAsync(List<BuildingNo> buildingNos, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingNos == null)
            {
                throw new ArgumentNullException(nameof(buildingNos));
            }
            Context.AddRange(buildingNos);
            await Context.SaveChangesAsync(cancellationToken);
            return buildingNos;
        }

        /// <summary>
        /// 删除楼栋批次
        /// </summary>
        /// <param name="user">登陆用户基本信息</param>
        /// <param name="buildingNo">楼栋批次实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task DeleteAsync(SimpleUser user, BuildingNo buildingNo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (buildingNo == null)
            {
                throw new ArgumentNullException(nameof(buildingNo));
            }
            //删除基本信息
            buildingNo.DeleteTime = DateTime.Now;
            buildingNo.DeleteUser = user.Id;
            buildingNo.IsDeleted = true;
            Context.Attach(buildingNo);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        /// <summary>
        /// 从数据库中删除List
        /// </summary>
        /// <param name="buildingNoList">集合</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task DeleteListAsync(List<BuildingNo> buildingNoList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingNoList == null)
            {
                throw new ArgumentNullException(nameof(buildingNoList));
            }
            Context.RemoveRange(buildingNoList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        /// <summary>
        /// 修改楼栋批次信息
        /// </summary>
        /// <param name="buildingNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateAsync(BuildingNo buildingNo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingNo == null)
            {
                throw new ArgumentNullException(nameof(buildingNo));
            }
            Context.Attach(buildingNo);
            Context.Update(buildingNo);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        /// <summary>
        /// 批量修改楼栋批次信息(一般修改删除状态)
        /// </summary>
        /// <param name="buildingNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateListAsync(List<BuildingNo> buildingNo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingNo == null)
            {
                throw new ArgumentNullException(nameof(buildingNo));
            }
            Context.AttachRange(buildingNo);
            Context.UpdateRange(buildingNo);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }
    }
}
