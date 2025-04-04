using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dotnet_api_erp.src.Domain.Entities.Base;
using dotnet_api_erp.src.Domain.Entities.UserContext;
using dotnet_api_erp.src.Domain.Enums;
using static dotnet_api_erp.src.Domain.DTOs.ProducContext.StockMovementDTO;

namespace dotnet_api_erp.src.Domain.Entities.ProductContext
{
    public class StockMovement : BaseEntity
    {
        [ForeignKey("Product"), Required(ErrorMessage = "Por favor, insira o ID do produto.")]
        public Guid ProductId { get; init; }
        public virtual Product? Product { get; set; }

        [ForeignKey("User"), Required(ErrorMessage = "Por favor insira o Id do Usuario")]
        public Guid UserId {get; init;}
        public virtual User? User {get; set;}
        
        [StringLength(100, ErrorMessage = "A Descrição deve ter no maximo 100 caracteres")]
        [Required(ErrorMessage = "A Descrição é obrigatória")]
        public string Description {get; set;} = string.Empty;

        [Required(ErrorMessage = "O tipo de movimento é obrigatório.")]
        public MovementType MovementType { get; init; }

        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade deve ser um valor positivo")]
        public int Quantity { get; init; }

        [Range(0.01, double.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public double Quote { get; init; }
        private StockMovement() { }
        public StockMovement(CreateStockMovementDTO dto)
        {
            ProductId = dto.ProductId;
            UserId = dto.UserId;
            Description = dto.Description;
            MovementType = dto.MovementType;
            Quantity = dto.Quantity;
            Quote = dto.Quote;
        }
    }
}