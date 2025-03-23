using dotnet_api_erp.src.Domain.Entities.ProductContext;
using dotnet_api_erp.src.Domain.Interfaces.ProductContext;
using dotnet_api_erp.src.Infrastructure.Data;
using dotnet_api_erp.src.Infrastructure.Repositories.Base;

namespace dotnet_api_erp.src.Infrastructure.Repositories.ProductContext
{
    public class CategoryProductRepository(ApplicationDbContext context) : BaseRepository<CategoryProduct>(context), ICategoryProductRepository
    {
        
    }
}