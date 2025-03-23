namespace dotnet_api_erp.src.Domain.DTOs.ProducContext
{
    public class ProductDTO
    {
        public record CreateProductDTO(Guid SupplierId, string Name, string Description, string Image, int Quantity, double Price, DateOnly Validity);
        public record UpdateProductDTO(string Name, string Description, string Image, int Quantity, double Price, DateOnly Validity);
    }
}
