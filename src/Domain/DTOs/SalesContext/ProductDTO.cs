namespace dotnet_api_erp.src.Domain.DTOs.SalesContext
{
    public class ProductDTO
    {
        public record CreateProductDTO(string Name, string Description, int Quantity, double Price, DateTime Validity);
        public record UpdateProductDTO(string Name, string Description, int Quantity, double Price, DateTime Validity);
    }
}
