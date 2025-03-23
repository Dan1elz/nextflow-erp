using dotnet_api_erp.src.Domain.Entities.UserContext;
using dotnet_api_erp.src.Domain.Interfaces.UserContext;
using dotnet_api_erp.src.Infrastructure.Data;
using dotnet_api_erp.src.Infrastructure.Repositories.Base;

namespace dotnet_api_erp.src.Infrastructure.Repositories.UserContext
{
    public class ContactRepository(ApplicationDbContext context) :  BaseRepository<Contact>(context), IContactRepository
    {
        
    }
}