using ApplicationCore;
using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace XYHShopsPlugin.Models
{
    public class ShopsDbContext : DbContext
    {
        public ShopsDbContext(DbContextOptions<ShopsDbContext> options)
            : base(options) { }


        public DbSet<ShopBaseInfo> ShopBaseInfos { get; set; }
        public DbSet<Buildings> Buildings { get; set; }
        public DbSet<AreaDefine> AreaDefines { get; set; }
        public DbSet<BuildingBaseInfo> BuildingBaseInfos { get; set; }
        public DbSet<BuildingFacilities> BuildingFacilities { get; set; }
        public DbSet<BuildingShopInfo> BuildingShopInfos { get; set; }
        public DbSet<BuildingTradeMixPlanning> BuildingTradeMixPlanning { get; set; }
        public DbSet<FileInfo> FileInfos { get; set; }
        public DbSet<ShopsFileScope> ShopsFileScopes { get; set; }
        public DbSet<BuildingFileScope> BuildingFileScopes { get; set; }
        public DbSet<Shops> Shops { get; set; }
        public DbSet<ShopFacilities> ShopFacilities { get; set; }
        public DbSet<ShopLeaseInfo> ShopLeaseInfos { get; set; }

        public DbSet<Users> Users { get; set; }
        public DbSet<Organizations> Organizations { get; set; }

        /// <summary>
        /// 用户不感兴趣的商铺列表
        /// </summary>
        public DbSet<CustomerNotShops> CustomerNotShops { get; set; }

        /// <summary>
        /// 楼盘收藏
        /// </summary>
        public DbSet<BuildingFavorite> BuildingFavorites { get; set; }

        /// <summary>
        /// 商铺收藏
        /// </summary>
        public DbSet<ShopsFavorite> ShopsFavorites { get; set; }

        /// <summary>
        /// 楼盘推荐
        /// </summary>
        public DbSet<BuildingRecommend> BuildingRecommends { get; set; }

        /// <summary>
        /// 楼栋批次
        /// </summary>
        public DbSet<BuildingNo> BuildingNos { get; set; }

        /// <summary>
        /// 楼盘报备规则
        /// </summary>
        public DbSet<BuildingRule> BuildingRules { get; set; }

        /// <summary>
        /// 楼盘报备规则
        /// </summary>
        public DbSet<SellHistory> SellHistorys { get; set; }
        /// <summary>
        /// 动态
        /// </summary>
        public DbSet<UpdateRecord> UpdateRecords { get; set; }
        /// <summary>
        /// 动态文件
        /// </summary>
        public DbSet<UpdateRecordFileScope> UpdateRecordFileScopes { get; set; }

        public DbSet<BuildingNotice> BuildingNotices { get; set; }

        public DbSet<BuildingNoticeFileScope> BuildingNoticeFileScopes { get; set; }

        /// <summary>
        /// 部门展开表
        /// </summary>
        public DbSet<OrganizationExpansion> OrganizationExpansions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Organizations>(b =>
            {
                b.ToTable("organizations");
            });
            builder.Entity<ShopBaseInfo>(b =>
            {
                b.ToTable("xyh_fy_shopbaseinfos");
            });
            builder.Entity<Buildings>(b =>
            {
                b.HasKey(k => new { k.Id });
                b.ToTable("xyh_fy_buildings");
            });
            builder.Entity<AreaDefine>(b =>
            {
                b.ToTable("xyh_base_areadefines");
            });
            builder.Entity<BuildingBaseInfo>(b =>
            {
                b.ToTable("xyh_fy_buildingbaseinfos");
            });
            builder.Entity<BuildingFacilities>(b =>
            {
                b.ToTable("xyh_fy_buildingfacilities");
            });
            builder.Entity<BuildingShopInfo>(b =>
            {
                b.ToTable("xyh_fy_buildingshopinfos");
            });
            builder.Entity<BuildingTradeMixPlanning>(b =>
            {
                b.ToTable("xyh_fy_buildingtrademixplanning");
                b.HasKey(k => new { k.Id, k.TradeMixPlanning });
            });
            builder.Entity<FileInfo>(b =>
            {
                b.ToTable("xyh_fy_fileinfos");
                b.HasKey(k => new { k.FileGuid, k.FileExt, k.Type });
            });
            builder.Entity<ShopsFileScope>(b =>
            {
                b.ToTable("xyh_fy_shopsfilescopes");
                b.HasKey(k => new { k.FileGuid, k.ShopsId });
            });
            builder.Entity<BuildingFileScope>(b =>
            {
                b.ToTable("xyh_fy_buildingfilescopes");
                b.HasKey(k => new { k.FileGuid, k.BuildingId });
            });
            builder.Entity<ShopFacilities>(b =>
            {
                b.ToTable("xyh_fy_shopfacilities");
            });
            builder.Entity<ShopLeaseInfo>(b =>
            {
                b.ToTable("xyh_fy_shopleaseinfos");
            });
            builder.Entity<Shops>(b =>
            {
                b.HasKey(k => new { k.Id });
                b.ToTable("xyh_fy_shops");
            });
            builder.Entity<Users>(b =>
            {
                b.HasKey(k => new { k.Id, k.IsDeleted });
                b.ToTable("identityuser");
            });

            builder.Entity<BuildingFavorite>(b =>
            {
                b.HasKey(k => new { k.Id });
                b.ToTable("xyh_fy_buildingfavorite");
            });

            builder.Entity<ShopsFavorite>(b =>
            {
                b.HasKey(k => new { k.Id });
                b.ToTable("xyh_fy_shopsfavorite");
            });

            builder.Entity<BuildingRecommend>(b =>
            {
                b.HasKey(k => new { k.Id });
                b.ToTable("xyh_fy_buildingrecommend");
            });

            builder.Entity<BuildingNo>(b =>
            {
                b.HasKey(k => new { k.Id });
                b.ToTable("xyh_fy_buildingno");
            });

            builder.Entity<BuildingRule>(b =>
            {
                b.HasKey(k => new { k.Id });
                b.ToTable("xyh_fy_buildingrules");
            });

            builder.Entity<SellHistory>(b =>
            {
                b.ToTable("xyh_fy_sellhistory");
            });
            builder.Entity<UpdateRecord>(b =>
            {
                b.ToTable("xyh_fy_updaterecord");
            });
            builder.Entity<UpdateRecordFileScope>(b =>
            {
                b.ToTable("xyh_fy_updaterecordfilescope");
                b.HasKey(k => new { k.FileGuid, k.UpdateRecordId });
            });
            builder.Entity<BuildingNotice>(b =>
            {
                b.ToTable("xyh_fy_buildingnotice");
            });
            builder.Entity<BuildingNoticeFileScope>(b =>
            {
                b.ToTable("xyh_fy_buildingnoticefilescope");
                b.HasKey(k => new { k.FileGuid, k.BuildingNoticeId });
            });
            builder.Entity<CustomerNotShops>(b =>
            {
                b.ToTable("xyh_fy_customernotshops");
                b.HasKey(k => new { k.CustomerId, k.ShopsId });
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
