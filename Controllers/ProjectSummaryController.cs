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
public class ProjectSummaryController : ControllerBase
{
    private readonly ILogger<ProjectSummaryController> _logger;

    public ProjectSummaryController(ILogger<ProjectSummaryController> logger)
    {
        _logger = logger;
    }

    // ตารางสรุปรวม
    [Route("")]
    [HttpGet]
    public IActionResult Get(uint SemesterId)
    {
        var db = new SeniorProjectDbContext();

        // เลือกทุกกลุ่มที่ส่งงานแล้วอย่างน้อย 1 งาน แสดงเฉพาะงานล่าสุดที่แต่ละกลุ่มส่ง
        var projects = from p in db.Project
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
                        orderby p.Id
                        select new
                        {
                            Id = p.Id,
                            No = p.No,
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
                            Student3 = p.StudentId3 == null ? null : s3.Id + " " + s3.Title + s3.FirstName + " " + s3.LastName
                            // GradeStudent1 = p.GradeStudent1,
                            // GradeStudent2 = p.GradeStudent2,
                            // GradeStudent3 = p.GradeStudent3
                        };

        List<DTOs.Project> L = new List<DTOs.Project>();
        foreach (var p in projects)
        {

            DTOs.Project x = new DTOs.Project();
            x.No = (p.No ?? 0);
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
            // x.GradeStudent1 = p.GradeStudent1;
            // x.GradeStudent2 = p.GradeStudent2;
            // x.GradeStudent3 = p.GradeStudent3;

            var db2 = new SeniorProjectDbContext();
            var Project = (from u in db2.ProjectUpload
                            where u.ProjectId == p.Id
                            join a in db2.ProjectAssignment on u.AssignmentId equals a.Id
                            select new
                            {
                                LastAssignmentName = a.AssignmentName,
                                LastAssignmentURL = Program.UploadURL + "/project/assignment/" + u.FileName,
                                LastSubmitDate = u.SubmitDate
                            }).FirstOrDefault();
            if (Project != null)
            {
                x.LatestAssignmentName = Project.LastAssignmentName;
                x.LastAssignmentURL = Project.LastAssignmentURL; // เผื่ออาจารย์อยากดาวน์โหลด จะได้คลิกได้เลย
                x.LatestSubmitDate = Project.LastSubmitDate;
            }
            else
            {
                x.LatestAssignmentName = null;
                x.LastAssignmentURL = null;
                x.LatestSubmitDate = null;
            }
            L.Add(x);
        }

        return Ok(L);
    }
}