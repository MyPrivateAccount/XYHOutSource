using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Models;
using System.Threading;
using System.Threading.Tasks;
using AuthorizationCenter.Models;
using AuthorizationCenter.DataSync;

namespace AuthorizationCenter.Models
{
    public class ApplicationDbContext : IdentityDbContext<Users, Roles, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }


        public DbSet<Organization> Organizations { get; set; }
        public DbSet<PermissionItem> PermissionItems { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<RoleApplication> RoleApplications { get; set; }
        public DbSet<PermissionExpansion> PermissionExpansions { get; set; }
        public DbSet<OrganizationExpansion> OrganizationExpansions { get; set; }
        public DbSet<PermissionOrganization> PermissionOrganizations { get; set; }
        public DbSet<Applications> Applications { get; set; }
        public DbSet<Authorization> Authorizations { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public new DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserLoginLog> UserLoginLogs { get; set; }
        public DbSet<DictionaryGroup> DictionaryGroups { get; set; }
        public DbSet<DictionaryDefine> DictionaryDefines { get; set; }

        public DbSet<UserExtensions> UserExtensions { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityUser>(b =>
            {
                //b.Property<bool>("IsDeleted");
                //b.HasQueryFilter(a => EF.Property<bool>(Set<IdentityUser>(), "IsDeleted") == false);
            });
            builder.Entity<IdentityRole>(b =>
            {
                b.Property<bool>("IsDeleted");
                //b.HasQueryFilter(a => EF.Property<bool>(Set<IdentityRole>(), "IsDeleted") == false);
            });
            builder.Entity<UserRole>(b =>
            {
                b.Property<bool>("IsDeleted");
                b.ToTable("aspnetuserroles");
                b.HasKey(k => new { k.UserId, k.RoleId });
                //b.HasQueryFilter(a => EF.Property<bool>(Set<IdentityUserRole<string>>(), "IsDeleted") == false);
            });

            builder.Entity<IdentityUserToken<string>>(b =>
            {
                b.Property<bool>("IsDeleted");
                //b.HasQueryFilter(a => EF.Property<bool>(Set<IdentityUserToken<string>>(), "IsDeleted") == false);
            });
            builder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.Property<bool>("IsDeleted");
                //b.HasQueryFilter(a => EF.Property<bool>(Set<IdentityUserLogin<string>>(), "IsDeleted") == false);
            });
            builder.Entity<IdentityUserClaim<string>>(b =>
            {
                b.Property<bool>("IsDeleted");
                //b.HasQueryFilter(a => EF.Property<bool>(Set<IdentityUserClaim<string>>(), "IsDeleted") == false);
            });
            builder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.Property<bool>("IsDeleted");
                //b.HasQueryFilter(a => EF.Property<bool>(Set<IdentityRoleClaim<string>>(), "IsDeleted") == false);
            });
            builder.Entity<PermissionItem>(b =>
            {
                b.Property<bool>("IsDeleted");
                //b.HasQueryFilter(a => EF.Property<bool>(PermissionItems, "IsDeleted") == false);
            });
            builder.Entity<RolePermission>(b =>
            {
                b.HasKey(k => new { k.RoleId, k.PermissionId, k.OrganizationScope });
                //b.HasQueryFilter(a => EF.Property<bool>(RolePermissions, "IsDeleted") == false);
            });
            builder.Entity<RoleApplication>(b =>
            {
                b.HasKey(k => new { k.RoleId, k.ApplicationId });
                //b.HasQueryFilter(a => EF.Property<bool>(RolePermissions, "IsDeleted") == false);
            });
            builder.Entity<Organization>(b =>
            {
                b.Property<bool>("IsDeleted");
                b.Property(p => p.ParentId).HasDefaultValue("0");
                //b.HasQueryFilter(a => EF.Property<bool>(Organizations, "IsDeleted") == false);
            });
            //builder.Entity<OpenIddictApplication>(b =>
            //{
            //    b.Property<bool>("IsDeleted");
            //    //b.HasQueryFilter(a => EF.Property<bool>(Set<OpenIddictApplication>(), "IsDeleted") == false);
            //});
            builder.Entity<Applications>(b =>
            {
                b.ToTable("openiddictapplications");
                b.Property<bool>("IsDeleted");
                //b.HasQueryFilter(a => EF.Property<bool>(Organizations, "IsDeleted") == false);
            });

            builder.Entity<Authorization>(b =>
            {
                b.ToTable("openiddictauthorizations");
                b.Property<bool>("IsDeleted");
                //b.HasQueryFilter(a => EF.Property<bool>(Set<OpenIddictAuthorization>(), "IsDeleted") == false);
            });
            builder.Entity<Token>(b =>
            {
                b.ToTable("openiddicttokens");
                b.Property<bool>("IsDeleted");
                //b.HasQueryFilter(a => EF.Property<bool>(Set<OpenIddictScope>(), "IsDeleted") == false);
            });
            builder.Entity<OpenIddictToken>(b =>
            {
                b.Property<bool>("IsDeleted");
                //b.HasQueryFilter(a => EF.Property<bool>(Set<OpenIddictToken>(), "IsDeleted") == false);
            });

            builder.Entity<OrganizationExpansion>(b =>
            {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
                b.HasKey(k => new { k.OrganizationId, k.SonId });
                //b.HasQueryFilter(a => EF.Property<bool>(Set<OpenIddictToken>(), "IsDeleted") == false);
            });
            builder.Entity<PermissionExpansion>(b =>
            {
                b.HasKey(k => new { k.UserId, k.PermissionId, k.OrganizationId });
                //b.HasQueryFilter(a => EF.Property<bool>(Set<OpenIddictToken>(), "IsDeleted") == false);
            });
            builder.Entity<PermissionOrganization>(b =>
            {
                b.HasKey(k => new { k.OrganizationScope, k.OrganizationId });
                //b.HasQueryFilter(a => EF.Property<bool>(Set<OpenIddictToken>(), "IsDeleted") == false);
            });
            builder.Entity<DictionaryGroup>(b =>
            {
                b.ToTable("xyh_base_dictionarygroups");
            });
            builder.Entity<DictionaryDefine>(b =>
            {
                b.ToTable("xyh_base_dictionarydefines");
                b.HasKey(x => new { x.GroupId, x.Value });
            });
            builder.Entity<UserLoginLog>(b =>
            {
                b.HasKey(x => new { x.UserId, x.LoginTime });
            });
            builder.Entity<UserExtensions>(b =>
            {
                b.ToTable("userextensions");
                b.HasKey(x => new { x.UserId, x.ParName });
            });
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
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
            //foreach (var entry in ChangeTracker.Entries())
            //{
            //    switch (entry.State)
            //    {
            //        case EntityState.Added:
            //            entry.CurrentValues["IsDeleted"] = false;
            //            break;

            //        case EntityState.Deleted:
            //            entry.State = EntityState.Modified;
            //            entry.CurrentValues["IsDeleted"] = true;
            //            break;
            //    }
            //}
        }

        //public DbSet<AuthorizationCenter.Models.Roles> Roles { get; set; }

        //public DbSet<AuthorizationCenter.Models.UserRole> UserRoles { get; set; }

    }
}
