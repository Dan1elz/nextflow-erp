namespace dotnet_api_erp.src.Application.DTOs
{
    public class OrderDto
    {
        public class CreateOrderDto
        {
            public Guid ClientId { get; set; }
            public double TotalAmount { get; set; }

            public List<CreateOrderItemDto> Items { get; set; } = [];
        }
        public class CreateOrderItemDto
        {
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
            public int Price { get; set; }
        }
     }
}