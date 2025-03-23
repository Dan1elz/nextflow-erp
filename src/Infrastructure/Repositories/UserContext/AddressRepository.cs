using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using dotnet_api_erp.src.Domain.Interfaces.UserContext;
using dotnet_api_erp.src.Infrastructure.Data;
using dotnet_api_erp.src.Infrastructure.Repositories.Base;

namespace dotnet_api_erp.src.Infrastructure.Repositories.UserContext
{
    public class AddressRepository(ApplicationDbContext context) :  BaseRepository<Address>(context), IAddressRepository
    {
        
    }
}