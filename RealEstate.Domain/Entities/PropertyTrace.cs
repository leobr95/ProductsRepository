namespace RealEstate.Domain.Entities;

public class PropertyTrace
{
    public string IdPropertyTrace { get; set; } = Guid.NewGuid().ToString();

    public string IdProperty { get; set; } = default!;
    public Property? Property { get; set; }

    public DateTime DateSale { get; set; }
    public string Name { get; set; } = default!;
    public decimal Value { get; set; }
    public decimal Tax { get; set; }
}
