namespace dotnet_api_erp.src.Domain.DTOs.ProducContext
{
    public class ProductDTO
    {
        public record CreateProductDTO(Guid SupplierId,string ProductCode, string Name, string Description, string Image, int Quantity, double Price, DateOnly Validity);
        public record UpdateProductDTO(string ProductCode,string Name, string Description, string Image, double Price, DateOnly Validity);
    }
}
