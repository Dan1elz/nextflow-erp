using dotnet_api_erp.src.Application.Exceptions;
using dotnet_api_erp.src.Application.Services.SalesContext;
using dotnet_api_erp.src.Application.Services.UserContext;
using dotnet_api_erp.src.Domain.Entities.SalesContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static dotnet_api_erp.src.Application.DTOs.OrderDto;
using static dotnet_api_erp.src.Application.DTOs.SharedDto;

namespace dotnet_api_erp.src.API.Controllers.SalesContext
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController(OrderService service, RefreshTokenService token) : ControllerBase
    {
        private readonly OrderService _service = service;
        private readonly RefreshTokenService _token = token;

        [HttpPost]
        public async Task<IActionResult> PostOrder(CreateOrderDto order, CancellationToken ct)
        {
            var token = await _token.AuthenticationToken(_token.GetTokenToString(HttpContext.Request.Headers.Authorization.ToString()), ct);
            var response = await _service.AddAsync(order, token!, ct);
            if (response != null)
            {
                return Ok(new ApiResponse<Order>
                {
                    Status = 201,
                    Message = "Sucesso ao criar Ordem",
                    Data = response
                });
            }

            throw new BadRequestException("Erro ao criar Ordem");
        }
        [HttpGet]
        public async Task<IActionResult> GetOrdems(CancellationToken ct, [FromQuery] int offset = 0, [FromQuery] int pageSize = 10)
        {
            var response = await _service.GetAllAsync(x => true, offset, pageSize, ct);
            if (response != null)
                return Ok(response);
            
            throw new BadRequestException("Erro ao pegar dados do Ordem");
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetOrderById(Guid Id, CancellationToken ct)
        {
            var response = await _service.GetByIdAsync(Id, ct);
            if (response != null)
            {
                return Ok(new ApiResponse<Order>
                {
                    Status = 201,
                    Message = "Sucesso ao pegar dados do Ordem",
                    Data = response
                });
            }

            throw new BadRequestException("Erro ao pegar dados do Ordem");
        }
        [HttpPut("{Id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteOrder(ListIdsGuidDto dto, CancellationToken ct)
        {
            var token = await _token.AuthenticationToken(_token.GetTokenToString(HttpContext.Request.Headers.Authorization.ToString()), ct);
            
            if (dto == null || dto.Ids == null || dto.Ids.Count == 0)
                throw new NotFoundException("No IDs provided.");
            
            await _service.DeleteRangeAsync(dto.Ids, token!, ct);

            return Ok(new ApiResponseMessage
            {
                Status = 201,
                Message = "Sucesso ao excluir Ordems",
            });
        }
    }
}