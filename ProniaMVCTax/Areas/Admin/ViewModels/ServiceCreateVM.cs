namespace ProniaMVCTax.Areas.Admin.ViewModels;

public class ServiceCreateVM
{
    public string Title { get; set; }
    public string ShortDescription { get; set; }
    public IFormFile IconImage { get; set; }
}

public class ServiceUpdateVM
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string ShortDescription { get; set; }
    public IFormFile? IconImage { get; set; }
}

