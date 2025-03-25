using static dotnet_api_erp.src.Application.DTOs.ProductDto;
using static dotnet_api_erp.src.Domain.DTOs.ProducContext.ProductDTO;

namespace dotnet_api_erp.src.Application.Mappers
{
    public class ProductMapper
    {
        public static CreateProductDTO ToCreateProductDTO(CreateProductDto dto, string fileName)
        {
            return new CreateProductDTO(dto.SupplierId, dto.ProductCode, dto.Name, dto.Description, fileName, dto.Quantity, dto.Price, dto.Validity);
        }
        public static UpdateProductDTO ToUpdateProductDTO(UpdateProductDto dto, string fileName)
        {
            return new UpdateProductDTO(dto.ProductCode, dto.Name, dto.Description, fileName, dto.Price, dto.Validity);
        }
    }
}