using Microsoft.AspNetCore.Mvc;

namespace FP.GitHubActions.DemoApp.Server.Controllers;

public class DefaultController : Controller
{
    [HttpGet("/")]
    public IActionResult GetDefault()
    {
        return NoContent();
    }
}