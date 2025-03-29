using dotnet_api_erp.src.Application.DTOs;
using dotnet_api_erp.src.Application.Exceptions;
using dotnet_api_erp.src.Application.Services.ProductContext;
using dotnet_api_erp.src.Application.Services.UserContext;
using dotnet_api_erp.src.Domain.Entities.ProductContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static dotnet_api_erp.src.Application.DTOs.ProductDto;
using static dotnet_api_erp.src.Application.DTOs.SharedDto;

namespace dotnet_api_erp.src.API.Controllers.ProductContext
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController(ProductService service,  RefreshTokenService token) : ControllerBase
    {
        private readonly ProductService _service = service;
        private readonly RefreshTokenService _token = token;

        [HttpPost]
        public async Task<IActionResult> PostProduct([FromForm] CreateProductDto Product, CancellationToken ct)
        {
            var token = await _token.AuthenticationToken(_token.GetTokenToString(HttpContext.Request.Headers.Authorization.ToString()), ct);
            var response = await _service.AddAsync(Product, token!, ct);
            if (response != null)
            {
                return Ok(new ApiResponse<Product>
                {
                    Status = 201,
                    Message = "Sucesso ao criar produto",
                    Data = response
                });
            }

            throw new BadRequestException("Erro ao criar produto");
        }
        [HttpGet]
        public async Task<IActionResult> GetProductes(CancellationToken ct, [FromQuery] int offset = 0, [FromQuery] int pageSize = 10)
        {
            var response = await _service.GetAllAsync(x => true, offset, pageSize, ct);
            if (response != null)
                return Ok(response);
            
            throw new BadRequestException("Erro ao pegar dados do produto");
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetProductById(Guid Id, CancellationToken ct)
        {
            var response = await _service.GetByIdAsync(Id, ct);
            if (response != null)
            {
                return Ok(new ApiResponse<Product>
                {
                    Status = 201,
                    Message = "Sucesso ao pegar dados do produto",
                    Data = response
                });
            }

            throw new BadRequestException("Erro ao pegar dados do produto");
        }
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateProduct(Guid Id, [FromForm] UpdateProductDto Product, CancellationToken ct)
        {
            var response = await _service.UpdateAsync(Id, Product, ct);
            if (response != null)
            {
                return Ok(new ApiResponse<Product>
                {
                    Status = 201,
                    Message = "Sucesso ao atualizar dados do produto",
                    Data = response
                });
            }

            throw new BadRequestException("Erro ao atualizar dados do produto");
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(ListIdsGuidDto dto, CancellationToken ct)
        {
            if (dto == null || dto.Ids == null || dto.Ids.Count == 0)
                throw new NotFoundException("No IDs provided.");

            await _service.DeleteRangeAsync(dto.Ids, ct);

            return Ok(new ApiResponseMessage
            {
                Status = 201,
                Message = "Sucesso ao excluir produtos",
            });
        }
        [HttpGet("Exportar")]
        public async Task<IActionResult> Exportar(
            [FromQuery] ListIdsGuidDto? dto,
            CancellationToken ct)
        {
            var data = await _service.Exportar(dto, ct);
            if (data != null)
            {
                return Ok(new ApiResponse<byte[]>
                {
                    Status = 201,
                    Message = "Sucesso ao exportar tabela",
                    Data = data!
                });
            }
            throw new BadRequestException("Erro ao exportar dados");
        }
        [HttpGet("ExportarBase")]
        public async Task<IActionResult> ExportarBase()
        {
           var data = await _service.ExportarBase();
           if (data != null)
           {
               return Ok(new ApiResponse<byte[]>
               {
                   Status = 201,
                   Message = "Sucesso ao exportar tabela",
                   Data = data
               });
           }
           throw new BadRequestException("Erro ao exportar dados");
        }
        [HttpPost("Importar")]
        public async Task<IActionResult> Importar([FromBody] FileDto request, CancellationToken ct)
        {
            await _service.Importar(request, ct);
            return Ok(new ApiResponseMessage
            {
                Status = 201,
                Message = "Sucesso ao importar tabela",
            });
        }
    }
}