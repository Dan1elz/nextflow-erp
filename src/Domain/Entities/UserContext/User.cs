using System.ComponentModel.DataAnnotations;
using dotnet_api_erp.src.Domain.Entities.Base;
using static dotnet_api_erp.src.Domain.DTOs.Base.PersonDTO;
using static dotnet_api_erp.src.Domain.DTOs.UserContext.UserDTO;

namespace dotnet_api_erp.src.Domain.Entities.UserContext
{
    public class User(CreateUserDTO dto) : Person((CreatePersonDTO)dto)
    {
        [MinLength(8, ErrorMessage = "A Senha deve ter no minimo 8 caracteres."), Required(ErrorMessage = "Informe a Senha do Usuario.")]
        public string Password { get; private set; } = dto.Password;
    }
}

