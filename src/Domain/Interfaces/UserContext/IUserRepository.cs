using dotnet_api_erp.src.Domain.Entities.UserContext;
using dotnet_api_erp.src.Domain.Interfaces.Base;

namespace dotnet_api_erp.src.Domain.Interfaces.UserContext
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> Login(string Email, CancellationToken ct);
    }
}