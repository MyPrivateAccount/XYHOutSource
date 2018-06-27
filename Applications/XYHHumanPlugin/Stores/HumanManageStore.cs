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
using ApplicationCore;

namespace XYHHumanPlugin.Stores
{
    public class HumanManageStore : IHumanManageStore
    {
        //一共俩份
        public const int CreateNOModifyType = 0;//无
        public const int CreateHumanModifyType = 1;//未入
        public const int EntryHumanModifyType = 3;//入职
        public const int BecomeHumanModifyType = 5;//转正
        public const int ChangeHumanModifyType = 6;//异动
        public const int LeaveHumanModifyType = 7;//离职

        public const int HumanBlackType = -1;//-1黑名单 0 未入职 1离职 2入职 3转正 
        public const int HumanCreateType = 0;
        public const int HumanLeaveType = 1;
        public const int HumanEntryType = 2;
        public const int HumanBecomeType = 3;

        public HumanManageStore(HumanDbContext hudb)
        {
            Context = hudb;
        }

        protected HumanDbContext Context { get; }

        public IQueryable<HumanInfo> SimpleQuery
        {
            get
            {
                var q = from hr in Context.HumanInfos.AsNoTracking()
                        join oe1 in Context.OrganizationExpansions.AsNoTracking() on new { hr.DepartmentId, Type = "Region" } equals new { DepartmentId = oe1.SonId, Type = oe1.Type } into oe2
                        from oe in oe2.DefaultIfEmpty()
                        join o1 in Context.Organizations.AsNoTracking() on hr.DepartmentId equals o1.Id into o2
                        from o in o2.DefaultIfEmpty()
                        join p1 in Context.PositionInfos.AsNoTracking() on hr.Position equals p1.ID into p2
                        from p in p2.DefaultIfEmpty()
                        select new HumanInfo()
                        {
                            Id = hr.Id,
                            UserID = hr.UserID,
                            Name = hr.Name,
                            Position = hr.Position,
                            DepartmentId = hr.DepartmentId,
                            PositionInfo = new PositionInfo()
                            {
                                ID = p.ID,
                                PositionName = p.PositionName,
                                PositionType = p.PositionType
                            },
                            Organizations = new Organizations()
                            {
                                Id = o.Id,
                                OrganizationName = o.OrganizationName,
                                Type = o.Type
                            },
                            OrganizationExpansion = new OrganizationExpansion()
                            {
                                OrganizationId = oe.OrganizationId,
                                SonId = oe.SonId,
                                FullName = oe.FullName
                            }
                        };
                return q;
            }
        }


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
            modify.Type = CreateHumanModifyType;//创建
            modify.IDCard = humaninfo.IDCard;
            modify.ModifyPepole = userinfo.Id;
            modify.ModifyStartTime = DateTime.Now;
            modify.ExamineStatus = (int)ExamineStatusEnum.Auditing;
            modify.ExamineTime = modify.ModifyStartTime;
            modify.ModifyCheck = checkaction;
            modify.Ext1 = humaninfo.Id;
            modify.Ext2 = humaninfo.Name;

            humaninfo.CreateUser = userinfo.Id;
            humaninfo.Modify = 1;
            humaninfo.RecentModify = modifyid;
            humaninfo.CreateTime = DateTime.Now;
            humaninfo.StaffStatus = StaffStatus.NonEntry;//-1黑名单 0 未入职 1离职 2入职 3转正 
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

        public async Task SetBlackAsync(BlackInfo salaryinfo, string id = null, CancellationToken cle = default(CancellationToken))
        {
            if (salaryinfo == null)
            {
                throw new ArgumentNullException(nameof(salaryinfo));
            }

            if (id != null)
            {
                HumanInfo buildings = new HumanInfo()
                {
                    Id = id,
                    StaffStatus = StaffStatus.Leave
                };

                Context.Attach(buildings);
                var entry = Context.Entry(buildings);
                entry.Property(x => x.StaffStatus).IsModified = true;
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

        public async Task SetAttendanceSettingAsync(AttendanceSettingInfo atteninfo, CancellationToken cle = default(CancellationToken))
        {
            if (atteninfo == null)
            {
                throw new ArgumentNullException(nameof(atteninfo));
            }

            if (Context.AttendanceSettingInfos.Any(x => x.Type == atteninfo.Type))
            {
                Context.Attach(atteninfo);
                Context.Update(atteninfo);
            }
            else
            {
                Context.Add(atteninfo);
            }

            await Context.SaveChangesAsync(cle);
        }

        public async Task AddAttendanceAsync(List<AttendanceInfo> atteninfo, CancellationToken cle = default(CancellationToken))
        {
            if (atteninfo == null)
            {
                throw new ArgumentNullException(nameof(atteninfo));
            }

            Context.AddRange(atteninfo);

            await Context.SaveChangesAsync(cle);
        }


        public async Task PreBecomeHuman(SimpleUser userinfo, string modifyid, string huid, string info, string idcard, string checkaction, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(modifyid))
            {
                modifyid = Guid.NewGuid().ToString();
            }

            var modify = new ModifyInfo();
            modify.ID = modifyid;
            modify.Type = BecomeHumanModifyType;
            modify.IDCard = idcard;
            modify.ModifyPepole = userinfo.Id;
            modify.ModifyStartTime = DateTime.Now;
            modify.ExamineStatus = (int)ExamineStatusEnum.Auditing;
            modify.ExamineTime = modify.ModifyStartTime;
            modify.ModifyCheck = checkaction;
            modify.Ext1 = huid;
            modify.Ext2 = info;

            Context.Add(modify);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task BecomeHuman(SocialInsurance info, string huid, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!string.IsNullOrEmpty(info.IDCard))
            {
                HumanInfo buildings = new HumanInfo()
                {
                    Id = huid,
                    //IsSocialInsurance = info.IsSocial,
                    //SocialInsuranceInfo = info.IDCard,
                    BecomeTime = info.EnTime,
                    StaffStatus = StaffStatus.Regular
                };

                Context.Attach(buildings);
                var entry = Context.Entry(buildings);
                //entry.Property(x => x.IsSocialInsurance).IsModified = true;
                //entry.Property(x => x.SocialInsuranceInfo).IsModified = true;
                entry.Property(x => x.BecomeTime).IsModified = true;
                entry.Property(x => x.StaffStatus).IsModified = true;

                if (Context.SocialInsurances.Any(x => x.IDCard == info.IDCard))
                {
                    Context.Attach(info);
                    Context.Update(info);
                }
                else
                {
                    Context.Add(info);
                }

                await Context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                throw new ArgumentNullException("have no IDCard");
            }

        }


        public async Task PreLeaveHuman(SimpleUser userinfo, string modifyid, string huid, string info, string idcard, string checkaction, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(modifyid))
            {
                modifyid = Guid.NewGuid().ToString();
            }

            var modify = new ModifyInfo();
            modify.ID = modifyid;
            modify.Type = LeaveHumanModifyType;
            modify.IDCard = idcard;
            modify.ModifyPepole = userinfo.Id;
            modify.ModifyStartTime = DateTime.Now;
            modify.ExamineStatus = (int)ExamineStatusEnum.Auditing;
            modify.ExamineTime = modify.ModifyStartTime;
            modify.ModifyCheck = checkaction;
            modify.Ext1 = huid;
            modify.Ext2 = info;

            Context.Add(modify);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task LeaveHuman(LeaveInfo info, string huid, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }
            HumanInfo buildings = new HumanInfo()
            {
                Id = huid,
                LeaveTime = DateTime.Now,
                StaffStatus = StaffStatus.Leave
            };

            if (Context.LeaveInfos.Any(x => x.IDCard == info.IDCard))
            {
                Context.Attach(info);
                Context.Update(info);
            }
            else
            {
                Context.Add(info);
            }


            Context.Attach(buildings);
            var entry = Context.Entry(buildings);
            entry.Property(x => x.LeaveTime).IsModified = true;
            entry.Property(x => x.StaffStatus).IsModified = true;

            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task PreChangeHuman(SimpleUser userinfo, string modifyid, string huid, string info, string idcard, string checkaction, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(modifyid))
            {
                modifyid = Guid.NewGuid().ToString();
            }

            var modify = new ModifyInfo();
            modify.ID = modifyid;
            modify.Type = ChangeHumanModifyType;
            modify.IDCard = idcard;
            modify.ModifyPepole = userinfo.Id;
            modify.ModifyStartTime = DateTime.Now;
            modify.ExamineStatus = (int)ExamineStatusEnum.Auditing;
            modify.ExamineTime = modify.ModifyStartTime;
            modify.ModifyCheck = checkaction;
            modify.Ext1 = huid;
            modify.Ext2 = info;

            Context.Add(modify);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task ChangeHuman(ChangeInfo info, string huid, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            HumanInfo buildings = new HumanInfo()
            {
                Id = huid,
                Position = info.NewPosition,
                DepartmentId = info.NewDepartmentId,
                //BaseSalary = info.BaseSalary,
                //Subsidy = info.Subsidy,
                ClothesBack = info.ClothesBack,
                AdministrativeBack = info.AdministrativeBack,
                PortBack = info.PortBack,
                OtherBack = info.OtherBack
            };

            if (Context.ChangeInfos.Any(x => x.IDCard == info.IDCard))
            {
                Context.Attach(info);
                Context.Update(info);
            }
            else
            {
                Context.Add(info);
            }

            Context.Attach(buildings);
            var entry = Context.Entry(buildings);
            entry.Property(x => x.Position).IsModified = true;
            entry.Property(x => x.DepartmentId).IsModified = true;
            //entry.Property(x => x.BaseSalary).IsModified = true;
            //entry.Property(x => x.Subsidy).IsModified = true;
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

        public async Task AddRPInfoeAsync(RewardPunishmentInfo rpinfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (rpinfo == null)
            {
                throw new ArgumentNullException(nameof(rpinfo));
            }

            Context.Add(rpinfo);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteRPInfoeAsync(RewardPunishmentInfo rpinfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (rpinfo == null)
            {
                throw new ArgumentNullException(nameof(rpinfo));
            }
            
            Context.Remove(rpinfo);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAttendenceAsync(AttendanceInfo monthinfo, CancellationToken cancellationToken = default(CancellationToken))
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

        public async Task<List<Organizations>> GetAllOrganization()
        {
            return Context.Organizations.Where(a => a.Id != "").ToList();
        }

        public async Task<SocialInsurance> GetSocialInfoAsync(string idcard)
        {
            return Context.SocialInsurances.AsNoTracking().Where(x => (x.IDCard == idcard)).FirstOrDefault();
        }
        public async Task<string> GetOrganizationFullName(string departmentid)
        {
            if (string.IsNullOrEmpty(departmentid))
            {
                return "未找到相应部门";
            }

            var response = Context.OrganizationExpansions.AsNoTracking().Where(x => (x.SonId == departmentid) || (x.OrganizationId == departmentid)).FirstOrDefault();
            return response == null ? "未找到相应部门" : response.FullName;
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

        public Task<TResult> GetHumanAsync<TResult>(Func<IQueryable<HumanInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.HumanInfos.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        public Task<TResult> GetStationAsync<TResult>(Func<IQueryable<PositionInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.PositionInfos.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
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

        public Task<List<AttendanceSettingInfo>> GetListAttendanceSettingAsync(CancellationToken cancellationToken = default(CancellationToken))
        {

            return Context.AttendanceSettingInfos.ToListAsync(cancellationToken);
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

        public async Task<ModifyInfo> UpdateExamineStatus(string modifyId, ExamineStatusEnum status, CancellationToken cancellationToken = default(CancellationToken))
        {
            var modify = await GetModifyAsync(a => a.Where(b => b.ID == modifyId));
            if (modify != null)
            {
                switch (modify.Type)
                {
                    case CreateHumanModifyType:
                        {
                            HumanInfo buildings = new HumanInfo()
                            {
                                Id = modify.Ext1,
                                StaffStatus = StaffStatus.Entry
                            };

                            Context.Attach(buildings);
                            var entry = Context.Entry(buildings);
                            entry.Property(x => x.StaffStatus).IsModified = true;

                        }
                        break;

                    case BecomeHumanModifyType:
                        {
                            SocialInsurance responinfo = JsonHelper.ToObject<SocialInsurance>(modify.Ext2);
                            await BecomeHuman(responinfo, modify.Ext1, cancellationToken);
                        }
                        break;

                    case ChangeHumanModifyType:
                        {
                            ChangeInfo responinfo = JsonHelper.ToObject<ChangeInfo>(modify.Ext2);
                            await ChangeHuman(responinfo, modify.Ext1, cancellationToken);
                        }
                        break;

                    case LeaveHumanModifyType:
                        {
                            LeaveInfo responinfo = JsonHelper.ToObject<LeaveInfo>(modify.Ext2);
                            await LeaveHuman(responinfo, modify.Ext1, cancellationToken);
                        }
                        break;

                    default: break;
                }

                /////////////////////
                ModifyInfo mbuildings = new ModifyInfo()
                {
                    ID = modifyId,
                    ExamineTime = DateTime.Now,
                    ExamineStatus = (int)status,
                };
                Context.Attach(mbuildings);
                var mentry = Context.Entry(mbuildings);
                mentry.Property(x => x.ExamineStatus).IsModified = true;
                mentry.Property(x => x.ExamineTime).IsModified = true;

                await Context.SaveChangesAsync(cancellationToken);
            }
            return modify;
        }
    }
}
