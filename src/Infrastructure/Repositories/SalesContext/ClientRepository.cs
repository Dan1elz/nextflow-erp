using dotnet_api_erp.src.Domain.Entities.SalesContext;
using dotnet_api_erp.src.Domain.Interfaces.SalesContext;
using dotnet_api_erp.src.Infrastructure.Data;
using dotnet_api_erp.src.Infrastructure.Repositories.Base;

namespace dotnet_api_erp.src.Infrastructure.Repositories.SalesContext
{
    public class ClientRepository(ApplicationDbContext context) :  BaseRepository<Client>(context), IClientRepository
    {
        
    }
}