using dotnet_api_erp.src.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static dotnet_api_erp.src.Domain.DTOs.ProducContext.ProductDTO;

namespace dotnet_api_erp.src.Domain.Entities.ProductContext
{
    public class Product: BaseEntity
    {
        [ForeignKey("Supplier"), Required(ErrorMessage = "Por favor, insira o ID do fornecedor.")]
        public Guid SupplierId { get; init; }
        public virtual Supplier? Supplier { get; set; }
    
        [Required(ErrorMessage = "O código é obrigatório")]
        public string ProductCode { get; set;} = string.Empty;

        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres")]
        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres")]
        [Required(ErrorMessage = "A descrição é obrigatória")]
        public string Description { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "O caminho da imagem não pode exceder 255 caracteres")]
        public string Image { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade deve ser um valor positivo")]
        public int Quantity { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "O preço deve ser um valor positivo")]
        public double Price { get; set; }

        [Required(ErrorMessage = "A validade é obrigatória")]
        public DateOnly Validity { get; set; }
        public virtual ICollection<CategoryProduct> CategoryProduct { get; set; } = [];
        public virtual ICollection<StockMovement> StockMovement { get; set; } = [];
        private Product(){ }
        public Product(CreateProductDTO dto) : base() 
        {
            SupplierId = dto.SupplierId;
            ProductCode = dto.ProductCode;
            Name = dto.Name;
            Description = dto.Description;
            Image = dto.Image;
            Quantity = dto.Quantity;
            Price = dto.Price;
            Validity = dto.Validity;
        }
        public void Update(UpdateProductDTO dto)
        {
            ProductCode = dto.ProductCode;
            Name = dto.Name;
            Description = dto.Description;
            Image = dto.Image;
            Quantity = dto.Quantity;
            Price = dto.Price;
            Validity = dto.Validity;
            base.Update();
        }
    }

}