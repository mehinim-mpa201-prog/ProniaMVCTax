using System.ComponentModel.DataAnnotations;

namespace ProniaMVCTax.Models;

public class Category:BaseEntity
{
    public string Name { get; set; }
    public ICollection<Product>? Products { get; set; }
}
