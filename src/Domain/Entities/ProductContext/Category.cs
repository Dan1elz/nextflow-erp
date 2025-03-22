using System.ComponentModel.DataAnnotations;
using dotnet_api_erp.src.Domain.Entities.Base;
using static dotnet_api_erp.src.Domain.DTOs.ProducContext.CategoryDTO;

namespace dotnet_api_erp.src.Domain.Entities.ProductContext
{
    public class Category(CreateCategoryDTO dto) : BaseEntity
    {
        [StringLength(100, ErrorMessage = "A Descrição deve ter no maximo 100 caracteres")]
        [Required(ErrorMessage = "A Descrição é obrigatória")]
        public string Description {get; private set;} = dto.Description;
        public virtual ICollection<CategoryProduct> CategoryProduct { get; set; } = [];

        public void Update(UpdateCategoryDTO dto)
        {
            Description = dto.Description;
            base.Update();
        }
    }
}