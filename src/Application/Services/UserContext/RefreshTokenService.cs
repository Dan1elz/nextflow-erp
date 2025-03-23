using dotnet_api_erp.src.Application.Exceptions;
using dotnet_api_erp.src.Application.Utils;
using dotnet_api_erp.src.Domain.Entities.UserContext;
using dotnet_api_erp.src.Domain.Interfaces.UserContext;

namespace dotnet_api_erp.src.Application.Services.UserContext
{
    public class RefreshTokenService(IRefreshTokenRepository repository, JwtUtils jwt)
    {
        private readonly IRefreshTokenRepository _repository = repository;
        private readonly JwtUtils _service = jwt;

        public async Task<RefreshToken?> AuthenticationToken(string _token, CancellationToken ct)
        {
            var token = await _repository.GetToken(_token, ct) ?? throw new Exception("Token not found");
            var valid = _service.ValidadeToken(token.Value);
            return !valid ? throw new NotAuthorizedException("Token expired, please log in again ") : token;
        }
        public async Task<string?> CreateToken(User user, CancellationToken ct)
        {
            var token = await _repository.GetTokenByUserID(user.Id, ct);
            if (token != null)
            {
                var valid = _service.ValidadeToken(token.Value);
                if (valid)
                {
                    return token.Value;
                }
                await _repository.Delete(token, ct);
            }

            token = _service.GenerateToken(user.Id);
            await _repository.Create(token, ct);

            return token.Value;
        }
        public async Task DeleteToken(Guid Id, CancellationToken ct)
        {
            var token = await _repository.GetTokenByID(Id, ct) ?? throw new NotFoundException("Token not found");
            await _repository.Delete(token, ct);
        }
        public string GetTokenToString(string token)
        {
            if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                throw new NotFoundException("Token not found");

            return token["Bearer ".Length..].Trim(); ;
        }
    }
}
