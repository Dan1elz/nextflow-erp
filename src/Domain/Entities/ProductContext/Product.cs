using dotnet_api_erp.src.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static dotnet_api_erp.src.Domain.DTOs.ProducContext.ProductDTO;

namespace dotnet_api_erp.src.Domain.Entities.ProductContext
{
    public class Product(CreateProductDTO dto) : BaseEntity
    {
        [ForeignKey("Supplier"), Required(ErrorMessage = "Por favor, insira o ID do fornecedor.")]
        public Guid SupplierId { get; private init; }  = dto.SupplierId;
        public virtual Supplier? Supplier { get; set; }

        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres")]
        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Name { get; private set; } = dto.Name;

        [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres")]
        [Required(ErrorMessage = "A descrição é obrigatória")]
        public string Description { get; private set; } = dto.Description;

        [StringLength(255, ErrorMessage = "O caminho da imagem não pode exceder 255 caracteres")]
        public string Image { get; private set; } = dto.Image;

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade deve ser um valor positivo")]
        public int Quantity { get; private set; } = dto.Quantity;

        [Range(0.0, double.MaxValue, ErrorMessage = "O preço deve ser um valor positivo")]
        public double Price { get; private set; } = dto.Price;

        [Required(ErrorMessage = "A validade é obrigatória")]
        public DateTime Validity { get; private set; } = dto.Validity;
        public virtual ICollection<CategoryProduct> CategoryProduct { get; set; } = [];
        public void Update(UpdateProductDTO dto)
        {
            Description = dto.Description;
            Image = dto.Image;
            Quantity = dto.Quantity;
            Price = dto.Price;
            Validity = dto.Validity;
            base.Update();
        }
    }

}