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
public class ProposalAssignmentsController : ControllerBase
{
    private readonly ILogger<ProposalAssignmentsController> _logger;

    public ProposalAssignmentsController(ILogger<ProposalAssignmentsController> logger)
    {
        _logger = logger;
    }

    [Route("")] // http://localhost:7001/proposalassignments?semesterid=1
    [HttpGet]
    public IActionResult Get(uint SemesterId)
    {
        var db = new SeniorProjectDbContext();
        var assignment = from a in db.ProposalAssignment where a.SemesterId == SemesterId select new {
            id = a.Id,
            semesterId = a.SemesterId,
            assignmentName = a.AssignmentName,
            fileType = a.FileType,
            deadline = a.Deadline,
            maxSize = a.MaxSize + " MB"
        };

        if(!assignment.Any()) return NoContent();

        return Ok(assignment);
    }

    [Route("{AssignmentId}")] // http://localhost:7001/proposalassignments/1?semesterid=1
    [HttpGet]
    public IActionResult Get2(uint SemesterId, uint AssignmentId)
    {
        var db = new SeniorProjectDbContext();
        var assignment = (from a in db.ProposalAssignment where a.SemesterId == SemesterId && a.Id == AssignmentId select new {
            assignmentId = a.Id,
            semesterId = a.SemesterId,
            assignmentName = a.AssignmentName,
            fileType = a.FileType,
            deadline = a.Deadline,
            maxSize = a.MaxSize + " MB"
        }).FirstOrDefault();

        if(assignment == null) return NotFound();

        return Ok(assignment);
    }
}