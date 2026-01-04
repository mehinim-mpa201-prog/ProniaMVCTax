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
    public IFormFile MainImage { get; set; }

    [Required]
    public IFormFile HoverImage { get; set; }

    public string? SKU { get; set; }

    public int CategoryId { get; set; }

    public int Star { get; set; }
    public List<IFormFile> Images { get; set; } = [];

    public int BrandId { get; set; }
    public List<int> TagIds { get; set; } = [];
}

