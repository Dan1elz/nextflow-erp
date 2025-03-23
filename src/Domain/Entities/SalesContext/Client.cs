using System.ComponentModel.DataAnnotations;
using dotnet_api_erp.src.Domain.Entities.Base;
using static dotnet_api_erp.src.Domain.DTOs.Base.PersonDTO;

namespace dotnet_api_erp.src.Domain.Entities.SalesContext
{
    public class Client: Person
    {
        [StringLength(20, ErrorMessage = "O número de telefone deve ter no máximo 20 caracteres.")]
        [Required(ErrorMessage = "O telefone é obrigatório.")]
        public string Phone { get; private set; } = string.Empty;
        
        [StringLength(15, ErrorMessage = "O CEP deve ter no máximo 15 caracteres.")]
        [Required(ErrorMessage = "O CEP é obrigatório.")]
        public string ZipCode { get; private set; } = string.Empty;
        
        [StringLength(150, ErrorMessage = "O endereço deve ter no máximo 150 caracteres.")]
        [Required(ErrorMessage = "O endereço é obrigatório.")]
        public string Address { get; private set; } = string.Empty;

        public virtual ICollection<Order> Orders { get; set; } = [];
        public Client() : base(new CreatePersonDTO(string.Empty, string.Empty, string.Empty, null)) { }
        public Client(CreateClientDTO dto) : base(new CreatePersonDTO(dto.Name, FormatarCpf(dto.Cpf), dto.Email, dto.BirthDate))
        {
            this.Phone = dto.Phone;
            this.ZipCode = dto.ZipCode;
            this.Address = dto.Address;
        }
        public void Update(UpdateClientDTO dto)
        {
            base.Name = dto.Name ?? base.Name;
            base.Cpf = dto.Cpf ?? base.Cpf;
            base.Email = dto.Email ?? base.Email;
            base.BirthDate = dto.BirthDate ?? base.BirthDate;
            this.Phone = dto.Phone ?? this.Phone;
            this.ZipCode = dto.ZipCode ?? this.ZipCode;
            this.Address = dto.Address ?? this.Address;
            base.Update();
        }
    }
}