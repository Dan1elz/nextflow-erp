using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dotnet_api_erp.src.Domain.Entities.Base;
using dotnet_api_erp.src.Domain.Enums;
using static dotnet_api_erp.src.Domain.DTOs.SalesContext.OrderDTO;

namespace dotnet_api_erp.src.Domain.Entities.SalesContext
{
    public class Order : BaseEntity
    {
        [ForeignKey("Client"), Required(ErrorMessage = "Por favor, insira o ID do Cliente.")]
        public Guid ClientId { get; set; }
        public virtual Client? Client { get; set; }
        [Required(ErrorMessage = "O Status do Pedido é obrigatório.")]
        public OrderStatus OrderStatus {get; set;} = OrderStatus.PendingPayment;
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor total deve ser maior que zero.")]
        [Required(ErrorMessage = "O valor total é obrigatório.")]
        public double TotalAmount {get; set;}
        public virtual ICollection<OrderItem> OrderItems { get; set; } = [];
        private Order() { }
        public Order(CreateOrderDTO dto) : base()
        {
            ClientId = dto.ClientId;
            TotalAmount = dto.TotalAmount;
        }
        public new void Delete()
        {
           base.Delete();
           OrderStatus = OrderStatus.Canceled;
        } 
        public void Update(OrderStatus orderStatus)
        {
            OrderStatus = orderStatus;
            base.Update();
        }
    }
}