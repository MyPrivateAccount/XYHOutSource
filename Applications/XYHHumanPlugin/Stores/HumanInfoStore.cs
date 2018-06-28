using ApplicationCore.Dto;
using ApplicationCore.Models;
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
    public class HumanInfoStore : IHumanInfoStore
    {
        protected HumanDbContext Context { get; }

        public IQueryable<HumanInfo> HumanInfos { get; set; }

        public HumanInfoStore(HumanDbContext context)
        {
            Context = context;
            HumanInfos = Context.HumanInfos;
        }

        public IQueryable<HumanInfo> SimpleQuery
        {
            get
            {
                var q = from hr in Context.HumanInfos.AsNoTracking()
                        join oe1 in Context.OrganizationExpansions.AsNoTracking() on new { hr.DepartmentId, Type = "Region" } equals new { DepartmentId = oe1.SonId, Type = oe1.Type } into oe2
                        from oe in oe2.DefaultIfEmpty()
                        join o1 in Context.Organizations.AsNoTracking() on hr.DepartmentId equals o1.Id into o2
                        from o in o2.DefaultIfEmpty()
                        select new HumanInfo()
                        {
                            Id = hr.Id,
                            UserID = hr.UserID,
                            Name = hr.Name,
                            Position = hr.Position,
                            DepartmentId = hr.DepartmentId,
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


        public IQueryable<HumanInfo> GetQuery()
        {
            var query = from r in Context.HumanInfos.AsNoTracking()
                        select new HumanInfo
                        {

                        };

            return query;
        }


        public IQueryable<HumanInfo> GetDetailQuery()
        {
            var query = from h in Context.HumanInfos.AsNoTracking()
                        join hc1 in Context.HumanContractInfos.AsNoTracking() on h.Id equals hc1.Id into hc2
                        from hc in hc2.DefaultIfEmpty()
                        join hs1 in Context.HumanSalaryStructures.AsNoTracking() on h.Id equals hs1.Id into hs2
                        from hs in hs2.DefaultIfEmpty()
                        join hss1 in Context.HumanSocialSecurities.AsNoTracking() on h.Id equals hss1.Id into hss2
                        from hss in hss2.DefaultIfEmpty()

                        join cu1 in Context.Users.AsNoTracking() on h.CreateUser equals cu1.Id into cu2
                        from cju in cu2.DefaultIfEmpty()
                        join uu1 in Context.Users.AsNoTracking() on h.UpdateUser equals uu1.Id into uu2
                        from uu in uu2.DefaultIfEmpty()
                        join du1 in Context.Users.AsNoTracking() on h.DeleteUser equals du1.Id into du2
                        from du in du2.DefaultIfEmpty()

                        join oe1 in Context.OrganizationExpansions.AsNoTracking() on new { h.DepartmentId, Type = "Region" } equals new { DepartmentId = oe1.SonId, Type = oe1.Type } into oe2
                        from oe in oe2.DefaultIfEmpty()
                        join o1 in Context.Organizations.AsNoTracking() on h.DepartmentId equals o1.Id into o2
                        from o in o2.DefaultIfEmpty()

                        select new HumanInfo()
                        {
                            Id = h.Id,
                            BankAccount = h.BankAccount,
                            BankName = h.BankName,
                            DepartmentId = h.DepartmentId,
                            EmailAddress = h.EmailAddress,
                            Desc = h.Desc,
                            EmergencyContact = h.EmergencyContact,
                            EmergencyContactPhone = h.EmergencyContactPhone,
                            EmergencyContactType = h.EmergencyContactType,
                            EntryTime = h.EntryTime,
                            HighestEducation = h.HighestEducation,
                            HealthCondition = h.HealthCondition,
                            DomicilePlace = h.DomicilePlace,
                            HouseholdType = h.HouseholdType,
                            IDCard = h.IDCard,
                            FamilyAddress = h.FamilyAddress,
                            LeaveTime = h.LeaveTime,
                            BecomeTime = h.BecomeTime,
                            Name = h.Name,
                            NativePlace = h.NativePlace,
                            Nationality = h.Nationality,
                            Picture = h.Picture,
                            Position = h.Position,
                            Phone = h.Phone,
                            PolicitalStatus = h.PolicitalStatus,
                            Sex = h.Sex,
                            UserID = h.UserID,
                            UpdateTime = h.UpdateTime,
                            CreateUser = h.CreateUser,
                            IsDeleted = h.IsDeleted,
                            StaffStatus = h.StaffStatus,
                            UpdateUser = h.UpdateUser,
                            Company = h.Company,

                            AdministrativeBack = h.AdministrativeBack,
                            ClothesBack = h.ClothesBack,
                            Modify = h.Modify,
                            OtherBack = h.OtherBack,
                            PortBack = h.PortBack,
                            RecentModify = h.RecentModify,

                            CreateTime = h.CreateTime,
                            DeleteTime = h.DeleteTime,
                            DeleteUser = h.DeleteUser,

                            HumanContractInfo = hc,
                            HumanSocialSecurity = hss,
                            HumanSalaryStructure = hs,

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
                            },
                            HumanEducationInfos = from he in Context.HumanEducationInfos.AsNoTracking()
                                                  where he.HumanId == h.Id
                                                  select new HumanEducationInfo
                                                  {
                                                      CreateTime = he.CreateTime,
                                                      CreateUser = he.CreateUser,
                                                      GetDegreeCompany = he.GetDegreeCompany,
                                                      DeleteTime = he.DeleteTime,
                                                      DeleteUser = he.DeleteUser,
                                                      IsDeleted = he.IsDeleted,
                                                      GetDegree = he.GetDegree,
                                                      Education = he.Education,
                                                      EnrolmentTime = he.EnrolmentTime,
                                                      GetDegreeTime = he.GetDegreeTime,
                                                      GraduationCertificate = he.GraduationCertificate,
                                                      GraduationSchool = he.GraduationSchool,
                                                      GraduationTime = he.GraduationTime,
                                                      HumanId = he.HumanId,
                                                      Id = he.Id,
                                                      LearningType = he.LearningType,
                                                      Major = he.Major,
                                                      UpdateTime = he.UpdateTime,
                                                      UpdateUser = he.UpdateUser
                                                  },
                            HumanTitleInfos = from ht in Context.HumanTitleInfos.AsNoTracking()
                                              where ht.HumanId == h.Id
                                              select new HumanTitleInfo
                                              {
                                                  CreateTime = ht.CreateTime,
                                                  CreateUser = ht.CreateUser,
                                                  DeleteTime = ht.DeleteTime,
                                                  DeleteUser = ht.DeleteUser,
                                                  GetTitleTime = ht.GetTitleTime,
                                                  HumanId = ht.HumanId,
                                                  Id = ht.Id,
                                                  IsDeleted = ht.IsDeleted,
                                                  TitleName = ht.TitleName,
                                                  UpdateTime = ht.UpdateTime,
                                                  UpdateUser = ht.UpdateUser
                                              },
                            HumanWorkHistories = from hw in Context.HumanWorkHistories.AsNoTracking()
                                                 where hw.HumanId == h.Id
                                                 select new HumanWorkHistory
                                                 {
                                                     Company = hw.Company,
                                                     CreateTime = hw.CreateTime,
                                                     CreateUser = hw.CreateUser,
                                                     DeleteTime = hw.DeleteTime,
                                                     DeleteUser = hw.DeleteUser,
                                                     EndTime = hw.EndTime,
                                                     HumanId = hw.HumanId,
                                                     Id = hw.Id,
                                                     IsDeleted = hw.IsDeleted,
                                                     Position = hw.Position,
                                                     StartTime = hw.StartTime,
                                                     UpdateTime = hw.UpdateTime,
                                                     UpdateUser = hw.UpdateUser
                                                 },
                            FileInfos = from f1 in Context.FileInfos.AsNoTracking()
                                        join file in Context.HumanFileScopes.AsNoTracking() on f1.FileGuid equals file.FileGuid
                                        orderby file.CreateTime descending
                                        where !f1.IsDeleted && file.HumanId == h.Id
                                        select new FileInfo
                                        {
                                            FileGuid = f1.FileGuid,
                                            FileExt = f1.FileExt,
                                            Uri = f1.Uri,
                                            Name = f1.Name,
                                            Type = f1.Type,
                                            Size = f1.Size,
                                            Height = f1.Height,
                                            Width = f1.Width,
                                            Summary = f1.Summary,
                                            Group = f1.Group,
                                            Driver = f1.Driver,
                                            Ext1 = f1.Ext1,
                                            Ext2 = f1.Ext2
                                        }
                        };
            return query;
        }


        public async Task<HumanInfo> SaveAsync(UserInfo user, HumanInfo humanInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfo == null)
            {
                throw new ArgumentNullException(nameof(humanInfo));
            }
            try
            {
                if (!Context.HumanInfos.Any(x => x.Id == humanInfo.Id))
                {
                    humanInfo.IsDeleted = false;
                    humanInfo.CreateTime = DateTime.Now;
                    humanInfo.CreateUser = user.Id;
                    Context.Add(humanInfo);
                    Context.Add(humanInfo.HumanSalaryStructure);
                    Context.Add(humanInfo.HumanSocialSecurity);
                    var HumanTitleInfos = humanInfo.HumanTitleInfos?.ToList();
                    if (HumanTitleInfos != null)
                    {
                        for (int i = 0; i < HumanTitleInfos.Count; i++)
                        {
                            HumanTitleInfos[i].Id = string.IsNullOrEmpty(HumanTitleInfos[i].Id) ? Guid.NewGuid().ToString() : HumanTitleInfos[i].Id;
                            HumanTitleInfos[i].IsDeleted = false;
                            HumanTitleInfos[i].CreateTime = DateTime.Now;
                            HumanTitleInfos[i].CreateUser = user.Id;
                        }
                        Context.AddRange(HumanTitleInfos);
                    }
                    var HumanWorkHistories = humanInfo.HumanWorkHistories?.ToList();
                    if (HumanWorkHistories != null)
                    {
                        for (int i = 0; i < HumanWorkHistories.Count; i++)
                        {
                            HumanWorkHistories[i].Id = string.IsNullOrEmpty(HumanWorkHistories[i].Id) ? Guid.NewGuid().ToString() : HumanWorkHistories[i].Id;

                            HumanWorkHistories[i].IsDeleted = false;
                            HumanWorkHistories[i].CreateTime = DateTime.Now;
                            HumanWorkHistories[i].CreateUser = user.Id;
                        }
                        Context.AddRange(HumanWorkHistories);
                    }
                    var HumanEducationInfos = humanInfo.HumanEducationInfos?.ToList();
                    if (HumanEducationInfos != null)
                    {
                        for (int i = 0; i < HumanEducationInfos.Count; i++)
                        {
                            HumanEducationInfos[i].Id = string.IsNullOrEmpty(HumanEducationInfos[i].Id) ? Guid.NewGuid().ToString() : HumanEducationInfos[i].Id;
                            HumanEducationInfos[i].IsDeleted = false;
                            HumanEducationInfos[i].CreateTime = DateTime.Now;
                            HumanEducationInfos[i].CreateUser = user.Id;
                        }
                        Context.AddRange(HumanEducationInfos);
                    }
                }
                else//更新
                {
                    var old = await GetDetailQuery().Where(x => x.Id == humanInfo.Id).FirstOrDefaultAsync(cancellationToken);
                    old.BankAccount = humanInfo.BankAccount;
                    old.BankName = humanInfo.BankName;
                    old.DepartmentId = humanInfo.DepartmentId;
                    old.EmailAddress = humanInfo.EmailAddress;
                    old.Desc = humanInfo.Desc;
                    old.EmergencyContact = humanInfo.EmergencyContact;
                    old.EmergencyContactPhone = humanInfo.EmergencyContactPhone;
                    old.EmergencyContactType = humanInfo.EmergencyContactType;
                    old.EntryTime = humanInfo.EntryTime;
                    old.HighestEducation = humanInfo.HighestEducation;
                    old.HealthCondition = humanInfo.HealthCondition;
                    old.DomicilePlace = humanInfo.DomicilePlace;
                    old.HouseholdType = humanInfo.HouseholdType;
                    old.IDCard = humanInfo.IDCard;
                    old.FamilyAddress = humanInfo.FamilyAddress;
                    old.LeaveTime = humanInfo.LeaveTime;
                    old.BecomeTime = humanInfo.BecomeTime;
                    old.Name = humanInfo.Name;
                    old.NativePlace = humanInfo.NativePlace;
                    old.Nationality = humanInfo.Nationality;
                    old.Picture = humanInfo.Picture;
                    old.Phone = humanInfo.Phone;
                    old.Position = humanInfo.Position;
                    old.PolicitalStatus = humanInfo.PolicitalStatus;
                    old.Sex = humanInfo.Sex;
                    old.UserID = humanInfo.UserID;
                    old.UpdateTime = DateTime.Now;
                    old.UpdateUser = user.Id;
                    old.StaffStatus = humanInfo.StaffStatus;
                    old.Company = humanInfo.Company;

                    old.AdministrativeBack = humanInfo.AdministrativeBack;
                    old.ClothesBack = humanInfo.ClothesBack;
                    old.Modify = humanInfo.Modify;
                    old.OtherBack = humanInfo.OtherBack;
                    old.PortBack = humanInfo.PortBack;
                    old.RecentModify = humanInfo.RecentModify;

                    Context.Attach(old);
                    Context.Update(old);

                    if (old.HumanContractInfo != null)
                    {
                        old.HumanContractInfo.ContractEndDate = humanInfo.HumanContractInfo.ContractEndDate;
                        old.HumanContractInfo.ContractNo = humanInfo.HumanContractInfo.ContractNo;
                        old.HumanContractInfo.ContractSignDate = humanInfo.HumanContractInfo.ContractSignDate;
                        old.HumanContractInfo.ContractStartDate = humanInfo.HumanContractInfo.ContractStartDate;
                        old.HumanContractInfo.ContractType = humanInfo.HumanContractInfo.ContractType;

                        Context.Attach(old.HumanContractInfo);
                        Context.Update(old.HumanContractInfo);
                    }
                    else if (humanInfo.HumanContractInfo != null)
                    {
                        Context.Add(humanInfo.HumanContractInfo);
                    }
                    if (old.HumanSalaryStructure != null)
                    {
                        old.HumanSalaryStructure.BaseWages = humanInfo.HumanSalaryStructure.BaseWages;
                        old.HumanSalaryStructure.CommunicationAllowance = humanInfo.HumanSalaryStructure.CommunicationAllowance;
                        old.HumanSalaryStructure.OtherAllowance = humanInfo.HumanSalaryStructure.OtherAllowance;
                        old.HumanSalaryStructure.PostWages = humanInfo.HumanSalaryStructure.PostWages;
                        old.HumanSalaryStructure.TrafficAllowance = humanInfo.HumanSalaryStructure.TrafficAllowance;

                        Context.Attach(old.HumanSalaryStructure);
                        Context.Update(old.HumanSalaryStructure);
                    }
                    else if (humanInfo.HumanSalaryStructure != null)
                    {
                        Context.Add(humanInfo.HumanSalaryStructure);
                    }
                    if (old.HumanSocialSecurity != null)
                    {
                        old.HumanSocialSecurity.EmploymentInjuryInsurance = humanInfo.HumanSocialSecurity.EmploymentInjuryInsurance;
                        old.HumanSocialSecurity.EndowmentInsurance = humanInfo.HumanSocialSecurity.EndowmentInsurance;
                        old.HumanSocialSecurity.HousingProvidentFund = humanInfo.HumanSocialSecurity.HousingProvidentFund;
                        old.HumanSocialSecurity.HousingProvidentFundAccount = humanInfo.HumanSocialSecurity.HousingProvidentFundAccount;
                        old.HumanSocialSecurity.InsuredAddress = humanInfo.HumanSocialSecurity.InsuredAddress;
                        old.HumanSocialSecurity.InsuredTime = humanInfo.HumanSocialSecurity.InsuredTime;
                        old.HumanSocialSecurity.IsGiveUp = humanInfo.HumanSocialSecurity.IsGiveUp;
                        old.HumanSocialSecurity.IsSignCommitment = humanInfo.HumanSocialSecurity.IsSignCommitment;
                        old.HumanSocialSecurity.MaternityInsurance = humanInfo.HumanSocialSecurity.MaternityInsurance;
                        old.HumanSocialSecurity.MedicalInsurance = humanInfo.HumanSocialSecurity.MedicalInsurance;
                        old.HumanSocialSecurity.MedicalInsuranceAccount = humanInfo.HumanSocialSecurity.MedicalInsuranceAccount;
                        old.HumanSocialSecurity.SocialSecurityAccount = humanInfo.HumanSocialSecurity.SocialSecurityAccount;
                        old.HumanSocialSecurity.UnemploymentInsurance = humanInfo.HumanSocialSecurity.UnemploymentInsurance;

                        Context.Attach(old.HumanSocialSecurity);
                        Context.Update(old.HumanSocialSecurity);
                    }
                    else if (humanInfo.HumanSocialSecurity != null)
                    {
                        Context.Add(humanInfo.HumanSocialSecurity);
                    }
                    if (old?.HumanEducationInfos?.Count() > 0)
                    {
                        var HumanEducationInfos = old.HumanEducationInfos.ToList();
                        for (int i = 0; i < HumanEducationInfos.Count; i++)
                        {
                            var newEducationInfo = humanInfo.HumanEducationInfos?.FirstOrDefault(a => a.Id == HumanEducationInfos[i].Id);
                            if (newEducationInfo != null)
                            {
                                HumanEducationInfos[i].GetDegreeCompany = newEducationInfo.GetDegreeCompany;
                                HumanEducationInfos[i].GetDegree = newEducationInfo.GetDegree;
                                HumanEducationInfos[i].Education = newEducationInfo.Education;
                                HumanEducationInfos[i].EnrolmentTime = newEducationInfo.EnrolmentTime;
                                HumanEducationInfos[i].GetDegreeTime = newEducationInfo.GetDegreeTime;
                                HumanEducationInfos[i].GraduationCertificate = newEducationInfo.GraduationCertificate;
                                HumanEducationInfos[i].GraduationSchool = newEducationInfo.GraduationSchool;
                                HumanEducationInfos[i].GraduationTime = newEducationInfo.GraduationTime;
                                HumanEducationInfos[i].HumanId = newEducationInfo.HumanId;
                                HumanEducationInfos[i].Id = newEducationInfo.Id;
                                HumanEducationInfos[i].LearningType = newEducationInfo.LearningType;
                                HumanEducationInfos[i].Major = newEducationInfo.Major;
                                HumanEducationInfos[i].UpdateTime = newEducationInfo.UpdateTime;
                                HumanEducationInfos[i].UpdateUser = newEducationInfo.UpdateUser;
                            }
                            else
                            {
                                HumanEducationInfos[i].IsDeleted = true;
                                HumanEducationInfos[i].DeleteTime = DateTime.Now;
                                HumanEducationInfos[i].DeleteUser = user.Id;
                            }
                            humanInfo.HumanEducationInfos = humanInfo.HumanEducationInfos.Where(a => a.Id != HumanEducationInfos[i].Id)?.ToList();
                            Context.Attach(HumanEducationInfos[i]);
                            Context.Update(HumanEducationInfos[i]);
                        }
                        if (humanInfo.HumanEducationInfos.Count() > 0)
                        {
                            var oldHumanEducationInfos = humanInfo.HumanEducationInfos.ToList();
                            for (int i = 0; i < oldHumanEducationInfos.Count; i++)
                            {
                                oldHumanEducationInfos[i].IsDeleted = false;
                                oldHumanEducationInfos[i].CreateTime = DateTime.Now;
                                oldHumanEducationInfos[i].CreateUser = user.Id;
                            }
                            Context.AddRange(oldHumanEducationInfos);
                        }
                    }
                    else if (humanInfo?.HumanEducationInfos?.Count() > 0)
                    {
                        var HumanEducationInfos = humanInfo.HumanEducationInfos.ToList();
                        for (int i = 0; i < HumanEducationInfos.Count; i++)
                        {
                            HumanEducationInfos[i].IsDeleted = false;
                            HumanEducationInfos[i].CreateTime = DateTime.Now;
                            HumanEducationInfos[i].CreateUser = user.Id;
                        }
                        Context.AddRange(HumanEducationInfos);
                    }
                    //职称信息处理
                    if (old?.HumanTitleInfos.Count() > 0)
                    {
                        var HumanTitleInfos = old.HumanTitleInfos.ToList();
                        for (int i = 0; i < HumanTitleInfos.Count; i++)
                        {
                            var newHumanTitleInfos = humanInfo.HumanTitleInfos?.FirstOrDefault(a => a.Id == HumanTitleInfos[i].Id);
                            if (newHumanTitleInfos != null)
                            {
                                HumanTitleInfos[i].GetTitleTime = newHumanTitleInfos.GetTitleTime;
                                HumanTitleInfos[i].TitleName = newHumanTitleInfos.TitleName;
                            }
                            else
                            {
                                HumanTitleInfos[i].IsDeleted = true;
                                HumanTitleInfos[i].DeleteTime = DateTime.Now;
                                HumanTitleInfos[i].DeleteUser = user.Id;
                            }
                            humanInfo.HumanTitleInfos = humanInfo.HumanTitleInfos.Where(a => a.Id != HumanTitleInfos[i].Id)?.ToList();
                            Context.Attach(HumanTitleInfos[i]);
                            Context.Update(HumanTitleInfos[i]);
                        }
                        if (humanInfo?.HumanTitleInfos?.Count() > 0)
                        {
                            var oldHumanTitleInfos = humanInfo.HumanTitleInfos.ToList();
                            for (int i = 0; i < oldHumanTitleInfos.Count; i++)
                            {
                                oldHumanTitleInfos[i].IsDeleted = false;
                                oldHumanTitleInfos[i].CreateTime = DateTime.Now;
                                oldHumanTitleInfos[i].CreateUser = user.Id;
                            }
                            Context.AddRange(oldHumanTitleInfos);
                        }
                    }
                    else if (humanInfo?.HumanTitleInfos?.Count() > 0)
                    {
                        var HumanTitleInfos = humanInfo.HumanTitleInfos.ToList();
                        for (int i = 0; i < HumanTitleInfos.Count; i++)
                        {
                            HumanTitleInfos[i].IsDeleted = false;
                            HumanTitleInfos[i].CreateTime = DateTime.Now;
                            HumanTitleInfos[i].CreateUser = user.Id;
                        }
                        Context.AddRange(HumanTitleInfos);
                    }
                    //工作经历信息处理
                    if (old?.HumanWorkHistories?.Count() > 0)
                    {
                        var HumanWorkHistories = old.HumanWorkHistories.ToList();
                        for (int i = 0; i < HumanWorkHistories.Count; i++)
                        {
                            var newHumanWorkHistories = humanInfo.HumanWorkHistories?.FirstOrDefault(a => a.Id == HumanWorkHistories[i].Id);
                            if (newHumanWorkHistories != null)
                            {
                                HumanWorkHistories[i].Company = newHumanWorkHistories.Company;
                                HumanWorkHistories[i].EndTime = newHumanWorkHistories.EndTime;
                                HumanWorkHistories[i].Position = newHumanWorkHistories.Position;
                                HumanWorkHistories[i].StartTime = newHumanWorkHistories.StartTime;
                            }
                            else
                            {
                                HumanWorkHistories[i].IsDeleted = true;
                                HumanWorkHistories[i].DeleteTime = DateTime.Now;
                                HumanWorkHistories[i].DeleteUser = user.Id;
                            }
                            humanInfo.HumanWorkHistories = humanInfo.HumanWorkHistories.Where(a => a.Id != HumanWorkHistories[i].Id)?.ToList();
                            Context.Attach(HumanWorkHistories[i]);
                            Context.Update(HumanWorkHistories[i]);
                        }
                        if (humanInfo.HumanWorkHistories?.Count() > 0)
                        {
                            var oldHumanWorkHistories = humanInfo.HumanWorkHistories.ToList();
                            for (int i = 0; i < oldHumanWorkHistories.Count; i++)
                            {
                                oldHumanWorkHistories[i].IsDeleted = false;
                                oldHumanWorkHistories[i].CreateTime = DateTime.Now;
                                oldHumanWorkHistories[i].CreateUser = user.Id;
                            }
                            Context.AddRange(oldHumanWorkHistories);
                        }
                    }
                    else if (humanInfo?.HumanWorkHistories?.Count() > 0)
                    {
                        var HumanWorkHistories = humanInfo.HumanWorkHistories.ToList();
                        for (int i = 0; i < HumanWorkHistories.Count; i++)
                        {
                            HumanWorkHistories[i].IsDeleted = false;
                            HumanWorkHistories[i].CreateTime = DateTime.Now;
                            HumanWorkHistories[i].CreateUser = user.Id;
                        }
                        Context.AddRange(HumanWorkHistories);
                    }
                }

                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException) { throw; }
            return humanInfo;
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        public async Task<HumanInfo> CreateAsync(HumanInfo humanInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (humanInfo == null)
            {
                throw new ArgumentNullException(nameof(humanInfo));
            }
            Context.Add(humanInfo);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException) { throw; }
            return humanInfo;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public async Task DeleteListAsync(UserInfo user, List<HumanInfo> list, CancellationToken cancellationToken = default(CancellationToken))
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
        /// 查询单个
        /// </summary>
        /// <returns></returns>
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<HumanInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(HumanInfos.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<HumanInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(HumanInfos.AsNoTracking()).ToListAsync(cancellationToken);
        }


    }
}
