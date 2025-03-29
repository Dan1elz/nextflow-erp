using dotnet_api_erp.src.Application.Exceptions;
using dotnet_api_erp.src.Application.Services.SalesContext;
using dotnet_api_erp.src.Application.Services.UserContext;
using dotnet_api_erp.src.Domain.Entities.SalesContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static dotnet_api_erp.src.Application.DTOs.SaleDTO;
using static dotnet_api_erp.src.Application.DTOs.SharedDto;

namespace dotnet_api_erp.src.API.Controllers.SalesContext
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaleController(SaleService service, RefreshTokenService token) : ControllerBase
    {
        private readonly SaleService _service = service;
        private readonly RefreshTokenService _token = token;

        [HttpPost]
        public async Task<IActionResult> PostSale(CreateSaleDto Sale, CancellationToken ct)
        {
            var token = await _token.AuthenticationToken(_token.GetTokenToString(HttpContext.Request.Headers.Authorization.ToString()), ct);
            var response = await _service.AddAsync(token!, Sale, ct);
            if (response != null)
            {
                return Ok(new ApiResponse<Sale>
                {
                    Status = 201,
                    Message = "Sucesso ao criar Venda",
                    Data = response
                });
            }

            throw new BadRequestException("Erro ao criar Venda");
        }
        [HttpGet]
        public async Task<IActionResult> GetSales(CancellationToken ct, [FromQuery] int offset = 0, [FromQuery] int pageSize = 10)
        {
            var response = await _service.GetAllAsync(x => true, offset, pageSize, ct);
            if (response != null)
                return Ok(response);
            
            throw new BadRequestException("Erro ao pegar dados da Venda");
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetSaleById(Guid Id, CancellationToken ct)
        {
            var response = await _service.GetByIdAsync(Id, ct);
            if (response != null)
            {
                return Ok(new ApiResponse<Sale>
                {
                    Status = 201,
                    Message = "Sucesso ao pegar dados da Venda",
                    Data = response
                });
            }

            throw new BadRequestException("Erro ao pegar dados da Venda");
        }
    }
}
