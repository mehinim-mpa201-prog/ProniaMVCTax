namespace ProniaMVCTax.Models;

public class SliderItem : BaseEntity
{

    public string Title { get; set; }
    public string Description { get; set; }
    public string ImgPath { get; set; }
    public int Offer { get; set; }
}