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
using static dotnet_api_erp.src.Domain.DTOs.Base.PersonDTO;
using static dotnet_api_erp.src.Domain.DTOs.UserContext.AddressDTO;

namespace dotnet_api_erp.src.Application.Services.UserContext
{
    public class AddressService(IAddressRepository repository, ApplicationDbContext context) : BaseService<Address, IAddressRepository>(repository, context)
    {
        public async Task<Address> UpdateAsync(Guid Id, UpdateAddressDTO address, CancellationToken ct)
        {
            var addressToUpdate = await _repository.GetByIdAsync(Id, ct) ?? throw new NotFoundException("Endereço não encontrado");

            addressToUpdate.Update(address);
            await _repository.Update(addressToUpdate, ct);
            return addressToUpdate;
        }
        public async Task<Address> AddAsync(CreateAddressDTO address, CancellationToken ct)
        {
            var addressEntity = new Address(address);
            addressEntity.Validate();
            await _repository.AddAsync(addressEntity, ct);
            return addressEntity;
        }
        public async Task<byte[]?> Exportar(ListIdsGuidDto? dto, CancellationToken ct)
        {
            Expression<Func<Address, bool>> expression = x => true;

            if (dto?.Ids?.Count > 0)
            {
                expression = x => dto.Ids.Contains(x.Id);
            }

            var data = new FileProcessorUtils<Address>(_context);
            var address = await data.ExportFileAsync(
                whereExpression: expression,
                selectExpression: x => new
                {
                    User = x.User != null ? x.User.Name : string.Empty,
                    x.Description,
                    x.Street,
                    x.Number,
                    x.District,
                    x.City,
                    x.State,
                    x.Complement,
                    x.ZipCode,
                    x.IsPrimary,
                },
                ct: ct,
                includeExpression: x => x.Include(u => u.User)
            );
            return address;
        }
        public Task<byte[]> ExportarBase()
        {
            List<Address> addresses =
            [
                new(new CreateAddressDTO(
                    UserId: Guid.NewGuid(),
                    Description: "Sample Description",
                    Street: "Sample Street",
                    Number: "123",
                    District: "Sample District",
                    City: "Sample City",
                    State: "Sample State",
                    Complement: "Sample Complement",
                    ZipCode: "12345-678",
                    IsPrimary: true
                ))
                {
                    User = new User(new CreatePersonDTO(
                    Name: "John Doe",
                    Cpf: "123.456.789-00",
                    Email: "johndoe@example.com",
                    BirthDate: new DateOnly(1990, 1, 1)
                    ))
                }
            ];

            var selectedData = addresses.Select(x => new
            {
                User = x.User!.Name,
                x.Description,
                x.Street,
                x.Number,
                x.District,
                x.City,
                x.State,
                x.Complement,
                x.ZipCode,
                x.IsPrimary,
            }).ToList();

            var data = new FileProcessorUtils<Address>(_context);
            byte[]? userBase = data.GenerateExcelFromData(selectedData);
            return Task.FromResult(userBase);
        }
        public async Task Importar(FileDto file, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(file.Base64))
                throw new  NotFoundException("Arquivo não encontrado.");
            
            var data = new FileProcessorUtils<Address>(_context);
            await data.ImportFileAsync(file, null, ct);
        }
    }
}