﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;

namespace XYHContractPlugin.Models
{
    public class ContractDbContext : DbContext
    {
        public ContractDbContext(DbContextOptions<ContractDbContext> opt)
            : base(opt) { }

        public DbSet<ContractInfo> ContractInfos { get; set; }
        public DbSet<ComplementInfo> ComplementInfos { get; set; }
        public DbSet<AnnexInfo> AnnexInfos { get; set; }
        public DbSet<CheckInfo> CheckInfos { get; set; }
        public DbSet<EstateInfo> EstateInfos { get; set; }
        public DbSet<ModifyInfo> ModifyInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ContractInfo>(b =>
            {
                b.HasKey(K => new { K.ID});
                b.ToTable("XYH_DT_CONTRACTINFO");
            });

            builder.Entity<ComplementInfo>(b =>
            {
                b.HasKey(K => new { K.ID });
                b.ToTable("XYH_DT_CONTRACTCOMPLEMENT");
            });

            builder.Entity<AnnexInfo>(b =>
            {
                b.HasKey(K => new { K.ID });
                b.ToTable("XYH_DT_CONTRACTANNEX");
            });

            builder.Entity<CheckInfo>(b =>
            {
                b.HasKey(K => new { K.ID });
                b.ToTable("XYH_DT_CHECK");
            });

            builder.Entity<EstateInfo>(b =>
            {
                b.HasKey(K => new { K.ID });
                b.ToTable("XYH_DT_CONTRACTESTATE");
            });

            builder.Entity<ModifyInfo>(b =>
            {
                b.HasKey(K => new { K.ID });
                b.ToTable("XYH_DT_MODIFY");
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
