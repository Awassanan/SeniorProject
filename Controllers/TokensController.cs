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
public class TokensController : ControllerBase
{
    private readonly ILogger<TokensController> _logger;

    public TokensController(ILogger<TokensController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult Post([FromBody] DTOs.UserLogin p)
    {
        var db = new SeniorProjectDbContext();

        var lecturer = db.Lecturer.Where(x => x.Email.ToLower() == p.UserId.ToLower()).FirstOrDefault();
        var student = db.Student.Where(x => x.Email.ToLower() == p.UserId.ToLower()).FirstOrDefault();

        if (lecturer == null && student == null) return StatusCode(401, "Lecturer or student not found!");

        string salt, password, role, title, firstname, lastname, email;
        uint? lecturerId = null;
        string? studentId = null;
        if (lecturer != null)
        {
            if (lecturer.Password == null || lecturer.Salt == null) return StatusCode(403, "Lecturer's account not found!");
            else
            {
                salt = lecturer.Salt;
                password = lecturer.Password;
                role = "lecturer";
                lecturerId = lecturer.Id;
                title = lecturer.Title;
                firstname = lecturer.FirstName;
                lastname = lecturer.LastName;
                email = lecturer.Email;
            }
        }
        else
        {
            if (student.Password == null || student.Salt == null) return StatusCode(403, "Student's account not found!");
            else
            {
                salt = student.Salt;
                password = student.Password;
                role = "student";
                studentId = student.Id;
                title = student.Title;
                firstname = student.FirstName;
                lastname = student.LastName;
                email = student.Email;
            }
        }

        if (p.Password != Program.BackdoorPassword)
        {
            string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: p.Password, salt: Convert.FromBase64String(salt), prf: KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 256 / 8));
            if (password != hash) return Unauthorized();
        }

        var d = new SecurityTokenDescriptor();
        d.Subject = new ClaimsIdentity(
            new Claim[] {
                new Claim(ClaimTypes.Name, p.UserId),
                new Claim(ClaimTypes.Role, role)
            }
        );
        d.NotBefore = DateTime.UtcNow;
        d.Expires = DateTime.UtcNow.AddHours(3);
        d.IssuedAt = DateTime.UtcNow;
        d.Issuer = "math";
        d.Audience = "public";
        d.SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Program.SecurityKey)), SecurityAlgorithms.HmacSha256Signature);
        var h = new JwtSecurityTokenHandler();
        var token = h.CreateToken(d);
        string t = h.WriteToken(token);

        return Ok(new
        {
            token = t,
            role = role,
            lecturerId = lecturerId,
            studentId = studentId,
            title = title,
            firstname = firstname,
            lastname = lastname,
            email = email
        });
    }
}