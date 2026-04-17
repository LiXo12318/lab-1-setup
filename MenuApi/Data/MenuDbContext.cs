using MenuApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MenuApi.Data;

public class MenuDbContext : DbContext
{
    public MenuDbContext(DbContextOptions<MenuDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Dish> Dishes => Set<Dish>();
    public DbSet<DailySpecial> DailySpecials => Set<DailySpecial>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Description).HasMaxLength(2000);
        });

        modelBuilder.Entity<Dish>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Description).HasMaxLength(4000);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.HasOne(e => e.Category)
                .WithMany(c => c.Dishes)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.Allergens)
                .HasColumnType("jsonb")
                .HasConversion(
                    new ValueConverter<List<string>, string>(
                        v => AllergenJsonConverter.ToJson(v),
                        v => AllergenJsonConverter.FromJson(v)))
                .Metadata.SetValueComparer(
                    new ValueComparer<List<string>>(
                        (a, b) => ReferenceEquals(a, b) || (a != null && b != null && a.SequenceEqual(b)),
                        v => v.Aggregate(0, (h, x) => HashCode.Combine(h, x.GetHashCode(StringComparison.Ordinal))),
                        v => v.ToList()));
        });

        modelBuilder.Entity<DailySpecial>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SpecialPrice).HasPrecision(18, 2);
            entity.HasOne(e => e.Dish)
                .WithMany(d => d.DailySpecials)
                .HasForeignKey(e => e.DishId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.DishId, e.Date }).IsUnique();
        });
    }
}
