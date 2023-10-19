using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiNet7WithJwt.Models.Custom;
using ApiNet7WithJwt.Services;
using System.IdentityModel.Tokens.Jwt;
using Azure.Core;
using Microsoft.AspNetCore.Cors;

namespace ApiNet7WithJwt.Controllers
{
  [EnableCors()]
  [Route("api/[controller]")]
  [ApiController]
  public class UserController : ControllerBase
  {
    private readonly IAuthorizationService _authorizationService;

    public UserController(IAuthorizationService authorizationService)
    {
      _authorizationService = authorizationService;
    }

    [HttpPost]
    [Route("Authentication")]
    public async Task<IActionResult> Authentication([FromBody] AuthorizationRequest authorizationRequest)
    {
      var resultAuth = await _authorizationService.ReturnToken(authorizationRequest);

      if (resultAuth == null) return Unauthorized();

      return Ok(resultAuth);
    }

    [HttpPost]
    [Route("GetRefreshToken")]
    public async Task<IActionResult> GetRefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var supposedExpiredToken = tokenHandler.ReadJwtToken(refreshTokenRequest.TokenExpirado);

      if (supposedExpiredToken.ValidTo > DateTime.UtcNow) 
        return BadRequest(new AuthorizationResponse { Resultado = false, Mensaje = "Token no ha expirado" });

      string userId = supposedExpiredToken.Claims.First(t => t.Type == JwtRegisteredClaimNames.NameId).Value.ToString();

      var authorizationReponse = await _authorizationService.ReturnRefreshToken(refreshTokenRequest, int.Parse(userId));

      return (authorizationReponse.Resultado)
        ? Ok(authorizationReponse)
        : BadRequest(authorizationReponse);
    }
  }
}
