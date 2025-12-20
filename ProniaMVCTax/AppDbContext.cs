using Microsoft.EntityFrameworkCore;
using ProniaMVCTax.Models;

namespace ProniaMVCTax;

public class AppDbContext:DbContext
{
    public DbSet<Service> Services { get; set; }
    public DbSet<SliderItem> SliderItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=DESKTOP-TAFIOU7\SQLEXPRESS;Database=ProniaMVCTaxDb;Trusted_Connection=true;TrustServerCertificate=true;");
        base.OnConfiguring(optionsBuilder);
    }
}


