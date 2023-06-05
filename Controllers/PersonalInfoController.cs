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

using System.Text.RegularExpressions;

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace SeniorProject.Controllers;
[ApiController]
[Route("[controller]")]
public class PersonalInfoController : ControllerBase
{
    private readonly ILogger<PersonalInfoController> _logger;

    public PersonalInfoController(ILogger<PersonalInfoController> logger)
    {
        _logger = logger;
    }

    // เรียกดูข้อมูลส่วนบุคคลสำหรับนิสิต
    [Route("student")]
    [HttpGet]
    [Authorize(Roles = "student")]
    public IActionResult Get1()
    {
        var db = new SeniorProjectDbContext();

        // var studentId = (User.Identity.Name.Split('@'))[0];
        var studentId = (from s in db.Student where s.Email == User.Identity.Name select s.Id).FirstOrDefault();

        var info = (from i in db.Student
                    where i.Id == studentId
                    select new
                    {
                        id = i.Id,
                        major = i.Major == "COMP" ? "วิทยาการคอมพิวเตอร์" : (i.Major == "MATH" ? "คณิตศาสตร์" : null),
                        title = i.Title,
                        firstName = i.FirstName,
                        lastname = i.LastName,
                        fullName = i.Title + i.FirstName + " " + i.LastName,
                        phoneNumber = i.Phone,
                        email = i.Email,
                        address = i.Address,
                        profilePicture = Program.UploadURL + "/profilepictures/student/" + i.ProfilePicture
                    }).FirstOrDefault();

        if (info == null) return NoContent();
        return Ok(info);
    }

    // เรียกดูข้อมูลส่วนบุคคลสำหรับอาจารย์
    [Route("lecturer")]
    [HttpGet]
    [Authorize(Roles = "lecturer")]
    public IActionResult Get2()
    {
        var db = new SeniorProjectDbContext();

        var email = User.Identity.Name;

        var info = (from i in db.Lecturer
                    where i.Email == email
                    select new
                    {
                        id = i.Id,
                        title = i.Title,
                        firstName = i.FirstName,
                        lastname = i.LastName,
                        fullName = i.Title + i.FirstName + " " + i.LastName,
                        phoneNumber = i.Phone,
                        email = i.Email,
                        profilePicture = Program.UploadURL + "/profilepictures/lecturer/" + i.ProfilePicture
                    }).FirstOrDefault();

        if (info == null) return NoContent();
        return Ok(info);
    }

    [Route("student/editPhone")]
    [HttpPut]
    [Authorize(Roles = "student")]
    public IActionResult Put1([FromBody] DTOs.Student studentInfo)
    {
        var db = new SeniorProjectDbContext();

        var studentId = (from s in db.Student where s.Email == User.Identity.Name select s.Id).FirstOrDefault();

        var student = (from s in db.Student
                       where s.Id == studentId
                       select s).FirstOrDefault();

        if (student == null) return Forbid();

        if (studentInfo.Phone == null || studentInfo.Phone == "") return StatusCode(403, "Phone number can't be null or empty string!");
        else
        {
            student.Phone = studentInfo.Phone;

            db.SaveChanges();

            return Ok();
        }
    }

    [Route("student/editAddress")]
    [HttpPut]
    [Authorize(Roles = "student")]
    public IActionResult Put2([FromBody] DTOs.Student studentInfo)
    {
        var db = new SeniorProjectDbContext();

        var studentId = (from s in db.Student where s.Email == User.Identity.Name select s.Id).FirstOrDefault();

        var student = (from s in db.Student
                       where s.Id == studentId
                       select s).FirstOrDefault();

        if (student == null) return Forbid();

        if (studentInfo.Address == null || studentInfo.Address == "") return StatusCode(403, "Address can't be null or empty string!");
        else
        {
            student.Address = studentInfo.Address;

            db.SaveChanges();

            return Ok();
        }
    }

    [Route("lecturer/editPhone")]
    [HttpPut]
    [Authorize(Roles = "lecturer")]
    public IActionResult Put3([FromBody] DTOs.Student lecturerInfo)
    {
        var db = new SeniorProjectDbContext();

        var email = User.Identity.Name;

        var lecturerId = (from t in db.Lecturer where t.Email == email select t.Id).FirstOrDefault();

        // var lecturer = (from t in db.Lecturer
        //                where t.Id == lecturerId
        //                select t).FirstOrDefault();

        var lecturer = db.Lecturer.Find(lecturerId);

        if (lecturer == null) return Forbid();

        lecturer.Phone = lecturerInfo.Phone;

        db.SaveChanges();

        return Ok();
    }

    // เปลี่ยนหรือเพิ่ม hint ของนิสิต
    [Route("student/editHint")]
    [HttpPut]
    [Authorize(Roles = "student")]
    public IActionResult Put4([FromBody] DTOs.Password p)
    {
        var db = new SeniorProjectDbContext();
        var studentId = (from s in db.Student where s.Email == User.Identity.Name select s.Id).FirstOrDefault();
        var student = db.Student.Find(studentId);

        if (student == null) return Forbid();

        if (student.Hint != p.oldHint) return StatusCode(403, "Incorrect oldHint!");

        else
        {
            if (p.newHint == null || p.newHint == "") return StatusCode(403, "newHint can't be null or empty string!");

            else
            {
                student.Hint = p.newHint;

                db.SaveChanges();
            }
        }

        return Ok();

    }

    // เปลี่ยนหรือเพิ่ม hint ของอาจารย์
    [Route("lecturer/editHint")]
    [HttpPut]
    [Authorize(Roles = "lecturer")]
    public IActionResult Put5([FromBody] DTOs.Password p)
    {
        var db = new SeniorProjectDbContext();
        var lecturerId = db.Lecturer.Where(x => x.Email == User.Identity.Name).Select(x => x.Id).FirstOrDefault();
        var lecturer = db.Lecturer.Find(lecturerId);

        if (lecturer == null) return Forbid();

        if (lecturer.Hint != p.oldHint) return StatusCode(403, "Incorrect oldHint!");

        else
        {
            if (p.newHint == null || p.newHint == "") return StatusCode(403, "newHint can't be null or empty string!");

            else
            {
                lecturer.Hint = p.newHint;

                db.SaveChanges();
            }
        }

        return Ok();

    }

    // เปลี่ยนหรือเพิ่ม keyword ของนิสิต
    [Route("student/editKeyword")]
    [HttpPut]
    [Authorize(Roles = "student")]
    public IActionResult Put6([FromBody] DTOs.Password p)
    {
        var db = new SeniorProjectDbContext();
        var studentId = (from s in db.Student where s.Email == User.Identity.Name select s.Id).FirstOrDefault();
        var student = db.Student.Find(studentId);

        if (student == null) return Forbid();

        if (student.Keyword != p.oldKeyword) return StatusCode(403, "Incorrect oldKeyword!");

        else
        {
            if (p.newKeyword == null || p.newKeyword == "") return StatusCode(403, "newKeyword can't be null or empty string!");

            else
            {
                student.Keyword = p.newKeyword;

                db.SaveChanges();
            }
        }

        return Ok();

    }

    // เปลี่ยนหรือเพิ่ม keyword ของอาจารย์
    [Route("lecturer/editKeyword")]
    [HttpPut]
    [Authorize(Roles = "lecturer")]
    public IActionResult Put7([FromBody] DTOs.Password p)
    {
        var db = new SeniorProjectDbContext();
        var lecturerId = db.Lecturer.Where(x => x.Email == User.Identity.Name).Select(x => x.Id).FirstOrDefault();
        var lecturer = db.Lecturer.Find(lecturerId);

        if (lecturer == null) return Forbid();

        if (lecturer.Keyword != p.oldKeyword) return StatusCode(403, "Incorrect oldKeyword!");

        else
        {
            if (p.newKeyword == null || p.newKeyword == "") return StatusCode(403, "newKeyword can't be null or empty string!");

            else
            {
                lecturer.Keyword = p.newKeyword;

                db.SaveChanges();
            }
        }

        return Ok();

    }

    // เพิ่มหรือเปลี่ยนรูปโปรไฟล์ นิสิต
    [Route("student/editPicture")]
    [HttpPut]
    [Authorize(Roles = "student")]
    public IActionResult Put1(IFormCollection FormData)
    {
        var db = new SeniorProjectDbContext();
        var studentId = (from s in db.Student where s.Email == User.Identity.Name select s.Id).FirstOrDefault();
        var student = db.Student.Find(studentId);
        if (student == null) return Forbid();

        var file = FormData.Files.GetFile("file");
        if (file == null) return Forbid();
        if (file.Length > 10 * 1000000) return StatusCode(413); // payload too large --> รูปห้ามเกิน 10MB

        string[] Extensions = new string[] { "jpg", "png" };
        string ext = file.FileName.Split(".").Last().ToLower();
        if (!Extensions.Contains(ext)) return StatusCode(415); // Unsupported Media Type

        // string path = @"C:\Users\awass\Desktop\"; // Windows
        string path = Program.UploadPath + "/profilepictures/student/"; // Linux

        string random = RandomString.CreateRandomString(10);

        string name = studentId + "_" + "profile" + "_" + random + "." + ext;

        if (student.ProfilePicture != null)
        {
            System.IO.File.Delete(path + student.ProfilePicture);
        }
        Stream fs = new FileStream(path + name, FileMode.Create);
        file.CopyTo(fs);
        fs.Close();
        student.ProfilePicture = name;
        db.SaveChanges();

        return Ok();
    }

    // เพิ่มหรือเปลี่ยนรูปโปรไฟล์ อาจารย์
    [Route("lecturer/editPicture")]
    [HttpPut]
    [Authorize(Roles = "lecturer")]
    public IActionResult Put2(IFormCollection FormData)
    {
        var db = new SeniorProjectDbContext();
        var lecturerId = db.Lecturer.Where(x => x.Email == User.Identity.Name).Select(x => x.Id).FirstOrDefault();
        var lecturer = db.Lecturer.Find(lecturerId);
        if (lecturer == null) return Forbid();

        var file = FormData.Files.GetFile("file");
        if (file == null) return Forbid();
        if (file.Length > 10 * 1000000) return StatusCode(413); // payload too large --> รูปห้ามเกิน 10MB

        string[] Extensions = new string[] { "jpg", "png" };
        string ext = file.FileName.Split(".").Last().ToLower();
        if (!Extensions.Contains(ext)) return StatusCode(415); // Unsupported Media Type

        // string path = @"C:\Users\awass\Desktop\"; // Windows
        string path = Program.UploadPath + "/profilepictures/lecturer/"; // Linux

        string random = RandomString.CreateRandomString(10);

        string name = lecturerId + "_" + "profile" + "_" + random + "." + ext;

        if (lecturer.ProfilePicture != null)
        {
            System.IO.File.Delete(path + lecturer.ProfilePicture);
        }
        Stream fs = new FileStream(path + name, FileMode.Create);
        file.CopyTo(fs);
        fs.Close();
        lecturer.ProfilePicture = name;
        db.SaveChanges();

        return Ok();
    }

    [Route("student/editPassword")]
    [HttpPut]
    [Authorize(Roles = "student")]
    public IActionResult Put1([FromBody] DTOs.Password p)
    {
        var db = new SeniorProjectDbContext();
        var studentId = (from s in db.Student where s.Email == User.Identity.Name select s.Id).FirstOrDefault();
        var student = db.Student.Find(studentId);

        if (student == null) return Forbid();

        if (p.oldPassword == p.newPassword) return Forbid();

        string regex = "^(?=.*[a-z])(?=."
                    + "*[A-Z])(?=.*\\d)"
                    + "(?=.*[-+_!@#$%^&*., ?]).+$";

        Regex r = new Regex(regex);
        Match m = r.Match(p.newPassword);

        string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: p.oldPassword,
                    salt: Convert.FromBase64String(student.Salt), // เปลี่ยนจาก string เป็น byte[]
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));
        string hashedPassword = student.Password;
        bool confirmOldPassword = hashedPassword.Equals(hash);

        if (!confirmOldPassword) return Forbid();

        if (m.Success && p.newPassword.Length >= 8)
        {
            if (p.newPassword != p.confirmNewPassword) return Forbid();
            // string newSalt = RandomString.CreateRandomString(24);
            //generate a 128-bit salt using a secure PRNG
            byte[] newSalt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(newSalt);
            }

            string newHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: p.newPassword,
                    salt: newSalt,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

            student.Password = newHash;
            // String newSaltString = new String(newSalt);
            student.Salt = Convert.ToBase64String(newSalt); // เปลี่ยนจาก byte[] เป็น string
            db.SaveChanges();
            return Ok();
        }

        else
        {
            return Forbid();
        }
    }

    [Route("lecturer/editPassword")]
    [HttpPut]
    [Authorize(Roles = "lecturer")]
    public IActionResult Put2([FromBody] DTOs.Password p)
    {
        var db = new SeniorProjectDbContext();
        var lecturerId = db.Lecturer.Where(x => x.Email == User.Identity.Name).Select(x => x.Id).FirstOrDefault();
        var lecturer = db.Lecturer.Find(lecturerId);

        if (lecturer == null) return Forbid();

        if (p.oldPassword == p.newPassword) return Forbid();

        string regex = "^(?=.*[a-z])(?=."
                    + "*[A-Z])(?=.*\\d)"
                    + "(?=.*[-+_!@#$%^&*., ?]).+$";

        Regex r = new Regex(regex);
        Match m = r.Match(p.newPassword);

        string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: p.oldPassword,
                    salt: Convert.FromBase64String(lecturer.Salt), // เปลี่ยนจาก string เป็น byte[]
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));
        string hashedPassword = lecturer.Password;
        bool confirmOldPassword = hashedPassword.Equals(hash);

        if (!confirmOldPassword) return Forbid();

        if (m.Success && p.newPassword.Length >= 8)
        {
            if (p.newPassword != p.confirmNewPassword) return Forbid();
            // string newSalt = RandomString.CreateRandomString(24);
            //generate a 128-bit salt using a secure PRNG
            byte[] newSalt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(newSalt);
            }

            string newHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: p.newPassword,
                    salt: newSalt,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

            lecturer.Password = newHash;
            // String newSaltString = new String(newSalt);
            lecturer.Salt = Convert.ToBase64String(newSalt); // เปลี่ยนจาก byte[] เป็น string
            db.SaveChanges();
            return Ok();
        }

        else
        {
            return Forbid();
        }
    }

    // Get hint เมื่อ keyword เมื่อลืมรหัสผ่าน ผิด
    [Route("getHint")]
    [HttpPost]
    public IActionResult Post([FromBody] DTOs.ForgetPassword f)
    {
        var db = new SeniorProjectDbContext();
        var student = db.Student.Where(x => x.Email.ToLower() == f.Email.ToLower()).FirstOrDefault();
        var lecturer = db.Lecturer.Where(x => x.Email.ToLower() == f.Email.ToLower()).FirstOrDefault();

        if (student != null || lecturer != null)
        {
            if (student != null && f.Keyword != student.Keyword)
                return (Ok(new
                {
                    hint = student.Hint
                }));
            else if (lecturer != null && f.Keyword != lecturer.Keyword)
                return (Ok(new
                {
                    hint = lecturer.Hint
                }));
        }
        return (Ok());
    }

    // ส่งรหัสผ่านใหม่ทางอีเมลเมื่อลืมรหัสผ่าน
    [Route("forgetPassword")]
    [HttpPut]
    public IActionResult Put2([FromBody] DTOs.ForgetPassword f)
    {

        var db = new SeniorProjectDbContext();

        // var lecturer = db.Lecturer.Where(x => x.Email.ToLower() == f.Email.ToLower() && x.Keyword == f.Keyword).FirstOrDefault();
        // var student = db.Student.Where(x => x.Email.ToLower() == f.Email.ToLower() && x.Keyword == f.Keyword).FirstOrDefault();
        var lecturer = (from t in db.Lecturer where t.Email == f.Email.ToLower() && t.Keyword == f.Keyword select t).FirstOrDefault();
        var student = (from s in db.Student where s.Email == f.Email.ToLower() && s.Keyword == f.Keyword select s).FirstOrDefault();
        
        if (student != null || lecturer != null)
        {
            Random random = new Random();
            string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string numeric = "0123456789";
            string specialCase = "?=.*[-+_!@#$%^&*,]";

            string randLower = new String(Enumerable.Repeat(lowerCase, 2).Select(s => s[random.Next(s.Length)]).ToArray()).ToString();
            string randUpper = new String(Enumerable.Repeat(upperCase, 2).Select(s => s[random.Next(s.Length)]).ToArray()).ToString();
            string randNumeric = new String(Enumerable.Repeat(numeric, 2).Select(s => s[random.Next(s.Length)]).ToArray()).ToString();
            string randSpecial = new String(Enumerable.Repeat(specialCase, 2).Select(s => s[random.Next(s.Length)]).ToArray()).ToString();

            string concat = randLower + randUpper + randNumeric + randSpecial;

            char[] chars = concat.ToCharArray();

            string randPassword = ShuffleString.KnuthShuffle<char>(chars);

            byte[] newSalt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(newSalt);
            }

            string newHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: randPassword,
                    salt: newSalt,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

            if (lecturer != null)
            {
                lecturer.Password = newHash;
                lecturer.Salt = Convert.ToBase64String(newSalt); // เปลี่ยนจาก byte[] เป็น string
                db.SaveChanges();

                if (lecturer.Id == null) return Forbid();
                else
                {
                    var email = new MimeMessage();
                    email.From.Add(new MailboxAddress("Chatchawit Aporntewan", "chatchawit.a@chula.ac.th"));
                    email.To.Add(MailboxAddress.Parse(lecturer.Email));
                    email.Subject = "รหัสผ่านใหม่เมื่อลืมรหัสผ่าน เว็บแอปพลิเคชัน Senior Project";
                    // email.Body = new TextPart(TextFormat.Html) { Text = "<p> รหัสผ่านของคุณคือ <br/>" + password + "<br/>" + "โปรดกรอกรหัสผ่านทันทีที่ได้รับ" };
                    email.Body = new TextPart(TextFormat.Html) { Text = System.IO.File.ReadAllText(Program.ContentPath + "/proposal/templates/password1.html") + randPassword + System.IO.File.ReadAllText(Program.ContentPath + "/proposal/templates/password2.html") };

                    var smtp = new SmtpClient();

                    smtp.Connect("email-smtp.ap-southeast-1.amazonaws.com", 587, SecureSocketOptions.StartTls);
                    smtp.Authenticate("AKIA2MYSA4BNV37CMUXO", "BDYpo8WMZM411m71iOjzBTI6Utwd6Lb3AqURBu2pxlBR");

                    //smtp.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);        
                    //smtp.Authenticate("chatchawit.a@chula.ac.th", password);

                    smtp.Send(email);
                    smtp.Disconnect(true);
                }
                return (Ok(new
                {
                    lecturerId = lecturer.Id,
                    fullname = lecturer.Title + lecturer.FirstName + " " + lecturer.LastName,
                    email = lecturer.Email,
                    password = randPassword
                }));
            }

            else if (student != null)
            {
                student.Password = newHash;
                student.Salt = Convert.ToBase64String(newSalt); // เปลี่ยนจาก byte[] เป็น string
                db.SaveChanges();

                if (student.Id == null) return Forbid();
                else
                {
                    var email = new MimeMessage();
                    email.From.Add(new MailboxAddress("Chatchawit Aporntewan", "chatchawit.a@chula.ac.th"));
                    email.To.Add(MailboxAddress.Parse(student.Email));
                    email.Subject = "รหัสผ่านใหม่เมื่อลืมรหัสผ่าน เว็บแอปพลิเคชัน Senior Project";
                    // email.Body = new TextPart(TextFormat.Html) { Text = "<p> รหัสผ่านของคุณคือ <br/>" + password + "<br/>" + "โปรดกรอกรหัสผ่านทันทีที่ได้รับ" };
                    email.Body = new TextPart(TextFormat.Html) { Text = System.IO.File.ReadAllText(Program.ContentPath + "/proposal/templates/password1.html") + randPassword + System.IO.File.ReadAllText(Program.ContentPath + "/proposal/templates/password2.html") };

                    var smtp = new SmtpClient();

                    smtp.Connect("email-smtp.ap-southeast-1.amazonaws.com", 587, SecureSocketOptions.StartTls);
                    smtp.Authenticate("AKIA2MYSA4BNV37CMUXO", "BDYpo8WMZM411m71iOjzBTI6Utwd6Lb3AqURBu2pxlBR");

                    //smtp.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);        
                    //smtp.Authenticate("chatchawit.a@chula.ac.th", password);

                    smtp.Send(email);
                    smtp.Disconnect(true);
                }
                return (Ok(new
                {
                    studentId = student.Id,
                    fullname = student.Title + student.FirstName + " " + student.LastName,
                    email = student.Email,
                    password = randPassword
                }));
            }
        }
        return StatusCode(403, "Lecturer or Student not found!");
    }
}