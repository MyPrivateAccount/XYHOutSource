using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XYHCustomerPlugin.Models
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options)
            : base(options) { }

        /// <summary>
        /// 客户基础信息
        /// </summary>
        public DbSet<CustomerInfo> CustomerInfos { get; set; }

        /// <summary>
        /// 客户需求信息
        /// </summary>
        public DbSet<CustomerDemand> CustomerDemands { get; set; }

        /// <summary>
        /// 员工信息
        /// </summary>
        public DbSet<Users> Users { get; set; }

        /// <summary>
        /// 部门信息
        /// </summary>
        public DbSet<Organizations> Organizations { get; set; }

        /// <summary>
        /// 约看
        /// </summary>
        public DbSet<AboutLook> AboutLooks { get; set; }

        /// <summary>
        /// 客户电话信息
        /// </summary>
        public DbSet<CustomerPhone> CustomerPhones { get; set; }

        /// <summary>
        /// 跟进
        /// </summary>
        public DbSet<CustomerFollowUp> CustomerFollowUps { get; set; }

        /// <summary>
        /// 报备
        /// </summary>
        public DbSet<CustomerReport> CustomerReports { get; set; }

        /// <summary>
        /// 带看
        /// </summary>
        public DbSet<BeltLook> BeltLooks { get; set; }

        /// <summary>
        /// 需求楼盘
        /// </summary>
        public DbSet<RelationHouse> RelationHouses { get; set; }

        public DbSet<ChekPhoneHistory> ChekPhoneHistories { get; set; }
        

        public DbSet<CustomerLoss> CustomerLosss { get; set; }

        public DbSet<CustomerPool> CustomerPools { get; set; }

        public DbSet<CustomerPoolDefine> CustomerPoolDefines { get; set; }

        /// <summary>
        /// 客户迁移记录表
        /// </summary>
        public DbSet<MigrationPoolHistory> MigrationPoolHistories { get; set; }
        
        public DbSet<CustomerReferral> CustomerReferrals { get; set; }

        /// <summary>
        /// 成交跟进
        /// </summary>
        public DbSet<CustomerTransactionsFollowUp> CustomerTransactionsFollowUps { get; set; }

        /// <summary>
        /// 成交跟进
        /// </summary>
        public DbSet<CustomerTransactions> CustomerTransactions { get; set; }

        /// <summary>
        /// 成交
        /// </summary>
        public DbSet<CustomerDeal> CustomerDeals { get; set; }

        /// <summary>
        /// 文件表
        /// </summary>
        public DbSet<CustomerDealFileInfo> FileInfos { get; set; }

        /// <summary>
        /// 成交信息文件关联表
        /// </summary>
        public DbSet<DealFileScope> DealFileScopes { get; set; }

        /// <summary>
        /// 客户文件关联表
        /// </summary>
        public DbSet<CustomerFileScope> CustomerFileScopes { get; set; }

        /// <summary>
        /// 部门展开表
        /// </summary>
        public DbSet<OrganizationExpansion> OrganizationExpansions { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CustomerInfo>(b =>
            {
                b.ToTable("xyh_ky_customerinfo");
            });

            builder.Entity<CustomerDemand>(b =>
            {
                b.ToTable("xyh_ky_customerdemand");
            });

            builder.Entity<ChekPhoneHistory>(b =>
            {
                b.ToTable("xyh_ky_checkphone");
            });

            builder.Entity<Users>(b =>
            {
                b.HasKey(k => new { k.Id, k.IsDeleted });
                b.ToTable("identityuser");
            });

            builder.Entity<Organizations>(b =>
            {
                b.ToTable("organizations");
            });

            builder.Entity<AboutLook>(b =>
            {
                b.ToTable("xyh_ky_aboutlook");
            });

            builder.Entity<CustomerReport>(b =>
            {
                b.ToTable("xyh_ky_customerreport");
            });

            builder.Entity<CustomerFollowUp>(b =>
            {
                b.ToTable("xyh_ky_customerfollowup");
            });

            builder.Entity<CustomerPhone>(b =>
            {
                b.ToTable("xyh_ky_customerphone");
            });

            builder.Entity<BeltLook>(b =>
            {
                b.ToTable("xyh_ky_beltlook");
            });

            builder.Entity<RelationHouse>(b =>
            {
                b.ToTable("xyh_ky_relationhouse");
            });

            builder.Entity<CustomerLoss>(b =>
            {
                b.ToTable("xyh_ky_customerloss");
            });
            builder.Entity<CustomerPool>(b =>
            {
                b.ToTable("xyh_ky_customerpool");
            });
            builder.Entity<CustomerPoolDefine>(b =>
            {
                b.ToTable("xyh_ky_customerpooldefine");
            });
            builder.Entity<MigrationPoolHistory>(b =>
            {
                b.ToTable("xyh_ky_migrationpoolhistory");
                b.HasKey(k => new { k.CustomerId, k.MigrationTime, k.OriginalDepartment, k.TargetDepartment });
            });

            builder.Entity<CustomerReferral>(b =>
            {
                b.ToTable("xyh_ky_customerreferral");
            });

            builder.Entity<CustomerTransactionsFollowUp>(b =>
            {
                b.ToTable("xyh_ky_customertransactionsfollowup");
            });

            builder.Entity<CustomerTransactions>(b =>
            {
                b.ToTable("xyh_ky_customertransactions");
            });

            builder.Entity<CustomerDeal>(b =>
            {
                b.ToTable("xyh_ky_customerdeal");
            });

            builder.Entity<CustomerDealFileInfo>(b =>
            {
                b.ToTable("xyh_ky_fileinfos");
                b.HasKey(k => new { k.FileGuid, k.FileExt, k.Type });
            });

            builder.Entity<DealFileScope>(b =>
            {
                b.ToTable("xyh_ky_dealfilescopes");
                b.HasKey(k => new { k.FileGuid, k.DealId });
            });

            builder.Entity<CustomerFileScope>(b =>
            {
                b.ToTable("xyh_ky_customerfilescopes");
                b.HasKey(k => new { k.FileGuid, k.CustomerId });
            });

            

            builder.Entity<OrganizationExpansion>(b =>
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
