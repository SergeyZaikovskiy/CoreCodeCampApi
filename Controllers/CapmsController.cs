using Microsoft.AspNetCore.Mvc;

namespace CoreCodeCampApi.Controllers
{
    [Route("api/[controller]")]
    public class CapmsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCamps()
        {
            return Ok(new { Moniker = "ATL305", Name = "MegaCity Code Camp" });
        }
    }
}
