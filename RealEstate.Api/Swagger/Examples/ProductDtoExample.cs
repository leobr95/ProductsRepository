
using Swashbuckle.AspNetCore.Filters;
using RealEstate.Application.DTOs;

namespace RealEstate.Api.Swagger.Examples;

public class ProductDtoExample : IExamplesProvider<ProductDto>
{
    public ProductDto GetExamples() => new ProductDto(
        Id: 101,
        Name: "Cola 2L",
        Price: 5.00m,
        Quantity: 200
    );
}

public class ProductListExample : IExamplesProvider<IEnumerable<ProductDto>>
{
    public IEnumerable<ProductDto> GetExamples() => new[]
    {
        new ProductDto(101,"Cola Can 330ml",2.00m,500),
        new ProductDto(102,"Cola 2L",5.00m,200),
        new ProductDto(103,"Orange 1L",4.50m,150)
    };
}

public class ProductCreateDtoExample : IExamplesProvider<ProductCreateDto>
{
    public ProductCreateDto GetExamples() => new ProductCreateDto(
        Name: "Water 600ml",
        Price: 1.50m,
        Quantity: 800
    );
}

public class ProductUpdateDtoExample : IExamplesProvider<ProductUpdateDto>
{
    public ProductUpdateDto GetExamples() => new ProductUpdateDto(
        Name: "Tonic 1L (New)",
        Price: 3.90m,
        Quantity: 130
    );
}
