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
    public class BuildingNoticeStore : IBuildingNoticeStore
    {
        public BuildingNoticeStore(ShopsDbContext shopsDbContext)
        {
            Context = shopsDbContext;
            BuildingNotices = Context.BuildingNotices;
        }

        protected ShopsDbContext Context { get; }

        public IQueryable<BuildingNotice> BuildingNotices { get; set; }



        public async Task<BuildingNotice> CreateAsync(BuildingNotice buildingNotice, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingNotice == null)
            {
                throw new ArgumentNullException(nameof(UpdateRecord));
            }
            Context.Add(buildingNotice);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return buildingNotice;
        }

        public async Task UpdateAsync(BuildingNotice buildingNotice, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingNotice == null)
            {
                throw new ArgumentNullException(nameof(buildingNotice));
            }
            Context.Attach(buildingNotice);
            Context.Update(buildingNotice);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }


        public Task<TResult> GetAsync<TResult>(Func<IQueryable<BuildingNotice>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.BuildingNotices.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        public IQueryable<BuildingNotice> GetQuery()
        {
            var query = from notice in Context.BuildingNotices.AsNoTracking()
                        join u1 in Context.Users.AsNoTracking() on notice.UserId equals u1.Id into u2
                        from u in u2.DefaultIfEmpty()

                        join o1 in Context.Organizations.AsNoTracking() on notice.OrganizationId equals o1.Id into o2
                        from o in o2.DefaultIfEmpty()

                        join bd in Context.Buildings.AsNoTracking() on notice.BuildingId equals bd.Id into bfa
                        from building in bfa.DefaultIfEmpty()
                        join basic1 in Context.BuildingBaseInfos.AsNoTracking() on notice.BuildingId equals basic1.Id into basic2
                        from basic in basic2.DefaultIfEmpty()

                            //区域
                        join c1 in Context.AreaDefines.AsNoTracking() on basic.City equals c1.Code into c2
                        from city in c2.DefaultIfEmpty()
                        join d1 in Context.AreaDefines.AsNoTracking() on basic.District equals d1.Code into d2
                        from district in d2.DefaultIfEmpty()
                        join a1 in Context.AreaDefines.AsNoTracking() on basic.Area equals a1.Code into a2
                        from area in a2.DefaultIfEmpty()

                        select new BuildingNotice()
                        {
                            BuildingId = notice.BuildingId,
                            Content = notice.Content,
                            CreateTime = notice.CreateTime,
                            DeleteTime = notice.DeleteTime,
                            DeleteUser = notice.DeleteUser,
                            Ext1 = notice.Ext1,
                            Ext2 = notice.Ext2,

                            //这儿暂时改为楼盘的icon
                            Icon = building.Icon,

                            Id = notice.Id,
                            IsDeleted = notice.IsDeleted,
                            OrganizationId = notice.OrganizationId,
                            OrganizationName = o.OrganizationName,
                            Title = notice.Title,
                            UserId = notice.UserId,
                            UserName = u.TrueName,

                            BuildingName = basic.Name,
                            AreaDefine = new AreaDefine()
                            {
                                Code = area.Code,
                                Name = area.Name
                            },
                            DistrictDefine = new AreaDefine()
                            {
                                Code = district.Code,
                                Name = district.Name
                            },
                            CityDefine = new AreaDefine()
                            {
                                Code = city.Code,
                                Name = city.Name
                            },

                            RecordFileInfos = from f1 in Context.FileInfos.AsNoTracking()
                                              join file in Context.BuildingNoticeFileScopes.AsNoTracking() on f1.FileGuid equals file.FileGuid
                                              where !file.IsDeleted && file.BuildingNoticeId == notice.Id
                                              select new FileInfo
                                              {
                                                  FileGuid = f1.FileGuid,
                                                  FileExt = f1.FileExt,
                                                  Uri = f1.Uri,
                                                  Name = f1.Name,
                                                  Type = f1.Type,
                                                  Group = file.Group,
                                                  Size = f1.Size,
                                                  Height = f1.Height,
                                                  Width = f1.Width,
                                                  Summary = f1.Summary,
                                                  Ext1 = f1.Ext1,
                                                  Ext2 = f1.Ext2
                                              }
                        };
            return query;
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<BuildingNotice>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.BuildingNotices.AsNoTracking()).ToListAsync(cancellationToken);
        }



    }
}
