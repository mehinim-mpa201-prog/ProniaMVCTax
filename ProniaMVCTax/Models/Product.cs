using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ProniaMVCTax.Models;

public class Product : BaseEntity
{

    public string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string MainImagePath { get; set; }
    public string HoverImagePath { get; set; }
    public string? SKU { get; set; }
    public int Star { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public int BrandId { get; set; }
    public Brand Brand { get; set; }
    public ICollection<ProductImage>? ProductImages { get; set; } = [];
    public ICollection<ProductTag> ProductTags { get; set; } = [];
}
