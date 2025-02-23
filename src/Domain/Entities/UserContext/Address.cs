using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dotnet_api_erp.src.Domain.Entities.Base;
using static dotnet_api_erp.src.Domain.DTOs.UserContext.AddressDTO;

namespace dotnet_api_erp.src.Domain.Entities.UserContext
{
    public class Address(CreateAddressDTO dto) : BaseEntity
    {
        [ForeignKey("User")]
        [Required(ErrorMessage = "Por favor, insira o ID do usuário.")]
        public Guid UserId { get; private set; }  = dto.UserId;
        public virtual User? User { get; set; }

        [StringLength(150, ErrorMessage = "O tamanho máximo é de 150 caracteres.")]
        [Required(ErrorMessage = "A descrição do endereço é obrigatória.")]
        public string Description { get; private set; } = dto.Description;

        [StringLength(150, ErrorMessage = "O nome da rua deve ter no máximo 150 caracteres.")]
        [Required(ErrorMessage = "O nome da rua é obrigatório.")]
        public string Street { get; private set; }  = dto.Street;

        [StringLength(10, ErrorMessage = "O número deve ter no máximo 10 caracteres.")]
        [Required(ErrorMessage = "O número do endereço é obrigatório.")]
        public string Number { get; private set; } = dto.Number;

        [StringLength(100, ErrorMessage = "O bairro deve ter no máximo 100 caracteres.")]
        [Required(ErrorMessage = "O bairro é obrigatório.")]
        public string District { get; private set; } = dto.District;

        [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres.")]
        [Required(ErrorMessage = "A cidade é obrigatória.")]
        public string City { get; private set; } = dto.City;

        [StringLength(50, ErrorMessage = "O estado deve ter no máximo 50 caracteres.")]
        [Required(ErrorMessage = "O estado é obrigatório.")]
        public string State { get; private set; } = dto.State;

        [StringLength(150, ErrorMessage = "O complemento deve ter no máximo 150 caracteres.")]
        public string? Complement { get; private set; } = dto.Complement;

        [StringLength(15, ErrorMessage = "O CEP deve ter no máximo 15 caracteres.")]
        [Required(ErrorMessage = "O CEP é obrigatório.")]
        public string ZipCode { get; private set; } = dto.ZipCode;

        public bool IsPrimary { get; private set; } = dto.IsPrimary;

        public void Update(UpdateAddressDTO dto)
        {
            Description = dto.Description;
            Street = dto.Street;
            Number = dto.Number;
            District = dto.District;
            City = dto.City;
            State = dto.State;
            Complement = dto.Complement;
            ZipCode = dto.ZipCode;
            IsPrimary = dto.IsPrimary;
            base.Update();
        }
    }
}