using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dotnet_api_erp.src.Domain.Entities.Base;
using static dotnet_api_erp.src.Domain.DTOs.ProducContext.CategoryDTO;

namespace dotnet_api_erp.src.Domain.Entities.ProductContext
{
    public class CategoryProduct(CreateCategoryProductDTO dto): BaseEntity
    {
        [ForeignKey("Category"), Required(ErrorMessage = "Por favor, insira o ID da categoria.")]
        public Guid CategoryId { get; private init; }  = dto.CategoryId;
        public virtual Category? Category { get; set; }

        [ForeignKey("Product"), Required(ErrorMessage = "Por favor, insira o ID do produto.")]
        public Guid ProductId { get; private init; }  = dto.ProductId;
        public virtual Product? Product { get; set; }
    }
}