using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dotnet_api_erp.src.Domain.Entities.Base;
using dotnet_api_erp.src.Domain.Enums;
using static dotnet_api_erp.src.Domain.DTOs.SalesContext.SaleDTO;

namespace dotnet_api_erp.src.Domain.Entities.SalesContext
{
    public class Payment : BaseEntity
    {
        [ForeignKey("Sale"), Required(ErrorMessage = "Por favor, insira o ID da Venda.")]
        public Guid SaleId { get; private init; }
        public virtual Sale? Sale { get; set; }
        [Required(ErrorMessage = "Por favor, o Metodo de Pagamento é obrigatório.")]
        public PaymentMethod PaymentMethod {get; private init;}
        
        [Range(0.0, double.MaxValue, ErrorMessage = "O preço deve ser um valor positivo")]
        [Required(ErrorMessage = "Por favor, insira o valor do pagamento.")]
        public double Amount { get; private set; }
        private Payment() { }
        public Payment(CreatePaymentDTO dto) : base()
        {
            SaleId = dto.SaleId;
            PaymentMethod = dto.PaymentMethod;
            Amount = dto.Amount;
        }
    }
}