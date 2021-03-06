﻿using System;
using System.Collections.Generic;
using System.Text;
using XYHContractPlugin.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Models;
using XYHContractPlugin.Dto.Response;

namespace XYHContractPlugin.Stores
{
    public class ContractInfoStore : IContractInfoStore
    {
        public ContractInfoStore(ContractDbContext contractdbcont)
        {
            Context = contractdbcont;
            ContractInfos = Context.ContractInfos;
        }

        protected ContractDbContext Context { get; }
        public IQueryable<ContractInfo> ContractInfos { get; set; }

        public IEnumerable<T> DapperSelect<T>(string sql)
        {
            return Context.Database.GetDbConnection().Query<T>(sql);
        }

        public async Task<ContractInfo> CreateAsync(SimpleUser userinfo, ContractInfo buildingBaseInfo, string modifyid, string checkaction, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfo == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfo));
            }

            if (string.IsNullOrEmpty(modifyid))
            {
                modifyid = Guid.NewGuid().ToString();
            }

            var modify = new ModifyInfo();
            modify.ID = modifyid;
            modify.Type = 1;//创建
            modify.ContractID = buildingBaseInfo.ID;
            modify.ModifyPepole = userinfo.Id;
            modify.ModifyStartTime = DateTime.Now;
            modify.ExamineStatus = (int)ExamineStatusEnum.Auditing;
            modify.ExamineTime = modify.ModifyStartTime;
            modify.ModifyCheck = checkaction;


            buildingBaseInfo.IsDelete = false;
            buildingBaseInfo.CreateUser = userinfo.Id;
            buildingBaseInfo.CreateDepartment = userinfo.OrganizationName;
            buildingBaseInfo.Modify = 1;
            buildingBaseInfo.CurrentModify = modifyid;
            buildingBaseInfo.CreateTime = DateTime.Now;
            Context.Add(buildingBaseInfo);
            Context.Add(modify);
            await Context.SaveChangesAsync(cancellationToken);
            return buildingBaseInfo;
        }
        public async Task<ContractInfo> CreateAsync(SimpleUser userinfo, ContractInfo buildingBaseInfo, string modifyid, string ext1, string ext2,string checkaction, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfo == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfo));
            }

            if (string.IsNullOrEmpty(modifyid))
            {
                modifyid = Guid.NewGuid().ToString();
            }

            var modify = new ModifyInfo();
            modify.ID = modifyid;
            modify.Type = 1;//创建
            modify.ContractID = buildingBaseInfo.ID;
            modify.ModifyPepole = userinfo.Id;
            modify.ModifyStartTime = DateTime.Now;
            modify.ExamineStatus = (int)ExamineStatusEnum.Auditing;
            modify.ExamineTime = modify.ModifyStartTime;
            modify.ModifyCheck = checkaction;
            modify.Ext1 = ext1;
            modify.Ext2 = ext2;

            buildingBaseInfo.IsDelete = false;
            buildingBaseInfo.CreateUser = userinfo.Id;
            buildingBaseInfo.CreateDepartment = userinfo.OrganizationName;
            buildingBaseInfo.Modify = 1;
            buildingBaseInfo.CurrentModify = modifyid;
            buildingBaseInfo.CreateTime = DateTime.Now;
            Context.Add(buildingBaseInfo);
            Context.Add(modify);
            await Context.SaveChangesAsync(cancellationToken);
            return buildingBaseInfo;
        }

        public async Task CreateModifyAsync(SimpleUser userinfo, string contractid, string modifyid, int ntype, string checkaction, ExamineStatusEnum exa = ExamineStatusEnum.UnSubmit,
            bool updatetocontract = true, string ext1 = null, string ext2= null, string ext3 = null, string ext4= null, string ext5 = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (contractid != null)
            {
                if (string.IsNullOrEmpty(modifyid))
                {
                    modifyid = Guid.NewGuid().ToString();
                }

                var tmodify = new ModifyInfo();
                tmodify.ID = modifyid;
                tmodify.ContractID = contractid;
                tmodify.ModifyStartTime = DateTime.Now;
                tmodify.ExamineTime = tmodify.ModifyStartTime;
                tmodify.Type = ntype;
                tmodify.ModifyPepole = userinfo.Id;
                tmodify.ExamineStatus = (int)exa;
                tmodify.Ext1 = ext1;
                tmodify.Ext2 = ext2;
                tmodify.Ext3 = ext3;
                tmodify.Ext4 = ext4;
                tmodify.Ext5 = ext5;
                tmodify.ModifyCheck = checkaction;

                if (updatetocontract)
                {
                    ContractInfo info = new ContractInfo()
                    {
                        ID = contractid,
                        CurrentModify = modifyid
                    };
                    var entry = Context.Attach(info);
                    entry.Property(x => x.CurrentModify).IsModified = true;
                }
               
                Context.Add(tmodify);
                await Context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<bool> CreateAsync(SimpleUser userinfo, List<AnnexInfo> annexinfo, CancellationToken cancle = default(CancellationToken))
        {
            if (annexinfo == null)
            {
                throw new ArgumentNullException(nameof(annexinfo));
            }

            if (annexinfo.Count > 0)
            {
                Context.AddRange(annexinfo);
                await Context.SaveChangesAsync(cancle);
            }
            
            return true;
        }

        public async Task<bool> AutoCreateAsync(SimpleUser userinfo, List<ComplementInfo> compleinfo, CancellationToken cancle = default(CancellationToken))
        {
            if (compleinfo == null)
            {
                throw new ArgumentNullException(nameof(compleinfo));
            }

            if (compleinfo.Count > 0)
            {
                foreach (var item in compleinfo)
                {
                    if (!Context.ComplementInfos.Any(x => x.ID == item.ID))
                    {
                        Context.Add(item);
                    }
                    else
                    {
                        //Context.Attach(file);
                        Context.Update(item);
                    }
                }

                await Context.SaveChangesAsync(cancle);
            }

            return true;
        }

        public async Task<bool> CreateAsync(SimpleUser userinfo, List<ComplementInfo> compleinfo, CancellationToken cancle = default(CancellationToken))
        {
            if (compleinfo == null)
            {
                throw new ArgumentNullException(nameof(compleinfo));
            }

            if (compleinfo.Count > 0)
            {
                Context.AddRange(compleinfo);
                await Context.SaveChangesAsync(cancle);
            }
            
            return true;
        }

        public async Task DeleteAsync(SimpleUser userinfo, string contractid, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (contractid == null)
            {
                throw new ArgumentNullException(nameof(contractid));
            }
            //Context.Remove(areaDefine);
            ContractInfo buildings = new ContractInfo()
            {
                ID = contractid,
                DeleteUser = userinfo.Id,
                DeleteTime = DateTime.Now,
                IsDelete = true
            };

            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<ContractInfo> areaDefineList, CancellationToken cancellationToken = default(CancellationToken))
        {           
            if (areaDefineList == null)
            {
                throw new ArgumentNullException(nameof(areaDefineList));
            }
            Context.RemoveRange(areaDefineList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<ContractInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ContractInfos.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<ContractInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ContractInfos.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public Task<TResult> GetModifyAsync<TResult>(Func<IQueryable<ModifyInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ModifyInfos.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        public Task<List<TResult>> GetListModifyAsync<TResult>(Func<IQueryable<ModifyInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ModifyInfos.AsNoTracking()).ToListAsync(cancellationToken);
        }

       public Task<List<TResult>> GetListAnnexAsync<TResult>(Func<IQueryable<AnnexInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.AnnexInfos.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public Task<List<TResult>> GetListComplementAsync<TResult>(Func<IQueryable<ComplementInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ComplementInfos.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(ContractInfo areaDefine, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (areaDefine == null)
            {
                throw new ArgumentNullException(nameof(areaDefine));
            }
            Context.ContractInfos.Attach(areaDefine);
            Context.ContractInfos.Update(areaDefine);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { throw; }
        }

        public async Task UpdateListAsync(List<AnnexInfo> areaDefine, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (areaDefine == null)
            {
                throw new ArgumentNullException(nameof(areaDefine));
            }
            //Context.AnnexInfos.Attach(areaDefine);
            Context.AnnexInfos.UpdateRange(areaDefine);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { throw; }
        }

        public async Task UpdateListAsync(List<ComplementInfo> areaDefine, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (areaDefine == null)
            {
                throw new ArgumentNullException(nameof(areaDefine));
            }
            //Context.ComplementInfos.Attach(areaDefine);
            Context.ComplementInfos.UpdateRange(areaDefine);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { throw; }
        }

        public async Task UpdateListAsync(List<ContractInfo> areaDefineList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (areaDefineList == null)
            {
                throw new ArgumentNullException(nameof(areaDefineList));
            }
            Context.AttachRange(areaDefineList);
            Context.UpdateRange(areaDefineList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        public async Task SaveAsync(SimpleUser user, ContractInfo contractinfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (contractinfo == null)
            {
                throw new ArgumentNullException(nameof(contractinfo));
            }

            //查看合同是否存在
            if (!Context.ContractInfos.Any(x => x.ID == contractinfo.ID))
            {
                ContractInfo buildings = new ContractInfo()
                {
                    ID = contractinfo.ID,
                    CreateUser = user.Id,
                    CreateTime = DateTime.Now,
                    IsDelete = false
                };

                Context.Add(buildings);
            }
            else
                Context.Update(contractinfo);

            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        public async Task UpdateExamineStatus(string modifyId, ExamineStatusEnum status, CancellationToken cancellationToken = default(CancellationToken))
        {
            ModifyInfo buildings = new ModifyInfo()
            {
                ID = modifyId,
                ExamineTime = DateTime.Now,
                ExamineStatus = (int)status
            };
            Context.Attach(buildings);
            var entry = Context.Entry(buildings);
            entry.Property(x => x.ExamineStatus).IsModified = true;
            entry.Property(x => x.ExamineTime).IsModified = true;
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { throw; }
        }
        public IQueryable<ContractInfo> ContractInfoAll()
        { 
            var a = Context.ContractInfos.AsNoTracking();
            var query =
                        from b in Context.ContractInfos.AsNoTracking()
                        join x in Context.ContractInfos.AsNoTracking() on b.FollowId equals x.ID into fol
                        from follow in fol.DefaultIfEmpty()

                        //from a in Context.ContractInfos.AsNoTracking()
                        join c in Context.CompanyAInfo.AsNoTracking() on b.CompanyAId equals c.ID into cy
                        from acy in cy.DefaultIfEmpty()
                            //人事和部门
                        join cu1 in Context.Users.AsNoTracking() on b.CreateUser equals cu1.Id into cu2
                        from cu in cu2.DefaultIfEmpty()

                        join eu1 in Context.Users.AsNoTracking() on b.DeleteUser equals eu1.Id into du2
                        from eu in du2.DefaultIfEmpty()

                        join ru1a in Context.Organizations.AsNoTracking() on b.OrganizateID equals ru1a.Id into ru1b
                        from ru1 in ru1b.DefaultIfEmpty()

                        join cru1a in Context.Organizations.AsNoTracking() on b.CreateDepartmentID equals cru1a.Id into cru1b
                        from cru1 in cru1b.DefaultIfEmpty()

                        join m in Context.ModifyInfos.AsNoTracking() on b.CurrentModify equals m.ID into mod
                        from modify in mod.DefaultIfEmpty()

                        //where !b.IsDelete 
                        //&& acy.IsDelete
                        select new ContractInfo()
                        {
                            ID = b.ID,
                            Type = b.Type,
                            Settleaccounts = b.Settleaccounts,
                            Commission = b.Commission,
                            Relation = b.Relation,
                            Name = b.Name,
                            ContractEstate = b.ContractEstate,
                            Modify = b.Modify,
                            CurrentModify = b.CurrentModify,
                            Annex = b.Annex,
                            Complement = b.Complement,
                            Follow = follow.Name,
                            Remark = b.Remark,
                            ProjectName = b.ProjectName,
                            ProjectType = b.ProjectType,
                            CompanyA = acy.Name,
                            CompanyAT = acy.Type,
                            PrincipalpepoleA = b.PrincipalpepoleA,
                            PrincipalpepoleB = b.PrincipalpepoleB,
                            ProprincipalPepole = b.ProprincipalPepole,
                            CreateUser = cu.UserName,
                            CreateTime = b.CreateTime,
                            FollowId = b.FollowId,
                            ProjectAddress = b.ProjectAddress,
                            FollowTime = b.FollowTime,
                            Ext1 = b.Ext1,
                            Ext2 = b.Ext2,
                            Num = b.Num,
                            IsFollow = b.IsFollow,
                            ReturnOrigin = b.ReturnOrigin,
                            Count = b.Count,
                            StartTime = b.StartTime,
                            EndTime = b.EndTime,
                            CommisionType = b.CommisionType,
                            DeleteTime = b.DeleteTime,
                            CreateDepartmentID = b.CreateDepartmentID,
                            CreateDepartment = cru1.OrganizationName,
                            IsDelete = b.IsDelete,
                            OrganizateID = ru1.Id,
                            Organizate = b.Organizate,//这个后面处理
                            OrganizateFullId = b.OrganizateFullId,
                            DeleteUser = eu.UserName,
                            CompanyAId = b.CompanyAId,
                            ExamineStatus = modify.ExamineStatus,
                        
                        };

      
      

            return query;
        }

    }



}
