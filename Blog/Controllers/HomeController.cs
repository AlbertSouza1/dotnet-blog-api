using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        [HttpGet("health-check")]
        public IActionResult Get() => Ok(new { message = "Request succeeded." });
    }
}