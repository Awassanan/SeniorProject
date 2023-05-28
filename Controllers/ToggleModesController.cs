using Microsoft.AspNetCore.Mvc;

namespace SeniorProject.Controllers;

[ApiController]
[Route("[controller]")]
public class ToggleModesController : ControllerBase
{
    private readonly ILogger<ToggleModesController> _logger;

    public ToggleModesController(ILogger<ToggleModesController> logger)
    {
        _logger = logger;
    }

    [Route("Current")]
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { mode = "project" }); // default toggle --> แก้ตามเทอม
    }
}