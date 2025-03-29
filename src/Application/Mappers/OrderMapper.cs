using static dotnet_api_erp.src.Application.DTOs.OrderDto;
using static dotnet_api_erp.src.Domain.DTOs.SalesContext.OrderDTO;

namespace dotnet_api_erp.src.Application.Mappers
{
    public class OrderMapper
    {
        public static CreateOrderDTO ToCreateOrderDTO(CreateOrderDto dto)
        {
            return new CreateOrderDTO(dto.ClientId, dto.TotalAmount);
        }
        public static List<CreateOrderItemDTO> ToCreateOrderItemDTOs(Guid orderId, List<CreateOrderItemDto> items)
        {
            return [.. items.Select(item => new CreateOrderItemDTO(orderId, item.ProductId, item.Quantity, item.Price))];
        }
    }
}