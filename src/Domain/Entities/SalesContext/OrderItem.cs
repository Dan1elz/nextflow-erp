using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dotnet_api_erp.src.Domain.Entities.Base;
using dotnet_api_erp.src.Domain.Entities.ProductContext;
using static dotnet_api_erp.src.Domain.DTOs.SalesContext.OrderDTO;

namespace dotnet_api_erp.src.Domain.Entities.SalesContext
{
    public class OrderItem : BaseEntity
    {
        [ForeignKey("Order"), Required(ErrorMessage = "Por favor, insira o ID da Ordem de Venda.")]
        public Guid OrderId { get; private init; }
        public virtual Order? Order { get; set; }

        [ForeignKey("Product"), Required(ErrorMessage = "Por favor, insira o ID do produto.")]
        public Guid ProductId { get; private init; }
        public virtual Product? Product { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade deve ser um valor positivo")]
        [Required(ErrorMessage = "Por favor, insira a quantidade.")]
        public int Quantity {get; private init;}
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser um valor positivo")]
        [Required(ErrorMessage = "Por favor, insira o preço.")]
        public double Price {get; private init;}
        private OrderItem() { }
        public OrderItem(CreateOrderItemDTO dto) : base()
        {
            OrderId = dto.OrderId;
            ProductId = dto.ProductId;
            Quantity = dto.Quantity;
            Price = dto.Price;
        }
    }
}