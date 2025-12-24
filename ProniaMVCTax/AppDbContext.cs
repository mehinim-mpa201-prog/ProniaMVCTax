using Microsoft.EntityFrameworkCore;
using ProniaMVCTax.Models;

namespace ProniaMVCTax;

public class AppDbContext:DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
        
    }
    public DbSet<Service> Services { get; set; }
    public DbSet<SliderItem> SliderItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

}


