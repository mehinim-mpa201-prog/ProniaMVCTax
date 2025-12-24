using System.ComponentModel.DataAnnotations;

namespace ProniaMVCTax.Models;

public class Category:BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    public ICollection<Product>? Products { get; set; }
}
