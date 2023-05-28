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
public class LineGroupsController : ControllerBase
{
    private readonly ILogger<LineGroupsController> _logger;

    public LineGroupsController(ILogger<LineGroupsController> logger)
    {
        _logger = logger;
    }

    [Route("proposal")] // https://cache111.com/seniorprojectapi/linegroups/proposal
    [HttpGet]
    public IActionResult Get1()
    {

        return Ok(new { url = "https://line.me/R/ti/g/not2webvGL", qr = Program.DomainName + "/content/seniorproject/proposal/2301399.png" });
    }

    [Route("project")] // https://cache111.com/seniorprojectapi/linegroups/project
    [HttpGet]
    public IActionResult Get2()
    {

        return Ok(new { url = "https://line.me/R/ti/g/GrET5Qkqf3", qr = Program.DomainName + "/content/seniorproject/project/2301499.png" });
    }
}