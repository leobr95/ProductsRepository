using System.Collections.Generic;

namespace RealEstate.Domain.Entities;

public class Property
{
    public string IdProperty { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public decimal Price { get; set; }
    public string? CodeInternal { get; set; }
    public int? Year { get; set; }

    public string IdOwner { get; set; } = default!;
    public Owner? Owner { get; set; }

    public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
    public ICollection<PropertyTrace> Traces { get; set; } = new List<PropertyTrace>();
}
