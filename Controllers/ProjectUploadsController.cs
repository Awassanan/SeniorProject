using Microsoft.AspNetCore.Mvc;
using SeniorProject.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.IO;

namespace SeniorProject.Controllers;

[ApiController]
[Route("[controller]")]
public class ProjectUploadsController : ControllerBase
{

    private readonly ILogger<ProjectUploadsController> _logger;

    public ProjectUploadsController(ILogger<ProjectUploadsController> logger)
    {
        _logger = logger;
    }

    // นิสิตเรียกดูประวัติการส่งงานของกลุ่มตนเอง
    [Route("student")]
    [HttpGet]
    [Authorize(Roles = "student")]
    public IActionResult Get1(uint SemesterId)
    {
        var db = new SeniorProjectDbContext();

        var studentId = (User.Identity.Name.Split('@'))[0];

        var uploadRecord = (from p in db.Project
                            join sem in db.Semester on p.SemesterId equals sem.Id
                            where p.SemesterId == SemesterId
                            join _pu in db.ProjectUpload on p.Id equals _pu.ProjectId into join8
                            from pu in join8.DefaultIfEmpty()
                            join a in db.ProjectAssignment on pu.AssignmentId equals a.Id
                            join _s1 in db.Student on p.StudentId1 equals _s1.Id into join1
                            from s1 in join1.DefaultIfEmpty()
                            join _s2 in db.Student on p.StudentId2 equals _s2.Id into join2
                            from s2 in join2.DefaultIfEmpty()
                            join _s3 in db.Student on p.StudentId3 equals _s3.Id into join3
                            from s3 in join3.DefaultIfEmpty()
                            where p.StudentId1 == studentId || p.StudentId2 == studentId || p.StudentId3 == studentId
                            orderby /*p.Id,*/ pu.SubmitDate descending
                            select new
                            {
                                AssignmentName = a.AssignmentName,
                                FileName = Program.UploadURL + "/project/assignment/" + pu.FileName,
                                SubmitDate = pu.SubmitDate,
                                Sender = pu.StudentId == s1.Id ? s1.Id + " " + s1.Title + s1.FirstName + " " + s1.LastName :
                                (pu.StudentId == s2.Id ? s2.Id + " " + s2.Title + s2.FirstName + " " + s2.LastName :
                                (pu.StudentId == s3.Id ? s3.Id + " " + s3.Title + s3.FirstName + " " + s3.LastName : null))
                            });

        return (Ok(uploadRecord));
    }

    // อาจารย์เรียกดูประวัติการส่งงานของกลุ่มที่ระบุ
    [Route("lecturer")]
    [HttpGet]
    [Authorize(Roles = "lecturer")]
    public IActionResult Get2(uint ProjectId)
    {
        var db = new SeniorProjectDbContext();

        var lecturerId = db.Lecturer.Where(x => x.Email == User.Identity.Name).Select(x => x.Id).FirstOrDefault();
        if (lecturerId == null) return Forbid();

        var uploadRecord = (from p in db.Project
                            join sem in db.Semester on p.SemesterId equals sem.Id
                            where p.Id == ProjectId
                            join _pu in db.ProjectUpload on p.Id equals _pu.ProjectId into join8
                            from pu in join8.DefaultIfEmpty()
                            join a in db.ProjectAssignment on pu.AssignmentId equals a.Id
                            join _s1 in db.Student on p.StudentId1 equals _s1.Id into join1
                            from s1 in join1.DefaultIfEmpty()
                            join _s2 in db.Student on p.StudentId2 equals _s2.Id into join2
                            from s2 in join2.DefaultIfEmpty()
                            join _s3 in db.Student on p.StudentId3 equals _s3.Id into join3
                            from s3 in join3.DefaultIfEmpty()
                                // where p.AdvisorId1 == lecturerId || p.AdvisorId2 == lecturerId || p.CommitteeId1 == lecturerId || p.CommitteeId2 == lecturerId
                            orderby /*p.Id,*/ pu.SubmitDate descending
                            select new
                            {
                                AssignmentName = a.AssignmentName,
                                FileName = Program.UploadURL + "/project/assignment/" + pu.FileName,
                                SubmitDate = pu.SubmitDate,
                                Sender = pu.StudentId == s1.Id ? s1.Id + " " + s1.Title + s1.FirstName + " " + s1.LastName :
                                (pu.StudentId == s2.Id ? s2.Id + " " + s2.Title + s2.FirstName + " " + s2.LastName :
                                (pu.StudentId == s3.Id ? s3.Id + " " + s3.Title + s3.FirstName + " " + s3.LastName : null))
                            });

        return (Ok(uploadRecord));
    }

    // นิสิตอัปโหลดงานที่ต้องการส่ง
    [HttpPost]
    [Authorize(Roles = "student")]
    public IActionResult Post(uint assignmentId, IFormCollection FormData)
    {
        var db = new SeniorProjectDbContext();

        var studentId = (User.Identity.Name.Split('@'))[0];

        var project = (from p in db.Project
                       where p.StudentId1 == studentId || p.StudentId2 == studentId || p.StudentId3 == studentId
                       orderby p.SemesterId descending
                       select p).FirstOrDefault();
        if (project == null) return StatusCode(403, new { error = "Project is null" });

        var Semester = db.Semester.Find(project.SemesterId);
        if (Semester == null) return StatusCode(403, new { error = "Semester is null" });

        var Assignment = db.ProjectAssignment.Find(assignmentId);
        if (Assignment == null) return StatusCode(403, new { error = "Assignment is null" });

        var file = FormData.Files.GetFile("file");
        if (file == null) return StatusCode(403, new { error = "File is null" });
        if (file.Length > Assignment.MaxSize * 1000000) return StatusCode(413); // payload too large

        string[] Extensions = Assignment.FileType.ToLower().Split(",");
        string ext = file.FileName.Split(".").Last().ToLower();
        if (!Extensions.Contains(ext)) return StatusCode(415); // Unsupported Media Type

        // string path = @"C:\Users\awass\Desktop\"; // Windows
        string path = Program.UploadPath + "/project/assignment/"; // Linux
        string name = Semester.AcademicYear + "_" + Semester.Term + "_" + Assignment.SaveName + "_" + project.No + "." + ext;

        Stream fs = new FileStream(path + name, FileMode.Create);
        file.CopyTo(fs);
        fs.Close();

        var upload = (from u in db.ProjectUpload
                      where u.ProjectId == project.Id && u.AssignmentId == assignmentId
                      select u).FirstOrDefault();

        if (upload != null)
        {
            upload.FileName = name;
            upload.FileType = ext;
            upload.SubmitDate = DateTime.Now;
            upload.StudentId = studentId;
            db.SaveChanges();
        }
        else
        {
            upload = new Models.ProjectUpload();
            upload.ProjectId = project.Id;
            upload.AssignmentId = assignmentId;
            upload.FileName = name;
            upload.FileType = ext;
            upload.SubmitDate = DateTime.Now;
            upload.StudentId = studentId;
            db.ProjectUpload.Add(upload);
            db.SaveChanges();
        }

        return Ok(new { id = upload.Id });
    }

    // อัปโหลดใบรายงานผลสอบ
    [HttpPut]
    [Authorize(Roles = "lecturer")]
    public IActionResult Put(uint ProjectId, IFormCollection FormData)
    {
        var db = new SeniorProjectDbContext();

        var lecturerId = db.Lecturer.Where(x => x.Email == User.Identity.Name).Select(x => x.Id).FirstOrDefault();
        if (lecturerId == null) return Forbid();

        var project = db.Project.Find(ProjectId);
        if (project == null) return Forbid();

        bool isAdvisor1 = false;
        bool isAdvisor2 = false;
        bool isCommittee1 = false;
        bool isCommittee2 = false;

        if (project.Major == "MATH")
        {
            if (project.AdvisorId1 == lecturerId) isAdvisor1 = true;
            if (!isAdvisor1) return Forbid();
        }

        if (project.Major == "COMP")
        {
            if (project.AdvisorId1 == lecturerId) isAdvisor1 = true;
            if (project.AdvisorId2 == lecturerId) isAdvisor2 = true;
            if (project.CommitteeId1 == lecturerId) isCommittee1 = true;
            if (project.CommitteeId2 == lecturerId) isCommittee2 = true;
            if (!isAdvisor1 && !isAdvisor2 && !isCommittee1 && !isCommittee2) return Forbid();
        }

        var Semester = db.Semester.Find(project.SemesterId);
        if (Semester == null) return Forbid();

        var file = FormData.Files.GetFile("file");
        if (file == null) return Forbid();
        if (file.Length > 10 * 1000000) return StatusCode(413); // payload too large

        string[] Extensions = new string[] { "pdf", "jpg", "jpeg", "png" };
        string ext = file.FileName.Split(".").Last().ToLower();
        if (!Extensions.Contains(ext)) return StatusCode(415); // Unsupported Media Type

        // string path = @"C:\Users\awass\Desktop\"; // Windows
        string path = Program.UploadPath + "/project/grading/"; // Linux
        string grader = "anonymous";
        if (isAdvisor1) grader = "GradeByAdvisor1";
        if (isAdvisor2) grader = "GradeByAdvisor2";
        if (isCommittee1) grader = "GradeByCommittee1";
        if (isCommittee2) grader = "GradeByCommittee2";

        // string random = "12345abcde";
        string random = RandomString.CreateRandomString(10);

        string name = Semester.AcademicYear + "_" + Semester.Term + "_" + project.Major + "_" + project.No + "_" +
            grader + "_" + random + "." + ext;

        // Stream fs = new FileStream(path + name, FileMode.Create);
        // file.CopyTo(fs);
        // fs.Close();

        // if (isAdvisor1) project.Advisor1UploadFile = name;
        // if (isAdvisor2) project.Advisor2UploadFile = name;
        // if (isCommittee1) project.Committee1UploadFile = name;
        // if (isCommittee2) project.Committee2UploadFile = name;
        // db.SaveChanges();

        if (isAdvisor1)
        {
            if (project.Advisor1UploadFile != null)
            {
                System.IO.File.Delete(path + project.Advisor1UploadFile);
            }
            Stream fs = new FileStream(path + name, FileMode.Create);
            file.CopyTo(fs);
            fs.Close();
            project.Advisor1UploadFile = name;
            db.SaveChanges();
        }

        if (isAdvisor2)
        {
            if (project.Advisor2UploadFile != null)
            {
                System.IO.File.Delete(path + project.Advisor2UploadFile);
            }
            Stream fs = new FileStream(path + name, FileMode.Create);
            file.CopyTo(fs);
            fs.Close();
            project.Advisor2UploadFile = name;
            db.SaveChanges();
        }

        if (isCommittee1)
        {
            if (project.Committee1UploadFile != null)
            {
                System.IO.File.Delete(path + project.Committee1UploadFile);
            }
            Stream fs = new FileStream(path + name, FileMode.Create);
            file.CopyTo(fs);
            fs.Close();
            project.Committee1UploadFile = name;
            db.SaveChanges();
        }

        if (isCommittee2)
        {
            if (project.Committee1UploadFile != null)
            {
                System.IO.File.Delete(path + project.Committee2UploadFile);
            }
            Stream fs = new FileStream(path + name, FileMode.Create);
            file.CopyTo(fs);
            fs.Close();
            project.Committee2UploadFile = name;
            db.SaveChanges();
        }

        return Ok();
    }

    // อาจารย์เรียกดูประวัติการอัปโหลดใบรายงานผลสอบ
    [Route("grading")]
    [HttpGet]
    [Authorize(Roles = "lecturer")]
    public IActionResult Get3(uint ProjectId)
    {
        var db = new SeniorProjectDbContext();

        var lecturerId = db.Lecturer.Where(x => x.Email == User.Identity.Name).Select(x => x.Id).FirstOrDefault();
        if (lecturerId == null) return Forbid();

        var filename = (from p in db.Project
                        where (p.Id == ProjectId) && (p.AdvisorId1 == lecturerId || p.AdvisorId2 == lecturerId || p.CommitteeId1 == lecturerId || p.CommitteeId2 == lecturerId)
                        orderby p.Id
                        select new
                        {
                            name = p.AdvisorId1 == lecturerId ? p.Advisor1UploadFile :
                                        (p.AdvisorId2 == lecturerId ? p.Advisor2UploadFile :
                                        (p.CommitteeId1 == lecturerId ? p.Committee1UploadFile :
                                        (p.CommitteeId2 == lecturerId ? p.Committee2UploadFile : null)))
                        }).FirstOrDefault();

        if(filename == null) return NotFound();

        DTOs.GradingRecord record = new DTOs.GradingRecord();
        record.FileName = filename.name;

        if(record.FileName == null) record.URL = null;
        else record.URL = Program.UploadURL + "/project/grading/" + record.FileName;

        return (Ok(record));
    }
}