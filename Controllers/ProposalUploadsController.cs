using Microsoft.AspNetCore.Mvc;
using SeniorProject.Models;
using Microsoft.AspNetCore.Authorization;

namespace SeniorProject.Controllers;

[ApiController]
[Route("[controller]")]
public class ProposalUploadsController : ControllerBase
{

    private readonly ILogger<ProposalUploadsController> _logger;

    public ProposalUploadsController(ILogger<ProposalUploadsController> logger)
    {
        _logger = logger;
    }

    [Route("student")]
    [HttpGet]
    [Authorize(Roles = "student")]
    public IActionResult Get1(uint SemesterId)
    {
        var db = new SeniorProjectDbContext();

        var studentId = (from s in db.Student where s.Email == User.Identity.Name select s.Id).FirstOrDefault();

        var uploadRecord = (from p in db.Proposal
                            join sem in db.Semester on p.SemesterId equals sem.Id
                            where p.SemesterId == SemesterId
                            join _pu in db.ProposalUpload on p.Id equals _pu.ProposalId into join8
                            from pu in join8.DefaultIfEmpty()
                            join a in db.ProposalAssignment on pu.AssignmentId equals a.Id
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
                                FileName = Program.UploadURL + "/proposal/assignment/" + pu.FileName,
                                SubmitDate = pu.SubmitDate,
                                Sender = pu.StudentId == s1.Id ? s1.Id + " " + s1.Title + s1.FirstName + " " + s1.LastName :
                                (pu.StudentId == s2.Id ? s2.Id + " " + s2.Title + s2.FirstName + " " + s2.LastName :
                                (pu.StudentId == s3.Id ? s3.Id + " " + s3.Title + s3.FirstName + " " + s3.LastName : null))
                            });

        return (Ok(uploadRecord));
    }

    [Route("lecturer")]
    [HttpGet]
    [Authorize(Roles = "lecturer")]
    public IActionResult Get2(uint ProposalId)
    {
        var db = new SeniorProjectDbContext();

        var lecturerId = db.Lecturer.Where(x => x.Email == User.Identity.Name).Select(x => x.Id).FirstOrDefault();
        if (lecturerId == null) return Forbid();

        var uploadRecord = (from p in db.Proposal
                            join sem in db.Semester on p.SemesterId equals sem.Id
                            where p.Id == ProposalId
                            join _pu in db.ProposalUpload on p.Id equals _pu.ProposalId into join8
                            from pu in join8.DefaultIfEmpty()
                            join a in db.ProposalAssignment on pu.AssignmentId equals a.Id
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
                                FileName = Program.UploadURL + "/proposal/assignment/" + pu.FileName,
                                SubmitDate = pu.SubmitDate,
                                Sender = pu.StudentId == s1.Id ? s1.Id + " " + s1.Title + s1.FirstName + " " + s1.LastName :
                                (pu.StudentId == s2.Id ? s2.Id + " " + s2.Title + s2.FirstName + " " + s2.LastName :
                                (pu.StudentId == s3.Id ? s3.Id + " " + s3.Title + s3.FirstName + " " + s3.LastName : null))
                            });

        return (Ok(uploadRecord));
    }

    [HttpPost]
    [Authorize(Roles = "student")]
    public IActionResult Post(uint assignmentId, IFormCollection FormData)
    {
        var db = new SeniorProjectDbContext();

        var studentId = (from s in db.Student where s.Email == User.Identity.Name select s.Id).FirstOrDefault();

        var proposal = (from p in db.Proposal
                        where p.StudentId1 == studentId || p.StudentId2 == studentId || p.StudentId3 == studentId
                        orderby p.SemesterId descending
                        select p).FirstOrDefault();
        if (proposal == null) return Forbid();

        var Semester = db.Semester.Find(proposal.SemesterId);
        if (Semester == null) return Forbid();

        var Assignment = db.ProposalAssignment.Find(assignmentId);
        if (Assignment == null) return Forbid();

        var file = FormData.Files.GetFile("file");
        if (file == null) return Forbid();
        if (file.Length > Assignment.MaxSize * 1000000) return StatusCode(413); // payload too large

        string[] Extensions = Assignment.FileType.ToLower().Split(",");
        string ext = file.FileName.Split(".").Last().ToLower();
        if (!Extensions.Contains(ext)) return StatusCode(415); // Unsupported Media Type

        // string path = @"C:\Users\awass\Desktop\"; // Windows
        string path = Program.UploadPath + "/proposal/assignment/"; // Linux
        string name = Semester.AcademicYear + "_" + Semester.Term + "_" + Assignment.SaveName + "_" + proposal.No + "." + ext;

        // Stream fs = new FileStream(path + name, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        Stream fs = new FileStream(path + name, FileMode.Create);
        file.CopyTo(fs);
        fs.Close();

        var upload = (from u in db.ProposalUpload
                      where u.ProposalId == proposal.Id && u.AssignmentId == assignmentId
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
            upload = new Models.ProposalUpload();
            upload.ProposalId = proposal.Id;
            upload.AssignmentId = assignmentId;
            upload.FileName = name;
            upload.FileType = ext;
            upload.SubmitDate = DateTime.Now;
            upload.StudentId = studentId;
            db.ProposalUpload.Add(upload);
            db.SaveChanges();
        }

        return Ok(new { id = upload.Id });
    }

    [HttpPut]
    [Authorize(Roles = "lecturer")]
    public IActionResult Put(uint ProposalId, IFormCollection FormData)
    {
        var db = new SeniorProjectDbContext();

        var lecturerId = db.Lecturer.Where(x => x.Email == User.Identity.Name).Select(x => x.Id).FirstOrDefault();
        if (lecturerId == null) return StatusCode(403,"Lecturer is null!");

        var proposal = db.Proposal.Find(ProposalId);
        if (proposal == null) return StatusCode(403,"Proposal is null!");

        bool isAdvisor1 = false;
        bool isAdvisor2 = false;
        bool isCommittee1 = false;
        bool isCommittee2 = false;

        if (proposal.Major == "MATH")
        {
            if (proposal.AdvisorId1 == lecturerId) isAdvisor1 = true;
            if (!isAdvisor1) return StatusCode(403,"Only Advisor1 can upload (MATH)!");
        }

        if (proposal.Major == "COMP")
        {
            if (proposal.AdvisorId1 == lecturerId) isAdvisor1 = true;
            if (proposal.AdvisorId2 == lecturerId) isAdvisor2 = true;
            if (proposal.CommitteeId1 == lecturerId) isCommittee1 = true;
            if (proposal.CommitteeId2 == lecturerId) isCommittee2 = true;
            if (!isAdvisor1 && !isAdvisor2 && !isCommittee1 && !isCommittee2) return StatusCode(403,"This lecturer is not related to this proposal!");
        }

        var Semester = db.Semester.Find(proposal.SemesterId);
        if (Semester == null) return StatusCode(403,"SemesterId is null!");

        var file = FormData.Files.GetFile("file");
        if (file == null) return StatusCode(403,"File is null!");
        if (file.Length > 10 * 1000000) return StatusCode(413); // payload too large

        string[] Extensions = new string[] { "pdf", "jpg", "jpeg", "png" };
        string ext = file.FileName.Split(".").Last().ToLower();
        if (!Extensions.Contains(ext)) return StatusCode(415,"Not support this file extension!");

        // string path = @"C:\Users\awass\Desktop\"; // Windows
        string path = Program.UploadPath + "/proposal/grading/"; // Linux
        string grader = "anonymous";
        if (isAdvisor1) grader = "GradeByAdvisor1";
        if (isAdvisor2) grader = "GradeByAdvisor2";
        if (isCommittee1) grader = "GradeByCommittee1";
        if (isCommittee2) grader = "GradeByCommittee2";

        // string random = "12345abcde";
        string random = RandomString.CreateRandomString(10);

        string name = Semester.AcademicYear + "_" + Semester.Term + "_" + proposal.Major + "_" + proposal.No + "_" +
            grader + "_" + random + "." + ext;

        // Stream fs = new FileStream(path + name, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        // Stream fs = new FileStream(path + name, FileMode.Create);
        // file.CopyTo(fs);
        // fs.Close();

        // if (isAdvisor1) proposal.Advisor1UploadFile = name;
        // if (isAdvisor2) proposal.Advisor2UploadFile = name;
        // if (isCommittee1) proposal.Committee1UploadFile = name;
        // if (isCommittee2) proposal.Committee2UploadFile = name;
        // db.SaveChanges();

        if (isAdvisor1)
        {
            if (proposal.Advisor1UploadFile != null)
            {
                System.IO.File.Delete(path + proposal.Advisor1UploadFile);
            }
            Stream fs = new FileStream(path + name, FileMode.Create);
            file.CopyTo(fs);
            fs.Close();
            proposal.Advisor1UploadFile = name;
            db.SaveChanges();
        }

        if (isAdvisor2)
        {
            if (proposal.Advisor2UploadFile != null)
            {
                System.IO.File.Delete(path + proposal.Advisor2UploadFile);
            }
            Stream fs = new FileStream(path + name, FileMode.Create);
            file.CopyTo(fs);
            fs.Close();
            proposal.Advisor2UploadFile = name;
            db.SaveChanges();
        }

        if (isCommittee1)
        {
            if (proposal.Committee1UploadFile != null)
            {
                System.IO.File.Delete(path + proposal.Committee1UploadFile);
            }
            Stream fs = new FileStream(path + name, FileMode.Create);
            file.CopyTo(fs);
            fs.Close();
            proposal.Committee1UploadFile = name;
            db.SaveChanges();
        }

        if (isCommittee2)
        {
            if (proposal.Committee2UploadFile != null)
            {
                System.IO.File.Delete(path + proposal.Committee2UploadFile);
            }
            Stream fs = new FileStream(path + name, FileMode.Create);
            file.CopyTo(fs);
            fs.Close();
            proposal.Committee2UploadFile = name;
            db.SaveChanges();
        }

        return Ok();
    }

    // อาจารย์เรียกดูประวัติการอัปโหลดใบรายงานผลสอบ
    [Route("grading")]
    [HttpGet]
    [Authorize(Roles = "lecturer")]
    public IActionResult Get3(uint ProposalId)
    {
        var db = new SeniorProjectDbContext();

        var lecturerId = db.Lecturer.Where(x => x.Email == User.Identity.Name).Select(x => x.Id).FirstOrDefault();
        if (lecturerId == null) return Forbid();

        var filename = (from p in db.Proposal
                        where (p.Id == ProposalId) && (p.AdvisorId1 == lecturerId || p.AdvisorId2 == lecturerId || p.CommitteeId1 == lecturerId || p.CommitteeId2 == lecturerId)
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
        else record.URL = Program.UploadURL + "/proposal/grading/" + record.FileName;

        return (Ok(record));
    }
}