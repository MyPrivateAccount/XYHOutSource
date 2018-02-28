using ApplicationCore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore
{
    public class CoreDbContext : DbContext
    {
        public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options)
        {
        }

        public DbSet<OrganizationExpansion> OrganizationExpansions { get; set; }
        public DbSet<PermissionExpansion> PermissionExpansions { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<OrganizationExpansion>(b =>
            {
                b.HasKey(k => new { k.OrganizationId, k.SonId });
            });
            builder.Entity<PermissionExpansion>(b =>
            {
                b.HasKey(k => new { k.UserId, k.PermissionId, k.OrganizationId });
            });
            builder.Entity<Users>(b =>
            {
                b.ToTable("identityuser");
            });
            builder.Entity<Feedback>(b =>
            {
                b.ToTable("xyh_fk_feedbacks");
            });
        }


    }
}
