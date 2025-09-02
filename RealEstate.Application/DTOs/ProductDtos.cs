using System;

namespace RealEstate.Application.DTOs;


public record ProductDto(int Id, string Name, decimal Price, int Quantity);

public record ProductCreateDto(string Name, decimal Price, int Quantity);

public record ProductUpdateDto(string Name, decimal Price, int Quantity);
