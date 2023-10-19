namespace ApiNet7WithJwt.Models.Custom
{
  public class AuthorizationResponse
  {
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public bool Resultado { get; set; }
    public string Mensaje { get; set; }
  }
}
