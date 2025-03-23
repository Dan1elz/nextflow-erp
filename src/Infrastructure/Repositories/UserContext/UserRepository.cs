using dotnet_api_erp.src.Domain.Entities.UserContext;
using dotnet_api_erp.src.Domain.Interfaces.UserContext;
using dotnet_api_erp.src.Infrastructure.Data;
using dotnet_api_erp.src.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace dotnet_api_erp.src.Infrastructure.Repositories.UserContext
{
    public class UserRepository(ApplicationDbContext context) : BaseRepository<User>(context), IUserRepository
    {
        public virtual async Task<User?> Login(string Email, CancellationToken ct)
        {
            return await _context.Users.SingleOrDefaultAsync(predicate: u => u.Email == Email, ct);
        }
    }
}