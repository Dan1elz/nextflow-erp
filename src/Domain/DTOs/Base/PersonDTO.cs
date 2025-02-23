using dotnet_api_erp.src.Domain.DTOs.UserContext;

namespace dotnet_api_erp.src.Domain.DTOs.Base
{
    public class PersonDTO
    {
        //record é uma classe que já tem imbutido seus getters e setters(setter pelo construtor)
        public record CreatePersonDTO(string Name, string LastName, string Cpf, string Email, DateOnly BirthDate)
        {
            public static explicit operator CreatePersonDTO(UserDTO.CreateUserDTO v)
            {
                throw new NotImplementedException();
            }
        }
    }
}