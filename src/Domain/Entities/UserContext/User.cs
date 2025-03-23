using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using dotnet_api_erp.src.Domain.Entities.Base;
using static dotnet_api_erp.src.Domain.DTOs.Base.PersonDTO;

namespace dotnet_api_erp.src.Domain.Entities.UserContext
{
    public class User : Person
    {
        [MinLength(8, ErrorMessage = "A Senha deve ter no minimo 8 caracteres")]
        [JsonIgnore] public string? Password { get; set; } = null;
        public virtual ICollection<Address> Addresses { get; set; } = [];
        public virtual ICollection<Contact> Contacts { get; set; } = [];
        private User() : base(new CreatePersonDTO(string.Empty, string.Empty, string.Empty, null)) { }
        public User(CreatePersonDTO dto) : base(new CreatePersonDTO(dto.Name, FormatarCpf(dto.Cpf), dto.Email, dto.BirthDate)) {} 
        public void Update(UpdateUserDTO dto)
        {
            base.Name = dto.Name;
            base.BirthDate = dto.BirthDate;
            base.Update();
        }
    }
}

