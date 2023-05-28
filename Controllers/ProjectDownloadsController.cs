using System;
using System.IO;
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
public class ProjectDownloadsController : ControllerBase
{
    private readonly ILogger<ProjectDownloadsController> _logger;

    public ProjectDownloadsController(ILogger<ProjectDownloadsController> logger)
    {
        _logger = logger;
    }

    [Route("")]
    [HttpGet]
    public IActionResult Get()
    {
        var db = new SeniorProjectDbContext();
        var download = from p in db.ProjectDownload select new {
            Id = p.Id,
            FileName = p.FileName,
            DownloadLink = Program.DomainName + p.DownloadLink,
            ModifiedDate = p.ModifiedDate,
            FileSize = p.FileSize + " kB"
        };

        if(!download.Any()) return NoContent();
        return Ok(download);
    }
}