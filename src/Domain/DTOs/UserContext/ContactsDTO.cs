namespace dotnet_api_erp.src.Domain.DTOs.UserContext
{
    public class ContactsDTO
    {
        public record CreateContactDTO(
            Guid UserId, 
            string Description, 
            string Phone, 
            string Email, 
            bool IsPrimary
        );
        public record UpdateContactDTO(
            string Description, 
            string Phone, 
            string Email, 
            bool IsPrimary
        );
    }
}