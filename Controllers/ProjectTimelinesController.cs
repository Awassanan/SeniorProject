using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

using SeniorProject.Models;

namespace SeniorProject.Controllers;

[ApiController]
[Route("[controller]")]
public class ProjectTimelinesController : ControllerBase
{
    private readonly ILogger<ProjectTimelinesController> _logger;

    public ProjectTimelinesController(ILogger<ProjectTimelinesController> logger)
    {
        _logger = logger;
    }

    [Route("")]
    [HttpGet]
    public IActionResult Get(uint SemesterId)
    {
        var db = new SeniorProjectDbContext();
    
        var timelines = from t in db.ProjectTimeline
            join s in db.Semester on t.SemesterId equals s.Id
            where t.SemesterId == SemesterId
            select new {
                id = t.Id,
                semesterId = t.SemesterId,
                semester = s.AcademicYear.ToString() + "/" + s.Term.ToString(),
                deadline = ((DateOnly)t.Deadline).ToString("yyyy-MM-dd"),
                todo = t.ToDo
            };

        if (!timelines.Any()) return NoContent();

        return Ok(timelines);
    }
}