using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dotnet_api_erp.src.Domain.Entities.Base;
using dotnet_api_erp.src.Domain.Enums;
using static dotnet_api_erp.src.Domain.DTOs.SalesContext.SaleDTO;

namespace dotnet_api_erp.src.Domain.Entities.SalesContext
{
    public class Payment(CreatePaymentDTO dto) : BaseEntity
    {
        [ForeignKey("Sale"), Required(ErrorMessage = "Por favor, insira o ID da Venda.")]
        public Guid SaleId { get; private init; }  = dto.SaleId;
        public virtual Sale? Sale { get; set; }
        [Required(ErrorMessage = "Por favor, o Metodo de Pagamento é obrigatório.")]
        public PaymentMethod PaymentMethod {get; private init;} = dto.PaymentMethod;
        
        [Range(0.0, double.MaxValue, ErrorMessage = "O preço deve ser um valor positivo")]
        [Required(ErrorMessage = "Por favor, insira o valor do pagamento.")]
        public double Amount { get; private set; } = dto.Amount;

    }
}