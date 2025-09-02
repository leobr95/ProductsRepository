namespace RealEstate.Application.Validation;

public class ValidationOptions
{
   public int NameMinLength { get; set; } = 1;
    public int NameMaxLength { get; set; } = 100;
    public decimal PriceMin { get; set; } = 0m;
    public decimal? PriceMax { get; set; }
    public int QuantityMin { get; set; } = 0;
    public int? QuantityMax { get; set; }
}
