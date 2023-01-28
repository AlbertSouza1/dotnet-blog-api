using Blog.Attributes;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        [HttpGet("")]
        //[ApiKey]
        public IActionResult Home() => Ok(new ResultViewModel<string>(data: "Welcome"));

        [HttpGet("health-check")]
        public IActionResult HealthCheck() => Ok(new ResultViewModel<string>(data: "Request succeeded."));
    }
}