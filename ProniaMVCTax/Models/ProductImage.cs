namespace ProniaMVCTax.Models;

public class ProductImage:BaseEntity
{
    public string ImagePath { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
}