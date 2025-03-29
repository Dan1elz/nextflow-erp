using dotnet_api_erp.src.Domain.Enums;

namespace dotnet_api_erp.src.Application.DTOs
{
    public class SaleDTO
    {
        public class CreateSaleDto
        {
            public Guid OrderId { get; set; }
            public List<CreatePaymentDto> Payments { get; set; } = [];
        }
        public class CreatePaymentDto
        {
            public double Amount { get; set; }
            public PaymentMethod PaymentMethod { get; set; }
        }
    }
}