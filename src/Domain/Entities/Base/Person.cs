using System.ComponentModel.DataAnnotations;
using static dotnet_api_erp.src.Domain.DTOs.Base.PersonDTO;

namespace dotnet_api_erp.src.Domain.Entities.Base
{
    public class Person(CreatePersonDTO createPersonDTO) : BaseEntity()
    {
        [StringLength(100, ErrorMessage = "O Nome deve ter no máximo 100 caracteres."), Required(ErrorMessage = "Informe o nome")]
        public string Name { get; protected set; } = createPersonDTO.Name;

        [StringLength(255, ErrorMessage = "O Email deve ter no máximo 255 caracteres."), EmailAddress(ErrorMessage = "E-mail inválido"), Required(ErrorMessage = "Informe o email")]
        public string Email { get; protected set; } = createPersonDTO.Email;

        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter exatamente 14 caracteres.")]
        public string Cpf { get; protected set; } = string.Empty; 
        public DateOnly? BirthDate { get; protected set; } = createPersonDTO.BirthDate;

        public static bool ValidateCPF(string cpf)
        {
          
            cpf = FormatarCpf(cpf);

            if (cpf.Length != 11)
                return false;

            bool allDigitsEqual = true;
            for (int i = 1; i < 11 && allDigitsEqual; i++)
            {
                if (cpf[i] != cpf[0])
                    allDigitsEqual = false;
            }
            if (allDigitsEqual)
                return false;

            // primeiro dígito verificador
            int[] peso1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (cpf[i] - '0') * peso1[i];
            int resto = soma % 11;
            int digito1 = (resto < 2) ? 0 : 11 - resto;

            // segundo dígito verificador
            int[] peso2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += (cpf[i] - '0') * peso2[i];
            resto = soma % 11;
            int digito2 = (resto < 2) ? 0 : 11 - resto;

            // Verificar se os dígitos calculados são iguais aos dígitos do CPF
            return (cpf[9] - '0' == digito1) && (cpf[10] - '0' == digito2);
        }
        public static string FormatarCpf(string cpf)
        {
            return cpf.Replace(".", "").Replace("-", "");
        }
    }
}
