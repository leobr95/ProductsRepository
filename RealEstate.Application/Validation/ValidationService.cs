using Microsoft.Extensions.Localization;          
using Microsoft.Extensions.Options;              
using RealEstate.Application.DTOs;
namespace RealEstate.Application.Validation;

public class ValidationService : IValidationService
{
    private readonly ValidationOptions _opts;
    private readonly IStringLocalizer _loc;

    public ValidationService(IOptions<ValidationOptions> opts, IStringLocalizer<SharedResources> loc)
    {
        _opts = opts.Value;
        _loc  = loc;
    }

    public IEnumerable<ValidationError> ValidateProduct(ProductCreateDto dto)
        => Validate(dto.Name, dto.Price, dto.Quantity);

    public IEnumerable<ValidationError> ValidateProduct(ProductUpdateDto dto)
        => Validate(dto.Name, dto.Price, dto.Quantity);

    private IEnumerable<ValidationError> Validate(string? name, decimal price, int quantity)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add(new("NameRequired", _loc["Validation.NameRequired"]));

        if (!string.IsNullOrWhiteSpace(name))
        {
            if (name.Length < _opts.NameMinLength)
                errors.Add(new("NameTooShort", _loc["Validation.NameMinLength", _opts.NameMinLength]));
            if (name.Length > _opts.NameMaxLength)
                errors.Add(new("NameTooLong", _loc["Validation.NameMaxLength", _opts.NameMaxLength]));
        }

        // Price bounds
        if (price < _opts.PriceMin)
            errors.Add(new("PriceTooLow", _loc["Validation.PriceMin", _opts.PriceMin]));
        if (_opts.PriceMax.HasValue && price > _opts.PriceMax.Value)
            errors.Add(new("PriceTooHigh", _loc["Validation.PriceMax", _opts.PriceMax.Value]));

        // Quantity bounds
        if (quantity < _opts.QuantityMin)
            errors.Add(new("QuantityTooLow", _loc["Validation.QuantityMin", _opts.QuantityMin]));
        if (_opts.QuantityMax.HasValue && quantity > _opts.QuantityMax.Value)
            errors.Add(new("QuantityTooHigh", _loc["Validation.QuantityMax", _opts.QuantityMax.Value]));

        return errors;
    }
}
