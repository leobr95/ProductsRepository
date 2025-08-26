namespace RealEstate.Domain.Entities;

public class PropertyImage
{
    public string IdPropertyImage { get; set; } = Guid.NewGuid().ToString();

    public string IdProperty { get; set; } = default!;
    public Property? Property { get; set; }

    public string File { get; set; } = default!;
    public bool Enabled { get; set; }
}
