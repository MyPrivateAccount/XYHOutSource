using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XYHChargePlugin.Models
{
    public class ChargeDbContext : DbContext
    {
        public ChargeDbContext(DbContextOptions<ChargeDbContext> opt)
            : base(opt) { }

        public DbSet<ChargeInfo> ChargeInfos { get; set; }
        public DbSet<CostInfo> CostInfos { get; set; }
        public DbSet<ReceiptInfo> ReceiptInfos { get; set; }
        public DbSet<LimitInfo> LimitInfos { get; set; }
        public DbSet<ModifyInfo> ModifyInfos { get; set; }
        public DbSet<FileInfo> FileInfos { get; set; }
        public DbSet<FileScopeInfo> FileScopeInfo { get; set; }

        public DbSet<Organizations> Organizations { get; set; }

        public DbSet<OrganizationExpansion> OrganizationExpansions { get; set; }
        public DbSet<PermissionExpansion> PermissionExpansions { get; set; }
        public DbSet<OrganizationPar> OrganizationPars { get; set; }

        public DbSet<HumanInfo> HumanInfo { get; set; }

        public DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<OrganizationExpansion>(b =>
            {
                b.HasKey(k => new { k.OrganizationId, k.SonId });
            });
            modelBuilder.Entity<PermissionExpansion>(b =>
            {
                b.HasKey(k => new { k.UserId, k.PermissionId, k.OrganizationId });
            });
            modelBuilder.Entity<Organizations>(b =>
            {
                b.HasKey(k => new { k.Id });
            });
            modelBuilder.Entity<OrganizationPar>(b =>
            {
                b.ToTable("organizationpar");
                b.HasKey(x => x.Id);
            });

            modelBuilder.Entity<ChargeInfo>(b =>
            {
                b.ToTable("XYH_CH_CHARGEMANAGE");
                b.HasKey(k => new { k.ID });
            });
            modelBuilder.Entity<CostInfo>(b => {
                b.HasKey(k => new { k.Id });
                b.ToTable("XYH_CH_COST");
            });
            modelBuilder.Entity<ReceiptInfo>(b => {
                b.HasKey(k => new { k.Id });
                b.ToTable("XYH_CH_RECEIPT");
            });
            modelBuilder.Entity<LimitInfo>(b => {
                b.HasKey(k => new { k.UserID });
                b.ToTable("XYH_CH_LIMIT");
            });
            modelBuilder.Entity<ModifyInfo>(b => {
                b.HasKey(k => new { k.ID });
                b.ToTable("XYH_CH_MODIFY");
            });
            modelBuilder.Entity<FileInfo>(b => {
                b.ToTable("XYH_CH_FILEINFOS");
                b.HasKey(k => new { k.FileGuid, k.FileExt, k.Type });
            });
            modelBuilder.Entity<FileScopeInfo>(b => {
                b.ToTable("XYH_CH_FILESCOPE");
                b.HasKey(k => new { k.ReceiptId , k.FileGuid});
            });

            modelBuilder.Entity<HumanInfo>(b => {
                b.ToTable("xyh_hu_humanmanage");
                b.HasKey(k => new { k.ID });
            });
            modelBuilder.Entity<Users>(b =>
            {
                b.ToTable("identityuser");
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
