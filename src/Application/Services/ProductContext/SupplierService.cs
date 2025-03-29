using System.Linq.Expressions;
using dotnet_api_erp.src.Application.DTOs;
using dotnet_api_erp.src.Application.Exceptions;
using dotnet_api_erp.src.Application.Services.Base;
using dotnet_api_erp.src.Application.Utils;
using dotnet_api_erp.src.Domain.Entities.ProductContext;
using dotnet_api_erp.src.Domain.Interfaces.ProductContext;
using dotnet_api_erp.src.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using static dotnet_api_erp.src.Application.DTOs.SharedDto;
using static dotnet_api_erp.src.Domain.DTOs.ProducContext.SuppliersDTO;

#pragma warning disable CS9107

namespace dotnet_api_erp.src.Application.Services.ProductContext
{
    public class SupplierService(ISuppliersRepository repository, ApplicationDbContext context) : BaseService<Supplier, ISuppliersRepository>(repository, context)
    {
        public async Task<Supplier> UpdateAsync(Guid Id, UpdateSupplierDTO supplier, CancellationToken ct)
        {
            var supplierToUpdate = await _repository.GetByIdAsync(Id, ct) ?? throw new NotFoundException("Fornecedor não encontrado");

            supplierToUpdate.Update(supplier);
            await _repository.Update(supplierToUpdate, ct);
            return supplierToUpdate;
        }
        public async Task<Supplier> AddAsync(CreateSupplierDTO supplier, CancellationToken ct)
        {
            var supplierEntity = new Supplier(supplier);
            supplierEntity.Validate();
            await _repository.AddAsync(supplierEntity, ct);
            return supplierEntity;
        }
        public async override Task DeleteAsync(Guid Id, CancellationToken ct)
        {
            Supplier supplier = await repository.GetByIdAsync(Id, ct, includeExpression: x => x.Include(u => u.Products)) ?? throw new NotFoundException("Fornecedor não encontrado");
            if (supplier.Products.Any(p => p.Active))
            {
                throw new BadRequestException("Não é possível excluir o fornecedor, pois existem produtos ativos associados a ele.");
            }
            supplier.Delete();
            await repository.Update(supplier, ct);
        }
        public async override Task DeleteRangeAsync(List<Guid> Ids, CancellationToken ct)
        {
            var tasks = Ids.Select(Id => DeleteAsync(Id, ct));
            await Task.WhenAll(tasks);
        }
        public async Task<byte[]?> Exportar(ListIdsGuidDto? dto, CancellationToken ct)
        {
            Expression<Func<Supplier, bool>> expression = x => true;

            if (dto?.Ids?.Count > 0)
            {
                expression = x => dto.Ids.Contains(x.Id);
            }

            var data = new FileProcessorUtils<Supplier>(_context);
            var address = await data.ExportFileAsync(
                whereExpression: expression,
                selectExpression: x => new
                {
                    x.Name,
                    x.Address,
                    x.ZipCode,
                    x.CNPJ,
                    x.Phone,
                    x.Email,
                },
                ct: ct
            );
            return address;
        }
        public Task<byte[]> ExportarBase()
        {
            List<Supplier> addresses =
            [
                new(new CreateSupplierDTO(
                    Name: "Fornecedora Brasil Ltda",
                    Address: "Rua das Indústrias, 123 - Centro, São Paulo - SP",
                    ZipCode: "01010-010",
                    CNPJ: "12.345.678/0001-90",
                    Phone: "(11) 98765-4321",
                    Email: "contato@fornecedorabrasil.com"
                ))
            ];

            var selectedData = addresses.Select(x => new
            {
                x.Name,
                x.Address,
                x.ZipCode,
                x.CNPJ,
                x.Phone,
                x.Email,
            }).ToList();

            var data = new FileProcessorUtils<Supplier>(_context);
            byte[]? userBase = data.GenerateExcelFromData(selectedData);
            return Task.FromResult(userBase);
        }
        public async Task Importar(FileDto file, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(file.Base64))
                throw new  NotFoundException("Arquivo não encontrado.");
            
            var data = new FileProcessorUtils<Supplier>(_context);
            await data.ImportFileAsync(file, null, ct);
        }
    }
}