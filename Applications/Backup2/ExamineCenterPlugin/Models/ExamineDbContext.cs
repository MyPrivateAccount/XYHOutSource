using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamineCenterPlugin.Models
{
    public class ExamineDbContext : DbContext
    {

        public ExamineDbContext(DbContextOptions<ExamineDbContext> options)
            : base(options)
        {
        }

        public DbSet<ExamineFlow> ExamineFlows { get; set; }
        public DbSet<ExamineNotice> ExamineNotices { get; set; }
        public DbSet<ExamineRecord> ExamineRecords { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Organizations> Organizations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ExamineFlow>(b =>
            {
                b.ToTable("xyh_sh_examineflows");
            });
            builder.Entity<ExamineNotice>(b =>
            {
                b.ToTable("xyh_sh_examinenotices");
            });
            builder.Entity<ExamineRecord>(b =>
            {
                b.ToTable("xyh_sh_examinerecords");
            });
            builder.Entity<Users>(b =>
            {
                b.HasKey(k => new { k.Id, k.IsDeleted });
                b.ToTable("identityuser");
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
