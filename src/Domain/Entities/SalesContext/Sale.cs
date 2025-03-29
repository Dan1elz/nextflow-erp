using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dotnet_api_erp.src.Domain.Entities.Base;
using dotnet_api_erp.src.Domain.Entities.UserContext;
using static dotnet_api_erp.src.Domain.DTOs.SalesContext.SaleDTO;

namespace dotnet_api_erp.src.Domain.Entities.SalesContext
{
    public class Sale : BaseEntity
    {
        [ForeignKey("Order"), Required(ErrorMessage = "Por favor, insira o ID da Ordem de Venda.")]
        public Guid OrderId { get; init; }
        public virtual Order? Order { get; set; }

        [ForeignKey("User"), Required(ErrorMessage = "Por favor insira o Id do Usuario")]
        public Guid UserId {get; set;}
        public virtual User? User {get; set;}
        public virtual ICollection<Payment> Payments { get; set; } = [];
        private Sale() { }
        public Sale(CreateSaleDTO dto) : base()
        {
            OrderId = dto.OrderId;
            UserId = dto.UserId;
        }
    }
}