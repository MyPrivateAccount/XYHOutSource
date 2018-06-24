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
        public DbSet<BlackInfo> BlackInfos { get; set; }
        public DbSet<AttendanceInfo> AttendanceInfos { get; set; }
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

        public DbSet<SocialInsurance> SocialInsurances { get; set; }
        public DbSet<Organizations> Organizations { get; set; }
        public DbSet<OrganizationExpansion> OrganizationExpansions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<HumanInfo>(b =>
            {
                b.ToTable("XYH_HU_HUMANMANAGE");
                b.HasKey(k => new { k.ID });

            });
            modelBuilder.Entity<HumanContractInfo>(b =>
            {
                b.HasKey(k => new { k.ID });
                b.ToTable("XYH_HU_CONTRACT");
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
