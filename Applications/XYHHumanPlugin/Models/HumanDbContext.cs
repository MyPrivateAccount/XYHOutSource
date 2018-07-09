using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using XYHHumanPlugin.Models;

namespace XYHHumanPlugin.Models
{
    public class HumanDbContext : DbContext
    {
        public HumanDbContext(DbContextOptions<HumanDbContext> opt)
            : base(opt) { }

        public DbSet<HumanInfo> HumanInfos { get; set; }
        public DbSet<HumanContractInfo> HumanContractInfos { get; set; }
        public DbSet<HumanSalaryStructure> HumanSalaryStructures { get; set; }
        public DbSet<HumanSocialSecurity> HumanSocialSecurities { get; set; }
        public DbSet<HumanTitleInfo> HumanTitleInfos { get; set; }

        public DbSet<HumanWorkHistory> HumanWorkHistories { get; set; }

        public DbSet<HumanEducationInfo> HumanEducationInfos { get; set; }

        public DbSet<HumanFileScope> HumanFileScopes { get; set; }
        public DbSet<HumanInfoBlack> HumanInfoBlacks { get; set; }
        public DbSet<HumanInfoChange> HumanInfoChanges { get; set; }
        public DbSet<HumanInfoLeave> HumanInfoLeaves { get; set; }
        public DbSet<HumanInfoPartPosition> HumanInfoPartPositions { get; set; }
        public DbSet<HumanInfoRegular> HumanInfoRegulars { get; set; }
        public DbSet<HumanInfoAdjustment> HumanInfoAdjustments { get; set; }
        public DbSet<PositionSalary> PositionSalaries { get; set; }

        public DbSet<HumanPosition> HumanPositions { get; set; }


        public DbSet<BlackInfo> BlackInfos { get; set; }
        public DbSet<AttendanceInfo> AttendanceInfos { get; set; }
        public DbSet<AttendanceSettingInfo> AttendanceSettingInfos { get; set; }
        public DbSet<PositionInfo> PositionInfos { get; set; }
        public DbSet<SalaryInfo> SalaryInfos { get; set; }
        public DbSet<MonthInfo> MonthInfos { get; set; }
        public DbSet<SalaryFormInfo> SalaryFormInfos { get; set; }
        public DbSet<AttendanceFormInfo> AttendanceFormInfos { get; set; }
        public DbSet<ModifyInfo> ModifyInfos { get; set; }
        public DbSet<AnnexInfo> AnnexInfos { get; set; }
        public DbSet<FileInfo> FileInfos { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<LeaveInfo> LeaveInfos { get; set; }
        public DbSet<ChangeInfo> ChangeInfos { get; set; }
        public DbSet<RewardPunishmentInfo> RewardPunishmentInfos { get; set; }

        public DbSet<SocialInsurance> SocialInsurances { get; set; }
        public DbSet<Organizations> Organizations { get; set; }
        public DbSet<OrganizationExpansion> OrganizationExpansions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<HumanInfo>(b =>
            {
                b.ToTable("xyh_hu_humaninfo");
                b.HasKey(k => new { k.Id });

            });
            modelBuilder.Entity<HumanContractInfo>(b =>
            {
                b.ToTable("xyh_hu_humancontractinfo");
            });

            modelBuilder.Entity<HumanSalaryStructure>(b =>
            {
                b.ToTable("xyh_hu_humansalarystructure");
            });

            modelBuilder.Entity<HumanSocialSecurity>(b =>
            {
                b.ToTable("xyh_hu_humansocialsecurity");
            });
            modelBuilder.Entity<HumanTitleInfo>(b =>
            {
                b.ToTable("xyh_hu_humantitleinfo");
            });
            modelBuilder.Entity<HumanWorkHistory>(b =>
            {
                b.ToTable("xyh_hu_humanworkhistory");
            });
            modelBuilder.Entity<HumanFileScope>(b =>
            {
                b.HasKey(k => new { k.HumanId, k.FileGuid });
                b.ToTable("xyh_hu_humanfilescope");
            });
            modelBuilder.Entity<HumanEducationInfo>(b =>
            {
                b.ToTable("xyh_hu_humaneducationinfo");
            });
            modelBuilder.Entity<HumanInfoBlack>(b =>
            {
                b.ToTable("xyh_hu_humaninfoblack");
            });


            modelBuilder.Entity<HumanInfoChange>(b =>
            {
                b.ToTable("xyh_hu_humaninfochange");
            });
            modelBuilder.Entity<HumanInfoLeave>(b =>
            {
                b.ToTable("xyh_hu_humaninfoleave");
            });
            modelBuilder.Entity<HumanInfoPartPosition>(b =>
            {
                b.ToTable("xyh_hu_humaninfopartposition");
            });
            modelBuilder.Entity<HumanInfoRegular>(b =>
            {
                b.ToTable("xyh_hu_humaninforegular");
            });
            modelBuilder.Entity<HumanInfoAdjustment>(b =>
            {
                b.ToTable("xyh_hu_humaninfoadjustment");
            });
            modelBuilder.Entity<PositionSalary>(b =>
            {
                b.ToTable("xyh_hu_positionsalary");
            });
            modelBuilder.Entity<HumanPosition>(b =>
            {
                b.ToTable("xyh_hu_humanposition");
            });


            modelBuilder.Entity<BlackInfo>(b =>
            {
                b.HasKey(k => new { k.IDCard });
                b.ToTable("XYH_HU_BLACKLIST");
            });
            modelBuilder.Entity<AttendanceInfo>(b =>
            {
                b.HasKey(k => new { k.ID });
                b.ToTable("XYH_HU_ATTENDANCE");
            });
            modelBuilder.Entity<AttendanceSettingInfo>(b =>
            {
                b.HasKey(k => new { k.Type });
                b.ToTable("XYH_HU_ATTENDANCESETTING");
            });
            modelBuilder.Entity<PositionInfo>(b =>
            {
                b.HasKey(k => new { k.ID });
                b.ToTable("XYH_HU_POSITION");
            });
            modelBuilder.Entity<SalaryInfo>(b =>
            {
                b.HasKey(k => new { k.ID });
                b.ToTable("XYH_HU_SALARY");
            });
            modelBuilder.Entity<MonthInfo>(b =>
            {
                b.HasKey(k => new { k.ID });
                b.ToTable("XYH_HU_MONTH");
            });
            modelBuilder.Entity<SalaryFormInfo>(b =>
            {
                b.HasKey(k => new { k.ID });
                b.ToTable("XYH_HU_SALARYFORM");
            });
            modelBuilder.Entity<AttendanceFormInfo>(b =>
            {
                b.HasKey(k => new { k.ID });
                b.ToTable("XYH_HU_ATTENDANCEFORM");
            });
            modelBuilder.Entity<ModifyInfo>(b =>
            {
                b.HasKey(k => new { k.ID });
                b.ToTable("XYH_HU_MODIFY");
            });

            modelBuilder.Entity<FileInfo>(b =>
            {
                b.ToTable("XYH_HU_FILEINFOS");
                b.HasKey(k => new { k.FileGuid, k.FileExt, k.Type });
            });

            modelBuilder.Entity<AnnexInfo>(b =>
            {
                b.HasKey(k => new { k.ID });
                b.ToTable("XYH_HU_ANNEX");
            });

            modelBuilder.Entity<SocialInsurance>(b =>
            {
                b.HasKey(k => new { k.IDCard });
                b.ToTable("XYH_HU_SOCIALINSURANCE");
            });
            modelBuilder.Entity<LeaveInfo>(b =>
            {
                b.HasKey(k => new { k.IDCard });
                b.ToTable("XYH_HU_LEAVEINFO");
            });

            modelBuilder.Entity<ChangeInfo>(b =>
            {
                b.HasKey(k => new { k.IDCard });
                b.ToTable("XYH_HU_CHANGE");
            });

            modelBuilder.Entity<RewardPunishmentInfo>(b =>
            {
                b.HasKey(k => new { k.ID });
                b.ToTable("XYH_HU_REWARDPUNISHMENT");
            });

            modelBuilder.Entity<Users>(b =>
            {
                b.HasKey(k => new { k.Id, k.IsDeleted });
                b.ToTable("identityuser");
            });
            modelBuilder.Entity<Organizations>(b =>
            {
                b.HasKey(k => new { k.Id });
                b.ToTable("organizations");
            });
            modelBuilder.Entity<OrganizationExpansion>(b =>
            {
                b.ToTable("organizationexpansions");
                b.HasKey(k => new { k.OrganizationId, k.SonId });
            });
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {

        }

    }
}
