using System.Linq.Expressions;
using dotnet_api_erp.src.Application.DTOs;
using dotnet_api_erp.src.Application.Exceptions;
using dotnet_api_erp.src.Application.Services.Base;
using dotnet_api_erp.src.Application.Utils;
using dotnet_api_erp.src.Domain.Entities.ProductContext;
using dotnet_api_erp.src.Domain.Interfaces.ProductContext;
using dotnet_api_erp.src.Infrastructure.Data;
using static dotnet_api_erp.src.Application.DTOs.SharedDto;
using static dotnet_api_erp.src.Domain.DTOs.ProducContext.CategoryDTO;

#pragma warning disable CS9107

namespace dotnet_api_erp.src.Application.Services.ProductContext
{
    public class CategoryService(ICategoryRepository repository,ICategoryProductRepository categoryProductRepository, ApplicationDbContext context) : BaseService<Category, ICategoryRepository>(repository, context)
    {
        protected readonly ICategoryProductRepository _categoryProductRepository = categoryProductRepository;

        public async Task<Category> UpdateAsync(Guid Id, UpdateCategoryDTO category, CancellationToken ct)
        {
            var categoryToUpdate = await _repository.GetByIdAsync(Id, ct) ?? throw new NotFoundException("Categoria não encontrada");

            categoryToUpdate.Update(category);
            await _repository.Update(categoryToUpdate, ct);
            return categoryToUpdate;
        }
        public async Task<Category> AddAsync(CreateCategoryDTO category, CancellationToken ct)
        {
            var categoryEntity = new Category(category);
            categoryEntity.Validate();
            await _repository.AddAsync(categoryEntity, ct);
            return categoryEntity;
        }
        public async Task<IEnumerable<Category>> AddRangeAsync(List<Guid> CategoryIds, Guid ProductId, CancellationToken ct)
        {
            var tasks = new List<Task>();
            var productCategorys = await _categoryProductRepository.GetAllAsync(x => x.ProductId == ProductId, 0, int.MaxValue, ct);
            foreach (var productCategory in productCategorys)
            {
                tasks.Add(_categoryProductRepository.Remove(productCategory, ct));
            }
            await Task.WhenAll(tasks);

            var categories = await _repository.GetAllAsync(x => CategoryIds.Contains(x.Id), 0, int.MaxValue, ct) ?? throw new NotFoundException("Ids da Categoria não são validos");

            var categoryProducts = (IEnumerable<CategoryProduct>)categories.Select(category => 
                new CreateCategoryProductDTO(ProductId, category.Id)
            );

            foreach (var categoryProduct in categoryProducts)
            {
                categoryProduct.Validate();
            }
            await _categoryProductRepository.AddRangeAsync(categoryProducts, ct);

            return categories;
        }
        public async Task<List<Category>> GetCategoriesAsync(Guid productId, CancellationToken ct)
        {
            var productCategories = 
                await _categoryProductRepository.GetAllAsync(x => x.ProductId == productId, 0, int.MaxValue, ct)  ?? throw new NotFoundException("Categoria não encontrada");

            var categoryIds = productCategories.Select(pc => pc.CategoryId).ToList();

            var categories = 
                await _repository.GetAllAsync(x => categoryIds.Contains(x.Id), 0, int.MaxValue, ct);

            return [.. categories];
        }
        public async Task<byte[]?> Exportar(ListIdsGuidDto? dto, CancellationToken ct)
        {
            Expression<Func<Category, bool>> expression = x => true;

            if (dto?.Ids?.Count > 0)
            {
                expression = x => dto.Ids.Contains(x.Id);
            }

            var data = new FileProcessorUtils<Category>(_context);
            var address = await data.ExportFileAsync(
                whereExpression: expression,
                selectExpression: x => new
                {
                    
                    x.Description,
                },
                ct: ct
            );
            return address;
        }
        public Task<byte[]> ExportarBase()
        {
            List<Category> addresses =
            [
                new(new CreateCategoryDTO(
                    Description: "Sample Description"
                ))
            ];

            var selectedData = addresses.Select(x => new
            {
                x.Description,
            }).ToList();

            var data = new FileProcessorUtils<Category>(_context);
            byte[]? userBase = data.GenerateExcelFromData(selectedData);
            return Task.FromResult(userBase);
        }
        public async Task Importar(FileDto file, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(file.Base64))
                throw new  NotFoundException("Arquivo não encontrado.");
            
            var data = new FileProcessorUtils<Category>(_context);
            await data.ImportFileAsync(file, null, ct);
        }
    }
}