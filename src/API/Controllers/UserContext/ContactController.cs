using dotnet_api_erp.src.Application.DTOs;
using dotnet_api_erp.src.Application.Exceptions;
using dotnet_api_erp.src.Application.Services.UserContext;
using dotnet_api_erp.src.Domain.Entities.UserContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static dotnet_api_erp.src.Application.DTOs.SharedDto;
using static dotnet_api_erp.src.Domain.DTOs.UserContext.ContactsDTO;

namespace dotnet_api_erp.src.API.Controllers.UserContext
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController(ContactService service) : ControllerBase
    {
        private readonly ContactService _service = service;

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostContact(CreateContactDTO Contact, CancellationToken ct)
        {
            var response = await _service.AddAsync(Contact, ct);
            if (response != null)
            {
                return Ok(new ApiResponse<Contact>
                {
                    Status = 201,
                    Message = "Sucesso ao criar contato",
                    Data = response
                });
            }

            throw new BadRequestException("Erro ao criar contato");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetContactes(CancellationToken ct, [FromQuery] int offset = 0, [FromQuery] int pageSize = 10)
        {
            var response = await _service.GetAllAsync(x => true, offset, pageSize, ct);
            if (response != null)
                return Ok(response);
            
            throw new BadRequestException("Erro ao pegar dados do contato");
        }

        [Authorize]
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetContactById(Guid Id, CancellationToken ct)
        {
            var response = await _service.GetByIdAsync(Id, ct);
            if (response != null)
            {
                return Ok(new ApiResponse<Contact>
                {
                    Status = 201,
                    Message = "Sucesso ao pegar dados do contato",
                    Data = response
                });
            }

            throw new BadRequestException("Erro ao pegar dados do contato");
        }
        
        [Authorize]
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateContact(Guid Id, UpdateContactDTO Contact, CancellationToken ct)
        {
            var response = await _service.UpdateAsync(Id,Contact, ct);
            if (response != null)
            {
                return Ok(new ApiResponse<Contact>
                {
                    Status = 201,
                    Message = "Sucesso ao atualizar dados do contato",
                    Data = response
                });
            }

            throw new BadRequestException("Erro ao atualizar dados do contato");
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteContact(ListIdsGuidDto dto, CancellationToken ct)
        {
            if (dto == null || dto.Ids == null || dto.Ids.Count == 0)
                throw new NotFoundException("No IDs provided.");

            await _service.DeleteRangeAsync(dto.Ids, ct);

            return Ok(new ApiResponseMessage
            {
                Status = 201,
                Message = "Sucesso ao excluir contatos",
            });
        }

        [Authorize]
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
        [Authorize]
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