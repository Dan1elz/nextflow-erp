using dotnet_api_erp.src.Domain.Entities.SalesContext;
using dotnet_api_erp.src.Domain.Interfaces.Base;

namespace dotnet_api_erp.src.Domain.Interfaces.SalesContext
{
    public interface IPaymentRepository : IBaseRepository<Payment>
    {
        
    }
}