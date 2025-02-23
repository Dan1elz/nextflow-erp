using System.ComponentModel.DataAnnotations;
using static dotnet_api_erp.src.Domain.DTOs.Base.PersonDTO;

namespace dotnet_api_erp.src.Domain.Entities.Base
{
    public class Person(CreatePersonDTO createPersonDTO) : BaseEntity()
    {
        [StringLength(100, ErrorMessage = "O Nome deve ter no máximo 100 caracteres."), Required(ErrorMessage = "Informe o nome")]
        public string Name { get; protected set; } = createPersonDTO.Name;

        [StringLength(255, ErrorMessage = "O Email deve ter no máximo 255 caracteres."), Required(ErrorMessage = "Informe o email")]
        public string Email { get; protected set; } = createPersonDTO.Email;

        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter exatamente 14 caracteres.")]
        public string Cpf { get; protected set; } = string.Empty; 
        public DateOnly BirthDate { get; protected set; } = createPersonDTO.BirthDate;

        public static bool ValidateCPF(string cpf)
        {
            // Validação do CPF
            string cpfFormatado = FormatarCpf(cpf);
            

            return false;
            // Implement CPF validation logic here
        }
        public static string FormatarCpf(string cpf)
        {
            return cpf.Replace(".", "").Replace("-", "");
        }
    }
}
