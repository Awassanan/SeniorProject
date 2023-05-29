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

using Microsoft.AspNetCore.Authorization;

using SeniorProject.Models;

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Net;

namespace SeniorProject.Controllers;

[ApiController]
[Route("[controller]")]

// static class PasswordDict{
//     public static IDictionary<string, string> studentPasswordDict = new Dictionary<string, string>();
//     public static IDictionary<uint, string> lecturerPasswordDict = new Dictionary<uint, string>();
// }
public class NewMemberController : ControllerBase
{
    static class PasswordDict
    {
        // public static IDictionary<string, string> studentPasswordDict = new Dictionary<string, string>();
        public static IDictionary<uint, string> lecturerPasswordDict = new Dictionary<uint, string>();
    }

    private readonly ILogger<NewMemberController> _logger;

    public NewMemberController(ILogger<NewMemberController> logger)
    {
        _logger = logger;
    }

    // 1. สุ่มรหัสผ่าน + สุ่ม Salt + hash + กรอกใส่ db ให้นิสิตและอาจารย์ (กำลังทำ) --> done
    [Route("randompassword")]
    [HttpPut]
    public IActionResult Put1(uint SemesterId, string role) // เฉพาะอาจารย์เท่านั้น
    {
        // if (!PasswordDict.studentPasswordDict.IsNullOrEmpty()) PasswordDict.studentPasswordDict.Clear();
        if (!PasswordDict.lecturerPasswordDict.IsNullOrEmpty()) PasswordDict.lecturerPasswordDict.Clear();

        var db = new SeniorProjectDbContext();
        // if (role == "student")
        // {
        //     var students = from s in db.Student
        //                    join p in db.Proposal on SemesterId equals p.SemesterId
        //                    where s.Id == p.StudentId1 || s.Id == p.StudentId2 || s.Id == p.StudentId3
        //                    orderby s.Id
        //                    select s;
        //     var studentArray = students.ToArray();
        //     int studentArrayLength = studentArray.Length;
        //     String[] passwordList = new String[studentArrayLength];

        //     for (int i = 0; i < studentArrayLength; i++)
        //     {
        //         Random random = new Random();
        //         string lowerCase = "abcdefghijklmnopqrstuvwxyz";
        //         string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        //         string numeric = "0123456789";
        //         string specialCase = "?=.*[-+_!@#$%^&*,]";

        //         string randLower = new String(Enumerable.Repeat(lowerCase, 2).Select(s => s[random.Next(s.Length)]).ToArray()).ToString();
        //         string randUpper = new String(Enumerable.Repeat(upperCase, 2).Select(s => s[random.Next(s.Length)]).ToArray()).ToString();
        //         string randNumeric = new String(Enumerable.Repeat(numeric, 2).Select(s => s[random.Next(s.Length)]).ToArray()).ToString();
        //         string randSpecial = new String(Enumerable.Repeat(specialCase, 2).Select(s => s[random.Next(s.Length)]).ToArray()).ToString();

        //         string concat = randLower + randUpper + randNumeric + randSpecial;

        //         char[] chars = concat.ToCharArray();

        //         string randPassword = ShuffleString.KnuthShuffle<char>(chars);
        //         passwordList[i] = randPassword;

        //         byte[] newSalt = new byte[128 / 8];
        //         using (var rng = RandomNumberGenerator.Create())
        //         {
        //             rng.GetBytes(newSalt);
        //         }

        //         string newHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        //                 password: passwordList[i],
        //                 salt: newSalt,
        //                 prf: KeyDerivationPrf.HMACSHA1,
        //                 iterationCount: 10000,
        //                 numBytesRequested: 256 / 8));

        //         studentArray[i].Password = newHash;
        //         studentArray[i].Salt = Convert.ToBase64String(newSalt); // เปลี่ยนจาก byte[] เป็น string
        //         db.SaveChanges();
        //     }

        //     List<DTOs.StudentRandomPassword> L = new List<DTOs.StudentRandomPassword>();
        //     for (int i = 0; i < studentArrayLength; i++)
        //     {
        //         DTOs.StudentRandomPassword x = new DTOs.StudentRandomPassword();
        //         x.studentId = studentArray[i].Id;
        //         x.fullname = studentArray[i].Title + studentArray[i].FirstName + " " + studentArray[i].LastName;
        //         x.email = studentArray[i].Email;
        //         x.password = passwordList[i];
        //         bool containKey = PasswordDict.studentPasswordDict.ContainsKey(x.studentId);
        //         if (!containKey)
        //         {
        //             PasswordDict.studentPasswordDict.Add(studentArray[i].Id, passwordList[i]);
        //         }
        //         // PasswordDict.studentPasswordDict.Add(studentArray[i].Id, passwordList[i]);
        //         L.Add(x);
        //     }
        //     return (Ok(L));
        //     // return (Ok(PasswordDict.studentPasswordDict));
        // }

        if (role == "lecturer")
        {
            var lecturers = from t in db.Lecturer
                            join p in db.Proposal on SemesterId equals p.SemesterId
                            where t.Id == p.AdvisorId1 || t.Id == p.AdvisorId2 || t.Id == p.CommitteeId1 || t.Id == p.CommitteeId2
                            orderby t.Id
                            select t;

            lecturers = lecturers.GroupBy(t => t.Id).Select(grp => grp.First()); // Select distinct lecturers

            var lecturerArray = lecturers.ToArray();
            int lecturerArrayLength = lecturerArray.Length;
            String[] passwordList = new String[lecturerArrayLength];

            for (int i = 0; i < lecturerArrayLength; i++)
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
                passwordList[i] = randPassword;

                byte[] newSalt = new byte[128 / 8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(newSalt);
                }

                string newHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: passwordList[i],
                        salt: newSalt,
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 10000,
                        numBytesRequested: 256 / 8));

                lecturerArray[i].Password = newHash;
                lecturerArray[i].Salt = Convert.ToBase64String(newSalt); // เปลี่ยนจาก byte[] เป็น string
                db.SaveChanges();
            }

            List<DTOs.LecturerRandomPassword> L = new List<DTOs.LecturerRandomPassword>();
            for (int i = 0; i < lecturerArrayLength; i++)
            {
                DTOs.LecturerRandomPassword x = new DTOs.LecturerRandomPassword();
                x.lecturerId = lecturerArray[i].Id;
                x.fullname = lecturerArray[i].Title + lecturerArray[i].FirstName + " " + lecturerArray[i].LastName;
                x.email = lecturerArray[i].Email;
                x.password = passwordList[i];
                bool containKey = PasswordDict.lecturerPasswordDict.ContainsKey((uint)x.lecturerId);
                if (!containKey)
                {
                    PasswordDict.lecturerPasswordDict.Add(lecturerArray[i].Id, passwordList[i]);
                }
                // PasswordDict.lecturerPasswordDict.Add(lecturerArray[i].Id, passwordList[i]);
                L.Add(x);
            }
            return (Ok(L));
            // return (Ok(PasswordDict.lecturerPasswordDict));
        }

        return (Ok());
    }

    // 2. ส่งรหัสผ่านให้นิสิตทางอีเมลจุฬา
    [Route("sendmail")]
    [HttpPost]
    public IActionResult Post1([FromBody] DTOs.Mail m)
    {
        var db = new SeniorProjectDbContext();

        var student = db.Student.Where(x => x.Email.ToLower() == m.studentEmail.ToLower()).FirstOrDefault();

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

        student.Password = newHash;
        student.Salt = Convert.ToBase64String(newSalt); // เปลี่ยนจาก byte[] เป็น string

        db.SaveChanges();

        if (student.Id == null) return StatusCode(403, "Student not found!");

        else if (student.Phone != null) return StatusCode(403, "Already registered!");

        else
        {
            // string password = PasswordDict.studentPasswordDict[student.Id];

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Chatchawit Aporntewan", "chatchawit.a@gmail.com"));
            email.To.Add(MailboxAddress.Parse(student.Email));
            email.Subject = "รหัสผ่านสำหรับเข้าสู่ระบบครั้งแรก เว็บแอปพลิเคชัน Senior Project";
            // email.Body = new TextPart(TextFormat.Html) { Text = "<p> รหัสผ่านของคุณคือ <br/>" + password + "<br/>" + "กรุณากรอกภายใน 1 นาที" };
            email.Body = new TextPart(TextFormat.Html) { Text = System.IO.File.ReadAllText("./index1.html") + randPassword + System.IO.File.ReadAllText("./index2.html") };

            var smtp = new SmtpClient();

            smtp.Connect("email-smtp.ap-southeast-1.amazonaws.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("AKIA2MYSA4BNV37CMUXO", "BDYpo8WMZM411m71iOjzBTI6Utwd6Lb3AqURBu2pxlBR");

            //smtp.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);        
            //smtp.Authenticate("chatchawit.a@chula.ac.th", password);

            smtp.Send(email);
            smtp.Disconnect(true);
        }
        return (Ok(new{
            studentId = student.Id,
            fullname = student.Title + student.FirstName + " " + student.LastName,
            password = randPassword
        }));
    }

    // // เช็คว่ารหัสผ่านที่กรอกตรงกับที่ส่งเมลให้หรือไม่ --> ใช้การขอ token แทน
    // [Route("verifyPassword")]
    // [HttpGet]
    // public IActionResult Get1([FromBody] DTOs.UserLogin u)
    // {
    //     var db = new SeniorProjectDbContext();
    //     var student = db.Student.Where(x => x.Email.ToLower() == u.UserId.ToLower()).FirstOrDefault();
    //     if (student == null) return StatusCode(403, "Student not found!");

    //     string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: u.Password, salt: Convert.FromBase64String(student.Salt), prf: KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 256 / 8));
    //     if (student.Password != hash) return StatusCode(401, "Invalid Password!");

    //     return Ok(new
    //     {
    //         studentId = student.Id,
    //         email = u.UserId,
    //         fullName = student.Title + student.FirstName + " " + student.LastName
    //     });
    // }

    // 3. ขอรหัสผ่านใหม่ กรณีไม่ได้รับอีเมลหรืออีเมลมีปัญหา --> สุ่มรายบุคคล
    [Route("sendMailAgain")]
    [HttpPut]
    public IActionResult Put2([FromBody] DTOs.Mail m)
    {

        var db = new SeniorProjectDbContext();

        var student = db.Student.Where(x => x.Email.ToLower() == m.studentEmail.ToLower()).FirstOrDefault();

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

        student.Password = newHash;
        student.Salt = Convert.ToBase64String(newSalt); // เปลี่ยนจาก byte[] เป็น string

        // PasswordDict.studentPasswordDict[student.Id] = randPassword;

        db.SaveChanges();

        if (student.Id == null) return Forbid();
        else
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Chatchawit Aporntewan", "chatchawit.a@gmail.com"));
            email.To.Add(MailboxAddress.Parse(student.Email));
            email.Subject = "รหัสผ่านใหม่สำหรับเข้าสู่ระบบครั้งแรก เว็บแอปพลิเคชัน Senior Project";
            // email.Body = new TextPart(TextFormat.Html) { Text = "<p> รหัสผ่านของคุณคือ <br/>" + password + "<br/>" + "กรุณากรอกภายใน 1 นาที" };
            email.Body = new TextPart(TextFormat.Html) { Text = System.IO.File.ReadAllText("./index1.html") + randPassword + System.IO.File.ReadAllText("./index2.html") };

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

    // 4. ดึงรายชื่ออาจารย์ทั้งหมดมาใส่ drop down ให้เลือกเป็นที่ปรึกษา
    [Route("lecturerlist")]
    [HttpGet]
    public IActionResult Get2()
    {
        var db = new SeniorProjectDbContext();
        var lecturerList = from t in db.Lecturer select t;
        if (!lecturerList.Any()) return NoContent();
        List<DTOs.LecturerList> L = new List<DTOs.LecturerList>();
        foreach (var t in lecturerList)
        {

            DTOs.LecturerList x = new DTOs.LecturerList();
            x.lecturerId = t.Id;
            x.fullName = t.Title + t.FirstName + " " + t.LastName;
            L.Add(x);
        }
        return Ok(L);
    }

    // 5. สร้างแถวเปล่าของข้อมูลโครงงานเพื่อเตรียมเพิ่มค่า
    [Route("newProject")]
    [HttpPost]
    public IActionResult Post2(uint SemesterId, [FromBody] DTOs.Mail mail)
    {
        var db = new SeniorProjectDbContext();
        var student = db.Student.Where(s => s.Email == mail.studentEmail).Select(s => s).FirstOrDefault();

        var prop = (from p in db.Proposal
                    join sem in db.Semester on p.SemesterId equals sem.Id
                    where p.SemesterId == SemesterId
                    join _s1 in db.Student on p.StudentId1 equals _s1.Id into join1
                    from s1 in join1.DefaultIfEmpty()
                    join _s2 in db.Student on p.StudentId2 equals _s2.Id into join2
                    from s2 in join2.DefaultIfEmpty()
                    join _s3 in db.Student on p.StudentId3 equals _s3.Id into join3
                    from s3 in join3.DefaultIfEmpty()
                    join _a1 in db.Lecturer on p.AdvisorId1 equals _a1.Id into join4
                    from a1 in join4.DefaultIfEmpty()
                    join _a2 in db.Lecturer on p.AdvisorId2 equals _a2.Id into join5
                    from a2 in join5.DefaultIfEmpty()
                    join _c1 in db.Lecturer on p.CommitteeId1 equals _c1.Id into join6
                    from c1 in join6.DefaultIfEmpty()
                    join _c2 in db.Lecturer on p.CommitteeId2 equals _c2.Id into join7
                    from c2 in join7.DefaultIfEmpty()
                    where (p.StudentId1 == student.Id || p.StudentId2 == student.Id || p.StudentId3 == student.Id) && p.SemesterId == SemesterId
                    select p).FirstOrDefault();

        if (prop != null)
        {
            var ProposalInfo = (from p in db.Proposal
                                join sem in db.Semester on p.SemesterId equals sem.Id
                                where p.SemesterId == SemesterId
                                join _s1 in db.Student on p.StudentId1 equals _s1.Id into join1
                                from s1 in join1.DefaultIfEmpty()
                                join _s2 in db.Student on p.StudentId2 equals _s2.Id into join2
                                from s2 in join2.DefaultIfEmpty()
                                join _s3 in db.Student on p.StudentId3 equals _s3.Id into join3
                                from s3 in join3.DefaultIfEmpty()
                                join _a1 in db.Lecturer on p.AdvisorId1 equals _a1.Id into join4
                                from a1 in join4.DefaultIfEmpty()
                                join _a2 in db.Lecturer on p.AdvisorId2 equals _a2.Id into join5
                                from a2 in join5.DefaultIfEmpty()
                                join _c1 in db.Lecturer on p.CommitteeId1 equals _c1.Id into join6
                                from c1 in join6.DefaultIfEmpty()
                                join _c2 in db.Lecturer on p.CommitteeId2 equals _c2.Id into join7
                                from c2 in join7.DefaultIfEmpty()
                                where (p.StudentId1 == student.Id || p.StudentId2 == student.Id || p.StudentId3 == student.Id) && p.SemesterId == SemesterId
                                select new
                                {
                                    // Id = p.Id,
                                    No = p.No ?? 0,
                                    Major = p.Major,
                                    ProjectNameTh = p.ProjectNameTh,
                                    ProjectNameEn = p.ProjectNameEn,
                                    Semester = sem.AcademicYear + "/" + sem.Term,
                                    Advisor1 = a1.Title + a1.FirstName + " " + a1.LastName,
                                    Advisor2 = p.AdvisorId2 == null ? null : a2.Title + a2.FirstName + " " + a2.LastName,
                                    Committee1 = p.CommitteeId1 == null ? null : c1.Title + c1.FirstName + " " + c1.LastName,
                                    Committee2 = p.CommitteeId2 == null ? null : c2.Title + c2.FirstName + " " + c2.LastName,
                                    Student1 = s1.Id + " " + s1.Title + s1.FirstName + " " + s1.LastName,
                                    Student2 = p.StudentId2 == null ? null : s2.Id + " " + s2.Title + s2.FirstName + " " + s2.LastName,
                                    Student3 = p.StudentId3 == null ? null : s3.Id + " " + s3.Title + s3.FirstName + " " + s3.LastName
                                }).FirstOrDefault();

            return Ok(ProposalInfo);
        }
        else
        {
            Models.Proposal newProposal = new Models.Proposal();
            Models.Project newProject = new Models.Project();

            var firstProposalNo = (from p in db.Proposal where p.SemesterId == SemesterId orderby p.Id select p.No).FirstOrDefault();
            var lastProposalNo = (from p in db.Proposal where p.SemesterId == SemesterId orderby p.Id select p.No).LastOrDefault();

            if (firstProposalNo == null) { newProposal.No = 1; }
            else { newProposal.No = lastProposalNo + 1; }

            newProposal.Major = student.Major;
            newProposal.SemesterId = SemesterId;
            newProposal.StudentId1 = student.Id;

            db.Proposal.Add(newProposal);
            db.SaveChanges();

            var firstProjectNo = (from p in db.Project where p.SemesterId == SemesterId + 1 orderby p.Id select p.No).FirstOrDefault();
            var lastProjectNo = (from p in db.Project where p.SemesterId == SemesterId + 1 orderby p.Id select p.No).LastOrDefault();

            if (firstProjectNo == null) { newProject.No = 1; }
            else { newProject.No = lastProjectNo + 1; }

            newProject.Major = student.Major;
            newProject.SemesterId = SemesterId + 1;
            newProject.StudentId1 = student.Id;
            db.Project.Add(newProject);
            db.SaveChanges();

            var ProposalInfo = (from p in db.Proposal
                                join sem in db.Semester on p.SemesterId equals sem.Id
                                where p.SemesterId == SemesterId
                                join _s1 in db.Student on p.StudentId1 equals _s1.Id into join1
                                from s1 in join1.DefaultIfEmpty()
                                join _s2 in db.Student on p.StudentId2 equals _s2.Id into join2
                                from s2 in join2.DefaultIfEmpty()
                                join _s3 in db.Student on p.StudentId3 equals _s3.Id into join3
                                from s3 in join3.DefaultIfEmpty()
                                join _a1 in db.Lecturer on p.AdvisorId1 equals _a1.Id into join4
                                from a1 in join4.DefaultIfEmpty()
                                join _a2 in db.Lecturer on p.AdvisorId2 equals _a2.Id into join5
                                from a2 in join5.DefaultIfEmpty()
                                join _c1 in db.Lecturer on p.CommitteeId1 equals _c1.Id into join6
                                from c1 in join6.DefaultIfEmpty()
                                join _c2 in db.Lecturer on p.CommitteeId2 equals _c2.Id into join7
                                from c2 in join7.DefaultIfEmpty()
                                where (p.StudentId1 == student.Id || p.StudentId2 == student.Id || p.StudentId3 == student.Id) && p.SemesterId == SemesterId
                                select new
                                {
                                    // Id = p.Id,
                                    No = p.No ?? 0,
                                    Major = p.Major,
                                    ProjectNameTh = p.ProjectNameTh ?? null,
                                    ProjectNameEn = p.ProjectNameEn ?? null,
                                    Semester = sem.AcademicYear + "/" + sem.Term,
                                    Advisor1 = p.AdvisorId1 == null ? null : a1.Title + a1.FirstName + " " + a1.LastName,
                                    Advisor2 = p.AdvisorId2 == null ? null : a2.Title + a2.FirstName + " " + a2.LastName,
                                    Committee1 = p.CommitteeId1 == null ? null : c1.Title + c1.FirstName + " " + c1.LastName,
                                    Committee2 = p.CommitteeId2 == null ? null : c2.Title + c2.FirstName + " " + c2.LastName,
                                    Student1 = p.StudentId1 == null ? null : s1.Id + " " + s1.Title + s1.FirstName + " " + s1.LastName,
                                    Student2 = p.StudentId2 == null ? null : s2.Id + " " + s2.Title + s2.FirstName + " " + s2.LastName,
                                    Student3 = p.StudentId3 == null ? null : s3.Id + " " + s3.Title + s3.FirstName + " " + s3.LastName
                                }).FirstOrDefault();

            return Ok(ProposalInfo); // ตรงนี้แปมไปเขียนโปรแกรมดักเองว่า ถ้า return ค่ามาแล้ว AdvisorId1 == null ให้แสดงฟอร์มให้กรอก แต่ถ้า AdvisorId1 != null ให้ดึงค่า proposalInfo มาแสดง

        }
    }

    // เพิ่ม major
    [Route("addMajor")]
    [HttpPut]
    public IActionResult addMajor(uint SemesterId, [FromBody] DTOs.NewProject np)
    {
        var db = new SeniorProjectDbContext();

        var student = db.Student.Where(s => s.Email == np.Email).Select(s => s).FirstOrDefault();

        var proposal = (from p in db.Proposal
                        where (p.StudentId1 == student.Id || p.StudentId2 == student.Id || p.StudentId3 == student.Id) && p.SemesterId == SemesterId
                        orderby p.SemesterId descending
                        select p).FirstOrDefault();

        var project = (from p in db.Project
                       where (p.StudentId1 == student.Id || p.StudentId2 == student.Id || p.StudentId3 == student.Id) && p.SemesterId == SemesterId + 1
                       orderby p.SemesterId descending
                       select p).FirstOrDefault();

        if (proposal == null) return StatusCode(403, "Proposal is null");

        if (np.Major == null) return StatusCode(403, "Major can't be null!");

        else
        {
            if (np.Major.ToUpper() != student.Major) return StatusCode(403, "Project's major must be the same as the student's major!");

            else
            {
                proposal.Major = np.Major.ToUpper();
                db.Proposal.Update(proposal);
                db.SaveChanges();

                project.Major = np.Major.ToUpper();
                db.Project.Update(project);
                db.SaveChanges();
            }

            return Ok();
        }
    }

    // 6. เพิ่ม projectNameTh
    [Route("addProjectNameTh")]
    [HttpPut]
    public IActionResult addProjectNameTh(uint SemesterId, [FromBody] DTOs.NewProject np)
    {
        var db = new SeniorProjectDbContext();

        var student = db.Student.Where(s => s.Email == np.Email).Select(s => s).FirstOrDefault();

        var proposal = (from p in db.Proposal
                        where (p.StudentId1 == student.Id || p.StudentId2 == student.Id || p.StudentId3 == student.Id) && p.SemesterId == SemesterId
                        orderby p.SemesterId descending
                        select p).FirstOrDefault();

        var project = (from p in db.Project
                       where (p.StudentId1 == student.Id || p.StudentId2 == student.Id || p.StudentId3 == student.Id) && p.SemesterId == SemesterId + 1
                       orderby p.SemesterId descending
                       select p).FirstOrDefault();

        if (proposal == null) return StatusCode(403, "Proposal is null");

        if (np.ProjectNameTh == null) return StatusCode(403, "ProjectNameTh can't be null or empty string");

        else
        {
            proposal.ProjectNameTh = np.ProjectNameTh;
            db.Proposal.Update(proposal);
            db.SaveChanges();

            project.ProjectNameTh = np.ProjectNameTh;
            db.Project.Update(project);
            db.SaveChanges();

            return Ok();
        }

    }

    // 7. เพิ่ม projectNameEn
    [Route("addProjectNameEn")]
    [HttpPut]
    public IActionResult addProjectNameEn(uint SemesterId, [FromBody] DTOs.NewProject np)
    {
        var db = new SeniorProjectDbContext();

        var student = db.Student.Where(s => s.Email == np.Email).Select(s => s).FirstOrDefault();

        var proposal = (from p in db.Proposal
                        where (p.StudentId1 == student.Id || p.StudentId2 == student.Id || p.StudentId3 == student.Id) && p.SemesterId == SemesterId
                        orderby p.SemesterId descending
                        select p).FirstOrDefault();

        var project = (from p in db.Project
                       where (p.StudentId1 == student.Id || p.StudentId2 == student.Id || p.StudentId3 == student.Id) && p.SemesterId == SemesterId + 1
                       orderby p.SemesterId descending
                       select p).FirstOrDefault();

        if (proposal == null) return StatusCode(403, "Proposal is null");

        if (np.ProjectNameEn == null) return StatusCode(403, "ProjectNameEn can't be null or empty string!");

        else
        {
            proposal.ProjectNameEn = np.ProjectNameEn;

            project.ProjectNameEn = np.ProjectNameEn;

            db.Proposal.Update(proposal);

            db.SaveChanges();

            db.Project.Update(project);

            db.SaveChanges();

            return Ok();
        }
    }

    // 8. เพิ่ม advisor1
    [Route("addAdvisor1")]
    [HttpPut]
    public IActionResult addAdvisor1(uint SemesterId, [FromBody] DTOs.NewProject np)
    {
        var db = new SeniorProjectDbContext();

        var student = db.Student.Where(s => s.Email == np.Email).Select(s => s).FirstOrDefault();

        var proposal = (from p in db.Proposal
                        where (p.StudentId1 == student.Id || p.StudentId2 == student.Id || p.StudentId3 == student.Id) && p.SemesterId == SemesterId
                        orderby p.SemesterId descending
                        select p).FirstOrDefault();

        var project = (from p in db.Project
                       where (p.StudentId1 == student.Id || p.StudentId2 == student.Id || p.StudentId3 == student.Id) && p.SemesterId == SemesterId + 1
                       orderby p.SemesterId descending
                       select p).FirstOrDefault();

        if (proposal == null) return StatusCode(403, "Proposal is null");

        if (np.AdvisorId1 == null) return StatusCode(403, "Advisor1 can't be null");

        else
        {
            if (db.Lecturer.Find(np.AdvisorId1) == null) return StatusCode(403, "Advisor1 not found!");

            else
            {
                proposal.AdvisorId1 = np.AdvisorId1;

                project.AdvisorId1 = np.AdvisorId1;

                db.Proposal.Update(proposal);

                db.SaveChanges();

                db.Project.Update(project);

                db.SaveChanges();

                return Ok();
            }
        }

    }

    // 9. เพิ่ม advisor2
    [Route("addAdvisor2")]
    [HttpPut]
    public IActionResult addAdvisor2(uint SemesterId, [FromBody] DTOs.NewProject np)
    {
        var db = new SeniorProjectDbContext();

        var student = db.Student.Where(s => s.Email == np.Email).Select(s => s).FirstOrDefault();

        var proposal = (from p in db.Proposal
                        where (p.StudentId1 == student.Id || p.StudentId2 == student.Id || p.StudentId3 == student.Id) && p.SemesterId == SemesterId
                        orderby p.SemesterId descending
                        select p).FirstOrDefault();

        var project = (from p in db.Project
                       where (p.StudentId1 == student.Id || p.StudentId2 == student.Id || p.StudentId3 == student.Id) && p.SemesterId == SemesterId + 1
                       orderby p.SemesterId descending
                       select p).FirstOrDefault();

        if (proposal == null) return StatusCode(403, "Proposal is null");

        if ((np.AdvisorId2 != null) && (db.Lecturer.Find(np.AdvisorId2) == null)) return StatusCode(403, "Advisor2 not found!");

        else
        {
            if (np.AdvisorId2 == proposal.AdvisorId1) return StatusCode(403, "AdvisorId2 must not be the same as the first!");

            else
            {
                proposal.AdvisorId2 = np.AdvisorId2;

                project.AdvisorId2 = np.AdvisorId2;

                db.Proposal.Update(proposal);

                db.SaveChanges();

                db.Project.Update(project);

                db.SaveChanges();

                return Ok();
            }
        }
    }

    // ดึงรายชื่อนิสิตทั้งหมดมาใส่ drop down ให้เลือกเป็นสมาชิก
    [Route("studentlist")]
    [HttpGet]
    public IActionResult Get3(uint SemesterId, string major)
    {
        var db = new SeniorProjectDbContext();

        var studentList = from s in db.Student
                          join _p1 in db.Proposal on s.Id equals _p1.StudentId1 into join1
                          from p1 in join1.DefaultIfEmpty()
                          join _p2 in db.Proposal on s.Id equals _p2.StudentId2 into join2
                          from p2 in join2.DefaultIfEmpty()
                          join _p3 in db.Proposal on s.Id equals _p3.StudentId3 into join3
                          from p3 in join3.DefaultIfEmpty()
                          where (s.Major == major)
                          && ((p1.SemesterId == SemesterId && p1.StudentId1 == s.Id) || (p2.SemesterId == SemesterId && p2.StudentId2 == s.Id) || (p3.SemesterId == SemesterId && p3.StudentId3 == s.Id))
                          select s;

        if (!studentList.Any()) return NoContent();
        List<DTOs.StudentList> L = new List<DTOs.StudentList>();
        foreach (var s in studentList)
        {

            DTOs.StudentList x = new DTOs.StudentList();
            x.studentId = s.Id;
            x.fullName = s.Title + s.FirstName + " " + s.LastName;
            L.Add(x);
        }
        return Ok(L);
    }

    // 10. เพิ่ม student1
    [Route("addStudent1")]
    [HttpPut]
    public IActionResult addStudent1(uint SemesterId, [FromBody] DTOs.NewProject np)
    {
        var db = new SeniorProjectDbContext();

        var student = db.Student.Where(s => s.Email == np.Email).Select(s => s).FirstOrDefault();

        var proposal = (from p in db.Proposal
                        where (p.StudentId1 == student.Id || p.StudentId2 == student.Id || p.StudentId3 == student.Id) && p.SemesterId == SemesterId
                        orderby p.SemesterId descending
                        select p).FirstOrDefault();

        var project = (from p in db.Project
                       where (p.StudentId1 == student.Id || p.StudentId2 == student.Id || p.StudentId3 == student.Id) && p.SemesterId == SemesterId + 1
                       orderby p.SemesterId descending
                       select p).FirstOrDefault();

        if (proposal == null) return StatusCode(403, "Proposal is null");

        if (np.StudentId1 == null) return StatusCode(403, "Student1 can't be null!");

        else
        {
            if (db.Student.Find(np.StudentId1) == null) return StatusCode(403, "Student1 not found!");

            else
            {

                if (np.StudentId1 != student.Id) return StatusCode(403, "StudentId1 must be the person filling out the form only!");

                else
                {
                    proposal.StudentId1 = np.StudentId1;

                    project.StudentId1 = np.StudentId1;

                    db.Proposal.Update(proposal);

                    db.SaveChanges();

                    db.Project.Update(project);

                    db.SaveChanges();

                    return Ok();
                }
            }
        }
    }

    // 11. เพิ่ม student2
    [Route("addStudent2")]
    [HttpPut]
    public IActionResult addStudent2(uint SemesterId, [FromBody] DTOs.NewProject np)
    {
        var db = new SeniorProjectDbContext();

        var student = db.Student.Where(s => s.Email == np.Email).Select(s => s).FirstOrDefault();

        var proposal = (from p in db.Proposal
                        where (p.StudentId1 == student.Id || p.StudentId2 == student.Id || p.StudentId3 == student.Id) && p.SemesterId == SemesterId
                        orderby p.SemesterId descending
                        select p).FirstOrDefault();

        var project = (from p in db.Project
                       where (p.StudentId1 == student.Id || p.StudentId2 == student.Id || p.StudentId3 == student.Id) && p.SemesterId == SemesterId + 1
                       orderby p.SemesterId descending
                       select p).FirstOrDefault();

        if (proposal == null) return StatusCode(403, "Proposal is null");

        if ((np.StudentId2 != null) && (db.Student.Find(np.StudentId2) == null)) return StatusCode(403, "Student2 not found!");

        else
        {

            if (proposal.StudentId3 != null && np.StudentId2 == proposal.StudentId3) return StatusCode(403, "StudentId2 must not be the same as the third!");

            if (np.StudentId2 == proposal.StudentId1) return StatusCode(403, "StudentId2 must not be the same as the first!");

            else
            {
                proposal.StudentId2 = np.StudentId2;

                project.StudentId2 = np.StudentId2;

                db.Proposal.Update(proposal);

                db.SaveChanges();

                db.Project.Update(project);

                db.SaveChanges();

                return Ok();
            }
        }
    }

    // 12. เพิ่ม student3
    [Route("addStudent3")]
    [HttpPut]
    public IActionResult addStudent3(uint SemesterId, [FromBody] DTOs.NewProject np)
    {
        var db = new SeniorProjectDbContext();

        var student = db.Student.Where(s => s.Email == np.Email).Select(s => s).FirstOrDefault();

        var proposal = (from p in db.Proposal
                        where (p.StudentId1 == student.Id || p.StudentId2 == student.Id || p.StudentId3 == student.Id) && p.SemesterId == SemesterId
                        orderby p.SemesterId descending
                        select p).FirstOrDefault();

        var project = (from p in db.Project
                       where (p.StudentId1 == student.Id || p.StudentId2 == student.Id || p.StudentId3 == student.Id) && p.SemesterId == SemesterId + 1
                       orderby p.SemesterId descending
                       select p).FirstOrDefault();

        if (proposal == null) return StatusCode(403, "Proposal is null");

        if ((np.StudentId3 != null) && (db.Student.Find(np.StudentId3) == null)) return StatusCode(403, "Student3 not found");

        else
        {

            if (proposal.StudentId2 != null && np.StudentId3 == proposal.StudentId2) return StatusCode(403, "StudentId3 must not be the same as the second!");

            if (np.StudentId3 == proposal.StudentId1) return StatusCode(403, "StudentId3 must not be the same as the first!");

            else
            {
                proposal.StudentId3 = np.StudentId3;

                project.StudentId3 = np.StudentId3;

                db.Proposal.Update(proposal);

                db.SaveChanges();

                db.Project.Update(project);

                db.SaveChanges();

                return Ok();
            }
        }
    }

    // เพิ่มข้อมูลส่วนบุคคล
    [Route("addPersonalInfo")]
    [HttpPut]
    public IActionResult Put1(uint SemesterId, [FromBody] DTOs.NewMember n)
    {
        var db = new SeniorProjectDbContext();

        // var student = (from s in db.Student
        //                join _p1 in db.Proposal on s.Id equals _p1.StudentId1 into join1
        //                from p1 in join1.DefaultIfEmpty()
        //                join _p2 in db.Proposal on s.Id equals _p2.StudentId2 into join2
        //                from p2 in join2.DefaultIfEmpty()
        //                join _p3 in db.Proposal on s.Id equals _p3.StudentId3 into join3
        //                from p3 in join3.DefaultIfEmpty()
        //                where (p1.SemesterId == SemesterId && p1.StudentId1 == s.Id) || (p2.SemesterId == SemesterId && p2.StudentId2 == s.Id) || (p3.SemesterId == SemesterId && p3.StudentId3 == s.Id)
        //                select s).FirstOrDefault();

        var student = (from s in db.Student where s.Email == n.Email select s).FirstOrDefault();

        if (student == null) return StatusCode(403, "Student is null!");

        // Add Address

        if (n.Address == null || n.Address == "") return StatusCode(403, "Address can't be null or empty string!");
        else
        {
            student.Address = n.Address;

            db.SaveChanges();
        }

        // Add Keyword

        if (n.Keyword == null || n.Keyword == "") return StatusCode(403, "Keyword can't be null or empty string!");

        else
        {
            student.Keyword = n.Keyword;

            db.SaveChanges();
        }


        // Add Hint

        if (n.Hint == null || n.Hint == "") return StatusCode(403, "Hint can't be null or empty string!");

        else
        {
            student.Hint = n.Hint;

            db.SaveChanges();
        }

        // Add Phone Number

        if (n.Phone == null || n.Phone == "") return StatusCode(403, "Phone number can't be null or empty string!");

        else
        {
            student.Phone = n.Phone;

            db.SaveChanges();

        }

        return Ok();
    }
    // เพิ่มข้อมูลโครงงาน
    [Route("addProjectInfo")]
    [HttpPut]
    public IActionResult Put2(uint SemesterId, [FromBody] DTOs.NewProject np)
    {
        var db = new SeniorProjectDbContext();

        if (np.Email == null || np.Email == "") return StatusCode(403, "Email can't be null or empty string!");

        var student = db.Student.Where(s => s.Email == np.Email).Select(s => s).FirstOrDefault();

        if (student == null) return StatusCode(403, "Student is null!");

        var proposal = (from p in db.Proposal
                        where (p.StudentId1 == student.Id || p.StudentId2 == student.Id || p.StudentId3 == student.Id) && p.SemesterId == SemesterId
                        orderby p.SemesterId descending
                        select p).FirstOrDefault();

        var project = (from p in db.Project
                       where (p.StudentId1 == student.Id || p.StudentId2 == student.Id || p.StudentId3 == student.Id) && p.SemesterId == SemesterId + 1
                       orderby p.SemesterId descending
                       select p).FirstOrDefault();

        if (proposal == null) return StatusCode(403, "Proposal is null");

        // addMajor
        if (np.Major == null) return StatusCode(403, "Major can't be null!");

        else
        {
            if (np.Major.ToUpper() != student.Major) return StatusCode(403, "Project's major must be the same as the student's major!");

            else
            {
                proposal.Major = np.Major.ToUpper();
                db.Proposal.Update(proposal);
                db.SaveChanges();

                project.Major = np.Major.ToUpper();
                db.Project.Update(project);
                db.SaveChanges();
            }
        }

        // addProjectNameTh
        if (np.ProjectNameTh == null) return StatusCode(403, "ProjectNameTh can't be null or empty string");

        else
        {
            proposal.ProjectNameTh = np.ProjectNameTh;
            db.Proposal.Update(proposal);
            db.SaveChanges();

            project.ProjectNameTh = np.ProjectNameTh;
            db.Project.Update(project);
            db.SaveChanges();

        }

        // addProjectNameEn
        if (np.ProjectNameEn == null) return StatusCode(403, "ProjectNameEn can't be null or empty string!");

        else
        {
            proposal.ProjectNameEn = np.ProjectNameEn;

            project.ProjectNameEn = np.ProjectNameEn;

            db.Proposal.Update(proposal);

            db.SaveChanges();

            db.Project.Update(project);

            db.SaveChanges();

        }

        // addAdvisor1
        if (np.AdvisorId1 == null) return StatusCode(403, "Advisor1 can't be null");

        else
        {
            if (db.Lecturer.Find(np.AdvisorId1) == null) return StatusCode(403, "Advisor1 not found!");

            else
            {
                proposal.AdvisorId1 = np.AdvisorId1;

                project.AdvisorId1 = np.AdvisorId1;

                db.Proposal.Update(proposal);

                db.SaveChanges();

                db.Project.Update(project);

                db.SaveChanges();

            }
        }

        // addAdvisor2
        if ((np.AdvisorId2 != null) && (db.Lecturer.Find(np.AdvisorId2) == null)) return StatusCode(403, "Advisor2 not found!");

        else
        {
            if (np.AdvisorId2 == proposal.AdvisorId1) return StatusCode(403, "AdvisorId2 must not be the same as the first!");

            else
            {
                proposal.AdvisorId2 = np.AdvisorId2;

                project.AdvisorId2 = np.AdvisorId2;

                db.Proposal.Update(proposal);

                db.SaveChanges();

                db.Project.Update(project);

                db.SaveChanges();

            }
        }

        // addStudent1
        if (np.StudentId1 == null || np.StudentId1 == "") return StatusCode(403, "Student1 can't be null!");

        else
        {
            if (db.Student.Find(np.StudentId1) == null) return StatusCode(403, "Student1 not found!");

            else
            {

                if (np.StudentId1 != student.Id) return StatusCode(403, "StudentId1 must be the person filling out the form only!");

                else
                {
                    proposal.StudentId1 = np.StudentId1;

                    project.StudentId1 = np.StudentId1;

                    db.Proposal.Update(proposal);

                    db.SaveChanges();

                    db.Project.Update(project);

                    db.SaveChanges();

                }
            }
        }

        // addStudent2
        if (np.StudentId1 == null) return StatusCode(403, "Can't add student2 if there are no student1!");

        else if ((np.StudentId2 != null) && (db.Student.Find(np.StudentId2) == null)) return StatusCode(403, "Student2 not found!");

        else
        {
            if (proposal.StudentId3 != null && np.StudentId2 == proposal.StudentId3) return StatusCode(403, "StudentId2 must not be the same as the third!");

            if (np.StudentId2 == proposal.StudentId1) return StatusCode(403, "StudentId2 must not be the same as the first!");

            else
            {

                proposal.StudentId2 = np.StudentId2;

                project.StudentId2 = np.StudentId2;

                db.Proposal.Update(proposal);

                db.SaveChanges();

                db.Project.Update(project);

                db.SaveChanges();

            }
        }

        // addStudent3
        if (np.StudentId3 == null || np.StudentId3 == "")
        {
            if (np.StudentId1 == null && np.StudentId2 == null) return StatusCode(403, "Can't add student3 if there are no student1 and student2!");
        }

        else
        {
            if (proposal.StudentId2 != null && np.StudentId3 == proposal.StudentId2) return StatusCode(403, "StudentId3 must not be the same as the second!");

            else if (np.StudentId1 != null && np.StudentId2 == null) return StatusCode(403, "Can't add student3 if there are no student2!");

            else if ((np.StudentId3 != null) && (db.Student.Find(np.StudentId3) == null)) return StatusCode(403, "Student3 not found");

            if (np.StudentId3 == proposal.StudentId1) return StatusCode(403, "StudentId3 must not be the same as the first!");

            else
            {
                proposal.StudentId3 = np.StudentId3;

                project.StudentId3 = np.StudentId3;

                db.Proposal.Update(proposal);

                db.SaveChanges();

                db.Project.Update(project);

                db.SaveChanges();

            }
        }

        return Ok();
    }

    // 13. ยืนยัน projectinfo --> ส่ง email verification link ให้ advisor1 + advisor2 (if any)
    [Route("verifyProjectInfo")]
    [HttpPost]
    public IActionResult Post3(uint SemesterId, [FromBody] DTOs.Mail m)
    {
        var db = new SeniorProjectDbContext();

        var student = db.Student.Where(x => x.Email.ToLower() == m.studentEmail.ToLower()).FirstOrDefault();
        var proposal = (from p in db.Proposal where student.Id == p.StudentId1 || student.Id == p.StudentId2 || student.Id == p.StudentId3 select p).FirstOrDefault();
        var advisor1Email = (from t in db.Lecturer where proposal.AdvisorId1 == t.Id select t.Email).FirstOrDefault();
        var proposalInfo = (from p in db.Proposal
                            join sem in db.Semester on p.SemesterId equals sem.Id
                            where p.SemesterId == SemesterId
                            join _s1 in db.Student on p.StudentId1 equals _s1.Id into join1
                            from s1 in join1.DefaultIfEmpty()
                            join _s2 in db.Student on p.StudentId2 equals _s2.Id into join2
                            from s2 in join2.DefaultIfEmpty()
                            join _s3 in db.Student on p.StudentId3 equals _s3.Id into join3
                            from s3 in join3.DefaultIfEmpty()
                            join _a1 in db.Lecturer on p.AdvisorId1 equals _a1.Id into join4
                            from a1 in join4.DefaultIfEmpty()
                            join _a2 in db.Lecturer on p.AdvisorId2 equals _a2.Id into join5
                            from a2 in join5.DefaultIfEmpty()
                            join _c1 in db.Lecturer on p.CommitteeId1 equals _c1.Id into join6
                            from c1 in join6.DefaultIfEmpty()
                            join _c2 in db.Lecturer on p.CommitteeId2 equals _c2.Id into join7
                            from c2 in join7.DefaultIfEmpty()
                            where (p.StudentId1 == student.Id || p.StudentId2 == student.Id || p.StudentId3 == student.Id) && p.SemesterId == SemesterId
                            select new
                            {
                                // Id = p.Id,
                                No = p.No ?? 0,
                                Major = p.Major,
                                ProjectNameTh = p.ProjectNameTh ?? null,
                                ProjectNameEn = p.ProjectNameEn ?? null,
                                Semester = sem.AcademicYear + "/" + sem.Term,
                                Advisor1 = p.AdvisorId1 == null ? null : a1.Title + a1.FirstName + " " + a1.LastName,
                                Advisor2 = p.AdvisorId2 == null ? null : a2.Title + a2.FirstName + " " + a2.LastName,
                                Committee1 = p.CommitteeId1 == null ? null : c1.Title + c1.FirstName + " " + c1.LastName,
                                Committee2 = p.CommitteeId2 == null ? null : c2.Title + c2.FirstName + " " + c2.LastName,
                                Student1 = p.StudentId1 == null ? null : s1.Id + " " + s1.Title + s1.FirstName + " " + s1.LastName,
                                Student2 = p.StudentId2 == null ? null : s2.Id + " " + s2.Title + s2.FirstName + " " + s2.LastName,
                                Student3 = p.StudentId3 == null ? null : s3.Id + " " + s3.Title + s3.FirstName + " " + s3.LastName
                            }).FirstOrDefault();

        if (student.Id == null) return Forbid();
        else
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Chatchawit Aporntewan", "chatchawit.a@gmail.com"));
            email.To.Add(MailboxAddress.Parse(advisor1Email));
            email.Subject = "กรุณาตรวจสอบความถูกต้องของข้อมูลโครงงาน หากไม่ถูกต้องโปรดแจ้งนิสิตให้ดำเนินการแก้ไขและติดต่ออ.ชัชวิทย์";
            email.Body = new TextPart(TextFormat.Html) { Text = System.IO.File.ReadAllText("./verify1.html") + proposalInfo.No + "</p>" + "<p style='line-height: 140%;'><b>สาขาวิชา/หลักสูตร: </b>" + proposalInfo.Major + "</p>" + "<p style='line-height: 140%;'><b>ชื่อโครงงานภาษาไทย: </b>" + proposalInfo.ProjectNameTh + "</p>" + "<p style='line-height: 140%;'><b>ชื่อโครงงานภาษาอังกฤษ: </b>" + proposalInfo.ProjectNameEn + "</p>" + "<p style='line-height: 140%;'><b>ภาคการศึกษา: </b>" + proposalInfo.Semester + "</p>" + "<p style='line-height: 140%;'><b>อาจารย์ที่ปรึกษาโครงงาน 1: </b>" + proposalInfo.Advisor1 + "</p>" + "<p style='line-height: 140%;'><b>อาจารย์ที่ปรึกษาโครงงาน 2: </b>" + proposalInfo.Advisor2 + "</p>" + "<p style='line-height: 140%;'><b>สมาชิกโครงงาน 1: </b>" + proposalInfo.Student1 + "</p>" + "<p style='line-height: 140%;'><b>สมาชิกโครงงาน 2: </b>" + proposalInfo.Student2 + "</p>" + "<p style='line-height: 140%;'><b>สมาชิกโครงงาน 3: </b>" + proposalInfo.Student3 + "</p>" + System.IO.File.ReadAllText("./verify2.html") };

            var smtp = new SmtpClient();

            smtp.Connect("email-smtp.ap-southeast-1.amazonaws.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("AKIA2MYSA4BNV37CMUXO", "BDYpo8WMZM411m71iOjzBTI6Utwd6Lb3AqURBu2pxlBR");

            //smtp.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);        
            //smtp.Authenticate("chatchawit.a@chula.ac.th", password);

            smtp.Send(email);
            smtp.Disconnect(true);

            if (proposal.AdvisorId2 != null)
            {
                var advisor2Email = (from t in db.Lecturer where proposal.AdvisorId2 == t.Id select t.Email).FirstOrDefault();

                email = new MimeMessage();
                email.From.Add(new MailboxAddress("Chatchawit Aporntewan", "chatchawit.a@gmail.com"));
                email.To.Add(MailboxAddress.Parse(advisor2Email));
                email.Subject = "กรุณาตรวจสอบความถูกต้องของข้อมูลโครงงาน กลุ่มที่" + proposalInfo.No + "หากไม่ถูกต้องโปรดแจ้งนิสิตให้ดำเนินการแก้ไขและติดต่ออ.ชัชวิทย์";
                // email.Body = new TextPart(TextFormat.Html) { Text = "<p> รหัสผ่านของคุณคือ <br/>" + password + "<br/>" + "กรุณากรอกภายใน 1 นาที" };
                email.Body = new TextPart(TextFormat.Html) { Text = System.IO.File.ReadAllText("./verify1.html") + proposalInfo.No + "</p>" + "<p style='line-height: 140%;'><b>สาขาวิชา/หลักสูตร: </b>" + proposalInfo.Major + "</p>" + "<p style='line-height: 140%;'><b>ชื่อโครงงานภาษาไทย: </b>" + proposalInfo.ProjectNameTh + "</p>" + "<p style='line-height: 140%;'><b>ชื่อโครงงานภาษาอังกฤษ: </b>" + proposalInfo.ProjectNameEn + "</p>" + "<p style='line-height: 140%;'><b>ภาคการศึกษา: </b>" + proposalInfo.Semester + "</p>" + "<p style='line-height: 140%;'><b>อาจารย์ที่ปรึกษาโครงงาน 1: </b>" + proposalInfo.Advisor1 + "</p>" + "<p style='line-height: 140%;'><b>อาจารย์ที่ปรึกษาโครงงาน 2: </b>" + proposalInfo.Advisor2 + "</p>" + "<p style='line-height: 140%;'><b>สมาชิกโครงงาน 1: </b>" + proposalInfo.Student1 + "</p>" + "<p style='line-height: 140%;'><b>สมาชิกโครงงาน 2: </b>" + proposalInfo.Student2 + "</p>" + "<p style='line-height: 140%;'><b>สมาชิกโครงงาน 3: </b>" + proposalInfo.Student3 + "</p>" + System.IO.File.ReadAllText("./verify2.html") };

                smtp = new SmtpClient();

                smtp.Connect("email-smtp.ap-southeast-1.amazonaws.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("AKIA2MYSA4BNV37CMUXO", "BDYpo8WMZM411m71iOjzBTI6Utwd6Lb3AqURBu2pxlBR");

                //smtp.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);        
                //smtp.Authenticate("chatchawit.a@chula.ac.th", password);

                smtp.Send(email);
                smtp.Disconnect(true);
            }
        }
        return (Ok());
    }

    // เพิ่มข้อมูลส่วนบุคคล

    /*
    List API ที่จะทำนะคะ
1. สุ่มรหัสผ่าน + สุ่ม Salt + hash + กรอกใส่ db ให้นิสิตและอาจารย์ (กำลังทำ) --> done
2. ส่งรหัสผ่านให้นิสิตทางอีเมลจุฬา --> done
3. ขอรหัสผ่านใหม่ กรณีไม่ได้รับอีเมลหรืออีเมลมีปัญหา --> ระบุอีเมลแล้วสุ่มรายบุคคล --> done
4. ดึงรายชื่ออาจารย์ทั้งหมดมาใส่ drop down ให้เลือกเป็นที่ปรึกษา --> done
5. สร้างแถวเปล่าของข้อมูลโครงงานเพื่อเตรียมเพิ่มค่า --> done
6. เพิ่ม projectNameTh --> done
7. เพิ่ม projectNameEn --> done
8. เพิ่ม advisor1 --> done
9. เพิ่ม advisor2 (if any) --> done
10. เพิ่ม student1 --> done
11. เพิ่ม student2 (if any) --> done
12. เพิ่ม student3 optional (if any) --> done
13. ยืนยัน projectinfo --> ส่ง email verification link ให้ advisor1 + advisor2 (if any) --> done
    */
}