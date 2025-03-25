using dotnet_api_erp.src.Application.DTOs;
using dotnet_api_erp.src.Application.Exceptions;
using dotnet_api_erp.src.Application.Services.ProductContext;
using dotnet_api_erp.src.Domain.Entities.ProductContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static dotnet_api_erp.src.Application.DTOs.SharedDto;
using static dotnet_api_erp.src.Domain.DTOs.ProducContext.CategoryDTO;

namespace dotnet_api_erp.src.API.Controllers.ProductContext
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController(CategoryService service) : ControllerBase
    {
        private readonly CategoryService _service = service;

        [HttpPost]
        public async Task<IActionResult> PostCategory(CreateCategoryDTO Category, CancellationToken ct)
        {
            var response = await _service.AddAsync(Category, ct);
            if (response != null)
            {
                return Ok(new ApiResponse<Category>
                {
                    Status = 201,
                    Message = "Sucesso ao criar endereço",
                    Data = response
                });
            }

            throw new BadRequestException("Erro ao criar endereço");
        }
        [HttpGet]
        public async Task<IActionResult> GetCategoryes(CancellationToken ct, [FromQuery] int offset = 0, [FromQuery] int pageSize = 10)
        {
            var response = await _service.GetAllAsync(x => true, offset, pageSize, ct);
            if (response != null)
                return Ok(response);
            
            throw new BadRequestException("Erro ao pegar dados do endereço");
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetCategoryById(Guid Id, CancellationToken ct)
        {
            var response = await _service.GetByIdAsync(Id, ct);
            if (response != null)
            {
                return Ok(new ApiResponse<Category>
                {
                    Status = 201,
                    Message = "Sucesso ao pegar dados do endereço",
                    Data = response
                });
            }

            throw new BadRequestException("Erro ao pegar dados do endereço");
        }
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateCategory(Guid Id, UpdateCategoryDTO Category, CancellationToken ct)
        {
            var response = await _service.UpdateAsync(Id, Category, ct);
            if (response != null)
            {
                return Ok(new ApiResponse<Category>
                {
                    Status = 201,
                    Message = "Sucesso ao atualizar dados do endereço",
                    Data = response
                });
            }

            throw new BadRequestException("Erro ao atualizar dados do endereço");
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(ListIdsGuidDto dto, CancellationToken ct)
        {
            if (dto == null || dto.Ids == null || dto.Ids.Count == 0)
                throw new NotFoundException("No IDs provided.");

            await _service.DeleteRangeAsync(dto.Ids, ct);

            return Ok(new ApiResponseMessage
            {
                Status = 201,
                Message = "Sucesso ao excluir endereços",
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