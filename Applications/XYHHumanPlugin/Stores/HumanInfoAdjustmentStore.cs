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
    public class HumanInfoAdjustmentStore : IHumanInfoAdjustmentStore
    {
        protected HumanDbContext Context { get; }

        public IQueryable<HumanInfoAdjustment> HumanInfoAdjustments { get; set; }

        public HumanInfoAdjustmentStore(HumanDbContext context)
        {
            Context = context;
            HumanInfoAdjustments = Context.HumanInfoAdjustments;
        }

        public IQueryable<HumanInfoAdjustment> SimpleQuery()
        {
            var q = from hra in Context.HumanInfoAdjustments.AsNoTracking()
                    join h1 in Context.HumanInfos.AsNoTracking() on hra.HumanId equals h1.Id into h2
                    from h in h2.DefaultIfEmpty()
                    select new HumanInfoAdjustment()
                    {
                        Id = hra.Id,
                        AdjustmentTime = hra.AdjustmentTime,
                        BaseWages = hra.BaseWages,
                        CommunicationAllowance = hra.CommunicationAllowance,
                        EmploymentInjuryInsurance = hra.EmploymentInjuryInsurance,
                        EndowmentInsurance = hra.EndowmentInsurance,
                        HousingProvidentFundAccount = hra.HousingProvidentFundAccount,
                        InsuredAddress = hra.InsuredAddress,
                        MedicalInsuranceAccount = hra.MedicalInsuranceAccount,
                        OtherAllowance = hra.OtherAllowance,
                        SocialSecurityAccount = hra.SocialSecurityAccount,
                        TrafficAllowance = hra.TrafficAllowance,
                        CreateTime = hra.CreateTime,
                        CreateUser = hra.CreateUser,
                        DeleteTime = hra.DeleteTime,
                        DeleteUser = hra.DeleteUser,
                        GrossPay = hra.GrossPay,
                        HousingProvidentFund = hra.HousingProvidentFund,
                        HumanId = hra.HumanId,
                        InsuredTime = hra.InsuredTime,
                        IsCurrent = hra.IsCurrent,
                        IsDeleted = hra.IsDeleted,
                        IsGiveUp = hra.IsGiveUp,
                        IsHave = hra.IsHave,
                        IsSignCommitment = hra.IsSignCommitment,
                        MaternityInsurance = hra.MaternityInsurance,
                        MedicalInsurance = hra.MedicalInsurance,
                        PostWages = hra.PostWages,
                        ProbationaryPay = hra.ProbationaryPay,
                        UnemploymentInsurance = hra.UnemploymentInsurance,
                        UpdateTime = hra.UpdateTime,
                        UpdateUser = hra.UpdateUser,

                        Position = hra.Position,
                        DepartmentId = hra.DepartmentId,
                        OrganizationId = h.DepartmentId,
                    };
            return q;
        }



        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        public async Task<HumanInfoAdjustment> CreateAsync(UserInfo user, HumanInfoAdjustment humanInfoAdjustment, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (humanInfoAdjustment == null)
            {
                throw new ArgumentNullException(nameof(humanInfoAdjustment));
            }
            if (string.IsNullOrEmpty(humanInfoAdjustment.Id))
            {
                humanInfoAdjustment.Id = Guid.NewGuid().ToString();
            }
            humanInfoAdjustment.CreateTime = DateTime.Now;
            humanInfoAdjustment.CreateUser = user.Id;
            humanInfoAdjustment.IsDeleted = false;
            Context.Add(humanInfoAdjustment);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException) { throw; }
            return humanInfoAdjustment;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        public async Task<HumanInfoAdjustment> UpdateAsync(UserInfo user, HumanInfoAdjustment humanInfoAdjustment, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (humanInfoAdjustment == null)
            {
                throw new ArgumentNullException(nameof(humanInfoAdjustment));
            }
            var old = HumanInfoAdjustments.Where(a => a.Id == humanInfoAdjustment.Id).SingleOrDefault();
            if (old == null)
            {
                throw new Exception("更新的对象不存在");
            }
            old.AdjustmentTime = humanInfoAdjustment.AdjustmentTime;
            old.BaseWages = humanInfoAdjustment.BaseWages;
            old.CommunicationAllowance = humanInfoAdjustment.CommunicationAllowance;
            old.DepartmentId = humanInfoAdjustment.DepartmentId;
            old.EmploymentInjuryInsurance = humanInfoAdjustment.EmploymentInjuryInsurance;
            old.EndowmentInsurance = humanInfoAdjustment.EndowmentInsurance;
            old.GrossPay = humanInfoAdjustment.GrossPay;
            old.HousingProvidentFund = humanInfoAdjustment.HousingProvidentFund;
            old.HousingProvidentFundAccount = humanInfoAdjustment.HousingProvidentFundAccount;
            old.InsuredAddress = humanInfoAdjustment.InsuredAddress;
            old.InsuredTime = humanInfoAdjustment.InsuredTime;
            old.IsGiveUp = humanInfoAdjustment.IsGiveUp;
            old.IsHave = humanInfoAdjustment.IsHave;
            old.IsSignCommitment = humanInfoAdjustment.IsSignCommitment;
            old.MaternityInsurance = humanInfoAdjustment.MaternityInsurance;
            old.MedicalInsurance = humanInfoAdjustment.MedicalInsurance;
            old.MedicalInsuranceAccount = humanInfoAdjustment.MedicalInsuranceAccount;
            old.OtherAllowance = humanInfoAdjustment.OtherAllowance;
            old.Position = humanInfoAdjustment.Position;
            old.PostWages = humanInfoAdjustment.PostWages;
            old.ProbationaryPay = humanInfoAdjustment.ProbationaryPay;
            old.SocialSecurityAccount = humanInfoAdjustment.SocialSecurityAccount;
            old.TrafficAllowance = humanInfoAdjustment.TrafficAllowance;
            old.UnemploymentInsurance = humanInfoAdjustment.UnemploymentInsurance;

            old.UpdateTime = DateTime.Now;
            old.UpdateUser = user.Id;
            Context.Update(old);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException) { throw; }
            return humanInfoAdjustment;
        }



        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAsync(UserInfo user, HumanInfoAdjustment humanInfoAdjustment, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoAdjustment == null)
            {
                throw new ArgumentNullException(nameof(humanInfoAdjustment));
            }
            humanInfoAdjustment.DeleteTime = DateTime.Now;
            humanInfoAdjustment.DeleteUser = user.Id;
            humanInfoAdjustment.IsDeleted = true;
            Context.Attach(humanInfoAdjustment);
            var entry = Context.Entry(humanInfoAdjustment);
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
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<HumanInfoAdjustment>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(HumanInfoAdjustments.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<HumanInfoAdjustment>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(HumanInfoAdjustments.AsNoTracking()).ToListAsync(cancellationToken);
        }

    }
}
