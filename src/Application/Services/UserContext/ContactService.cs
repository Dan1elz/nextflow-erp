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
using static dotnet_api_erp.src.Domain.DTOs.UserContext.ContactsDTO;

namespace dotnet_api_erp.src.Application.Services.UserContext
{
    public class ContactService(IContactRepository repository, ApplicationDbContext context) : BaseService<Contact, IContactRepository>(repository, context)
    {
        public async Task<Contact> UpdateAsync(Guid Id, UpdateContactDTO contact, CancellationToken ct)
        {
            var contactToUpdate = await _repository.GetByIdAsync(Id, ct) ?? throw new NotFoundException("Contato não encontrado");

            contactToUpdate.Update(contact);
            await _repository.Update(contactToUpdate, ct);
            return contactToUpdate;
        }
        public async Task<Contact> AddAsync(CreateContactDTO contact, CancellationToken ct)
        {
            var contactEntity = new Contact(contact);
            contactEntity.Validate();
            await _repository.AddAsync(contactEntity, ct);
            return contactEntity;
        }
        public async Task<byte[]?> Exportar(ListIdsGuidDto? dto, CancellationToken ct)
        {
            Expression<Func<Contact, bool>> expression = x => true;

            if (dto?.Ids?.Count > 0)
            {
                expression = x => dto.Ids.Contains(x.Id);
            }

            var data = new FileProcessorUtils<Contact>(_context);
            var contact = await data.ExportFileAsync(
                whereExpression: expression,
                selectExpression: x => new
                {
                    User = x.User != null ? x.User.Name : string.Empty,
                    x.Description,
                    x.Phone,
                    x.Email,
                    x.IsPrimary,
                },
                ct: ct,
                includeExpression: x => x.Include(u => u.User)
            );
            return contact;
        }
        public Task<byte[]> ExportarBase()
        {
            List<Contact> contacts =
            [
                new(new CreateContactDTO(
                    UserId: Guid.NewGuid(),
                    Description: "Sample Description",
                    Phone: "(10) 1234-5678",
                    Email: "johndoe@example.com",
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

            var selectedData = contacts.Select(x => new
            {
                User = x.User!.Name,
                x.Description,
                x.Phone,
                x.Email,
                x.IsPrimary,
            }).ToList();

            var data = new FileProcessorUtils<Contact>(_context);
            byte[]? userBase = data.GenerateExcelFromData(selectedData);
            return Task.FromResult(userBase);
        }
        public async Task Importar(FileDto file, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(file.Base64))
                throw new  NotFoundException("Arquivo não encontrado.");

            var data = new FileProcessorUtils<Contact>(_context);
            await data.ImportFileAsync(file, null, ct);
        }
    }
}