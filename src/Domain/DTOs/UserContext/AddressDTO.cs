namespace dotnet_api_erp.src.Domain.DTOs.UserContext
{
    public class AddressDTO
    {
        public record CreateAddressDTO(
            Guid UserId,
            string Description,
            string Street,
            string Number,
            string District,
            string City,
            string State,
            string? Complement,
            string ZipCode,
            bool IsPrimary
        );
        public record UpdateAddressDTO(
            string Description,
            string Street,
            string Number,
            string District,
            string City,
            string State,
            string? Complement,
            string ZipCode,
            bool IsPrimary
        );
    }
}