using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ProniaMVCTax.Areas.Admin.ViewModels;

public class ProductCreateVM
{
    public string Name { get; set; }

    [Precision(18, 2)]
    public decimal Price { get; set; }

    public string? Description { get; set; }

    [Required]
    public string MainImagePath { get; set; }

    [Required]
    public string HoverImagePath { get; set; }

    public string? SKU { get; set; }

    public int CategoryId { get; set; }
}