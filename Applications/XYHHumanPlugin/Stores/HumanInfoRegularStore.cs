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
    public class HumanInfoRegularStore : IHumanInfoRegularStore
    {
        protected HumanDbContext Context { get; }

        public IQueryable<HumanInfoRegular> HumanInfoRegulars { get; set; }

        public HumanInfoRegularStore(HumanDbContext context)
        {
            Context = context;
            HumanInfoRegulars = Context.HumanInfoRegulars;
        }


        public IQueryable<HumanInfoRegular> SimpleQuery()
        {
            var q = from hrr in Context.HumanInfoRegulars.AsNoTracking()
                    join h1 in Context.HumanInfos.AsNoTracking() on hrr.HumanId equals h1.Id into h2
                    from h in h2.DefaultIfEmpty()
                    select new HumanInfoRegular()
                    {
                        Id = hrr.Id,
                        EmploymentInjuryInsurance = hrr.EmploymentInjuryInsurance,
                        EndowmentInsurance = hrr.EndowmentInsurance,
                        HousingProvidentFundAccount = hrr.HousingProvidentFundAccount,
                        InsuredAddress = hrr.InsuredAddress,
                        MedicalInsuranceAccount = hrr.MedicalInsuranceAccount,
                        SocialSecurityAccount = hrr.SocialSecurityAccount,
                        CreateTime = hrr.CreateTime,
                        CreateUser = hrr.CreateUser,
                        DeleteTime = hrr.DeleteTime,
                        DeleteUser = hrr.DeleteUser,
                        HousingProvidentFund = hrr.HousingProvidentFund,
                        HumanId = hrr.HumanId,
                        InsuredTime = hrr.InsuredTime,
                        IsCurrent = hrr.IsCurrent,
                        IsDeleted = hrr.IsDeleted,
                        IsGiveUp = hrr.IsGiveUp,
                        IsHave = hrr.IsHave,
                        IsSignCommitment = hrr.IsSignCommitment,
                        MaternityInsurance = hrr.MaternityInsurance,
                        MedicalInsurance = hrr.MedicalInsurance,

                        UnemploymentInsurance = hrr.UnemploymentInsurance,
                        UpdateTime = hrr.UpdateTime,
                        UpdateUser = hrr.UpdateUser,
                        RegularTime = hrr.RegularTime,

                        OrganizationId = h.DepartmentId,
                    };
            return q;
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        public async Task<HumanInfoRegular> CreateAsync(UserInfo user, HumanInfoRegular humanInfoRegular, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (humanInfoRegular == null)
            {
                throw new ArgumentNullException(nameof(humanInfoRegular));
            }
            if (string.IsNullOrEmpty(humanInfoRegular.Id))
            {
                humanInfoRegular.Id = Guid.NewGuid().ToString();
            }
            humanInfoRegular.CreateTime = DateTime.Now;
            humanInfoRegular.CreateUser = user.Id;
            humanInfoRegular.IsDeleted = false;
            Context.Add(humanInfoRegular);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException) { throw; }
            return humanInfoRegular;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        public async Task<HumanInfoRegular> UpdateAsync(UserInfo user, HumanInfoRegular humanInfoRegular, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (humanInfoRegular == null)
            {
                throw new ArgumentNullException(nameof(humanInfoRegular));
            }
            var old = HumanInfoRegulars.Where(a => a.Id == humanInfoRegular.Id).SingleOrDefault();
            if (old == null)
            {
                throw new Exception("更新的对象不存在");
            }
            old.EmploymentInjuryInsurance = humanInfoRegular.EmploymentInjuryInsurance;
            old.EndowmentInsurance = humanInfoRegular.EndowmentInsurance;
            old.HousingProvidentFund = humanInfoRegular.HousingProvidentFund;
            old.HousingProvidentFundAccount = humanInfoRegular.HousingProvidentFundAccount;
            old.InsuredAddress = humanInfoRegular.InsuredAddress;
            old.InsuredTime = humanInfoRegular.InsuredTime;
            old.IsGiveUp = humanInfoRegular.IsGiveUp;
            old.IsHave = humanInfoRegular.IsHave;
            old.IsSignCommitment = humanInfoRegular.IsSignCommitment;
            old.MaternityInsurance = humanInfoRegular.MaternityInsurance;
            old.MedicalInsurance = humanInfoRegular.MedicalInsurance;
            old.MedicalInsuranceAccount = humanInfoRegular.MedicalInsuranceAccount;
            old.RegularTime = humanInfoRegular.RegularTime;
            old.SocialSecurityAccount = humanInfoRegular.SocialSecurityAccount;
            old.UnemploymentInsurance = humanInfoRegular.UnemploymentInsurance;

            old.UpdateTime = DateTime.Now;
            old.UpdateUser = user.Id;
            Context.Update(old);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException) { throw; }
            return humanInfoRegular;
        }



        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAsync(UserInfo user, HumanInfoRegular humanInfoRegular, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoRegular == null)
            {
                throw new ArgumentNullException(nameof(humanInfoRegular));
            }
            humanInfoRegular.DeleteTime = DateTime.Now;
            humanInfoRegular.DeleteUser = user.Id;
            humanInfoRegular.IsDeleted = true;
            Context.Attach(humanInfoRegular);
            var entry = Context.Entry(humanInfoRegular);
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
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<HumanInfoRegular>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(HumanInfoRegulars.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<HumanInfoRegular>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(HumanInfoRegulars.AsNoTracking()).ToListAsync(cancellationToken);
        }

    }
}
