using dotnet_api_erp.src.Domain.Enums;

namespace dotnet_api_erp.src.Application.DTOs
{
    public class CreateStockMovementDto
    {
        public Guid ProductId {get; set;}
        public required string Description {get; set;}
        public MovementType MovementType {get; set;}
        public int Quantity {get; set;}
    }
}