using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using dotnet_api_erp.src.Domain.Entities.Base;
using static dotnet_api_erp.src.Domain.DTOs.Base.PersonDTO;

namespace dotnet_api_erp.src.Domain.Entities.UserContext
{
    public class User(CreatePersonDTO dto) : Person(dto)
    {
        [MinLength(8, ErrorMessage = "A Senha deve ter no minimo 8 caracteres")]
        [JsonIgnore] public string? Password { get; set; } = null;

        public void Update(UpdateUserDTO dto)
        {
            base.Name = dto.Name;
            base.BirthDate = dto.BirthDate;
            base.Update();
        }
    }
}

