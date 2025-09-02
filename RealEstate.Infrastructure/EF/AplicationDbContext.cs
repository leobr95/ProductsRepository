using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.EF;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Product>(eb =>
        {
            eb.ToTable(tb =>
            {
                tb.HasTrigger("TR_Products_SetUpdatedAt"); 
                tb.UseSqlOutputClause(false);             
            });

            eb.HasKey(x => x.ProductId);

            eb.Property(x => x.Name)
              .IsRequired()
              .HasMaxLength(100);

            eb.Property(x => x.Price)
              .HasColumnType("decimal(10,2)");

            eb.Property(x => x.Quantity)
              .IsRequired();

            // Server-generated timestamps
            eb.Property(x => x.CreatedAt)
              .HasDefaultValueSql("SYSUTCDATETIME()")
              .ValueGeneratedOnAdd();                 

            eb.Property(x => x.UpdatedAt)
              .HasDefaultValueSql("SYSUTCDATETIME()")
              .ValueGeneratedOnAddOrUpdate();         

           
            eb.HasIndex(x => x.Name).IsUnique();
            eb.HasIndex(x => x.Price);
            eb.HasIndex(x => x.Quantity);
        });
    }
}
