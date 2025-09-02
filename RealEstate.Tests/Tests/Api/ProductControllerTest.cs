using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Moq;
using NUnit.Framework;
using RealEstate.Application;
using RealEstate.Application.DTOs;
using RealEstate.Application.Interfaces;
using RealEstate.Controllers;
using RealEstate.Tests.Tests.Fakes;

namespace RealEstate.Tests.Tests.Api;

[TestFixture]
public class ProductsControllerTests
{
    private static ProductsController BuildController(Mock<IProductService> svcMock)
    {
        IStringLocalizer<SharedResources> loc = new FakeStringLocalizer<SharedResources>();
        return new ProductsController(svcMock.Object, loc);
    }

    [Test]
    public async Task GetAll_Should_Return_Ok_With_List()
    {
        var svc = new Mock<IProductService>();
        svc.Setup(s => s.GetAllAsync(null, It.IsAny<CancellationToken>()))
           .ReturnsAsync(new[] { new ProductDto(1, "Cola", 2m, 10) });

        var ctrl = BuildController(svc);
        var res = await ctrl.GetAll(null, default);

        var ok = res.Result as OkObjectResult;
        ok.Should().NotBeNull();
        var list = ok!.Value as IEnumerable<ProductDto>;
        list.Should().NotBeNull();
        list!.Should().HaveCount(1);
    }

    [Test]
    public async Task GetById_NotFound_Should_Return_404()
    {
        var svc = new Mock<IProductService>();
        svc.Setup(s => s.GetByIdAsync(99, It.IsAny<CancellationToken>()))
           .ReturnsAsync((ProductDto?)null);

        var ctrl = BuildController(svc);
        var res = await ctrl.GetById(99, default);

        res.Result.Should().BeOfType<NotFoundObjectResult>();
    }

[Test]
public async Task Create_Valid_Should_Return_Created()
{
    // Arrange
    var svc = new Mock<IProductService>();
    svc.Setup(s => s.CreateAsync(It.IsAny<ProductCreateDto>(), It.IsAny<CancellationToken>()))
       .ReturnsAsync((true, 123, Array.Empty<string>()));

    var ctrl = BuildController(svc); 

    // Act
    var res = await ctrl.Create(new ProductCreateDto("Water", 1.5m, 5), default);

    // Assert
    res.Should().BeOfType<CreatedAtActionResult>();
    var created = (CreatedAtActionResult)res;

    // 1) Asegura que apunta al endpoint correcto
    created.ActionName.Should().Be(nameof(ProductsController.GetById));

    // 2) Valida que el route value 'id' exista y tenga 123
    created.RouteValues.Should().NotBeNull();
    created.RouteValues!.Should().ContainKey("id");
    var routeId = created.RouteValues["id"];
    // RouteValues guarda object; conviértelo a int
    Convert.ToInt32(routeId).Should().Be(123);

    // 3) (Opcional) Si devuelves cuerpo con { id = 123 }, léelo por reflexión, no dynamic
    if (created.Value is not null)
    {
        var prop = created.Value.GetType().GetProperty("id"); // minúscula, tal como new { id }
        prop.Should().NotBeNull("the created payload should have an 'id' property");
        var payloadId = prop!.GetValue(created.Value, null);
        Convert.ToInt32(payloadId).Should().Be(123);
    }
}


    [Test]
    public async Task Update_Invalid_Should_Return_BadRequest()
    {
        var svc = new Mock<IProductService>();
        svc.Setup(s => s.UpdateAsync(1, It.IsAny<ProductUpdateDto>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync((false, new[] { "Invalid" }));

        var ctrl = BuildController(svc);
        var res = await ctrl.Update(1, new ProductUpdateDto("", -1m, -1), default);

        res.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test]
    public async Task Delete_NotFound_Should_Return_404()
    {
        var svc = new Mock<IProductService>();
        svc.Setup(s => s.DeleteAsync(77, It.IsAny<CancellationToken>()))
           .ReturnsAsync(false);

        var ctrl = BuildController(svc);
        var res = await ctrl.Delete(77, default);

        res.Should().BeOfType<NotFoundObjectResult>();
    }
}




