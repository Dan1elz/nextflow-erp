using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dotnet_api_erp.src.Domain.Entities.Base;
using dotnet_api_erp.src.Domain.Enums;
using static dotnet_api_erp.src.Domain.DTOs.SalesContext.OrderDTO;

namespace dotnet_api_erp.src.Domain.Entities.SalesContext
{
    public class Order(CreateOrderDTO dto) : BaseEntity
    {
        [ForeignKey("Client")]
        [Required(ErrorMessage = "Por favor, insira o ID do Cliente.")]
        public Guid ClientId { get; private set; }  = dto.ClientId;
        public virtual Client? Client { get; set; }
        public OrderStatus OrderStatus {get; set;} = OrderStatus.PendingPayment;
        public double TotalAmount {get; private set;} = dto.TotalAmount;

        public void Update(UpdateOrderDTO dto)
        {
            OrderStatus = dto.Status;
            base.Update();
        }
    }
}