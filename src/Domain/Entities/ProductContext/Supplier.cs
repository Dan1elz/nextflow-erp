using dotnet_api_erp.src.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using static dotnet_api_erp.src.Domain.DTOs.ProducContext.SuppliersDTO;

namespace dotnet_api_erp.src.Domain.Entities.ProductContext
{
    public class Supplier : BaseEntity
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(150, ErrorMessage = "O nome não pode exceder 150 caracteres")]
        public string Name { get; private set; } = string.Empty;

        [Required(ErrorMessage = "O endereço é obrigatório")]
        [StringLength(250, ErrorMessage = "O endereço não pode exceder 250 caracteres")]
        public string Address { get; private set; } = string.Empty;

        [Required(ErrorMessage = "O CEP é obrigatório")]
        [StringLength(10, ErrorMessage = "O CEP deve ter no máximo 10 caracteres")]
        public string ZipCode { get; private set; } = string.Empty;

        [Required(ErrorMessage = "O CNPJ é obrigatório")]
        [StringLength(18, MinimumLength = 14, ErrorMessage = "O CNPJ deve ter entre 14 e 18 caracteres")]
        public string CNPJ { get; private set; } = string.Empty;

        [Required(ErrorMessage = "O telefone é obrigatório")]
        [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
        public string Phone { get; private set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        [StringLength(150, ErrorMessage = "O e-mail não pode exceder 150 caracteres")]
        public string Email { get; private set; } = string.Empty;
        public virtual ICollection<Product> Products { get; set; } = [];
        private Supplier() { }
        public Supplier(CreateSupplierDTO dto) : base()
        {
            Name = dto.Name;
            Address = dto.Address;
            ZipCode = dto.ZipCode;
            CNPJ = dto.CNPJ;
            Phone = dto.Phone;
            Email = dto.Email;
        }
        public void Update(UpdateSupplierDTO dto)
        {
            Name = dto.Name;
            Address = dto.Address;
            ZipCode = dto.ZipCode;
            CNPJ = dto.CNPJ;
            Phone = dto.Phone;
            Email = dto.Email;
            base.Update();
        }
    }
}
