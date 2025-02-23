namespace dotnet_api_erp.src.Domain.DTOs.UserContext
{
    public class UserDTO
    {
        public record CreateUserDTO(string Name, string LastName, string Cpf, string Email, DateOnly BirthDate, string Password);
    }
}