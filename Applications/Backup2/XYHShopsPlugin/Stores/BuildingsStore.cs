using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Dto;
using XYHShopsPlugin.Models;

namespace XYHShopsPlugin.Stores
{
    public class BuildingsStore : IBuildingsStore
    {
        public BuildingsStore(ShopsDbContext baseDataDbContext)
        {
            Context = baseDataDbContext;
            Buildings = Context.Buildings;
        }

        protected ShopsDbContext Context { get; }

        public IQueryable<Buildings> Buildings { get; set; }


        public IQueryable<Buildings> GetSimpleSerchQuery()
        {
            var query = from b in Context.Buildings.AsNoTracking()
                        join basic1 in Context.BuildingBaseInfos.AsNoTracking() on b.Id equals basic1.Id into basic2
                        from basic in basic2.DefaultIfEmpty()
                        join f1 in Context.BuildingFacilities.AsNoTracking() on b.Id equals f1.Id into f2
                        from f in f2.DefaultIfEmpty()
                        join s1 in Context.BuildingShopInfos.AsNoTracking() on b.Id equals s1.Id into s2
                        from s in s2.DefaultIfEmpty()
                        join rule1 in Context.BuildingRules.AsNoTracking() on b.Id equals rule1.Id into rule2
                        from rule in rule2.DefaultIfEmpty()
                            //user
                        join cu1 in Context.Users.AsNoTracking() on b.CreateUser equals cu1.Id into cu2
                        from cu in cu2.DefaultIfEmpty()
                        join ru1a in Context.Users.AsNoTracking() on b.ResidentUser1 equals ru1a.Id into ru1b
                        from ru1 in ru1b.DefaultIfEmpty()
                        join ru2a in Context.Users.AsNoTracking() on b.ResidentUser2 equals ru2a.Id into ru2b
                        from ru2 in ru2b.DefaultIfEmpty()
                        join ru3a in Context.Users.AsNoTracking() on b.ResidentUser3 equals ru3a.Id into ru3b
                        from ru3 in ru3b.DefaultIfEmpty()
                        join ru4a in Context.Users.AsNoTracking() on b.ResidentUser4 equals ru4a.Id into ru4b
                        from ru4 in ru4b.DefaultIfEmpty()

                            //区域
                        join c1 in Context.AreaDefines.AsNoTracking() on basic.City equals c1.Code into c2
                        from city in c2.DefaultIfEmpty()
                        join d1 in Context.AreaDefines.AsNoTracking() on basic.District equals d1.Code into d2
                        from district in d2.DefaultIfEmpty()
                        join a1 in Context.AreaDefines.AsNoTracking() on basic.Area equals a1.Code into a2
                        from area in a2.DefaultIfEmpty()

                        select new Buildings()
                        {
                            Id = b.Id,
                            OrganizationId = b.OrganizationId,
                            ExamineStatus = b.ExamineStatus,
                            BuildingBaseInfo = new BuildingBaseInfo()
                            {
                                Name = basic.Name,
                                Address = basic.Address,
                                City = basic.City,
                                District = basic.District,
                                Area = basic.Area,
                                OpenDate = basic.OpenDate,
                                MaxPrice = basic.MaxPrice,
                                MinPrice = basic.MinPrice,
                                FloorSurface = basic.FloorSurface,
                                BuiltupArea = basic.BuiltupArea,
                                Developer = basic.Developer,
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
                            BuildingRule = rule,
                            BuildingFacilities = new BuildingFacilities()
                            {
                                HasBus = f.HasBus,
                                HasRail = f.HasRail,
                                HasOther = f.HasOther,
                                HasOtherTraffic = f.HasOtherTraffic,
                                HasKindergarten = f.HasKindergarten,
                                HasPrimarySchool = f.HasPrimarySchool,
                                HasMarket = f.HasMarket,
                                HasMiddleSchool = f.HasMiddleSchool,
                                HasUniversity = f.HasUniversity,
                                HasBank = f.HasBank,
                                HasHospital = f.HasHospital,
                                HasSupermarket = f.HasSupermarket
                            },
                            BuildingShopInfo = new BuildingShopInfo()
                            {
                                SaleStatus = s.SaleStatus,
                                ShopCategory = s.ShopCategory,
                                SaleMode = s.SaleMode,
                                Populations = s.Populations
                            },
                            ResidentUser1 = b.ResidentUser1,
                            ResidentUser2 = b.ResidentUser2,
                            ResidentUser3 = b.ResidentUser3,
                            ResidentUser4 = b.ResidentUser4,
                            ResidentUser1Info = new SimpleUser()
                            {
                                Id = ru1.Id,
                                UserName = ru1.TrueName,
                                PhoneNumber = ru1.PhoneNumber
                            },
                            ResidentUser2Info = new SimpleUser()
                            {
                                Id = ru2.Id,
                                UserName = ru2.TrueName,
                                PhoneNumber = ru2.PhoneNumber
                            },
                            ResidentUser3Info = new SimpleUser()
                            {
                                Id = ru3.Id,
                                UserName = ru3.TrueName,
                                PhoneNumber = ru3.PhoneNumber
                            },
                            ResidentUser4Info = new SimpleUser()
                            {
                                Id = ru4.Id,
                                UserName = ru4.TrueName,
                                PhoneNumber = ru4.PhoneNumber
                            },
                            CreateTime = b.CreateTime,
                            CreateUser = b.CreateUser,
                            UpdateTime = b.UpdateTime,
                            UpdateUser = b.UpdateUser,
                            DeleteTime = b.DeleteTime,
                            DeleteUser = b.DeleteUser,
                            IsDeleted = b.IsDeleted,
                            Icon = b.Icon,
                            BuildingNoInfo = from dd in Context.BuildingNos.AsNoTracking()
                                             where !dd.IsDeleted && dd.BuildingId == b.Id
                                             select new BuildingNo
                                             {
                                                 BuildingId = dd.BuildingId,
                                                 DeliveryDate = dd.DeliveryDate,
                                                 Desc = dd.Desc,
                                                 Id = dd.Id,
                                                 OpenDate = dd.OpenDate,
                                                 OrganizationId = dd.OrganizationId,
                                                 UserId = dd.UserId,
                                                 Storied = dd.Storied
                                             }
                        };
            return query;
        }


        public IQueryable<Buildings> GetDetailQuery()
        {
            var query = from b in Context.Buildings.AsNoTracking()
                        join basic1 in Context.BuildingBaseInfos.AsNoTracking() on b.Id equals basic1.Id into basic2
                        from basic in basic2.DefaultIfEmpty()
                        join f1 in Context.BuildingFacilities.AsNoTracking() on b.Id equals f1.Id into f2
                        from f in f2.DefaultIfEmpty()
                        join s1 in Context.BuildingShopInfos.AsNoTracking() on b.Id equals s1.Id into s2
                        from s in s2.DefaultIfEmpty()
                        join rule1 in Context.BuildingRules.AsNoTracking() on b.Id equals rule1.Id into rule2
                        from rule in rule2.DefaultIfEmpty()
                            //user
                        join cu1 in Context.Users.AsNoTracking() on b.CreateUser equals cu1.Id into cu2
                        from cu in cu2.DefaultIfEmpty()
                        join uu1 in Context.Users.AsNoTracking() on b.UpdateUser equals uu1.Id into uu2
                        from uu in uu2.DefaultIfEmpty()
                        join du1 in Context.Users.AsNoTracking() on b.DeleteUser equals du1.Id into du2
                        from du in du2.DefaultIfEmpty()
                        join ru1a in Context.Users.AsNoTracking() on b.ResidentUser1 equals ru1a.Id into ru1b
                        from ru1 in ru1b.DefaultIfEmpty()
                        join ru2a in Context.Users.AsNoTracking() on b.ResidentUser2 equals ru2a.Id into ru2b
                        from ru2 in ru2b.DefaultIfEmpty()
                        join ru3a in Context.Users.AsNoTracking() on b.ResidentUser3 equals ru3a.Id into ru3b
                        from ru3 in ru3b.DefaultIfEmpty()
                        join ru4a in Context.Users.AsNoTracking() on b.ResidentUser4 equals ru4a.Id into ru4b
                        from ru4 in ru4b.DefaultIfEmpty()

                            //组织
                        join o1 in Context.Organizations.AsNoTracking() on b.OrganizationId equals o1.Id into o2
                        from o in o2.DefaultIfEmpty()

                            //区域
                        join c1 in Context.AreaDefines.AsNoTracking() on basic.City equals c1.Code into c2
                        from city in c2.DefaultIfEmpty()
                        join d1 in Context.AreaDefines.AsNoTracking() on basic.District equals d1.Code into d2
                        from district in d2.DefaultIfEmpty()
                        join a1 in Context.AreaDefines.AsNoTracking() on basic.Area equals a1.Code into a2
                        from area in a2.DefaultIfEmpty()

                        select new Buildings()
                        {
                            Id = b.Id,
                            BuildingBaseInfo = new BuildingBaseInfo()
                            {
                                Id = basic.Id,
                                Name = basic.Name,
                                Address = basic.Address,
                                City = basic.City,
                                District = basic.District,
                                Area = basic.Area,
                                MaxPrice = basic.MaxPrice,
                                MinPrice = basic.MinPrice,
                                FloorSurface = basic.FloorSurface,
                                BuiltupArea = basic.BuiltupArea,
                                Developer = basic.Developer,
                                BasementParkingSpace = basic.BasementParkingSpace,
                                BuildingNum = basic.BuildingNum,
                                DeliveryDate = basic.DeliveryDate,
                                LandExpireDate = basic.LandExpireDate,
                                OpenDate = basic.OpenDate,
                                GreeningRate = basic.GreeningRate,
                                HouseHolds = basic.HouseHolds,
                                PlotRatio = basic.PlotRatio,
                                ParkingSpace = basic.ParkingSpace,
                                PMC = basic.PMC,
                                PMF = basic.PMF,
                                PropertyTerm = basic.PropertyTerm,
                                Shops = basic.Shops,
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
                            BuildingRule = rule,
                            BuildingFacilities = new BuildingFacilities()
                            {
                                HasBus = f.HasBus,
                                HasRail = f.HasRail,
                                HasOther = f.HasOther,
                                HasOtherTraffic = f.HasOtherTraffic,
                                HasKindergarten = f.HasKindergarten,
                                HasPrimarySchool = f.HasPrimarySchool,
                                HasMarket = f.HasMarket,
                                HasMiddleSchool = f.HasMiddleSchool,
                                HasUniversity = f.HasUniversity,
                                HasBank = f.HasBank,
                                HasHospital = f.HasHospital,
                                HasSupermarket = f.HasSupermarket,
                                BankDesc = f.BankDesc,
                                BusDesc = f.BusDesc,
                                HospitalDesc = f.HospitalDesc,
                                KindergartenDesc = f.KindergartenDesc,
                                Id = f.Id,
                                MarketDesc = f.MarketDesc,
                                MiddleSchoolDesc = f.MiddleSchoolDesc,
                                OtherDesc = f.OtherDesc,
                                OtherTrafficDesc = f.OtherTrafficDesc,
                                PrimarySchoolDesc = f.PrimarySchoolDesc,
                                RailDesc = f.RailDesc,
                                SupermarketDesc = f.SupermarketDesc,
                                UniversityDesc = f.UniversityDesc
                            },
                            BuildingShopInfo = new BuildingShopInfo()
                            {
                                SaleStatus = s.SaleStatus,
                                ShopCategory = s.ShopCategory,
                                SaleMode = s.SaleMode,
                                Populations = s.Populations,
                                Id = s.Id,
                                PreferentialPolicies = s.PreferentialPolicies,
                                TradeMixPlanning = s.TradeMixPlanning
                            },
                            ResidentUser1 = b.ResidentUser1,
                            ResidentUser2 = b.ResidentUser2,
                            ResidentUser3 = b.ResidentUser3,
                            ResidentUser4 = b.ResidentUser4,
                            Source = b.Source,
                            SourceId = b.SourceId,
                            ResidentUser1Info = new SimpleUser()
                            {
                                Id = ru1.Id,
                                UserName = ru1.TrueName,
                                PhoneNumber = ru1.PhoneNumber
                            },
                            ResidentUser2Info = new SimpleUser()
                            {
                                Id = ru2.Id,
                                UserName = ru2.TrueName,
                                PhoneNumber = ru2.PhoneNumber
                            },
                            ResidentUser3Info = new SimpleUser()
                            {
                                Id = ru3.Id,
                                UserName = ru3.TrueName,
                                PhoneNumber = ru3.PhoneNumber
                            },
                            ResidentUser4Info = new SimpleUser()
                            {
                                Id = ru4.Id,
                                UserName = ru4.TrueName,
                                PhoneNumber = ru4.PhoneNumber
                            },
                            CreateTime = b.CreateTime,
                            CreateUser = b.CreateUser,
                            UpdateTime = b.UpdateTime,
                            UpdateUser = b.UpdateUser,
                            DeleteTime = b.DeleteTime,
                            DeleteUser = b.DeleteUser,
                            IsDeleted = b.IsDeleted,
                            ExamineStatus = b.ExamineStatus,
                            CreateUserInfo = new SimpleUser()
                            {
                                Id = b.CreateUser,
                                UserName = cu.TrueName
                            },
                            DeleteUserInfo = new SimpleUser()
                            {
                                Id = b.DeleteUser,
                                UserName = du.TrueName
                            },
                            UpdateUserInfo = new SimpleUser()
                            {
                                Id = b.UpdateUser,
                                UserName = uu.TrueName
                            },
                            OrganizationId = b.OrganizationId,
                            OrganizationInfo = new Organizations()
                            {
                                Id = b.OrganizationId,
                                OrganizationName = o.OrganizationName
                            },
                            Summary = b.Summary,
                            CommissionPlan = b.CommissionPlan,
                            Icon = b.Icon,
                            BuildingFileInfos = from f1 in Context.FileInfos.AsNoTracking()
                                                join file in Context.BuildingFileScopes.AsNoTracking() on f1.FileGuid equals file.FileGuid
                                                where !file.IsDeleted && file.BuildingId == b.Id
                                                orderby file.CreateTime descending
                                                select new FileInfo
                                                {
                                                    FileGuid = f1.FileGuid,
                                                    IsDeleted = f1.IsDeleted,
                                                    Driver = f1.Driver,
                                                    FileExt = f1.FileExt,
                                                    Ext1 = f1.Ext1,
                                                    Ext2 = f1.Ext2,
                                                    Height = f1.Height,
                                                    Name = f1.Name,
                                                    Size = f1.Size,
                                                    Group = file.Group,
                                                    Summary = f1.Summary,
                                                    Type = f1.Type,
                                                    Uri = f1.Uri,
                                                    Width = f1.Width
                                                },
                            BuildingNoInfo = from dd in Context.BuildingNos.AsNoTracking()
                                             where !dd.IsDeleted && dd.BuildingId == b.Id
                                             select new BuildingNo
                                             {
                                                 BuildingId = dd.BuildingId,
                                                 DeliveryDate = dd.DeliveryDate,
                                                 Desc = dd.Desc,
                                                 Id = dd.Id,
                                                 OpenDate = dd.OpenDate,
                                                 OrganizationId = dd.OrganizationId,
                                                 UserId = dd.UserId,
                                                 Storied = dd.Storied,
                                                 IsDeleted = dd.IsDeleted
                                             },
                            UpdateRecords= from record in Context.UpdateRecords.AsNoTracking()
                                           where !record.IsDeleted && record.ContentId == b.Id
                                           orderby record.SubmitTime descending
                                           select new UpdateRecord
                                           {
                                               Content = record.Content,
                                               ContentId = record.ContentId,
                                               ContentType = record.ContentType,
                                               ExamineStatus = record.ExamineStatus,
                                               Ext1 = record.Ext1,
                                               Ext2 = record.Ext2,
                                               Ext3 = record.Ext3,
                                               Ext4 = record.Ext4,
                                               Ext5 = record.Ext5,
                                               Ext6 = record.Ext6,
                                               Ext7 = record.Ext7,
                                               Ext8 = record.Ext8,
                                               Id = record.Id,
                                               IsDeleted = record.IsDeleted,
                                               IsCurrent = record.IsCurrent,
                                               OrganizationId = record.OrganizationId,
                                               RecordFileInfos = record.RecordFileInfos,
                                               SubmitTime = record.SubmitTime,
                                               Title = record.Title,
                                               UpdateType = record.UpdateType,
                                               UpdateContent = record.UpdateContent,
                                               UserId = record.UserId

                                           }
                        };
            return query;
        }

        public async Task<Buildings> CreateAsync(Buildings buildings, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildings == null)
            {
                throw new ArgumentNullException(nameof(buildings));
            }
            Context.Add(buildings);
            await Context.SaveChangesAsync(cancellationToken);
            return buildings;
        }


        public async Task DeleteAsync(SimpleUser user, Buildings buildings, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (buildings == null)
            {
                throw new ArgumentNullException(nameof(buildings));
            }

            buildings.DeleteTime = DateTime.Now;
            buildings.DeleteUser = user.Id;
            buildings.IsDeleted = true;
            Context.Attach(buildings);
            var entry = Context.Entry(buildings);
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

        public async Task DeleteListAsync(List<Buildings> buildingsList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingsList == null)
            {
                throw new ArgumentNullException(nameof(buildingsList));
            }




            Context.RemoveRange(buildingsList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<Buildings>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.Buildings.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<Buildings>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.Buildings.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(Buildings buildings, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildings == null)
            {
                throw new ArgumentNullException(nameof(buildings));
            }
            Context.Attach(buildings);
            Context.Update(buildings);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        public async Task UpdateListAsync(List<Buildings> buildingsList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingsList == null)
            {
                throw new ArgumentNullException(nameof(buildingsList));
            }
            Context.AttachRange(buildingsList);
            Context.UpdateRange(buildingsList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        public async Task SaveSummaryAsync(SimpleUser user, string buildingId, string summary, string source, string sourceId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            //查看楼盘是否存在
            if (!Context.Buildings.Any(x => x.Id == buildingId))
            {
                Buildings buildings = new Buildings()
                {
                    Id = buildingId,
                    CreateUser = user.Id,
                    ResidentUser1 = user.Id,
                    CreateTime = DateTime.Now,
                    Summary = summary,
                    OrganizationId = user.OrganizationId,
                    ExamineStatus = 0,
                    Source = source,
                    SourceId = sourceId
                };

                Context.Add(buildings);
            }
            else
            {
                Buildings buildings = new Buildings()
                {
                    Id = buildingId,
                    Summary = summary,
                    UpdateUser = user.Id,
                    UpdateTime = DateTime.Now,
                    Source = source,
                    SourceId = sourceId
                };
                Context.Attach(buildings);
                var entry = Context.Entry(buildings);
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

        public async Task SaveCommissionAsync(SimpleUser user, string buildingId, string commission, string source, string sourceId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            //查看楼盘是否存在
            if (!Context.Buildings.Any(x => x.Id == buildingId))
            {
                Buildings buildings = new Buildings()
                {
                    Id = buildingId,
                    CreateUser = user.Id,
                    ResidentUser1 = user.Id,
                    CreateTime = DateTime.Now,
                    CommissionPlan = commission,
                    OrganizationId = user.OrganizationId,
                    ExamineStatus = 0,
                    Source = source,
                    SourceId = sourceId
                };

                Context.Add(buildings);
            }
            else
            {
                Buildings buildings = new Buildings()
                {
                    Id = buildingId,
                    CommissionPlan = commission,
                    UpdateUser = user.Id,
                    UpdateTime = DateTime.Now,
                    Source = source,
                    SourceId = sourceId
                };
                Context.Attach(buildings);
                var entry = Context.Entry(buildings);
                entry.Property(x => x.CommissionPlan).IsModified = true;
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
            catch (DbUpdateConcurrencyException) { throw; }
        }

        public async Task UpdateExamineStatus(string buildingId, Models.ExamineStatusEnum status, CancellationToken cancellationToken = default(CancellationToken))
        {
            Buildings buildings = new Buildings()
            {
                Id = buildingId,
                UpdateTime = DateTime.Now,
                ExamineStatus = (int)status
            };
            Context.Attach(buildings);
            var entry = Context.Entry(buildings);
            entry.Property(x => x.ExamineStatus).IsModified = true;
            entry.Property(x => x.UpdateTime).IsModified = true;
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { throw; }
        }
    }
}
