using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Options;
using RealEstate.Application;
using RealEstate.Application.DTOs;
using RealEstate.Application.Validation;
using RealEstate.Tests.Tests.Fakes;

namespace RealEstate.Tests.Tests.Validation;

[TestFixture]
public class ValidationServiceTests
{
    private static ValidationService BuildService(ValidationOptions? opts = null)
    {
        var options = Options.Create(opts ?? new ValidationOptions
        {
            NameMinLength = 3,
            NameMaxLength = 100,
            PriceMin = 0m,
            PriceMax = 1000m,
            QuantityMin = 0,
            QuantityMax = 100000
        });

        var loc = new FakeStringLocalizer<SharedResources>();
        return new ValidationService(options, loc);
    }

    [Test]
    public void EmptyName_Should_Return_Error()
    {
        var svc = BuildService();
        var dto = new ProductCreateDto(Name: "", Price: 1.0m, Quantity: 1);

        var errors = svc.ValidateProduct(dto).ToArray();

        errors.Should().NotBeEmpty();
        errors.Any(e => e.Code == "NameRequired").Should().BeTrue();
    }

    [Test]
    public void ShortName_Should_Return_MinLength_Error()
    {
        var svc = BuildService(new ValidationOptions { NameMinLength = 3 });
        var dto = new ProductCreateDto(Name: "ab", Price: 1.0m, Quantity: 1);

        var errors = svc.ValidateProduct(dto).ToArray();

        errors.Any(e => e.Code == "NameTooShort").Should().BeTrue();
    }

    [Test]
    public void PriceBelowMin_Should_Return_Error()
    {
        var svc = BuildService(new ValidationOptions { PriceMin = 1m });
        var dto = new ProductCreateDto(Name: "OK", Price: 0.5m, Quantity: 1);

        var errors = svc.ValidateProduct(dto).ToArray();

        errors.Any(e => e.Code == "PriceTooLow").Should().BeTrue();
    }

    [Test]
    public void Valid_Product_Should_Have_No_Errors()
    {
        var svc = BuildService();
        var dto = new ProductCreateDto(Name: "Water", Price: 2.5m, Quantity: 10);

        svc.ValidateProduct(dto).Should().BeEmpty();
    }
}
