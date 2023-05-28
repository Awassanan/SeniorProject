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
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;

    public TestController(ILogger<TestController> logger)
    {
        _logger = logger;
    }

    [Route("")] // https://cache111.com/seniorprojectapi/faqs
    [HttpGet]
    public IActionResult Get(uint SemesterId)
    {
        var db = new SeniorProjectDbContext();
        var students = from s in db.Student
                       join p in db.Proposal on SemesterId equals p.SemesterId
                       where s.Id == p.StudentId1 || s.Id == p.StudentId2 || s.Id == p.StudentId3
                       orderby s.Id
                       select s;
                       
        for (int i=1; i<=5; i++)
			{
				Console.WriteLine("C# For Loop: Iteration {0}", i);
			}
        
        return Ok(students);
    }
}