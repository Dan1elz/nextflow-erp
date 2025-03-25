namespace dotnet_api_erp.src.Application.DTOs
{
    public class ProductDto
    {
        public class CreateProductDto
        {
            public Guid SupplierId { get; set;}
            public required string ProductCode { get; set;} 
            public required string Name { get; set;}
            public required string Description { get; set;}
            public required IFormFile Image { get; set;}
            public int Quantity { get; set;}
            public double Price { get; set;}
            public DateOnly Validity { get; set;}
            public List<Guid>? CategoryIds { get; set;} 
        }
        public class UpdateProductDto
        {
            public required string ProductCode { get; set;} 
            public required string Name { get; set;}
            public required string Description { get; set;}
            public required IFormFile Image { get; set;}
            public double Price { get; set;}
            public DateOnly Validity { get; set;}
            public List<Guid>? CategoryIds { get; set;} 
        }
    }
}