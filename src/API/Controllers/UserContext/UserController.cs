using dotnet_api_erp.src.Application.DTOs;
using dotnet_api_erp.src.Application.Exceptions;
using dotnet_api_erp.src.Application.Services.UserContext;
using dotnet_api_erp.src.Domain.Entities.UserContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static dotnet_api_erp.src.Application.DTOs.SharedDto;
using static dotnet_api_erp.src.Application.DTOs.UserDto;
using static dotnet_api_erp.src.Domain.DTOs.Base.PersonDTO;

namespace dotnet_api_erp.src.API.Controllers.UserContext
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(UserService service,  RefreshTokenService token) : ControllerBase
    {
        private readonly UserService _service = service;
        private readonly RefreshTokenService _token = token;

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostUser(CreatePersonDTO user, CancellationToken ct)
        {
            var response = await _service.AddAsync(user, ct);
            if (response != null)
            {
                return Ok(new ApiResponse<User>
                {
                    Status = 201,
                    Message = "Sucesso ao criar usuário",
                    Data = response
                });
            }

            throw new BadRequestException("Erro ao criar usuario");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser(LoginUserDto user, CancellationToken ct)
        {
            var response = await _service.Login(user, ct);
            if (response != null)
            {
                return Ok(new ApiResponse<LoginResponseDto>
                {
                    Status = 201,
                    Message = "Sucesso ao logar usuário",
                    Data = response
                });
            }

            throw new BadRequestException("Erro ao logar usuario");
        }

        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> LogoutUser(CancellationToken ct)
        {
            var token = await _token.AuthenticationToken(_token.GetTokenToString(HttpContext.Request.Headers.Authorization.ToString()), ct);
            await _token.DeleteToken(token!.Id, ct);
           
            return Ok(new ApiResponseMessage
            {
                Status = 201,
                Message = "Sucesso ao deslogar usuário",
            });
        }
        
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUsers(CancellationToken ct, [FromQuery] int offset = 0, [FromQuery] int pageSize = 10)
        {
            var response = await _service.GetAllAsync(x => true, offset, pageSize, ct);
            if (response != null)
                return Ok(response);
            
            throw new BadRequestException("Erro ao pegar dados do usuário");
        }

        [Authorize]
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetUserById(Guid Id, CancellationToken ct)
        {
            var response = await _service.GetByIdAsync(Id, ct);
            if (response != null)
            {
                return Ok(new ApiResponse<User>
                {
                    Status = 201,
                    Message = "Sucesso ao pegar dados do usuário",
                    Data = response
                });
            }

            throw new BadRequestException("Erro ao pegar dados do usuário");
        }
        
        [Authorize]
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateUser(Guid Id, UpdateUserDTO user, CancellationToken ct)
        {
            var response = await _service.UpdateAsync(Id, user, ct);
            if (response != null)
            {
                return Ok(new ApiResponse<User>
                {
                    Status = 201,
                    Message = "Sucesso ao atualizar dados do usuário",
                    Data = response
                });
            }

            throw new BadRequestException("Erro ao atualizar dados do usuário");
        }
        [Authorize]
        [HttpPatch("NewPassword")]
        public async Task<IActionResult> UpdatePassword(UserPasswordDto user, CancellationToken ct)
        {
            var token = await _token.AuthenticationToken(_token.GetTokenToString(HttpContext.Request.Headers.Authorization.ToString()), ct);

            var userUpdate = await _service.UpdatePassword(token!.UserId, user, ct);
            await _token.DeleteToken(token.Id, ct);
            var newToken = await _token.CreateToken(userUpdate, ct);

            return Ok(new ApiResponse<string>
            {
                Status = 201,
                Message = "Sucesso ao atualizar a senha do usuário",
                Data = newToken!
            });
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(ListIdsGuidDto dto, CancellationToken ct)
        {
            if (dto == null || dto.Ids == null || dto.Ids.Count == 0)
                throw new NotFoundException("No IDs provided.");

            await _service.DeleteRangeAsync(dto.Ids, ct);

            return Ok(new ApiResponseMessage
            {
                Status = 201,
                Message = "Sucesso ao excluir usuarios",
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
        public async Task<IActionResult> ImportarUsers([FromBody] FileDto request, CancellationToken ct)
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