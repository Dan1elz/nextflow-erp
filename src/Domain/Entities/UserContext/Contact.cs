using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dotnet_api_erp.src.Domain.Entities.Base;
using static dotnet_api_erp.src.Domain.DTOs.UserContext.ContactsDTO;

namespace dotnet_api_erp.src.Domain.Entities.UserContext
{
   public class Contact(CreateContactDTO dto) : BaseEntity
    {
        [ForeignKey("User"), Required(ErrorMessage = "Por favor insira o Id do Usuario")]
        public Guid UserId { get; private set; } = dto.UserId;
        public virtual User? User {get; set;}

        [StringLength(150, ErrorMessage = "O tamanho máximo é de 255 caracteres.")]
        [Required(ErrorMessage = "A descrição do contato é obrigatória.")]
        public string Description { get; private set; } =  dto.Description;

        [StringLength(20, ErrorMessage = "O número de telefone deve ter no máximo 20 caracteres.")]
        public string? Phone { get; private set; } =  dto.Phone;

        [StringLength(100, ErrorMessage = "O endereço de email deve ter no máximo 100 caracteres.")]
        [EmailAddress(ErrorMessage = "O endereço de email não é válido.")]
        public string? Email { get; private set; } =  dto.Email;
        public bool IsPrimary { get; private set; } = dto.IsPrimary;

        public void Update(UpdateContactDTO dto)
        {
            Description = dto.Description;
            Phone = dto.Phone;
            Email = dto.Email;
            IsPrimary = dto.IsPrimary;
            base.Update();
        }
    }
}