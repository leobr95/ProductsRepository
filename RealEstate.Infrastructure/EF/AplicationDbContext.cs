using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.EF;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Owner> Owners => Set<Owner>();
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
    public DbSet<PropertyTrace> PropertyTraces => Set<PropertyTrace>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Owner>().HasKey(x => x.IdOwner);
        b.Entity<Property>().HasKey(x => x.IdProperty);
        b.Entity<PropertyImage>().HasKey(x => x.IdPropertyImage);
        b.Entity<PropertyTrace>().HasKey(x => x.IdPropertyTrace);

        b.Entity<Property>()
            .HasOne(p => p.Owner)
            .WithMany(o => o.PropertyList)
            .HasForeignKey(p => p.IdOwner)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<PropertyImage>()
            .HasOne(i => i.Property)
            .WithMany(p => p.Images)
            .HasForeignKey(i => i.IdProperty);

        b.Entity<PropertyTrace>()
            .HasOne(t => t.Property)
            .WithMany(p => p.Traces)
            .HasForeignKey(t => t.IdProperty);

        b.Entity<Property>().Property(x => x.Price).HasColumnType("decimal(18,2)");
        b.Entity<PropertyTrace>().Property(x => x.Value).HasColumnType("decimal(18,2)");
        b.Entity<PropertyTrace>().Property(x => x.Tax).HasColumnType("decimal(18,2)");
    }
}
