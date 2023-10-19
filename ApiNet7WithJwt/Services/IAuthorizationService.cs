using ApiNet7WithJwt.Models.Custom;

namespace ApiNet7WithJwt.Services
{
  public interface IAuthorizationService
  {
    Task<AuthorizationResponse> ReturnToken(AuthorizationRequest authorization);
    Task<AuthorizationResponse> ReturnRefreshToken(RefreshTokenRequest refreshTokenRequest, int userId);

  }
}
