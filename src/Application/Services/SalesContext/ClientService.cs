using System.Linq.Expressions;
using dotnet_api_erp.src.Application.DTOs;
using dotnet_api_erp.src.Application.Exceptions;
using dotnet_api_erp.src.Application.Services.Base;
using dotnet_api_erp.src.Application.Utils;
using dotnet_api_erp.src.Domain.Entities.SalesContext;
using dotnet_api_erp.src.Domain.Interfaces.SalesContext;
using dotnet_api_erp.src.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using static dotnet_api_erp.src.Application.DTOs.SharedDto;
using static dotnet_api_erp.src.Domain.DTOs.Base.PersonDTO;

#pragma warning disable CS9107

namespace dotnet_api_erp.src.Application.Services.SalesContext
{
    public class ClientService(IClientRepository repository, ApplicationDbContext context) : BaseService<Client, IClientRepository>(repository, context)
    {
        public async Task<Client> UpdateAsync(Guid Id, UpdateClientDTO client, CancellationToken ct)
        {
            var clientToUpdate = await _repository.GetByIdAsync(Id, ct) ?? throw new NotFoundException("Cliente não encontrado");

            clientToUpdate.Update(client);
            await _repository.Update(clientToUpdate, ct);
            return clientToUpdate;
        }
        public async Task<Client> AddAsync(CreateClientDTO client, CancellationToken ct)
        {
            var clientEntity = new Client(client);
            clientEntity.Validate();
            await _repository.AddAsync(clientEntity, ct);
            return clientEntity;
        }
        public async override Task DeleteAsync(Guid Id, CancellationToken ct)
        {
            Client Client = await repository.GetByIdAsync(Id, ct, includeExpression: x => x.Include(u => u.Orders)) ?? throw new NotFoundException("Cliente não encontrado");
            if (Client.Orders.Any(p => p.Active))
            {
                throw new BadRequestException("Não é possível excluir o cliente, pois existem ordens ativas associadas a ele.");
            }
            Client.Delete();
            await repository.Update(Client, ct);
        }
        public async override Task DeleteRangeAsync(List<Guid> Ids, CancellationToken ct)
        {
            var tasks = Ids.Select(Id => DeleteAsync(Id, ct));
            await Task.WhenAll(tasks);
        }
        public async Task<byte[]?> Exportar(ListIdsGuidDto? dto, CancellationToken ct)
        {
            Expression<Func<Client, bool>> expression = x => true;

            if (dto?.Ids?.Count > 0)
            {
                expression = x => dto.Ids.Contains(x.Id);
            }

            var data = new FileProcessorUtils<Client>(_context);
            var address = await data.ExportFileAsync(
                whereExpression: expression,
                selectExpression: x => new
                {
                    x.Name,
                    x.Cpf,
                    x.Email,
                    x.BirthDate,
                    x.Phone,
                    x.ZipCode,
                    x.Address,
                },
                ct: ct
            );
            return address;
        }
        public Task<byte[]> ExportarBase()
        {
            List<Client> addresses =
            [
                new(new CreateClientDTO(
                    Name: "John Doe",
                    Cpf: "123.456.789-00",
                    Email: "johndoe@example.com",
                    BirthDate: new DateOnly(1990, 1, 1),
                    Phone: "(11) 98765-4321",
                    ZipCode: "01010-010",
                    Address: "Rua das Indústrias, 123 - Centro, São Paulo - SP"
                ))
            ];

            var selectedData = addresses.Select(x => new
            {
                x.Name,
                x.Cpf,
                x.Email,
                x.BirthDate,
                x.Phone,
                x.ZipCode,
                x.Address,
            }).ToList();

            var data = new FileProcessorUtils<Client>(_context);
            byte[]? userBase = data.GenerateExcelFromData(selectedData);
            return Task.FromResult(userBase);
        }
        public async Task Importar(FileDto file, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(file.Base64))
                throw new  NotFoundException("Arquivo não encontrado.");
            
            var data = new FileProcessorUtils<Client>(_context);
            await data.ImportFileAsync(file, null, ct);
        }
    }
}