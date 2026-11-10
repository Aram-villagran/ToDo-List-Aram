using Microsoft.AspNetCore.Mvc;

namespace ToDoExample.Controllers.v1;

public class AccountController : ControllerBase
{
    // GET
    public IActionResult Index()
    {
        return Ok();
    }
}