using System.Linq.Expressions;
using dotnet_api_erp.src.Application.DTOs;
using dotnet_api_erp.src.Application.Exceptions;
using dotnet_api_erp.src.Application.Mappers;
using dotnet_api_erp.src.Application.Services.Base;
using dotnet_api_erp.src.Application.Utils;
using dotnet_api_erp.src.Domain.Entities.ProductContext;
using dotnet_api_erp.src.Domain.Entities.UserContext;
using dotnet_api_erp.src.Domain.Enums;
using dotnet_api_erp.src.Domain.Interfaces.ProductContext;
using dotnet_api_erp.src.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using static dotnet_api_erp.src.Application.DTOs.ProductDto;
using static dotnet_api_erp.src.Application.DTOs.SharedDto;
using static dotnet_api_erp.src.Domain.DTOs.ProducContext.ProductDTO;
using static dotnet_api_erp.src.Domain.DTOs.ProducContext.SuppliersDTO;

#pragma warning disable CS9107

namespace dotnet_api_erp.src.Application.Services.ProductContext
{
    public class ProductService(IProductRepository repository,IStockMovementRepository stockMovementRepository, ImageUtils imageUtils, CategoryService categoryService, ApplicationDbContext context) : BaseService<Product, IProductRepository>(repository, context)
    {
        private readonly ImageUtils _imageUtils = imageUtils;
        private readonly CategoryService _categoryService = categoryService;
        private readonly string filePath = "assets/images/products";
        
        public async Task<Product> UpdateAsync(Guid Id, UpdateProductDto product, CancellationToken ct)
        {
            var productToUpdate = await _repository.GetByIdAsync(Id, ct) ?? throw new NotFoundException("Produto não encontrado");
            var fileName = "";
            if (product.Image != null)
            {
                _imageUtils.RemoveImg(filePath, productToUpdate.Image);
                fileName = await _imageUtils.SaveImg(product.Image, filePath, ct);
            }

            productToUpdate.Update(ProductMapper.ToUpdateProductDTO(product, fileName ?? productToUpdate.Image));
            await _repository.Update(productToUpdate, ct);
            if (product.CategoryIds != null)
                await _categoryService.AddRangeAsync(product.CategoryIds, productToUpdate.Id, ct);

            return productToUpdate;
        }
        public async Task<Product> AddAsync(CreateProductDto product, RefreshToken token, CancellationToken ct)
        {
            var fileName = await _imageUtils.SaveImg(product.Image, filePath, ct);

            var productEntity = new Product(ProductMapper.ToCreateProductDTO(product, fileName));
            productEntity.Validate();
            await _repository.AddAsync(productEntity, ct);

            if (product.CategoryIds != null)
            await _categoryService.AddRangeAsync(product.CategoryIds, productEntity.Id, ct);
            
            CreateStockMovementDto createStockMovement = new()
            {
                ProductId = productEntity.Id,
                Description = "Entrada de produto",
                MovementType = MovementType.Entry,
                Quantity = product.Quantity,
            };

            var StockMovement = new StockMovement(StockMovementMapper.ToCreateStockMovementDTO(createStockMovement, productEntity, token.UserId));
            StockMovement.Validate();
            await stockMovementRepository.AddAsync(StockMovement, ct);

            return productEntity;
        }
        public async override Task DeleteAsync(Guid Id, CancellationToken ct)
        {
            Product Product = await repository.GetByIdAsync(Id, ct) ?? throw new NotFoundException("Produto não encontrado");
            if (Product.Quantity > 0)
            {
                throw new BadRequestException("Não é possível excluir o produto, pois existem produtos ativos associados a ele.");
            }
            Product.Delete();
            await repository.Update(Product, ct);
        }
        public async override Task DeleteRangeAsync(List<Guid> Ids, CancellationToken ct)
        {
            var tasks = Ids.Select(Id => DeleteAsync(Id, ct));
            await Task.WhenAll(tasks);
        }
        public async Task<byte[]?> Exportar(ListIdsGuidDto? dto, CancellationToken ct)
        {
            Expression<Func<Product, bool>> expression = x => true;

            if (dto?.Ids?.Count > 0)
            {
                expression = x => dto.Ids.Contains(x.Id);
            }

            var data = new FileProcessorUtils<Product>(_context);
            var address = await data.ExportFileAsync(
                whereExpression: expression,
                selectExpression: x => new
                {
                    Supplier = x.Supplier!.CNPJ,
                    x.ProductCode,
                    x.Name,
                    x.Description,
                    x.Quantity,
                    x.Price,
                    x.Validity,
                },
                ct: ct,
                includeExpression: x => x.Include(u => u.Supplier)
            );
            return address;
        }
        public Task<byte[]> ExportarBase()
        {
            List<Product> addresses =
            [
                new(new CreateProductDTO(
                    SupplierId: Guid.NewGuid(),
                    ProductCode: "PROD-20240324",
                    Name: "Café Gourmet Extra Forte",
                    Description: "Café torrado e moído, 100% arábica, sabor intenso e encorpado.",
                    Image: "https://example.com/images/cafe-gourmet.jpg",
                    Quantity: 150,
                    Price: 29.90,
                    Validity: new DateOnly(2025, 12, 31)
                )){
                    Supplier = new(new CreateSupplierDTO(
                        Name: "Fornecedora Brasil Ltda",
                        Address: "Rua das Indústrias, 123 - Centro, São Paulo - SP",
                        ZipCode: "01010-010",
                        CNPJ: "12.345.678/0001-90",
                        Phone: "(11) 98765-4321",
                        Email: "contato@fornecedorabrasil.com"
                    ))
                }
            ];

            var selectedData = addresses.Select(x => new
            {
                Supplier = x.Supplier!.CNPJ,
                x.ProductCode,
                x.Name,
                x.Description,
                x.Quantity,
                x.Price,
                x.Validity,
            }).ToList();

            var data = new FileProcessorUtils<Product>(_context);
            byte[]? userBase = data.GenerateExcelFromData(selectedData);
            return Task.FromResult(userBase);
        }
        public async Task Importar(FileDto file, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(file.Base64))
                throw new  NotFoundException("Arquivo não encontrado.");
            
            var data = new FileProcessorUtils<Product>(_context);
            await data.ImportFileAsync(file, null, ct);
        }
    }
}