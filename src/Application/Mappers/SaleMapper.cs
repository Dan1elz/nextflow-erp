using static dotnet_api_erp.src.Application.DTOs.SaleDTO;
using static dotnet_api_erp.src.Domain.DTOs.SalesContext.SaleDTO;

namespace dotnet_api_erp.src.Application.Mappers
{
    public class SaleMapper
    {
        public static CreateSaleDTO ToCreateSaleDTO(Guid UserId, CreateSaleDto dto)
        {
            return new CreateSaleDTO(dto.OrderId, UserId);
        }
        public static List<CreatePaymentDTO> ToCreatePaymentDTO(Guid SaleId, List<CreatePaymentDto> dtos)
        {
            return [.. dtos.Select(dto => new CreatePaymentDTO(SaleId, dto.Amount, dto. PaymentMethod))];

        }
    }
}