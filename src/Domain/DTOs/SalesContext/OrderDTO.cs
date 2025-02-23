using dotnet_api_erp.src.Domain.Enums;

namespace dotnet_api_erp.src.Domain.DTOs.SalesContext
{
    public class OrderDTO
    {
        public record CreateOrderDTO(Guid ClientId, double TotalAmount);
        public record UpdateOrderDTO(OrderStatus Status);
        public record CreateOrderItemDTO(Guid OrderId, Guid ProductId, int Quantity, int Price);
    }
}