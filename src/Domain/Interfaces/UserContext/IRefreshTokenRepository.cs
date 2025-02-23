using dotnet_api_erp.src.Domain.Entities.UserContext;

namespace dotnet_api_erp.src.Domain.Interfaces.UserContext
{
    public interface IRefreshTokenRepository
    {
        Task Create(RefreshToken token, CancellationToken ct);
        Task Delete(RefreshToken token, CancellationToken ct);
        Task<RefreshToken?> GetToken(string value, CancellationToken ct);
        Task<RefreshToken?> GetTokenByID(Guid Id, CancellationToken ct);
    }
}