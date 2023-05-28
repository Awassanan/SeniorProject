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
using Microsoft.AspNetCore.Authorization;

namespace SeniorProject.Controllers;
[ApiController]
[Route("[controller]")]
public class SemestersController : ControllerBase
{
    private readonly ILogger<SemestersController> _logger;

    public SemestersController(ILogger<SemestersController> logger)
    {
        _logger = logger;
    }

    [Route("")]
    [HttpGet]
    public IActionResult Get()
    {
        var db = new SeniorProjectDbContext();
        var semester = from s in db.Semester orderby s.StartDate descending select new {
            id = s.Id,
            fullSemester = s.AcademicYear + "/" + s.Term,
            academicYear = s.AcademicYear,
            term = s.Term,
            startDate = ((DateOnly)s.StartDate).ToString("yyyy-MM-dd"),
            endDate = ((DateOnly)s.EndDate).ToString("yyyy-MM-dd")
        };

        if(!semester.Any()) return NoContent();

        return Ok(semester); // default toggle --> แก้ตามเทอม
    }

    [Route("{id}")]
    [HttpGet]
    public IActionResult Get(uint id)
    {
        var db = new SeniorProjectDbContext();
        var semester = (from s in db.Semester where s.Id == id select new {
            id = s.Id,
            fullSemester = s.AcademicYear + "/" + s.Term,
            academicYear = s.AcademicYear,
            term = s.Term,
            startDate = ((DateOnly)s.StartDate).ToString("yyyy-MM-dd"),
            endDate = ((DateOnly)s.EndDate).ToString("yyyy-MM-dd")
        }).FirstOrDefault();

        if(semester == null) return NotFound();

        return Ok(semester); // default toggle --> แก้ตามเทอม
    }
}