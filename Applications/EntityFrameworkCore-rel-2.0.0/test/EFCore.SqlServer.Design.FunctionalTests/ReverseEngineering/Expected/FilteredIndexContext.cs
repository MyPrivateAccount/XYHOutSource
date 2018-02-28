using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace E2ETest.Namespace
{
    public partial class FilteredIndexContext : DbContext
    {
        public virtual DbSet<FilteredIndex> FilteredIndex { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(@"{{connectionString}}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FilteredIndex>(entity =>
            {
                entity.HasIndex(e => e.Number)
                    .HasName("Unicorn_Filtered_Index")
                    .HasFilter("([Number]>(10))");

                entity.Property(e => e.Id).ValueGeneratedNever();
            });
        }
    }
}
