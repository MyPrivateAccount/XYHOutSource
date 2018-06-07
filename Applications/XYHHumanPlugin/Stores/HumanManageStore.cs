using System;
using System.Collections.Generic;
using System.Text;
using XYHHumanPlugin.Stores;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XYHHumanPlugin.Models;
using XYHHumanPlugin.Dto.Response;
using System.Linq;
using ApplicationCore.Models;
using Dapper;

namespace XYHHumanPlugin.Stores
{
    public class HumanManageStore : IHumanManageStore
    {

        public HumanManageStore(HumanDbContext hudb)
        {
            Context = hudb;
        }

        protected HumanDbContext Context { get; }

        public IEnumerable<T> DapperSelect<T>(string sql)
        {
            return Context.Database.GetDbConnection().Query<T>(sql);
        }

        public async Task<HumanInfo> CreateAsync(SimpleUser userinfo, HumanInfo humaninfo, string modifyid, string checkaction, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userinfo == null)
            {
                throw new ArgumentNullException(nameof(userinfo));
            }

            if (string.IsNullOrEmpty(modifyid))
            {
                modifyid = Guid.NewGuid().ToString();
            }

            var modify = new ModifyInfo();
            modify.ID = modifyid;
            modify.Type = 1;//创建
            modify.IDCard = humaninfo.IDCard;
            modify.ModifyPepole = userinfo.Id;
            modify.ModifyStartTime = DateTime.Now;
            modify.ExamineStatus = (int)ExamineStatusEnum.Auditing;
            modify.ExamineTime = modify.ModifyStartTime;
            modify.ModifyCheck = checkaction;

            humaninfo.CreateUser = userinfo.Id;
            humaninfo.Modify = 1;
            humaninfo.RecentModify = modifyid;
            humaninfo.CreateTime = DateTime.Now;
            humaninfo.StaffStatus = 0;//回调2
            Context.Add(humaninfo);
            Context.Add(modify);

            await Context.SaveChangesAsync(cancellationToken);
            return humaninfo;
        }

        public Task CreateAsync(AnnexInfo annexinfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (annexinfo == null)
            {
                throw new ArgumentNullException(nameof(annexinfo));
            }

            if (string.IsNullOrEmpty(annexinfo.ID))
            {
                annexinfo.ID = Guid.NewGuid().ToString();
            }

            Context.Add(annexinfo);
            return Context.SaveChangesAsync(cancellationToken);
        }

        public Task CreateAsync(FileInfo fileinfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fileinfo == null)
            {
                throw new ArgumentNullException(nameof(fileinfo));
            }


            Context.Add(fileinfo);
            return Context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateListAsync(List<FileInfo> fileInfoList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fileInfoList == null)
            {
                throw new ArgumentNullException(nameof(fileInfoList));
            }
            Context.AddRange(fileInfoList);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateMonthAsync(SimpleUser userinfo, MonthInfo monthinf, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (monthinf == null)
            {
                throw new ArgumentNullException(nameof(monthinf));
            }

            Context.Add(monthinf);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task SetSalaryAsync(SalaryInfo salaryinfo, CancellationToken cle = default(CancellationToken))
        {
            if (salaryinfo == null)
            {
                throw new ArgumentNullException(nameof(salaryinfo));
            }

            if (Context.PositionInfos.Any(x => x.ID == salaryinfo.ID))
            {
                Context.Attach(salaryinfo);
                Context.Update(salaryinfo);
            }
            else
            {
                Context.Add(salaryinfo);
            }

            await Context.SaveChangesAsync(cle);
        }

        public async Task SetBlackAsync(BlackInfo salaryinfo, CancellationToken cle = default(CancellationToken))
        {
            if (salaryinfo == null)
            {
                throw new ArgumentNullException(nameof(salaryinfo));
            }

            if (Context.BlackInfos.Any(x => x.IDCard == salaryinfo.IDCard))
            {
                Context.Attach(salaryinfo);
                Context.Update(salaryinfo);
            }
            else
            {
                Context.Add(salaryinfo);
            }

            await Context.SaveChangesAsync(cle);
        }

        public async Task CreateMonthSalaryAsync(SalaryFormInfo forminfo, CancellationToken cle = default(CancellationToken))
        {
            if (forminfo == null)
            {
                throw new ArgumentNullException(nameof(forminfo));
            }

            Context.Add(forminfo);
            await Context.SaveChangesAsync(cle);
        }

        public async Task CreateMonthAttendanceAsync(AttendanceFormInfo forminfo, CancellationToken cle = default(CancellationToken))
        {
            if (forminfo == null)
            {
                throw new ArgumentNullException(nameof(forminfo));
            }

            Context.Add(forminfo);
            await Context.SaveChangesAsync(cle);
        }

        public async Task SetStationAsync(PositionInfo positioninfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (positioninfo == null)
            {
                throw new ArgumentNullException(nameof(positioninfo));
            }

            if (Context.PositionInfos.Any(x => x.ID == positioninfo.ID))
            {
                Context.Attach(positioninfo);
                Context.Update(positioninfo);
            }
            else
            {
                Context.Add(positioninfo);
            }

            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task BecomeHuman(SocialInsurance info, string huid, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!string.IsNullOrEmpty(info.IDCard))
            {
                HumanInfo buildings = new HumanInfo()
                {
                    ID = huid,
                    IsSocialInsurance = info.IsSocial,
                    SocialInsuranceInfo = info.IDCard,
                    BecomeTime = info.EnTime,
                    StaffStatus = 3
                };

                Context.Attach(buildings);
                var entry = Context.Entry(buildings);
                entry.Property(x => x.IsSocialInsurance).IsModified = true;
                entry.Property(x => x.SocialInsuranceInfo).IsModified = true;
                entry.Property(x => x.BecomeTime).IsModified = true;
                entry.Property(x => x.StaffStatus).IsModified = true;

                Context.Add(info);
                await Context.SaveChangesAsync(cancellationToken);
            }
            else {
                throw new ArgumentNullException("have no IDCard");
            }
            
        }

        public async Task LeaveHuman(LeaveInfo info, string huid, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }
            HumanInfo buildings = new HumanInfo()
            {
                ID = huid,
                LeaveTime = DateTime.Now,
                StaffStatus = 1
            };

            Context.Add(info);
            Context.Attach(buildings);
            var entry = Context.Entry(buildings);
            entry.Property(x => x.LeaveTime).IsModified = true;
            entry.Property(x => x.StaffStatus).IsModified = true;
            
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task Task ChangeHuman(ChangeInfo info, string huid, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            HumanInfo buildings = new HumanInfo()
            {
                ID = huid,
                Position = info.NewPosition,
                DepartmentId = info.NewDepartmentId,
                BaseSalary = info.BaseSalary,
                Subsidy = info.Subsidy,
                ClothesBack = info.ClothesBack,
                AdministrativeBack = info.AdministrativeBack,
                PortBack = info.PortBack,
                OtherBack = info.OtherBack
            };

            Context.Add(info);
            Context.Attach(buildings);
            var entry = Context.Entry(buildings);
            entry.Property(x => x.Position).IsModified = true;
            entry.Property(x => x.DepartmentId).IsModified = true;
            entry.Property(x => x.BaseSalary).IsModified = true;
            entry.Property(x => x.Subsidy).IsModified = true;
            entry.Property(x => x.ClothesBack).IsModified = true;
            entry.Property(x => x.AdministrativeBack).IsModified = true;
            entry.Property(x => x.PortBack).IsModified = true;
            entry.Property(x => x.OtherBack).IsModified = true;
            
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteStationAsync(PositionInfo positioninfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (positioninfo == null)
            {
                throw new ArgumentNullException(nameof(positioninfo));
            }
            Context.Remove(positioninfo);
            
            await Context.SaveChangesAsync(cancellationToken);
           
        }

        public async Task DeleteSalaryAsync(SalaryInfo monthinfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (monthinfo == null)
            {
                throw new ArgumentNullException(nameof(monthinfo));
            }
            Context.Remove(monthinfo);

            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteBlackAsync(BlackInfo monthinfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (monthinfo == null)
            {
                throw new ArgumentNullException(nameof(monthinfo));
            }
            Context.Remove(monthinfo);

            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(HumanInfo userinfo, string contractid, CancellationToken cancellationToken = default(CancellationToken))
        { }

        public async Task DeleteAsync(MonthInfo monthinfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (monthinfo == null)
            {
                throw new ArgumentNullException(nameof(monthinfo));
            }
            Context.Remove(monthinfo);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        
        public async Task DeleteListAsync(List<HumanInfo> buildingBaseList, CancellationToken cancellationToken = default(CancellationToken))
        { }

        public async Task DeleteListAsync(List<MonthInfo> monthList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (monthList == null)
            {
                throw new ArgumentNullException(nameof(monthList));
            }
            Context.RemoveRange(monthList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<SalaryFormInfo> monthList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (monthList == null)
            {
                throw new ArgumentNullException(nameof(monthList));
            }
            Context.RemoveRange(monthList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }
        public async Task DeleteListAsync(List<AttendanceFormInfo> monthList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (monthList == null)
            {
                throw new ArgumentNullException(nameof(monthList));
            }
            Context.RemoveRange(monthList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<HumanInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.HumanInfos.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        public Task<List<TResult>> GetFileListAsync<TResult>(Func<IQueryable<FileInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.FileInfos.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public Task<List<TResult>> GetScopeFileListAsync<TResult>(Func<IQueryable<AnnexInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.AnnexInfos.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public Task<TResult> GetSalaryAsync<TResult>(Func<IQueryable<SalaryInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.SalaryInfos.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        public Task<List<TResult>> GetStationListAsync<TResult>(Func<IQueryable<PositionInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.PositionInfos.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public Task<TResult> GetMonthAsync<TResult>(Func<IQueryable<MonthInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.MonthInfos.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }
        public Task<List<TResult>> GetHumanListAsync<TResult>(Func<IQueryable<HumanInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.HumanInfos.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public Task<List<TResult>> GetListSalaryFormAsync<TResult>(Func<IQueryable<SalaryFormInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.SalaryFormInfos.AsNoTracking()).ToListAsync(cancellationToken);
        }
        public Task<List<TResult>> GetListAttendanceFormAsync<TResult>(Func<IQueryable<AttendanceFormInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.AttendanceFormInfos.AsNoTracking()).ToListAsync(cancellationToken);
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

        public Task<List<TResult>> GetListMonthAsync<TResult>(Func<IQueryable<MonthInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.MonthInfos.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(HumanInfo buildingBase, CancellationToken cancellationToken = default(CancellationToken))
        {
        }

        public async Task UpdateListAsync(List<HumanInfo> buildingBaseList, CancellationToken cancellationToken = default(CancellationToken))
        { }

        public async Task SaveAsync(HumanInfo huinfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            
        }
        public async Task UpdateExamineStatus(string modifyId, ExamineStatusEnum status, int type, CancellationToken cancellationToken = default(CancellationToken))
        {
            ModifyInfo buildings = new ModifyInfo()
            {
                ID = modifyId,
                ExamineTime = DateTime.Now,
                ExamineStatus = (int)status,
                Type = type
            };
            Context.Attach(buildings);
            var entry = Context.Entry(buildings);
            entry.Property(x => x.ExamineStatus).IsModified = true;
            entry.Property(x => x.Type).IsModified = true;
            entry.Property(x => x.ExamineTime).IsModified = true;
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { throw; }
        }
    }
}
