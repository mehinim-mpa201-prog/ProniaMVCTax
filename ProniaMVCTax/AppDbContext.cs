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

}


