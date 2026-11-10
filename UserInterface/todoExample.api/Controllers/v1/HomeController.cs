using Microsoft.AspNetCore.Mvc;

namespace todoExample.api.Controllers.v1;

[ApiController]
[Route("/api/v1/[controller]")]
public class HomeController : ControllerBase
{
    // GET
    public IActionResult Index()
    {
        return Ok("Hello World!");
    }
}