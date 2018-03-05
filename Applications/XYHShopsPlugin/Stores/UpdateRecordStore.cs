using ApplicationCore.Dto;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Dto;
using XYHShopsPlugin.Dto.Request;
using XYHShopsPlugin.Models;

namespace XYHShopsPlugin.Stores
{
    public class UpdateRecordStore : IUpdateRecordStore
    {

        public UpdateRecordStore(ShopsDbContext shopsDbContext)
        {
            Context = shopsDbContext;
            UpdateRecords = Context.UpdateRecords;
        }

        protected ShopsDbContext Context { get; }

        public IQueryable<UpdateRecord> UpdateRecords { get; set; }

        public IQueryable<UpdateRecord> GetDetail()
        {
            var response = from record in Context.UpdateRecords.AsNoTracking()

                           join u1 in Context.Users.AsNoTracking() on record.UserId equals u1.Id into u2
                           from u in u2.DefaultIfEmpty()



                           join bd in Context.Buildings.AsNoTracking() on record.ContentId equals bd.Id into bfa
                           from building in bfa.DefaultIfEmpty()
                           join basic1 in Context.BuildingBaseInfos.AsNoTracking() on record.ContentId equals basic1.Id into basic2
                           from basic in basic2.DefaultIfEmpty()

                               //区域
                           join c1 in Context.AreaDefines.AsNoTracking() on basic.City equals c1.Code into c2
                           from city in c2.DefaultIfEmpty()
                           join d1 in Context.AreaDefines.AsNoTracking() on basic.District equals d1.Code into d2
                           from district in d2.DefaultIfEmpty()
                           join a1 in Context.AreaDefines.AsNoTracking() on basic.Area equals a1.Code into a2
                           from area in a2.DefaultIfEmpty()

                           where !building.IsDeleted
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
                               //这儿暂时改为楼盘的icon
                               Icon = building.Icon,
                               Id = record.Id,
                               IsDeleted = record.IsDeleted,
                               IsCurrent = record.IsCurrent,
                               OrganizationId = record.OrganizationId,
                               RecordFileInfos = record.RecordFileInfos,
                               SubmitTime = record.SubmitTime,
                               Title = record.Title,
                               UpdateType = record.UpdateType,
                               UpdateContent = record.UpdateContent,
                               UserId = record.UserId,
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
                               UserName = u.TrueName,

                           };
            return response;
        }

        public IQueryable<UpdateRecord> GetFollowDetail(string userId)
        {
            var response = from record in Context.UpdateRecords.AsNoTracking()

                           join u1 in Context.Users.AsNoTracking() on record.UserId equals u1.Id into u2
                           from u in u2.DefaultIfEmpty()

                           join f1 in Context.BuildingFavorites.AsNoTracking() on record.ContentId equals f1.BuildingId into f2
                           from f in f2.DefaultIfEmpty()


                           join bd in Context.Buildings.AsNoTracking() on record.ContentId equals bd.Id into bfa
                           from building in bfa.DefaultIfEmpty()
                           join basic1 in Context.BuildingBaseInfos.AsNoTracking() on record.ContentId equals basic1.Id into basic2
                           from basic in basic2.DefaultIfEmpty()

                               //区域
                           join c1 in Context.AreaDefines.AsNoTracking() on basic.City equals c1.Code into c2
                           from city in c2.DefaultIfEmpty()
                           join d1 in Context.AreaDefines.AsNoTracking() on basic.District equals d1.Code into d2
                           from district in d2.DefaultIfEmpty()
                           join a1 in Context.AreaDefines.AsNoTracking() on basic.Area equals a1.Code into a2
                           from area in a2.DefaultIfEmpty()

                           where f.UserId == userId && !f.IsDeleted
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
                               //这儿暂时改为楼盘的icon
                               Icon = building.Icon,
                               Id = record.Id,
                               IsDeleted = record.IsDeleted,
                               IsCurrent = record.IsCurrent,
                               OrganizationId = record.OrganizationId,
                               RecordFileInfos = record.RecordFileInfos,
                               SubmitTime = record.SubmitTime,
                               Title = record.Title,
                               UpdateType = record.UpdateType,
                               UpdateContent = record.UpdateContent,
                               UserId = record.UserId,
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
                               UserName = u.TrueName,

                           };
            return response;
        }



        public async Task<UpdateRecord> CreateUpdateRecordAsync(UpdateRecord updateRecord, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (updateRecord == null)
            {
                throw new ArgumentNullException(nameof(UpdateRecord));
            }
            var record = await Context.UpdateRecords.FirstOrDefaultAsync(a => a.UpdateType == updateRecord.UpdateType && a.ContentType == updateRecord.ContentType && a.ContentId == updateRecord.ContentId && a.ExamineStatus == Models.ExamineStatusEnum.Auditing);
            if (record != null)
            {
                throw new Exception("上次审核未完成，不能再次进行提交");
            }
            var newrecoed = await Context.UpdateRecords.FirstOrDefaultAsync(a => a.UpdateType == updateRecord.UpdateType && a.ContentType == updateRecord.ContentType && a.ContentId == updateRecord.ContentId && a.IsCurrent);
            if (newrecoed != null)
            {
                newrecoed.IsCurrent = false;
                Context.Update(newrecoed);
            }
            Context.Add(updateRecord);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return updateRecord;
        }

        public async Task UpdateUpdateRecordAsync(UpdateRecord updateRecord, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (updateRecord == null)
            {
                throw new ArgumentNullException(nameof(UpdateRecord));
            }
            Context.Attach(updateRecord);
            Context.Update(updateRecord);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }


        public Task<TResult> GetAsync<TResult>(Func<IQueryable<UpdateRecord>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.UpdateRecords.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        public IQueryable<UpdateRecord> GetQuery()
        {
            var query = from record in Context.UpdateRecords.AsNoTracking()
                        join u1 in Context.Users.AsNoTracking() on record.UserId equals u1.Id into u2
                        from u in u2.DefaultIfEmpty()

                        join bd in Context.Buildings.AsNoTracking() on record.ContentId equals bd.Id into bfa
                        from building in bfa.DefaultIfEmpty()
                        join basic1 in Context.BuildingBaseInfos.AsNoTracking() on record.ContentId equals basic1.Id into basic2
                        from basic in basic2.DefaultIfEmpty()

                            //区域
                        join c1 in Context.AreaDefines.AsNoTracking() on basic.City equals c1.Code into c2
                        from city in c2.DefaultIfEmpty()
                        join d1 in Context.AreaDefines.AsNoTracking() on basic.District equals d1.Code into d2
                        from district in d2.DefaultIfEmpty()
                        join a1 in Context.AreaDefines.AsNoTracking() on basic.Area equals a1.Code into a2
                        from area in a2.DefaultIfEmpty()
                        select new UpdateRecord()
                        {
                            Content = record.Content,
                            ContentId = record.ContentId,
                            ContentType = record.ContentType,
                            DeleteTime = record.DeleteTime,
                            DeleteUserId = record.DeleteUserId,
                            ExamineStatus = record.ExamineStatus,
                            Ext1 = record.Ext1,
                            Ext2 = record.Ext2,
                            Ext3 = record.Ext3,
                            Ext4 = record.Ext4,
                            Ext5 = record.Ext5,
                            Ext6 = record.Ext6,
                            Ext7 = record.Ext7,
                            Ext8 = record.Ext8,
                            //这儿暂时改为楼盘的icon
                            Icon = building.Icon,
                            Id = record.Id,
                            IsCurrent = record.IsCurrent,
                            IsDeleted = record.IsDeleted,
                            OrganizationId = record.OrganizationId,
                            SubmitTime = record.SubmitTime,
                            Title = record.Title,
                            UpdateContent = record.UpdateContent,
                            UpdateTime = record.UpdateTime,
                            UpdateType = record.UpdateType,
                            UserId = record.UserId,
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
                                              join file in Context.UpdateRecordFileScopes.AsNoTracking() on f1.FileGuid equals file.FileGuid
                                              where !file.IsDeleted && file.UpdateRecordId == record.Id
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


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<UpdateRecord>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.UpdateRecords.AsNoTracking()).ToListAsync(cancellationToken);
        }


        public async Task ShopsHotCallbackAsync(ExamineResponse examineResponse)
        {
            var updaterecord = await Context.UpdateRecords.FirstOrDefaultAsync(a => a.Id == examineResponse.SubmitDefineId);
            if (updaterecord == null)
            {
                throw new Exception("未找到动态记录");
            }
            var building = await Context.BuildingShopInfos.FirstOrDefaultAsync(a => a.Id == examineResponse.ContentId);
            if (building == null)
            {
                throw new Exception("未找到楼盘");
            }
            if (examineResponse.ExamineStatus == ExamineStatus.Examined)
            {
                updaterecord.ExamineStatus = Models.ExamineStatusEnum.Approved;
                BuildingShopsUpdateModel buildingShopsUpdateModel = JsonConvert.DeserializeObject<BuildingShopsUpdateModel>(updaterecord.UpdateContent);
                var list = buildingShopsUpdateModel.ShopList.Select(a => a.Id);
                var shops = (from shop in Context.ShopBaseInfos.AsNoTracking()
                             join b1 in Context.Shops.AsNoTracking() on shop.Id equals b1.Id into b2
                             from b in b2.DefaultIfEmpty()
                             where b.BuildingId == updaterecord.ContentId
                             select shop).ToList();
                for (int i = 0; i < shops.Count; i++)
                {
                    if (list.Contains(shops[i].Id))
                    {
                        shops[i].IsHot = true;
                    }
                    else
                    {
                        shops[i].IsHot = false;
                    }
                }
                Context.Update(updaterecord);
                Context.UpdateRange(shops);
            }
            else if (examineResponse.ExamineStatus == ExamineStatus.Reject)
            {
                updaterecord.ExamineStatus = Models.ExamineStatusEnum.Reject;
                Context.Update(updaterecord);
            }
            else
            {
                throw new Exception("审核状态不正确");
            }
            try
            {
                Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task ShopsAddCallbackAsync(ExamineResponse examineResponse)
        {
            var updaterecord = await Context.UpdateRecords.FirstOrDefaultAsync(a => a.Id == examineResponse.SubmitDefineId);
            if (updaterecord == null)
            {
                throw new Exception("未找到动态记录");
            }
            var building = await Context.BuildingShopInfos.FirstOrDefaultAsync(a => a.Id == examineResponse.ContentId);
            if (building == null)
            {
                throw new Exception("未找到楼盘");
            }
            if (examineResponse.ExamineStatus == ExamineStatus.Examined)
            {
                updaterecord.ExamineStatus = Models.ExamineStatusEnum.Approved;
                BuildingShopsUpdateModel buildingShopsUpdateModel = JsonConvert.DeserializeObject<BuildingShopsUpdateModel>(updaterecord.UpdateContent);
                var list = buildingShopsUpdateModel.ShopList.Select(a => a.Id);
                var shops = await Context.ShopBaseInfos.Where(a => list.Contains(a.Id)).ToListAsync();
                for (int i = 0; i < shops.Count; i++)
                {
                    shops[i].SaleStatus = "2";
                }
                Context.Update(updaterecord);
                Context.UpdateRange(shops);
            }
            else if (examineResponse.ExamineStatus == ExamineStatus.Reject)
            {
                updaterecord.ExamineStatus = Models.ExamineStatusEnum.Reject;
                Context.Update(updaterecord);
            }
            else
            {
                throw new Exception("审核状态不正确");
            }
            try
            {
                Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task ReportRuleCallbackAsync(ExamineResponse examineResponse)
        {
            var updaterecord = await Context.UpdateRecords.FirstOrDefaultAsync(a => a.Id == examineResponse.SubmitDefineId);
            if (updaterecord == null)
            {
                throw new Exception("未找到动态记录");
            }
            if (examineResponse.ExamineStatus == ExamineStatus.Examined)
            {
                var buildingRule = await Context.BuildingRules.FirstOrDefaultAsync(a => a.Id == examineResponse.ContentId);
                updaterecord.ExamineStatus = Models.ExamineStatusEnum.Approved;
                ReportRuleUpdateModel reportRuleUpdateModel = JsonConvert.DeserializeObject<ReportRuleUpdateModel>(updaterecord.UpdateContent);
                if (buildingRule == null)
                {
                    buildingRule = new BuildingRule();
                    buildingRule.Id = examineResponse.ContentId;
                    buildingRule.AdvanceTime = reportRuleUpdateModel.AdvanceTime;
                    buildingRule.BeltProtectDay = reportRuleUpdateModel.BeltProtectDay;
                    buildingRule.IsCompletenessPhone = reportRuleUpdateModel.IsCompletenessPhone;
                    buildingRule.LiberatingEnd = reportRuleUpdateModel.LiberatingEnd;
                    buildingRule.LiberatingStart = reportRuleUpdateModel.LiberatingStart;
                    buildingRule.Mark = reportRuleUpdateModel.Mark;
                    buildingRule.MaxCustomer = reportRuleUpdateModel.MaxCustomer;
                    buildingRule.ReportedTemplate = reportRuleUpdateModel.ReportedTemplate;
                    buildingRule.ReportTime = reportRuleUpdateModel.ReportTime;
                    buildingRule.ValidityDay = reportRuleUpdateModel.ValidityDay;
                    buildingRule.IsUse = reportRuleUpdateModel.IsUse;
                    Context.Update(updaterecord);
                    Context.Add(buildingRule);
                }
                else
                {
                    buildingRule.AdvanceTime = reportRuleUpdateModel.AdvanceTime;
                    buildingRule.BeltProtectDay = reportRuleUpdateModel.BeltProtectDay;
                    buildingRule.IsCompletenessPhone = reportRuleUpdateModel.IsCompletenessPhone;
                    buildingRule.LiberatingEnd = reportRuleUpdateModel.LiberatingEnd;
                    buildingRule.LiberatingStart = reportRuleUpdateModel.LiberatingStart;
                    buildingRule.Mark = reportRuleUpdateModel.Mark;
                    buildingRule.MaxCustomer = reportRuleUpdateModel.MaxCustomer;
                    buildingRule.ReportedTemplate = reportRuleUpdateModel.ReportedTemplate;
                    buildingRule.ReportTime = reportRuleUpdateModel.ReportTime;
                    buildingRule.ValidityDay = reportRuleUpdateModel.ValidityDay;
                    buildingRule.IsUse = reportRuleUpdateModel.IsUse;
                    Context.Update(updaterecord);
                    Context.Update(buildingRule);
                }
            }
            else if (examineResponse.ExamineStatus == ExamineStatus.Reject)
            {
                updaterecord.ExamineStatus = Models.ExamineStatusEnum.Reject;
                Context.Update(updaterecord);
            }
            else
            {
                throw new Exception("审核状态不正确");
            }
            try
            {
                Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task CommissionTypeCallbackAsync(ExamineResponse examineResponse)
        {
            var updaterecord = await Context.UpdateRecords.FirstOrDefaultAsync(a => a.Id == examineResponse.SubmitDefineId);
            if (updaterecord == null)
            {
                throw new Exception("未找到动态记录");
            }
            var building = await Context.Buildings.FirstOrDefaultAsync(a => a.Id == examineResponse.ContentId);
            if (building == null)
            {
                throw new Exception("未找到楼盘");
            }
            if (examineResponse.ExamineStatus == ExamineStatus.Examined)
            {
                updaterecord.ExamineStatus = Models.ExamineStatusEnum.Approved;
                CommissionTypeUpdateModel commissionTypeUpdateModel = JsonConvert.DeserializeObject<CommissionTypeUpdateModel>(updaterecord.UpdateContent);
                building.CommissionPlan = commissionTypeUpdateModel.CommissionPlan;
                Context.Update(updaterecord);
                Context.Update(building);
            }
            else if (examineResponse.ExamineStatus == ExamineStatus.Reject)
            {
                updaterecord.ExamineStatus = Models.ExamineStatusEnum.Reject;
                Context.Update(updaterecord);
            }
            else
            {
                throw new Exception("审核状态不正确");
            }
            try
            {
                Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task BuildingNoCallbackAsync(ExamineResponse examineResponse)
        {
            var updaterecord = await Context.UpdateRecords.FirstOrDefaultAsync(a => a.Id == examineResponse.SubmitDefineId);
            if (updaterecord == null)
            {
                throw new Exception("未找到动态记录");
            }
            var builindnos = Context.BuildingNos.Where(a => a.BuildingId == examineResponse.ContentId);
            if (examineResponse.ExamineStatus == ExamineStatus.Examined)
            {
                updaterecord.ExamineStatus = Models.ExamineStatusEnum.Approved;
                if (await builindnos.CountAsync() > 0)
                {
                    Context.RemoveRange(builindnos);
                }
                List<BuildingNoCreateRequest> list = JsonConvert.DeserializeObject<List<BuildingNoCreateRequest>>(updaterecord.UpdateContent);
                List<BuildingNo> nolist = new List<BuildingNo>();
                foreach (var item in list)
                {
                    nolist.Add(new BuildingNo
                    {
                        Id = Guid.NewGuid().ToString(),
                        BuildingId = examineResponse.ContentId,
                        Storied = item.Storied,
                        OpenDate = item.OpenDate,
                        DeliveryDate = item.DeliveryDate,
                        UserId = updaterecord.UserId,
                        OrganizationId = updaterecord.OrganizationId,
                        CreateUser = updaterecord.UserId,
                        CreateTime = DateTime.Now
                    });
                }
                Context.Update(updaterecord);
                Context.AddRange(nolist);
            }
            else if (examineResponse.ExamineStatus == ExamineStatus.Reject)
            {
                updaterecord.ExamineStatus = Models.ExamineStatusEnum.Reject;
                Context.Update(updaterecord);
            }
            else
            {
                throw new Exception("审核状态不正确");
            }
            try
            {
                Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DiscountPolicyCallbackAsync(ExamineResponse examineResponse)
        {
            var updaterecord = await Context.UpdateRecords.FirstOrDefaultAsync(a => a.Id == examineResponse.SubmitDefineId);
            if (updaterecord == null)
            {
                throw new Exception("未找到动态记录");
            }
            var buildingShop = await Context.BuildingShopInfos.FirstOrDefaultAsync(a => a.Id == examineResponse.ContentId);
            if (buildingShop == null)
            {
                throw new Exception("未找到楼盘");
            }
            if (examineResponse.ExamineStatus == ExamineStatus.Examined)
            {
                updaterecord.ExamineStatus = Models.ExamineStatusEnum.Approved;
                DiscountPolicyUpdateModel discountPolicyUpdateModel = JsonConvert.DeserializeObject<DiscountPolicyUpdateModel>(updaterecord.UpdateContent);
                buildingShop.PreferentialPolicies = discountPolicyUpdateModel.PreferentialPolicies;
                Context.Update(updaterecord);
                Context.Update(buildingShop);
            }
            else if (examineResponse.ExamineStatus == ExamineStatus.Reject)
            {
                updaterecord.ExamineStatus = Models.ExamineStatusEnum.Reject;
                Context.Update(updaterecord);
            }
            else
            {
                throw new Exception("审核状态不正确");
            }
            try
            {
                Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

        }

        public async Task PriceCallbackAsync(ExamineResponse examineResponse)
        {
            var updaterecord = await Context.UpdateRecords.FirstOrDefaultAsync(a => a.Id == examineResponse.SubmitDefineId);
            if (updaterecord == null)
            {
                throw new Exception("未找到动态记录");
            }
            var shops = await Context.ShopBaseInfos.FirstOrDefaultAsync(a => a.Id == examineResponse.ContentId);
            if (shops == null)
            {
                throw new Exception("未找到商铺");
            }
            if (examineResponse.ExamineStatus == ExamineStatus.Examined)
            {
                updaterecord.ExamineStatus = Models.ExamineStatusEnum.Approved;
                ShopPriceUpdateModel shopPriceUpdateModel = JsonConvert.DeserializeObject<ShopPriceUpdateModel>(updaterecord.UpdateContent);
                shops.GuidingPrice = shopPriceUpdateModel.GuidingPrice;
                shops.TotalPrice = shopPriceUpdateModel.TotalPrice;
                shops.Price = shopPriceUpdateModel.TotalPrice / (decimal)(shops.BuildingArea.HasValue ? shops.HouseArea.Value : 1);
                Context.Update(updaterecord);
                Context.Update(shops);
            }
            else if (examineResponse.ExamineStatus == ExamineStatus.Reject)
            {
                updaterecord.ExamineStatus = Models.ExamineStatusEnum.Reject;
                Context.Update(updaterecord);
            }
            else
            {
                throw new Exception("审核状态不正确");
            }
            try
            {
                Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }
    }
}
