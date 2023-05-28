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
public class FAQsController : ControllerBase
{
    private readonly ILogger<FAQsController> _logger;

    public FAQsController(ILogger<FAQsController> logger)
    {
        _logger = logger;
    }

    [Route("")] // https://cache111.com/seniorprojectapi/faqs
    [HttpGet]
    public IActionResult Get()
    {
        var db = new SeniorProjectDbContext();
        var faq = from f in db.Faq select f;
        if(!faq.Any()) return NoContent();
        return Ok(faq);
    }
}