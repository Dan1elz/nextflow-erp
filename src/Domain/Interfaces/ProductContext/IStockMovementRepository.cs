using dotnet_api_erp.src.Domain.Entities.ProductContext;
using dotnet_api_erp.src.Domain.Interfaces.Base;

namespace dotnet_api_erp.src.Domain.Interfaces.ProductContext
{
    public interface IStockMovementRepository : IBaseRepository<StockMovement>
    {
        
    }
}