using RealEstate.Application.DTOs;


namespace RealEstate.Application.Validation;

public interface IValidationService
{
    IEnumerable<ValidationError> ValidateProduct(ProductCreateDto dto);
    IEnumerable<ValidationError> ValidateProduct(ProductUpdateDto dto);
}
