using dotnet_api_erp.src.Application.DTOs;
using dotnet_api_erp.src.Application.Exceptions;
using dotnet_api_erp.src.Application.Services.SalesContext;
using dotnet_api_erp.src.Domain.Entities.SalesContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static dotnet_api_erp.src.Application.DTOs.SharedDto;
using static dotnet_api_erp.src.Domain.DTOs.Base.PersonDTO;

namespace dotnet_api_erp.src.API.Controllers.SalesContext
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientController(ClientService service) : ControllerBase
    {
        private readonly ClientService _service = service;

        [HttpPost]
        public async Task<IActionResult> PostClient(CreateClientDTO Client, CancellationToken ct)
        {
            var response = await _service.AddAsync(Client, ct);
            if (response != null)
            {
                return Ok(new ApiResponse<Client>
                {
                    Status = 201,
                    Message = "Sucesso ao criar cliente",
                    Data = response
                });
            }

            throw new BadRequestException("Erro ao criar cliente");
        }
        [HttpGet]
        public async Task<IActionResult> GetClientes(CancellationToken ct, [FromQuery] int offset = 0, [FromQuery] int pageSize = 10)
        {
            var response = await _service.GetAllAsync(x => true, offset, pageSize, ct);
            if (response != null)
                return Ok(response);
            
            throw new BadRequestException("Erro ao pegar dados do cliente");
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetClientById(Guid Id, CancellationToken ct)
        {
            var response = await _service.GetByIdAsync(Id, ct);
            if (response != null)
            {
                return Ok(new ApiResponse<Client>
                {
                    Status = 201,
                    Message = "Sucesso ao pegar dados do cliente",
                    Data = response
                });
            }

            throw new BadRequestException("Erro ao pegar dados do cliente");
        }
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateClient(Guid Id, UpdateClientDTO Client, CancellationToken ct)
        {
            var response = await _service.UpdateAsync(Id, Client, ct);
            if (response != null)
            {
                return Ok(new ApiResponse<Client>
                {
                    Status = 201,
                    Message = "Sucesso ao atualizar dados do cliente",
                    Data = response
                });
            }

            throw new BadRequestException("Erro ao atualizar dados do cliente");
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteClient(ListIdsGuidDto dto, CancellationToken ct)
        {
            if (dto == null || dto.Ids == null || dto.Ids.Count == 0)
                throw new NotFoundException("No IDs provided.");

            await _service.DeleteRangeAsync(dto.Ids, ct);
            

            return Ok(new ApiResponseMessage
            {
                Status = 201,
                Message = "Sucesso ao excluir clientes",
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