using Microsoft.AspNetCore.Mvc;

namespace ToDoExample.Controllers.v1;

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