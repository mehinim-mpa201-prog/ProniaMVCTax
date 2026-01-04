using ProniaMVCTax.Models;

namespace ProniaMVCTax.ViewModels;

public class DetailVM
{
    public Product Product { get; set; }
    public List<Service> Services { get; set; }
}
