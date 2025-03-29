using System.Linq.Expressions;
using dotnet_api_erp.src.Application.DTOs;
using dotnet_api_erp.src.Application.Exceptions;
using dotnet_api_erp.src.Application.Services.Base;
using dotnet_api_erp.src.Application.Utils;
using dotnet_api_erp.src.Domain.Entities.UserContext;
using dotnet_api_erp.src.Domain.Interfaces.UserContext;
using dotnet_api_erp.src.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using static dotnet_api_erp.src.Application.DTOs.SharedDto;
using static dotnet_api_erp.src.Application.DTOs.UserDto;
using static dotnet_api_erp.src.Domain.DTOs.Base.PersonDTO;

namespace dotnet_api_erp.src.Application.Services.UserContext
{
    public class UserService(IUserRepository repository, RefreshTokenService service, ApplicationDbContext context) : BaseService<User, IUserRepository>(repository, context)
    {
        private readonly RefreshTokenService _service = service;

        public async Task<LoginResponseDto> Login(LoginUserDto login, CancellationToken ct)
        {
            var user = await _repository.Login(login.Email, ct) ?? throw new BadRequestException("Email inválido.");

            if (!BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
                throw new BadRequestException("Senha inválida.");

            var token = await _service.CreateToken(user, ct) ?? throw new BadRequestException("Erro ao criar token. Contacte o suporte.");

            return new LoginResponseDto
            {
                UserId = user.Id,
                Token = token
            };
        }
        public async Task<User> UpdateAsync(Guid Id, UpdateUserDTO user, CancellationToken ct)
        {
            var userToUpdate = await _repository.GetByIdAsync(Id, ct) ?? throw new NotFoundException("Usuario não encontrado");

            userToUpdate.Update(user);
            await _repository.Update(userToUpdate, ct);
            return userToUpdate;
        }
        public async Task<User> UpdatePassword(Guid Id, UserPasswordDto dto, CancellationToken ct)
        {
            var userToUpdate = await _repository.GetAsync(u => u.Id == Id, ct) ?? throw new NotFoundException("Usuario não encontrado");

            userToUpdate.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            userToUpdate.Validate();
            await _repository.Update(userToUpdate, ct);
            return userToUpdate;
        }
        public async Task<User> AddAsync(CreatePersonDTO user, CancellationToken ct)
        {
            var userEntity = new User(user)
            {
                Password = BCrypt.Net.BCrypt.HashPassword("Empresa@2025"),
            };
            userEntity.Validate();
            await _repository.AddAsync(userEntity, ct);
            return userEntity;
        }
        public override async Task<User> GetByIdAsync(Guid Id, CancellationToken ct) 
        {
            return await _repository.GetByIdAsync(Id, ct,includeExpression: x => x.Include(u => u.Addresses).Include(u => u.Contacts) ) ?? throw new NotFoundException("Entity not found");
        }
        public async Task<byte[]?> Exportar(ListIdsGuidDto? dto, CancellationToken ct)
        {
            Expression<Func<User, bool>> expression = x => true;

            if (dto?.Ids?.Count > 0)
            {
                expression = x => dto.Ids.Contains(x.Id);
            }

            var data = new FileProcessorUtils<User>(_context);
            var users =
                await data.ExportFileAsync(
                    whereExpression: expression,
                    selectExpression: x => new { 
                        x.Name, 
                        x.Email,
                        x.Cpf,
                        x.BirthDate,
                    },
                    ct: ct
                );
            return users;
        }
        public Task<byte[]> ExportarBase()
        {
            List<User> value =
            [
                new User(new CreatePersonDTO(
                    "John Doe",
                    "123.456.789-00",
                    "johndoe@example.com",
                    new DateOnly(1990, 1, 1)
                )),
            ];

            var selectedData = value.Select(c => new
            {
                c.Name,
                c.Cpf,
                c.Email,
                c.BirthDate

            }).ToList();

            var data = new FileProcessorUtils<User>(_context);
            byte[]? userBase = data.GenerateExcelFromData(selectedData);
            return Task.FromResult(userBase);
        }
        public async Task Importar(FileDto file, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(file.Base64))
                throw new  NotFoundException("Arquivo não encontrado.");
            

            var data = new FileProcessorUtils<User>(_context);
            await data.ImportFileAsync(file, null, ct);
        }
    }
}