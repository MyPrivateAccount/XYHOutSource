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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ChargeInfo>(b =>
            {
                b.ToTable("XYH_CH_CHARGEMANAGE");
                b.HasKey(k => new { k.ID });
            });
            modelBuilder.Entity<CostInfo>(b => {
                b.HasKey(k => new { k.ID });
                b.ToTable("XYH_CH_COST");
            });
            modelBuilder.Entity<ReceiptInfo>(b => {
                b.HasKey(k => new { k.ID });
                b.ToTable("XYH_CH_RECEIPT");
            });
            modelBuilder.Entity<LimitInfo>(b => {
                b.HasKey(k => new { k.ID });
                b.ToTable("XYH_CH_LIMIT");
            });
            modelBuilder.Entity<ModifyInfo>(b => {
                b.HasKey(k => new { k.ID });
                b.ToTable("XYH_CH_MODIFY");
            });
            modelBuilder.Entity<FileInfo>(b => {
                b.ToTable("XYH_HU_FILEINFOS");
                b.HasKey(k => new { k.FileGuid, k.FileExt, k.Type });
            });
            modelBuilder.Entity<FileScopeInfo>(b => {
                b.ToTable("XYH_CH_FILESCOPE");
                b.HasKey(k => new { k.ReceiptID});
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
