using dotnet_api_erp.src.Domain.Entities.UserContext;
using dotnet_api_erp.src.Domain.Interfaces.UserContext;
using dotnet_api_erp.src.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace dotnet_api_erp.src.Infrastructure.Repositories.UserContext
{
    public class RefreshTokenRepository(ApplicationDbContext context) : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context = context;

        public virtual async Task Create(RefreshToken token, CancellationToken ct)
        {
            await _context.RefreshTokens.AddAsync(token, ct);
            await _context.SaveChangesAsync(ct);
        }
        public virtual async Task Delete(RefreshToken token, CancellationToken ct)
        {
            _context.RefreshTokens.Remove(token);
            await _context.SaveChangesAsync(ct);
        }
        public virtual async Task<RefreshToken?> GetToken(string value, CancellationToken ct)
        {
            return await _context.RefreshTokens.SingleOrDefaultAsync(predicate: u => u.Value == value, ct);
        }
        public virtual async Task<RefreshToken?> GetTokenByID(Guid Id, CancellationToken ct)
        {
            return await _context.RefreshTokens.SingleOrDefaultAsync(predicate: u => u.Id == Id, ct);
        }
        public virtual async Task<RefreshToken?> GetTokenByUserID(Guid Id, CancellationToken ct)
        {
            return await _context.RefreshTokens.SingleOrDefaultAsync(predicate: u => u.UserId == Id, ct);
        }
    }
}
