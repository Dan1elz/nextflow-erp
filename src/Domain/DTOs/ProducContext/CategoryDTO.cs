namespace dotnet_api_erp.src.Domain.DTOs.ProducContext
{
    public class CategoryDTO
    {
        public record CreateCategoryDTO(string Description);
        public record UpdateCategoryDTO(string Description);

        public record CreateCategoryProductDTO(Guid CategoryId, Guid ProductId);
    }
}