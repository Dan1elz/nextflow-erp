using dotnet_api_erp.src.Application.DTOs;
using dotnet_api_erp.src.Application.Exceptions;
using dotnet_api_erp.src.Application.Services.ProductContext;
using dotnet_api_erp.src.Application.Services.UserContext;
using dotnet_api_erp.src.Domain.Entities.ProductContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static dotnet_api_erp.src.Application.DTOs.SharedDto;

namespace dotnet_api_erp.src.API.Controllers.ProductContext
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StockMovementController(StockMovementService service,  RefreshTokenService token) : ControllerBase
    {
        private readonly StockMovementService _service = service;
        private readonly RefreshTokenService _token = token;

        [HttpPost]
        public async Task<IActionResult> PostStockMovement(CreateStockMovementDto StockMovement, CancellationToken ct)
        {
            var token = await _token.AuthenticationToken(_token.GetTokenToString(HttpContext.Request.Headers.Authorization.ToString()), ct);
            var response = await _service.AddAsync(StockMovement, token!, ct);
            if (response != null)
            {
                return Ok(new ApiResponse<StockMovement>
                {
                    Status = 201,
                    Message = "Sucesso ao criar endereço",
                    Data = response
                });
            }

            throw new BadRequestException("Erro ao criar endereço");
        }
        [HttpGet]
        public async Task<IActionResult> GetStockMovementes(CancellationToken ct, [FromQuery] int offset = 0, [FromQuery] int pageSize = 10)
        {
            var response = await _service.GetAllAsync(x => true, offset, pageSize, ct);
            if (response != null)
                return Ok(response);
            
            throw new BadRequestException("Erro ao pegar dados do endereço");
        }
        [HttpGet("product/{Id}")]
        public async Task<IActionResult> GetStockMovementesByProduct(Guid Id, CancellationToken ct, [FromQuery] int offset = 0, [FromQuery] int pageSize = 10)
        {
            var response = await _service.GetAllAsync(x => x.ProductId == Id, offset, pageSize, ct);
            if (response != null)
                return Ok(response);
            
            throw new BadRequestException("Erro ao pegar dados do endereço");
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetStockMovementById(Guid Id, CancellationToken ct)
        {
            var response = await _service.GetByIdAsync(Id, ct);
            if (response != null)
            {
                return Ok(new ApiResponse<StockMovement>
                {
                    Status = 201,
                    Message = "Sucesso ao pegar dados do endereço",
                    Data = response
                });
            }

            throw new BadRequestException("Erro ao pegar dados do endereço");
        }
        
        [HttpDelete]
        public async Task<IActionResult> DeleteStockMovement(ListIdsGuidDto dto, CancellationToken ct)
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
    }
}