using System;
using System.Collections.Generic;

namespace SeniorProject.Models
{
    public partial class Student
    {
        public Student()
        {
            ProjectStudentId1Navigation = new HashSet<Project>();
            ProjectStudentId2Navigation = new HashSet<Project>();
            ProjectStudentId3Navigation = new HashSet<Project>();
            ProjectUpload = new HashSet<ProjectUpload>();
            ProposalStudentId1Navigation = new HashSet<Proposal>();
            ProposalStudentId2Navigation = new HashSet<Proposal>();
            ProposalStudentId3Navigation = new HashSet<Proposal>();
            ProposalUpload = new HashSet<ProposalUpload>();
        }

        public string Id { get; set; } = null!;
        public string Major { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        /// <summary>
        /// xxx-xxx-xxxx
        /// </summary>
        public string? Phone { get; set; }
        public string Email { get; set; } = null!;
        public string? Salt { get; set; }
        public string? Password { get; set; }
        /// <summary>
        /// keyword ยืนยันตัวตนตอนเปลี่ยน/ลืมรหัสผ่าน
        /// </summary>
        public string? Keyword { get; set; }
        /// <summary>
        /// คำใบ้สำหรับ keyword กรณีลืมรหัสและพิมพ์ keyword ผิดเกิน 3 ครั้ง
        /// </summary>
        public string? Hint { get; set; }
        /// <summary>
        /// ตามบัตรปชช.
        /// </summary>
        public string? Address { get; set; }
        public string? ProfilePicture { get; set; }

        public virtual ICollection<Project> ProjectStudentId1Navigation { get; set; }
        public virtual ICollection<Project> ProjectStudentId2Navigation { get; set; }
        public virtual ICollection<Project> ProjectStudentId3Navigation { get; set; }
        public virtual ICollection<ProjectUpload> ProjectUpload { get; set; }
        public virtual ICollection<Proposal> ProposalStudentId1Navigation { get; set; }
        public virtual ICollection<Proposal> ProposalStudentId2Navigation { get; set; }
        public virtual ICollection<Proposal> ProposalStudentId3Navigation { get; set; }
        public virtual ICollection<ProposalUpload> ProposalUpload { get; set; }
    }
}
