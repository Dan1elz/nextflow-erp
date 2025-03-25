using dotnet_api_erp.src.Application.DTOs;
using dotnet_api_erp.src.Domain.Entities.ProductContext;
using static dotnet_api_erp.src.Domain.DTOs.ProducContext.StockMovementDTO;

namespace dotnet_api_erp.src.Application.Mappers
{
    public class StockMovementMapper
    {
        public static CreateStockMovementDTO ToCreateStockMovementDTO(CreateStockMovementDto dto, Product product, Guid UserId)
        {
            return new CreateStockMovementDTO(dto.ProductId, UserId, dto.Description, dto.MovementType, dto.Quantity, product.Price);
        }
    }
}