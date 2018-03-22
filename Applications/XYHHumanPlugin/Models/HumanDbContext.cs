using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XYHHumanPlugin.Models;

namespace XYHHumanPlugin.Models
{
    class HumanDbContext : DbContext
    {
        public HumanDbContext(DbContextOptions<HumanDbContext> opt)
            : base(opt) { }

        public DbSet<HumanInfo> HumanInfos { get; set; }
        public DbSet<ContractInfo> ContractInfos { get; set; }
        public DbSet<BlackInfo> BlackInfos { get; set; }
        public DbSet<AttendanceInfo> AttendanceInfos { get; set; }
        public DbSet<PositionInfo> PositionInfos { get; set; }
        public DbSet<SalaryInfo> SalaryInfos { get; set; }
        public DbSet<MonthInfo> MonthInfos { get; set; }
        public DbSet<SalaryFormInfo> SalaryFormInfos { get; set; }
        public DbSet<AttendanceFormInfo> AttendanceFormInfos { get; set; }

        protected internal virtual void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<HumanInfo>(b =>
            {
                b.HasKey(k => k.ID);
                b.ToTable("XYH_HU_HUMANMANAGE");
            });
            modelBuilder.Entity<ContractInfo>(b => {
                b.HasKey(k => k.ID);
                b.ToTable("XYH_HU_CONTRACT");
            });
            modelBuilder.Entity<BlackInfo>(b => {
                b.HasKey(k => k.IDCard);
                b.ToTable("XYH_HU_BLACKLIST");
            });
            modelBuilder.Entity<AttendanceInfo>(b => {
                b.HasKey(k => k.ID);
                b.ToTable("XYH_HU_ATTENDANCE");
            });
            modelBuilder.Entity<PositionInfo>(b => {
                b.HasKey(k => k.ID);
                b.ToTable("XYH_HU_POSITION");
            });
            modelBuilder.Entity<SalaryInfo>(b => {
                b.HasKey(k => k.ID);
                b.ToTable("XYH_HU_SALARY");
            });
            modelBuilder.Entity<MonthInfo>(b => {
                b.HasKey(k => k.ID);
                b.ToTable("XYH_HU_MONTH");
            });
            modelBuilder.Entity<SalaryFormInfo>(b => {
                b.HasKey(k => k.ID);
                b.ToTable("XYH_HU_SALARYFORM");
            });
            modelBuilder.Entity<AttendanceFormInfo>(b => {
                b.HasKey(k => k.ID);
                b.ToTable("XYH_HU_ATTENDANCEFORM");
            });
        }

    }
}
