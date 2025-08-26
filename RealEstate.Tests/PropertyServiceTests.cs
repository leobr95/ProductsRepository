using Moq;
using RealEstate.Application.Services;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces;

namespace RealEstate.Tests;

public class PropertyServiceTests
{
    [Test]
    public async Task Maps_First_Enabled_Image_To_Dto()
    {
        var repo = new Mock<IPropertyRepository>();
        repo.Setup(r => r.SearchAsync(null,null,null,null,1,10,It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Property>{
                new Property {
                    IdProperty = "PRO-1",
                    IdOwner = "OWN-1",
                    Name = "Test",
                    Address = "Some St",
                    Price = 123,
                    Images = new List<PropertyImage>{
                        new(){ IdPropertyImage="img2", File="second.jpg", Enabled=false },
                        new(){ IdPropertyImage="img1", File="first.jpg",  Enabled=true  }
                    }
                }
            }, 1));

        var sut = new PropertyService(repo.Object);
        var res = await sut.GetAsync(null,null,null,null,1,10, default);

        Assert.That(res.Total, Is.EqualTo(1));
        Assert.That(res.Items[0].ImageUrl, Is.EqualTo("first.jpg"));
    }
}
