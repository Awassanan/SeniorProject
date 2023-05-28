using System;
using System.Collections.Generic;

namespace SeniorProject.Models
{
    public partial class Project
    {
        public Project()
        {
            ProjectUpload = new HashSet<ProjectUpload>();
        }

        public uint Id { get; set; }
        public uint? No { get; set; }
        public string? Major { get; set; }
        public string? ProjectNameTh { get; set; }
        public string? ProjectNameEn { get; set; }
        public uint? SemesterId { get; set; }
        public uint? AdvisorId1 { get; set; }
        public uint? AdvisorId2 { get; set; }
        public uint? CommitteeId1 { get; set; }
        public uint? CommitteeId2 { get; set; }
        public string? StudentId1 { get; set; }
        public string? StudentId2 { get; set; }
        public string? StudentId3 { get; set; }
        /// <summary>
        /// เก็บชื่อไฟล์ ให้อาจารย์ส่งใบรายงานผลสอบหลังกรอกคะแนนแล้ว (All)
        /// </summary>
        public string? Advisor1UploadFile { get; set; }
        /// <summary>
        /// COMP
        /// </summary>
        public string? Advisor2UploadFile { get; set; }
        /// <summary>
        /// COMP
        /// </summary>
        public string? Committee1UploadFile { get; set; }
        /// <summary>
        /// COMP
        /// </summary>
        public string? Committee2UploadFile { get; set; }
        public string? GradeStudent1 { get; set; }
        public string? GradeStudent2 { get; set; }
        public string? GradeStudent3 { get; set; }

        public virtual Lecturer? AdvisorId1Navigation { get; set; }
        public virtual Lecturer? AdvisorId2Navigation { get; set; }
        public virtual Lecturer? CommitteeId1Navigation { get; set; }
        public virtual Lecturer? CommitteeId2Navigation { get; set; }
        public virtual Semester? Semester { get; set; }
        public virtual Student? StudentId1Navigation { get; set; }
        public virtual Student? StudentId2Navigation { get; set; }
        public virtual Student? StudentId3Navigation { get; set; }
        public virtual ICollection<ProjectUpload> ProjectUpload { get; set; }
    }
}
