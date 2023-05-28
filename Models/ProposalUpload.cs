using System;
using System.Collections.Generic;

namespace SeniorProject.Models
{
    public partial class ProposalUpload
    {
        public uint Id { get; set; }
        public uint ProposalId { get; set; }
        public uint AssignmentId { get; set; }
        public string FileName { get; set; } = null!;
        public string FileType { get; set; } = null!;
        public DateTime SubmitDate { get; set; }
        public string StudentId { get; set; } = null!;

        public virtual ProposalAssignment Assignment { get; set; } = null!;
        public virtual Proposal Proposal { get; set; } = null!;
        public virtual Student Student { get; set; } = null!;
    }
}
