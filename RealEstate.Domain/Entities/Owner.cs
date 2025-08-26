using System.Collections.Generic;

namespace RealEstate.Domain.Entities;

public class Owner
{
    public string IdOwner { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string? Photo { get; set; }
    public DateTime? Birthday { get; set; }

    public ICollection<Property> PropertyList { get; set; } = new List<Property>();
}
