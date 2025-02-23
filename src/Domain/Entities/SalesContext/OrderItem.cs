using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dotnet_api_erp.src.Domain.Entities.Base;
using dotnet_api_erp.src.Domain.Entities.ProductContext;
using static dotnet_api_erp.src.Domain.DTOs.SalesContext.OrderDTO;

namespace dotnet_api_erp.src.Domain.Entities.SalesContext
{
    public class OrderItem(CreateOrderItemDTO dto): BaseEntity
    {
        [ForeignKey("Order")]
        [Required(ErrorMessage = "Por favor, insira o ID da Ordem de Venda.")]
        public Guid OrderId { get; private init; }  = dto.OrderId;
        public virtual Order? Order { get; set; }

        [ForeignKey("Product")]
        [Required(ErrorMessage = "Por favor, insira o ID do produto.")]
        public Guid ProductId { get; private init; }  = dto.ProductId;
        public virtual Product? Product { get; set; }
        public int Quantity {get; private init;} = dto.Quantity;
        public double Price{get; private init;} = dto.Price;
    }
}