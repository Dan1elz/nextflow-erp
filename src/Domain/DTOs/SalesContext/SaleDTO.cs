using dotnet_api_erp.src.Domain.Enums;

namespace dotnet_api_erp.src.Domain.DTOs.SalesContext
{
    public class SaleDTO
    {
        public record CreateSaleDTO(Guid OrderId, Guid UserId);
        public record CreatePaymentDTO(Guid SaleId, double Amount, PaymentMethod PaymentMethod);
    }
}