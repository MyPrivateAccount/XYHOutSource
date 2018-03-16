using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Models;
using ApplicationCore.Models;

namespace XYHShopsPlugin.Stores
{
    public class ShopsStore : IShopsStore
    {
        public ShopsStore(ShopsDbContext baseDataDbContext)
        {
            Context = baseDataDbContext;
            Shops = Context.Shops;
        }

        protected ShopsDbContext Context { get; }

        public IQueryable<Shops> Shops { get; set; }

        public IQueryable<Shops> GetSimpleQuery()
        {
            var query = from shop in Context.Shops.AsNoTracking()
                        join b1 in Context.ShopBaseInfos.AsNoTracking() on shop.Id equals b1.Id into b2
                        from b in b2.DefaultIfEmpty()
                        join f1 in Context.ShopFacilities.AsNoTracking() on shop.Id equals f1.Id into f2
                        from f in f2.DefaultIfEmpty()
                        join l1 in Context.ShopLeaseInfos.AsNoTracking() on shop.Id equals l1.Id into l2
                        from l in l2.DefaultIfEmpty()
                        join bn1 in Context.BuildingNos.AsNoTracking() on
                        new { BuildingNo = b.BuildingNo, BuildingId = shop.BuildingId } equals
                        new { BuildingNo = bn1.Storied, BuildingId = bn1.BuildingId }
                        into bn2
                        from bn in bn2.DefaultIfEmpty()

                        select new Shops()
                        {
                            BuildingId = shop.BuildingId,
                            Id = shop.Id,
                            CreateTime = shop.CreateTime,
                            CreateUser = shop.CreateUser,
                            ExamineStatus = shop.ExamineStatus,
                            IsDeleted = shop.IsDeleted,
                            OrganizationId = shop.OrganizationId,
                            Icon = shop.Icon,

                            BuildingNo = new BuildingNo()
                            {
                                Storied = bn.Storied,
                                OpenDate = bn.OpenDate,
                                DeliveryDate = bn.DeliveryDate
                            },
                            ShopBaseInfo = new ShopBaseInfo()
                            {
                                Name = b.Name,
                                BuildingArea = b.BuildingArea,
                                SaleStatus = b.SaleStatus,
                                BuildingNo = b.BuildingNo,
                                FloorNo = b.FloorNo,
                                Floors = b.Floors,
                                Number = b.Number,
                                Price = b.Price,
                                GuidingPrice = b.GuidingPrice,
                                TotalPrice = b.TotalPrice,
                                HouseArea = b.HouseArea,
                                OutsideArea = b.OutsideArea,
                                Width = b.Width,
                                Height = b.Height,
                                Depth = b.Depth,
                                Toward = b.Toward

                            },
                            ShopFacilities = new ShopFacilities()
                            {
                                Id = f.Id
                            },
                            ShopLeaseInfo = new ShopLeaseInfo()
                            {
                                HasLease = l.HasLease
                            }
                        };


            return query;

        }


        public IQueryable<Shops> GetDetailQuery()
        {
            var query = from shop in Context.Shops.AsNoTracking()
                        join f1 in Context.ShopFacilities.AsNoTracking() on shop.Id equals f1.Id into f2
                        from f in f2.DefaultIfEmpty()
                        join l1 in Context.ShopLeaseInfos.AsNoTracking() on shop.Id equals l1.Id into l2
                        from l in l2.DefaultIfEmpty()
                        join sbi1 in Context.ShopBaseInfos.AsNoTracking() on shop.Id equals sbi1.Id into sbi2
                        from sbi in sbi2.DefaultIfEmpty()

                            //用户
                        join u in Context.Users.AsNoTracking() on shop.CreateUser equals u.Id
                        join uu1 in Context.Users.AsNoTracking() on shop.UpdateUser equals uu1.Id into uu2
                        from uu in uu2.DefaultIfEmpty()
                        join du1 in Context.Users.AsNoTracking() on shop.DeleteUser equals du1.Id into du2
                        from du in du2.DefaultIfEmpty()

                            //组织
                        join o1 in Context.Organizations.AsNoTracking() on shop.OrganizationId equals o1.Id into o2
                        from o in o2.DefaultIfEmpty()

                            //楼盘
                        join bu1 in Context.Buildings.AsNoTracking() on shop.BuildingId equals bu1.Id into bu2
                        from bu in bu2.DefaultIfEmpty()
                        join bb1 in Context.BuildingBaseInfos.AsNoTracking() on bu.Id equals bb1.Id into bb2
                        from bb in bb2.DefaultIfEmpty()
                        join br1 in Context.BuildingRules.AsNoTracking() on bu.Id equals br1.Id into br2
                        from br in br2.DefaultIfEmpty()
                            //区域
                        join c1 in Context.AreaDefines.AsNoTracking() on bb.City equals c1.Code into c2
                        from city in c2.DefaultIfEmpty()
                        join d1 in Context.AreaDefines.AsNoTracking() on bb.District equals d1.Code into d2
                        from district in d2.DefaultIfEmpty()
                        join a1 in Context.AreaDefines.AsNoTracking() on bb.Area equals a1.Code into a2
                        from area in a2.DefaultIfEmpty()

                        select new Shops()
                        {
                            BuildingId = shop.BuildingId,
                            Id = shop.Id,
                            CreateTime = shop.CreateTime,
                            CreateUser = shop.CreateUser,
                            ExamineStatus = shop.ExamineStatus,
                            IsDeleted = shop.IsDeleted,
                            OrganizationId = shop.OrganizationId,
                            ShopBaseInfo = new ShopBaseInfo()
                            {
                                Name = sbi.Name,
                                BuildingArea = sbi.BuildingArea,
                                SaleStatus = sbi.SaleStatus,
                                BuildingNo = sbi.BuildingNo,
                                FloorNo = sbi.FloorNo,
                                Floors = sbi.Floors,
                                Number = sbi.Number,
                                Price = sbi.Price,
                                GuidingPrice = sbi.GuidingPrice,
                                TotalPrice = sbi.TotalPrice,
                                HouseArea = sbi.HouseArea,
                                OutsideArea = sbi.OutsideArea,
                                BuildingId = sbi.BuildingId,
                                Depth = sbi.Depth,
                                HasFree = sbi.HasFree,
                                HasStreet = sbi.HasStreet,
                                Height = sbi.Height,
                                FreeArea = sbi.FreeArea,
                                Id = sbi.Id,
                                IsCorner = sbi.IsCorner,
                                IsFaceStreet = sbi.IsFaceStreet,
                                ShopCategory = sbi.ShopCategory,
                                Status = sbi.Status,
                                StreetDistance = sbi.StreetDistance,
                                Toward = sbi.Toward,
                                Width = sbi.Width,
                                IsHot = sbi.IsHot,
                                LockTime = sbi.LockTime
                            },
                            ShopFacilities = new ShopFacilities()
                            {
                                Id = f.Id,
                                Blowoff = f.Blowoff,
                                DownWater = f.DownWater,
                                Capacitance = f.Capacitance,
                                Chimney = f.Chimney,
                                Elevator = f.Elevator,
                                Gas = f.Gas,
                                OpenFloor = f.OpenFloor,
                                Outside = f.Outside,
                                ParkingSpace = f.ParkingSpace,
                                Split = f.Split,
                                Staircase = f.Staircase,
                                UpperWater = f.UpperWater,
                                Voltage = f.Voltage
                            },
                            ShopLeaseInfo = new ShopLeaseInfo()
                            {
                                HasLease = l.HasLease,
                                BackMonth = l.BackMonth,
                                BackRate = l.BackRate,
                                CurrentOperation = l.CurrentOperation,
                                Deposit = l.Deposit,
                                EndDate = l.EndDate,
                                HasLeaseback = l.HasLeaseback,
                                Id = l.Id,
                                Memo = l.Memo,
                                StartDate = l.StartDate,
                                PaymentTime = l.PaymentTime,
                                Rental = l.Rental,
                                Upscale = l.Upscale,
                                DepositType = l.DepositType,
                                UpscaleInterval = l.UpscaleInterval,
                                UpscaleStartYear = l.UpscaleStartYear
                            },
                            CreateUserInfo = new SimpleUser()
                            {
                                Id = shop.CreateUser,
                                UserName = u.TrueName
                            },
                            UpdateUserInfo = new SimpleUser()
                            {
                                Id = shop.UpdateUser,
                                UserName = uu.TrueName
                            },
                            DeleteUserInfo = new SimpleUser()
                            {
                                Id = shop.DeleteUser,
                                UserName = du.TrueName
                            },
                            DeleteTime = shop.DeleteTime,
                            DeleteUser = shop.DeleteUser,
                            Summary = shop.Summary,
                            UpdateTime = shop.UpdateTime,
                            UpdateUser = shop.UpdateUser,
                            Source = shop.Source,
                            SourceId = shop.SourceId,
                            Icon = shop.Icon,
                            Buildings = new Buildings()
                            {
                                BuildingBaseInfo = new BuildingBaseInfo()
                                {
                                    Area = bb.Area,
                                    City = bb.City,
                                    Name = bb.Name,
                                    Id = bu.Id,
                                    Address = bb.Address,
                                    District = bb.District,
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
                                    }
                                },
                                BuildingRule = new BuildingRule()
                                {
                                    ReportTime = br.ReportTime,
                                    ReportedTemplate = br.ReportedTemplate
                                }
                            },
                            ShopsFileInfos = from f1 in Context.FileInfos.AsNoTracking()
                                             join file in Context.ShopsFileScopes.AsNoTracking() on f1.FileGuid equals file.FileGuid
                                             orderby file.CreateTime descending
                                             where !file.IsDeleted && file.ShopsId == shop.Id
                                             select new FileInfo
                                             {
                                                 FileGuid = f1.FileGuid,
                                                 FileExt = f1.FileExt,
                                                 Uri = f1.Uri,
                                                 Name = f1.Name,
                                                 Type = f1.Type,
                                                 Size = f1.Size,
                                                 Group = file.Group,
                                                 Height = f1.Height,
                                                 Width = f1.Width,
                                                 Summary = f1.Summary,
                                                 Ext1 = f1.Ext1,
                                                 Ext2 = f1.Ext2
                                             }
                        };


            return query;
        }

        public async Task<Shops> CreateAsync(Shops shops, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shops == null)
            {
                throw new ArgumentNullException(nameof(shops));
            }
            Context.Add(shops);
            await Context.SaveChangesAsync(cancellationToken);
            return shops;
        }


        public async Task DeleteAsync(SimpleUser user, Shops shops, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (shops == null)
            {
                throw new ArgumentNullException(nameof(shops));
            }

            shops.DeleteTime = DateTime.Now;
            shops.DeleteUser = user.Id;
            shops.IsDeleted = true;
            Context.Attach(shops);
            var entry = Context.Entry(shops);
            entry.Property(x => x.IsDeleted).IsModified = true;
            entry.Property(x => x.DeleteUser).IsModified = true;
            entry.Property(x => x.DeleteTime).IsModified = true;

            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<Shops> shops, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shops == null)
            {
                throw new ArgumentNullException(nameof(shops));
            }
            Context.RemoveRange(shops);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<Shops>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.Shops.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<Shops>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.Shops.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(Shops shops, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shops == null)
            {
                throw new ArgumentNullException(nameof(shops));
            }
            Context.Attach(shops);
            Context.Update(shops);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        public async Task UpdateListAsync(List<Shops> shopsList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopsList == null)
            {
                throw new ArgumentNullException(nameof(shopsList));
            }
            Context.AttachRange(shopsList);
            Context.UpdateRange(shopsList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        public async Task SaveSummaryAsync(SimpleUser user, string buildingId, string shopId, string summary, string source, string sourceId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            //查看楼盘是否存在
            if (!Context.Shops.Any(x => x.Id == shopId))
            {
                Shops shops = new Shops()
                {
                    Id = shopId,
                    BuildingId = buildingId,
                    CreateUser = user.Id,
                    CreateTime = DateTime.Now,
                    Summary = summary,
                    OrganizationId = user.OrganizationId,
                    ExamineStatus = 0
                };

                Context.Add(shops);
            }
            else
            {
                Shops shops = new Shops()
                {
                    Id = shopId,
                    Summary = summary,
                    UpdateUser = user.Id,
                    UpdateTime = DateTime.Now,
                    Source = source,
                    SourceId = sourceId

                };
                Context.Attach(shops);
                var entry = Context.Entry(shops);
                entry.Property(x => x.Summary).IsModified = true;

                entry.Property(x => x.UpdateUser).IsModified = true;
                entry.Property(x => x.UpdateTime).IsModified = true;
                if (!String.IsNullOrEmpty(source))
                {
                    entry.Property(x => x.Source).IsModified = true;
                }
                if (!String.IsNullOrEmpty(sourceId))
                {
                    entry.Property(x => x.SourceId).IsModified = true;
                }
            }

            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }


        public async Task UpdateExamineStatus(string shopId, ExamineStatusEnum status, CancellationToken cancellationToken = default(CancellationToken))
        {
            Shops shops = new Shops()
            {
                Id = shopId,
                ExamineStatus = (int)status
            };
            Context.Attach(shops);
            var entry = Context.Entry(shops);
            entry.Property(x => x.ExamineStatus).IsModified = true;

            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { throw; }
        }

        /// <summary>
        /// 查询不感兴趣商铺列表
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<List<TResult>> ListNotShopsAsync<TResult>(Func<IQueryable<CustomerNotShops>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerNotShops.AsNoTracking()).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 新增客源不感兴趣的商铺列表信息
        /// </summary>
        /// <param name="customerNotShops">实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task<CustomerNotShops> CreateCustomerNotShopAsync(CustomerNotShops customerNotShops, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerNotShops == null)
            {
                throw new ArgumentNullException(nameof(customerNotShops));
            }
            Context.Add(customerNotShops);
            await Context.SaveChangesAsync(cancellationToken);
            return customerNotShops;
        }
    }
}
