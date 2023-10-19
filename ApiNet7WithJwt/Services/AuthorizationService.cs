using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiNet7WithJwt.Models;
using ApiNet7WithJwt.Models.Custom;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using Microsoft.OpenApi.Writers;
using System.Security.Cryptography;

namespace ApiNet7WithJwt.Services
{
  public class AuthorizationService : IAuthorizationService
  {
    private readonly AuthJwtDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthorizationService(AuthJwtDbContext context, IConfiguration configuration)
    {
      _context = context;
      _configuration = configuration;
    }

    private string GenerateToken(string userId)
    {
      var key = _configuration.GetValue<string>("JwtSettings:Key");
      var keyBytes = Encoding.ASCII.GetBytes(key);

      var claims = new ClaimsIdentity();
      claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));

      var credentialsToken = new SigningCredentials(
        new SymmetricSecurityKey(keyBytes),
        SecurityAlgorithms.HmacSha256Signature
        );

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = claims,
        Expires = DateTime.UtcNow.AddMinutes(1),
        SigningCredentials = credentialsToken
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

      var createdToken = tokenHandler.WriteToken(tokenConfig);
      return createdToken;
    }

    public async Task<AuthorizationResponse> ReturnToken(AuthorizationRequest authorization)
    {
      var userFinded = _context.Usuarios.FirstOrDefault(u =>
        u.Nombre == authorization.Nombre &&
        u.Contrasena == authorization.Contrasena
      );

      if (userFinded == null) return await Task.FromResult<AuthorizationResponse>(null);

      string createdToken = GenerateToken(userFinded.Id.ToString());

      string refreshTokenCreated = GenerateRefreshToken();

      //return new AuthorizationResponse()
      //{
      //  Token = createdToken,
      //  Resultado = true,
      //  Mensaje = "Ok"
      //};
      return await SaveHistorialRefreshToken(userFinded.Id, createdToken, refreshTokenCreated);
    }

    private string GenerateRefreshToken()
    {
      var byteArray = new byte[64];
      var refreshToken = string.Empty;

      using (var randomNumber = RandomNumberGenerator.Create())
      {
        randomNumber.GetBytes(byteArray);
        refreshToken = Convert.ToBase64String(byteArray);
      }

      return refreshToken;
    }

    private async Task<AuthorizationResponse> SaveHistorialRefreshToken(int userId, string token, string refreshToken)
    {
      var historialRefreshToken = new HistorialRefreshToken
      {
        IdUsuario = userId,
        Token = token,
        RefreshToken = refreshToken,
        FechaCreacion = DateTime.UtcNow,
        FechaExpiracion = DateTime.UtcNow.AddMinutes(2),
      };

      await _context.HistorialRefreshTokens.AddAsync(historialRefreshToken);
      await _context.SaveChangesAsync();

      return new AuthorizationResponse { Token = token, RefreshToken = refreshToken, Resultado = true, Mensaje = "Ok" };
    }

    public async Task<AuthorizationResponse> ReturnRefreshToken(RefreshTokenRequest refreshTokenRequest, int userId)
    {
      var refreshTokenFinded = _context.HistorialRefreshTokens.FirstOrDefault(h =>
        h.Token == refreshTokenRequest.TokenExpirado &&
        h.RefreshToken == refreshTokenRequest.RefreshToken &&
        h.IdUsuario == userId
      );

      if (refreshTokenFinded == null)
        return new AuthorizationResponse { Resultado = false, Mensaje = "No existe refresh token" };

      var refreshTokenCreated = GenerateRefreshToken();
      var tokenCreated = GenerateToken(userId.ToString());

      return await SaveHistorialRefreshToken(userId, tokenCreated, refreshTokenCreated);
    }
  }
}
