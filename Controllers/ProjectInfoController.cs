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
public class ProjectInfoController : ControllerBase
{
    private readonly ILogger<ProjectInfoController> _logger;

    public ProjectInfoController(ILogger<ProjectInfoController> logger)
    {
        _logger = logger;
    }

    // ตารางสรุปเฉพาะกลุ่ม ที่ต้องให้เลือก semesterid ได้คือเผื่อ F วิชาเดิมลงใหม่ ก็ยังสามารถดูข้อมูลเก่ากลุ่มตัวเองเมื่อปีที่แล้วได้
    [Route("student")]
    [HttpGet]
    [Authorize(Roles = "student")]
    public IActionResult Get(uint SemesterId)
    {
        var db = new SeniorProjectDbContext();

        var studentId = (User.Identity.Name.Split('@'))[0];

        var projectInfo = (from p in db.Project
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
                           where p.StudentId1 == studentId || p.StudentId2 == studentId || p.StudentId3 == studentId
                           select new
                           {
                               Id = p.Id,
                               No = p.No ?? 0,
                               Major = p.Major,
                               ProjectNameTh = p.ProjectNameTh,
                               ProjectNameEn = p.ProjectNameEn,
                               Semester = sem.AcademicYear + "/" + sem.Term,
                               Advisor1 = a1.Title + a1.FirstName + " " + a1.LastName,
                               Advisor2 = p.AdvisorId2 == null ? null : a2.Title + a2.FirstName + " " + a2.LastName,
                               Committee1 = c1.Title + c1.FirstName + " " + c1.LastName,
                               Committee2 = c2.Title + c2.FirstName + " " + c2.LastName,
                               Student1 = s1.Id + " " + s1.Title + s1.FirstName + " " + s1.LastName,
                               Student2 = p.StudentId2 == null ? null : s2.Id + " " + s2.Title + s2.FirstName + " " + s2.LastName,
                               Student3 = p.StudentId3 == null ? null : s3.Id + " " + s3.Title + s3.FirstName + " " + s3.LastName,
                               GradeStudent1 = p.GradeStudent1,
                               GradeStudent2 = p.GradeStudent2,
                               GradeStudent3 = p.GradeStudent3
                           }).FirstOrDefault();

        return Ok(projectInfo);
    }

    [Route("lecturer")]
    [HttpGet]
    [Authorize(Roles = "lecturer")]
    public IActionResult Get(uint SemesterId, string filter)
    {
        var db = new SeniorProjectDbContext();

        var email = User.Identity.Name;

        var lecturerId = (from t in db.Lecturer where t.Email == email select t.Id).FirstOrDefault();

        var projectInfo = from p in db.Project
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
                          where filter == "advisor" ? p.AdvisorId1 == lecturerId || p.AdvisorId2 == lecturerId :
                          (filter == "committee" ? p.CommitteeId1 == lecturerId || p.CommitteeId2 == lecturerId :
                          (filter == "relevance" && (p.AdvisorId1 == lecturerId || p.AdvisorId2 == lecturerId || p.CommitteeId1 == lecturerId || p.CommitteeId2 == lecturerId)))
                          select new
                          {
                              Id = p.Id,
                              No = p.No ?? 0,
                              Role = filter,
                              Major = p.Major,
                              ProjectNameTh = p.ProjectNameTh,
                              ProjectNameEn = p.ProjectNameEn,
                              Semester = sem.AcademicYear + "/" + sem.Term,
                              Advisor1 = a1.Title + a1.FirstName + " " + a1.LastName,
                              Advisor2 = p.AdvisorId2 == null ? null : a2.Title + a2.FirstName + " " + a2.LastName,
                              Committee1 = c1.Title + c1.FirstName + " " + c1.LastName,
                              Committee2 = c2.Title + c2.FirstName + " " + c2.LastName,
                              Student1 = s1.Id + " " + s1.Title + s1.FirstName + " " + s1.LastName,
                              Student2 = p.StudentId2 == null ? null : s2.Id + " " + s2.Title + s2.FirstName + " " + s2.LastName,
                              Student3 = p.StudentId3 == null ? null : s3.Id + " " + s3.Title + s3.FirstName + " " + s3.LastName,
                              Advisor1UploadFile = p.Advisor1UploadFile,
                              Advisor2UploadFile = p.Advisor2UploadFile,
                              Committee1UploadFile = p.Committee1UploadFile,
                              Committee2UploadFile = p.Committee2UploadFile,
                              GradeStudent1 = p.GradeStudent1,
                              GradeStudent2 = p.GradeStudent2,
                              GradeStudent3 = p.GradeStudent3
                          };

        List<DTOs.ProjectInfo> L = new List<DTOs.ProjectInfo>();
        foreach (var p in projectInfo)
        {

            DTOs.ProjectInfo x = new DTOs.ProjectInfo();
            x.Id = p.Id;
            x.No = p.No;

            var db3 = new SeniorProjectDbContext();
            var lecturer = (from pr in db3.Project
                            join sem in db3.Semester on pr.SemesterId equals sem.Id
                            where pr.SemesterId == SemesterId
                            join _a1 in db3.Lecturer on pr.AdvisorId1 equals _a1.Id into join4
                            from a1 in join4.DefaultIfEmpty()
                            join _a2 in db3.Lecturer on pr.AdvisorId2 equals _a2.Id into join5
                            from a2 in join5.DefaultIfEmpty()
                            join _c1 in db3.Lecturer on pr.CommitteeId1 equals _c1.Id into join6
                            from c1 in join6.DefaultIfEmpty()
                            join _c2 in db3.Lecturer on pr.CommitteeId2 equals _c2.Id into join7
                            from c2 in join7.DefaultIfEmpty()
                            where pr.Id == p.Id

                            select new
                            {
                                AdvisorId1 = pr.AdvisorId1,
                                AdvisorId2 = pr.AdvisorId2,
                                CommitteeId1 = pr.CommitteeId1,
                                CommitteeId2 = pr.CommitteeId2,
                            }).FirstOrDefault();

            if (filter == "advisor")
            {
                x.Role = "ที่ปรึกษา";
            }

            else if (filter == "committee")
            {
                x.Role = "กรรมการ";
            }

            else if (filter == "relevance")
            {
                if (lecturer.AdvisorId1 == lecturerId || lecturer.AdvisorId2 == lecturerId) { x.Role = "ที่ปรึกษา"; }
                else if (lecturer.CommitteeId1 == lecturerId || lecturer.CommitteeId2 == lecturerId) { x.Role = "กรรมการ"; }
            }

            x.Major = p.Major;
            x.ProjectNameTh = p.ProjectNameTh;
            x.ProjectNameEn = p.ProjectNameEn;
            x.Semester = p.Semester;
            x.Advisor1 = p.Advisor1;
            x.Advisor2 = p.Advisor2;
            x.Committee1 = p.Committee1;
            x.Committee2 = p.Committee2;
            x.Student1 = p.Student1;
            x.Student2 = p.Student2;
            x.Student3 = p.Student3;

            bool isAdvisor1 = (lecturer.AdvisorId1 == lecturerId);
            bool isAdvisor2 = (lecturer.AdvisorId2 == lecturerId);
            bool isCommittee1 = (lecturer.CommitteeId1 == lecturerId);
            bool isCommittee2 = (lecturer.CommitteeId2 == lecturerId);

            x.Advisor1UploadFile = (p.Advisor1UploadFile == null ? null : (isAdvisor1 ? Program.UploadURL + "/project/grading/" + p.Advisor1UploadFile : "#"));
            x.Advisor2UploadFile = (p.Advisor2UploadFile == null ? null : (isAdvisor2 ? Program.UploadURL + "/project/grading/" + p.Advisor2UploadFile : "#"));
            x.Committee1UploadFile = (p.Committee1UploadFile == null ? null : (isCommittee1 ? Program.UploadURL + "/project/grading/" + p.Committee1UploadFile : "#"));
            x.Committee2UploadFile = (p.Committee2UploadFile == null ? null : (isCommittee2 ? Program.UploadURL + "/project/grading/" + p.Committee2UploadFile : "#"));

            x.GradeStudent1 = p.GradeStudent1;
            x.GradeStudent2 = p.GradeStudent2;
            x.GradeStudent3 = p.GradeStudent3;
            L.Add(x);
        }

        return Ok(L);
    }

    [Route("editProjectNameTh")]
    [HttpPut]
    [Authorize(Roles = "student")]
    public IActionResult Put1([FromBody] DTOs.ProjectInfo projInfo)
    {
        var db = new SeniorProjectDbContext();

        var studentId = (User.Identity.Name.Split('@'))[0];

        var project = (from p in db.Project
                       where p.StudentId1 == studentId || p.StudentId2 == studentId || p.StudentId3 == studentId
                       orderby p.SemesterId descending
                       select p).FirstOrDefault();

        if (project == null) return Forbid();

        project.ProjectNameTh = projInfo.ProjectNameTh;

        db.Project.Update(project);

        db.SaveChanges();

        return Ok();
    }

    [Route("editProjectNameEn")]
    [HttpPut]
    [Authorize(Roles = "student")]
    public IActionResult Put2([FromBody] DTOs.ProjectInfo projInfo)
    {
        var db = new SeniorProjectDbContext();

        var studentId = (User.Identity.Name.Split('@'))[0];

        var project = (from p in db.Project
                       where p.StudentId1 == studentId || p.StudentId2 == studentId || p.StudentId3 == studentId
                       orderby p.SemesterId descending
                       select p).FirstOrDefault();

        if (project == null) return Forbid();

        project.ProjectNameEn = projInfo.ProjectNameEn;

        db.Project.Update(project);

        db.SaveChanges();

        return Ok();
    }
}