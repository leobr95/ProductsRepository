using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RealEstate.Application;
using RealEstate.Application.DTOs;
using RealEstate.Application.Services;
using RealEstate.Application.Validation;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces;
using RealEstate.Tests.Tests.Fakes;

namespace RealEstate.Tests.Tests.Application;

[TestFixture]
public class ProductServiceTests
{
    private static IValidationService BuildValidator()
    {
        var options = Options.Create(new ValidationOptions
        {
            NameMinLength = 1,
            NameMaxLength = 100,
            PriceMin = 0m,
            QuantityMin = 0
        });
        var loc = new FakeStringLocalizer<SharedResources>();
        return new ValidationService(options, loc);
    }

    [Test]
    public async Task Create_Valid_Product_Should_Return_Id()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.CreateAsync(It.IsAny<Product>(), default))
            .ReturnsAsync(123);

        var svc = new ProductService(repo.Object, BuildValidator());
        var dto = new ProductCreateDto("Cola", 2.0m, 10);

        var (ok, id, errors) = await svc.CreateAsync(dto);

        ok.Should().BeTrue();
        id.Should().Be(123);
        errors.Should().BeEmpty();
        repo.Verify(r => r.CreateAsync(It.Is<Product>(p => p.Name == "Cola" && p.Price == 2.0m && p.Quantity == 10), default), Times.Once);
    }

    [Test]
    public async Task Create_Invalid_Product_Should_Return_Errors()
    {
        var repo = new Mock<IProductRepository>();
        var svc = new ProductService(repo.Object, BuildValidator());
        var dto = new ProductCreateDto("", -1m, -5);

        var (ok, id, errors) = await svc.CreateAsync(dto);

        ok.Should().BeFalse();
        id.Should().BeNull();
        errors.Should().NotBeEmpty();
        repo.Verify(r => r.CreateAsync(It.IsAny<Product>(), default), Times.Never);
    }

    [Test]
    public async Task Update_NotFound_Should_Return_False()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((Product?)null);

        var svc = new ProductService(repo.Object, BuildValidator());
        var (ok, errors) = await svc.UpdateAsync(999, new ProductUpdateDto("x", 1m, 1));

        ok.Should().BeFalse();
        errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Test]
    public async Task Update_Valid_Should_Return_True()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(new Product { ProductId = 1, Name = "Old", Price = 1m, Quantity = 1 });

        repo.Setup(r => r.UpdateAsync(It.Is<Product>(p => p.ProductId == 1), default))
            .ReturnsAsync(true);

        var svc = new ProductService(repo.Object, BuildValidator());
        var (ok, errors) = await svc.UpdateAsync(1, new ProductUpdateDto("New", 2m, 10));

        ok.Should().BeTrue();
        errors.Should().BeEmpty();
        repo.VerifyAll();
    }

    [Test]
    public async Task Delete_Should_Propagate_Repository_Result()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.DeleteAsync(5, default)).ReturnsAsync(true);

        var svc = new ProductService(repo.Object, BuildValidator());
        var ok = await svc.DeleteAsync(5);

        ok.Should().BeTrue();
        repo.Verify(r => r.DeleteAsync(5, default), Times.Once);
    }
}
