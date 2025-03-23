namespace dotnet_api_erp.src.Domain.DTOs.Base
{
    public class PersonDTO
    {
        //record é uma classe que já tem imbutido seus getters e setters(setter pelo construtor)
        public record CreatePersonDTO(string Name, string Cpf, string Email, DateOnly? BirthDate);
        public record UpdateUserDTO(string Name, DateOnly BirthDate);
        public record CreateClientDTO(string Name, string Cpf, string Email, DateOnly BirthDate, string Phone, string ZipCode, string Address);
        public record UpdateClientDTO(string? Name, string? Cpf, string? Email, DateOnly? BirthDate, string? Phone, string? ZipCode, string? Address);
    }
}