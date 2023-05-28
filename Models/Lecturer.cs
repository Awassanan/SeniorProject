using System;
using System.Collections.Generic;

namespace SeniorProject.Models
{
    public partial class Lecturer
    {
        public Lecturer()
        {
            ProjectAdvisorId1Navigation = new HashSet<Project>();
            ProjectAdvisorId2Navigation = new HashSet<Project>();
            ProjectCommitteeId1Navigation = new HashSet<Project>();
            ProjectCommitteeId2Navigation = new HashSet<Project>();
            ProposalAdvisorId1Navigation = new HashSet<Proposal>();
            ProposalAdvisorId2Navigation = new HashSet<Proposal>();
            ProposalCommitteeId1Navigation = new HashSet<Proposal>();
            ProposalCommitteeId2Navigation = new HashSet<Proposal>();
        }

        public uint Id { get; set; }
        public string Title { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        /// <summary>
        /// xxx-xxx-xxxx
        /// </summary>
        public string Phone { get; set; } = null!;
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
        public string? ProfilePicture { get; set; }

        public virtual ICollection<Project> ProjectAdvisorId1Navigation { get; set; }
        public virtual ICollection<Project> ProjectAdvisorId2Navigation { get; set; }
        public virtual ICollection<Project> ProjectCommitteeId1Navigation { get; set; }
        public virtual ICollection<Project> ProjectCommitteeId2Navigation { get; set; }
        public virtual ICollection<Proposal> ProposalAdvisorId1Navigation { get; set; }
        public virtual ICollection<Proposal> ProposalAdvisorId2Navigation { get; set; }
        public virtual ICollection<Proposal> ProposalCommitteeId1Navigation { get; set; }
        public virtual ICollection<Proposal> ProposalCommitteeId2Navigation { get; set; }
    }
}
