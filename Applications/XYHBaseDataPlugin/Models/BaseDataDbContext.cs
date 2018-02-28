using ApplicationCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XYHBaseDataPlugin.Models
{
    public class BaseDataDbContext : DbContext
    {
        public BaseDataDbContext(DbContextOptions<BaseDataDbContext> options)
            : base(options)
        {
        }

        public DbSet<DictionaryGroup> DictionaryGroups { get; set; }
        public DbSet<DictionaryDefine> DictionaryDefines { get; set; }
        public DbSet<AreaDefine> AreaDefines { get; set; }

        /// <summary>
        /// 用户定义字段
        /// </summary>
        public DbSet<UserTypeValue> UserTypeValues { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<DictionaryGroup>(b =>
            {
                b.ToTable("xyh_base_dictionarygroups");
            });
            builder.Entity<DictionaryDefine>(b =>
            {
                b.ToTable("xyh_base_dictionarydefines");
                b.HasKey(x => new { x.GroupId, x.Value });
            });
            builder.Entity<AreaDefine>(b =>
            {
                b.ToTable("xyh_base_areadefines");
            });

            builder.Entity<UserTypeValue>(b =>
            {
                b.ToTable("xyh_base_usertypevalue");
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
