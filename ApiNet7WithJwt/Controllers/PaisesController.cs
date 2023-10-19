using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiNet7WithJwt.Controllers
{
  [EnableCors()]
  [Route("api/[controller]")]
  [ApiController]
  public class PaisesController : ControllerBase
  {
    [Authorize]
    [HttpGet]
    [Route("GetAll")]
    public async Task<IActionResult> GetAllContries()
    {
      var countries = await Task.FromResult(new List<string>
      {
        "Francia ", "Mexico","Argentina"
      });

      return Ok(countries);
    }
  }
}
