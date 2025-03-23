using dotnet_api_erp.src.Domain.Enums;

namespace dotnet_api_erp.src.Domain.DTOs.ProducContext
{
    public class StockMovementDTO
    {
        public record CreateStockMovementDTO(Guid ProductId,  Guid UserId, string Description, MovementType MovementType, int Quantity, double Quote);
    }
}